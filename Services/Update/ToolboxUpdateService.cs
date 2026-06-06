using LLC_MOD_Toolbox.Services.Network;
using LLC_MOD_Toolbox.Services.UI;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace LLC_MOD_Toolbox.Services.Update
{
    public sealed class ToolboxUpdateService : IToolboxUpdateService
    {
        private const string OfficialToolboxPackageUrl = "https://download.zeroasso.top/files/LLC_MOD_Toolbox.7z";

        private readonly AppState _appState;
        private readonly IHttpService _httpService;
        private readonly INodeService _nodeService;
        private readonly IMirrorChyanService _mirrorChyanService;

        public ToolboxUpdateService(
            AppState appState,
            IHttpService httpService,
            INodeService nodeService,
            IMirrorChyanService mirrorChyanService,
            IDialogService dialogService)
        {
            _appState = appState;
            _httpService = httpService;
            _nodeService = nodeService;
            _mirrorChyanService = mirrorChyanService;
        }

        public async Task<ToolboxUpdateCheckResult> CheckForUpdateAsync()
        {
            bool isDebug = false;
            if (isDebug)
            {
                string raw = await _httpService.GetTextAsync(
                        _nodeService.ResolveApiUrl("v2/get_api/get/repos/LocalizeLimbusCompany/LLC_Mod_Toolbox/releases/latest"),
                        reportError: false);
                var jsonObject = JObject.Parse(raw);
                string latestReleaseTag = jsonObject["tag_name"]!.Value<string>()!.TrimStart('v', 'V');
                Log.logger.Info("最新工具箱tag：" + latestReleaseTag);
                bool hasUpdate = new System.Version(latestReleaseTag) > Assembly.GetExecutingAssembly().GetName().Version;
                return new ToolboxUpdateCheckResult { HasUpdate = true, IsMirrorChyan = false, LatestVersion = latestReleaseTag };
            }
            try
            {
                Log.logger.Info("正在检查工具箱更新。");
                if (_appState.IsMirrorChyanMode)
                {
                    var (hasUpdate, latestVersion) = await _mirrorChyanService.CheckToolboxUpdateAsync();
                    return new ToolboxUpdateCheckResult { HasUpdate = hasUpdate, IsMirrorChyan = true, LatestVersion = latestVersion };
                }
                else
                {
                    string raw = await _httpService.GetTextAsync(
                        _nodeService.ResolveApiUrl("v2/get_api/get/repos/LocalizeLimbusCompany/LLC_Mod_Toolbox/releases/latest"),
                        reportError: false);
                    var jsonObject = JObject.Parse(raw);
                    string latestReleaseTag = jsonObject["tag_name"]!.Value<string>()!.TrimStart('v', 'V');
                    Log.logger.Info("最新工具箱tag：" + latestReleaseTag);
                    bool hasUpdate = new System.Version(latestReleaseTag) > Assembly.GetExecutingAssembly().GetName().Version;
                    return new ToolboxUpdateCheckResult { HasUpdate = hasUpdate, IsMirrorChyan = false, LatestVersion = latestReleaseTag };
                }
            }
            catch (Exception ex)
            {
                Log.logger.Error("检查工具箱更新出现问题。", ex);
                return new ToolboxUpdateCheckResult { Error = ex };
            }
        }

        public async Task<bool> PerformUpdateAsync(ToolboxUpdateCheckResult result, IProgress<float>? progress = null)
        {
            if (result.Error != null || !result.HasUpdate)
                return false;

            Log.logger.Info("工具箱存在更新。");
            if (progress != null)
            {
                await DownloadAndStageUpdateAsync(result, progress);
                return true;
            }

            var dialog = new ToolboxUpdateDialog(
                _appState.Version,
                result.LatestVersion,
                result.IsMirrorChyan ? "Mirror 酱源" : "官方源",
                async p => await DownloadAndStageUpdateAsync(result, p));

            Window? owner = Application.Current?.MainWindow;
            if (owner != null && owner.IsVisible)
            {
                dialog.Owner = owner;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            bool? dialogResult = dialog.ShowDialog();
            return dialogResult == true && dialog.UpdateSucceeded;
        }

        private async Task DownloadAndStageUpdateAsync(ToolboxUpdateCheckResult result, IProgress<float>? progress)
        {
            string updateDir = Path.Combine(Path.GetTempPath(), "LLC_MOD_Toolbox_Update_" + Guid.NewGuid().ToString("N"));
            string archivePath = Path.Combine(updateDir, "LLC_MOD_Toolbox.7z");
            string batPath = Path.Combine(Path.GetTempPath(), "LLC_MOD_Toolbox_Update_" + Guid.NewGuid().ToString("N") + ".bat");
            bool batchStarted = false;

            Log.logger.Info("用户选择下载更新。");
            Directory.CreateDirectory(updateDir);

            try
            {
                string downloadUrl = await ResolveToolboxPackageUrlAsync(result);
                if (string.IsNullOrWhiteSpace(downloadUrl))
                    throw new InvalidOperationException("工具箱更新包下载地址为空。");

                await _httpService.DownloadFileAsync(downloadUrl, archivePath, progress);
                if (!File.Exists(archivePath) || new FileInfo(archivePath).Length == 0)
                    throw new FileNotFoundException("工具箱更新包下载完成后未找到有效文件。", archivePath);

                string sevenZipExePath = CopyUpdaterFileToTemp("7z.exe", updateDir);
                CopyUpdaterFileToTemp("7z.dll", updateDir);

                CreateBatchFile(batPath, updateDir, archivePath, sevenZipExePath);
                StartBatchProcess(batPath);
                batchStarted = true;
                Log.logger.Info("工具箱覆盖更新脚本已启动。");
            }
            catch (Exception ex)
            {
                Log.logger.Error("准备工具箱更新失败。", ex);
                if (!batchStarted)
                    TryDeleteDirectory(updateDir);
                throw;
            }
        }

        private async Task<string> ResolveToolboxPackageUrlAsync(ToolboxUpdateCheckResult result)
        {
            return result.IsMirrorChyan
                ? await _mirrorChyanService.GetToolboxDownloadUrlAsync()
                : OfficialToolboxPackageUrl;
        }

        private string CopyUpdaterFileToTemp(string fileName, string updateDir)
        {
            string sourcePath = Path.Combine(_appState.CurrentDir, fileName);
            if (!File.Exists(sourcePath))
                throw new FileNotFoundException($"缺少自更新所需文件：{fileName}", sourcePath);

            string targetPath = Path.Combine(updateDir, fileName);
            File.Copy(sourcePath, targetPath, overwrite: true);
            return targetPath;
        }

        private void CreateBatchFile(string batPath, string updateDir, string archivePath, string sevenZipExePath)
        {
            string toolboxDir = TrimTrailingDirectorySeparator(Path.GetFullPath(_appState.CurrentDir));
            string toolboxExePath = Path.Combine(toolboxDir, "LLC_MOD_Toolbox.exe");
            int currentProcessId = Process.GetCurrentProcess().Id;
            string logPath = Path.Combine(updateDir, "update.log");

            string batContent = $"""
                @echo off
                setlocal
                set "UPDATE_DIR={EscapeBatchValue(updateDir)}"
                set "ARCHIVE={EscapeBatchValue(archivePath)}"
                set "SEVENZIP={EscapeBatchValue(sevenZipExePath)}"
                set "TOOLBOX_DIR={EscapeBatchValue(toolboxDir)}"
                set "TOOLBOX_EXE={EscapeBatchValue(toolboxExePath)}"
                set "LOG_PATH={EscapeBatchValue(logPath)}"
                set "SCRIPT_PATH=%~f0"
                set "TOOLBOX_PID={currentProcessId}"

                echo [%date% %time%] Waiting for toolbox process %TOOLBOX_PID% to exit. > "%LOG_PATH%"
                :wait_toolbox
                tasklist /FI "PID eq %TOOLBOX_PID%" 2>nul | find "%TOOLBOX_PID%" >nul
                if not errorlevel 1 (
                    timeout /t 1 /nobreak >nul
                    goto wait_toolbox
                )

                echo [%date% %time%] Extracting update package. >> "%LOG_PATH%"
                "%SEVENZIP%" x "%ARCHIVE%" -o"%TOOLBOX_DIR%" -aoa -y >> "%LOG_PATH%" 2>&1
                if errorlevel 1 (
                    echo [%date% %time%] Update extraction failed. >> "%LOG_PATH%"
                    start "" "%TOOLBOX_EXE%"
                    exit /b 1
                )

                echo [%date% %time%] Restarting toolbox. >> "%LOG_PATH%"
                start "" "%TOOLBOX_EXE%"
                timeout /t 1 /nobreak >nul
                cd /d "%TEMP%"
                rmdir /s /q "%UPDATE_DIR%" 2>nul
                del /f /q "%SCRIPT_PATH%" 2>nul
                """;
            File.WriteAllText(batPath, batContent);
        }

        private static void StartBatchProcess(string batPath)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{batPath}\"",
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            Process.Start(processInfo);
        }

        private static string EscapeBatchValue(string value)
        {
            return value.Replace("%", "%%");
        }

        private static string TrimTrailingDirectorySeparator(string path)
        {
            string root = Path.GetPathRoot(path) ?? string.Empty;
            string trimmed = path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return string.IsNullOrEmpty(trimmed) || string.Equals(trimmed, root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), StringComparison.OrdinalIgnoreCase)
                ? path
                : trimmed;
        }

        private static void TryDeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, recursive: true);
            }
            catch (Exception ex)
            {
                Log.logger.Warn($"清理更新临时目录失败：{path} - {ex.Message}");
            }
        }
    }
}
