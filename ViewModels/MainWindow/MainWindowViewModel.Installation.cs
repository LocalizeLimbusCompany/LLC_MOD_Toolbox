using LLC_MOD_Toolbox.Services.Installation;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace LLC_MOD_Toolbox.ViewModels
{
    public sealed partial class MainWindowViewModel
    {
        private void ExecuteUninstall()
        {
            if (!_dialogService.ShowConfirm("删除后你需要重新安装汉化补丁。\n确定继续吗？", "警告"))
                return;

            Log.logger.Info("确定删除模组。");
            try
            {
                IsGlobalOperationsEnabled = false;
                _uninstallService.UninstallAll();
                IsGlobalOperationsEnabled = true;
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("删除过程中出现了一些问题： " + ex.ToString(), "警告");
            }
            _dialogService.ShowMessage("删除完成。", "提示");
            CurrentVersionText = "当前版本：未安装";
            Log.logger.Info("删除完成。");
        }

        private async Task ExecuteAutoInstallAsync()
        {
            long installProgressSession = ++_installProgressSession;
            ProgressPercentage = 0;
            IsInstalling = true;
            Log.logger.Info("开始安装。");

            try
            {
                if (!await ConfirmAutoInstallPreconditionsAsync())
                    return;

                var progress = new Progress<InstallProgress>(p =>
                {
                    if (IsInstalling &&
                        installProgressSession == _installProgressSession &&
                        p.Percentage >= ProgressPercentage)
                        ProgressPercentage = p.Percentage;
                });
                InstallResult result = await _installService.InstallAsync(progress);
                if (result != InstallResult.Succeeded)
                {
                    Log.logger.Warn("安装已中止。");
                    ResetInstallState();
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.logger.Error("安装出错。", ex);
                await StopInstallationAsync();
                _dialogService.ShowMessage($"安装出错。\n您可以尝试在设置中切换节点。\n错误：{Services.Network.HttpService.GetExceptionText(ex)}", "错误");
                return;
            }

            ProgressPercentage = 100;
            Log.logger.Info("安装完成。");

            try
            {
                var refreshedVersion = await _versionService.CheckVersionsAsync();
                CurrentVersionText = refreshedVersion.CurrentText;
                LatestVersionText = refreshedVersion.LatestText;
            }
            catch (Exception ex)
            {
                Log.logger.Error("安装后刷新版本信息失败。", ex);
            }

            if (_config.Settings.Installation.CloseAfterInstallation || _appState.IsLauncherMode)
            {
                try
                {
                    OpenUrl("steam://rungameid/1973530");
                    Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    Log.logger.Error("安装完成后启动游戏失败。", ex);
                    ResetInstallState();
                    _dialogService.ShowMessage("出现了问题。\n" + ex, "提示");
                }
                return;
            }

            bool runResult;
            try
            {
                runResult = _dialogService.ShowConfirm("安装已完成！\n点击\"是\"立刻运行边狱公司。\n点击\"否\"关闭弹窗。", "提示");
            }
            finally
            {
                ResetInstallState();
            }

            if (runResult)
            {
                try { OpenUrl("steam://rungameid/1973530"); }
                catch (Exception ex) { _dialogService.ShowMessage("出现了问题。\n" + ex.ToString(), "提示"); }
            }
        }

        private async Task<bool> ConfirmAutoInstallPreconditionsAsync()
        {
            if (File.Exists(Path.Combine(_appState.GameDirectory, "version.dll")) &&
                !_dialogService.ShowConfirm("检测到MelonLoader框架！\nMelonLoader框架已过时，且其可能导致您的账号遭到封禁，导致您无法进行游戏！\n建议您进行一次卸载后继续安装模组。\n若您**及其确定这是个误判**，请点击是，否则请点击否返回。", "警告"))
            {
                await StopInstallationAsync();
                return false;
            }

            if (File.Exists(Path.Combine(_appState.GameDirectory, "winhttp.dll")) &&
                !_dialogService.ShowConfirm("检测到BepInEx框架（旧版本模组）！\n使用旧版本汉化模组可能遭到月亮计划的封禁！\n建议您进行一次卸载后继续安装模组。\n若您**及其确定这是个误判**，请点击\"是\"。", "警告"))
            {
                await StopInstallationAsync();
                return false;
            }

            Process[] limbusProcess = Process.GetProcessesByName("LimbusCompany");
            if (limbusProcess.Length > 0 &&
                !_dialogService.ShowConfirm("检测到 Limbus Company 仍然处于开启状态！\n建议您关闭游戏后继续安装模组。\n若您已经关闭了 Limbus Company，请点击是。", "警告"))
            {
                await StopInstallationAsync();
                return false;
            }

            return true;
        }

        private async Task StopInstallationAsync()
        {
            ResetInstallState();
            try
            {
                await _installService.StopInstallAsync();
            }
            catch (Exception ex)
            {
                Log.logger.Error("清理安装临时文件失败。", ex);
            }
        }

        private void ResetInstallState()
        {
            _installProgressSession++;
            ProgressPercentage = 0;
            IsInstalling = false;
        }

        private void LogModInstallationStatus()
        {
            try
            {
                string fontDir = Path.Combine(_appState.GameDirectory, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font");
                if (Directory.Exists(fontDir))
                    Log.logger.Info("模组已安装。");
                else
                    Log.logger.Info("模组未安装。");
            }
            catch (Exception ex)
            {
                Log.logger.Error("出现问题。" + ex.ToString());
            }
        }

        private bool CheckLimbusCompanyPath()
        {
            string? configPath = _config.Settings.General.GameDirectory;
            string limbusDir = string.Empty;

            if (!string.IsNullOrWhiteSpace(configPath))
                limbusDir = configPath;
            else
            {
                limbusDir = Microsoft.Win32.Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 1973530",
                    "InstallLocation", null) as string ?? string.Empty;
            }

            if (!IsValidLimbusDir(limbusDir))
            {
                try
                {
                    limbusDir = SteamLocator.FindLimbusCompanyPath("1973530", "LimbusCompany.exe");
                }
                catch { limbusDir = string.Empty; }
            }

            if (IsValidLimbusDir(limbusDir))
            {
                bool confirmed = _config.Settings.General.SkipGamePathConfirmation ||
                    _dialogService.ShowConfirm($"这是您的边狱公司地址吗？\n{limbusDir}", "检查路径");
                if (confirmed)
                {
                    _appState.GameDirectory = limbusDir;
                    _appState.GameExecutablePath = Path.Combine(limbusDir, "LimbusCompany.exe");
                    if (_config.Settings.General.GameDirectory != limbusDir || !_config.Settings.General.SkipGamePathConfirmation)
                    {
                        _config.Settings.General.GameDirectory = limbusDir;
                        _config.Settings.General.SkipGamePathConfirmation = true;
                        _config.Save();
                    }
                    Log.logger.Info("边狱公司路径：" + limbusDir);
                    return true;
                }
            }

            string? selected = _dialogService.ShowOpenFileDialog("请选择你的边狱公司游戏文件", "LimbusCompany.exe|LimbusCompany.exe", limbusDir);
            if (selected != null)
            {
                limbusDir = Path.GetDirectoryName(selected) ?? string.Empty;
            }

            if (!IsValidLimbusDir(limbusDir))
            {
                _dialogService.ShowMessage("选择目录有误，没有在当前目录找到游戏。", "错误");
                Application.Current.Shutdown();
                return false;
            }

            _appState.GameDirectory = limbusDir;
            _appState.GameExecutablePath = Path.Combine(limbusDir, "LimbusCompany.exe");
            _config.Settings.General.GameDirectory = limbusDir;
            _config.Settings.General.SkipGamePathConfirmation = true;
            _config.Save();
            Log.logger.Info("边狱公司路径：" + limbusDir);
            return true;
        }

        private static bool IsValidLimbusDir(string? directory)
        {
            if (string.IsNullOrWhiteSpace(directory)) return false;
            return File.Exists(Path.Combine(directory, "LimbusCompany.exe")) &&
                   File.Exists(Path.Combine(directory, "LimbusCompany_Data", "resources.assets"));
        }
    }
}
