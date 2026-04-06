using LLC_MOD_Toolbox.Models;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        #region 安装功能
        /// <summary>
        /// 处理自动安装页面的安装按钮。
        /// </summary>
        private async void InstallButtonClick()
        {
            ViewModel.IsInstalling = true;
            isNewestModVersion = true;
            await RefreshPage();
            Log.logger.Info("开始安装。");
            Log.logger.Info("**********安装信息打印**********");
            Log.logger.Info("本次安装信息：");
            PrintInstallInfo("是否使用Github：", useGithub);
            PrintInstallInfo("Limbus公司目录：", limbusCompanyDir);
            PrintInstallInfo("Limbus公司游戏目录：", limbusCompanyGameDir);
            PrintInstallInfo("节点列表数量：", nodeList.Count);
            PrintInstallInfo("使用节点", useEndPoint);
            PrintInstallInfo("灰度测试状态：", greytestStatus);
            PrintInstallInfo("MirrorChyan模式：", isMirrorChyanMode);
            Log.logger.Info("**********安装信息打印**********");
            if (useEndPoint == null)
            {
                Log.logger.Warn("下载节点为空。");
            }
            installPhase = 0;
            TurnAnotherLoadingText();
            if (File.Exists(limbusCompanyDir + "/version.dll"))
            {
                Log.logger.Warn("检测到落后800年的Melonloader.");
                bool confirmed = UniversalDialog.ShowConfirm("检测到MelonLoader框架！\nMelonLoader框架已过时，且其可能导致您的账号遭到封禁，导致您无法进行游戏！\n建议您进行一次卸载后继续安装模组。\n若您**及其确定这是个误判**，请点击是，否则请点击否返回，之后您可以在设置中找到卸载，将MelonLoader卸载后重新安装。", "警告", this);
                if (!confirmed)
                {
                    await StopInstall();
                    return;
                }
                Log.logger.Warn("用户选择无视警告。");
            }
            if (File.Exists(limbusCompanyDir + "/winhttp.dll"))
            {
                Log.logger.Warn("检测到BepInEx框架.");
                bool confirmed = UniversalDialog.ShowConfirm("检测到BepInEx框架（旧版本模组）！\n使用旧版本汉化模组可能遭到月亮计划的封禁！\n建议您进行一次卸载后继续安装模组。\n若您**及其确定这是个误判**，请点击“是”。\n否则，请点击“否”停止安装，之后您可以在设置中找到卸载，将BepInEx卸载后重新安装。", "警告", this);
                if (!confirmed)
                {
                    await StopInstall();
                    return;
                }
                Log.logger.Warn("用户选择无视警告。");
            }
            Process[] limbusProcess = Process.GetProcessesByName("LimbusCompany");
            if (limbusProcess.Length > 0)
            {
                Log.logger.Warn("LimbusCompany仍然开启。");
                bool confirmed = UniversalDialog.ShowConfirm("检测到 Limbus Company 仍然处于开启状态！\n建议您关闭游戏后继续安装模组。\n若您已经关闭了 Limbus Company，请点击是，否则请点击否返回。", "警告", this);
                if (!confirmed)
                {
                    await StopInstall();
                    return;
                }
                Log.logger.Warn("用户选择无视警告。");
            }
            try
            {
                StartProgressTimer();
                if (!greytestStatus)
                {
                    if (!isMirrorChyanMode)
                    {
                        await CachedHash();
                    }
                    await InstallFont();
                    await InstallMod();
                }
                else
                {
                    await InstallGreytestMod();
                }
                ChangeLCBLangConfig("LLC_zh-CN");
            }
            catch (Exception ex)
            {
                ErrorReport(ex, true, "您可以尝试在设置中切换节点。\n");
            }
            installPhase = 0;
            Log.logger.Info("安装完成。");
            if (configuation.Settings.install.afterInstallClose || isLauncherMode)
            {
                OpenUrl("steam://rungameid/1973530");
                Application.Current.Shutdown();
                return;
            }
            bool runResult = false;
            if (isNewestModVersion)
            {
                runResult = UniversalDialog.ShowConfirm("没有检测到新版本模组！\n您的模组已经为最新。\n点击“是”立刻运行边狱公司。\n点击“否”关闭弹窗。\n加载时请耐心等待。", "提示", this);
            }
            else
            {
                runResult = UniversalDialog.ShowConfirm("安装已完成！\n点击“是”立刻运行边狱公司。\n点击“否”关闭弹窗。\n加载时请耐心等待。", "提示", this);
            }
            if (runResult)
            {
                try
                {
                    OpenUrl("steam://rungameid/1973530");
                }
                catch (Exception ex)
                {
                    Log.logger.Error("出现了问题： ", ex);
                    UniversalDialog.ShowMessage("出现了问题。\n" + ex.ToString(), "提示", null, this);
                }
            }
            hashCacheObject = null;
            ViewModel.IsInstalling = false;
            progressPercentage = 0;
            await ChangeProgressValue(0);
            await RefreshPage();
        }

        private async Task StopInstall()
        {
            ViewModel.IsInstalling = false;
            installPhase = 0;
            progressPercentage = 0;
            DeleteFile(limbusCompanyDir + "/BepInEx-IL2CPP-x64.7z");
            DeleteFile(limbusCompanyDir + "/tmpchinesefont_BIE.7z");
            DeleteFile(limbusCompanyDir + "/LimbusLocalize_BIE.7z");
            DeleteFile(limbusCompanyDir + "/LimbusLocalize_Dev.7z");
            hashCacheObject = null;
            await ChangeProgressValue(progressPercentage);
            await RefreshPage();
        }

        private async Task InstallFont()
        {
            if (isMirrorChyanMode)
            {
                await InstallFontWithMirrorChyan();
            }
            else
            {
                await InstallFontWithoutMirrorChyan();
            }
        }

        private async Task InstallFontWithMirrorChyan()
        {
            await Task.Run(async () =>
            {
                Log.logger.Info("正在安装字体文件。");
                installPhase = 1;
                string fontDir = Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context");
                Directory.CreateDirectory(fontDir);
                string fontZIPFile = Path.Combine(limbusCompanyDir, "LLCCN-Font.7z");
                string fontChinese = Path.Combine(fontDir, "ChineseFont.ttf");
                string fontBackup = Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "BackupFont", "ChineseFont.ttf.bak");
                if (File.Exists(fontChinese) || File.Exists(fontBackup))
                {
                    Log.logger.Info("检测到已安装字体文件。");
                    return;
                }
                isNewestModVersion = false;
                string url = "";
                string sha256 = "";
                (url, sha256) = await GetFontInfoWithMirrorChyan();
                if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(sha256))
                {
                    await StopInstall();
                }
                await DownloadFileAsync(url, fontZIPFile);
                if (CalculateSHA256(fontZIPFile) == sha256)
                {
                    Log.logger.Info("解压字体包中。");
                    Unarchive(fontZIPFile, limbusCompanyDir);
                    Log.logger.Info("删除字体包。");
                    File.Delete(fontZIPFile);
                }
                else
                {
                    Log.logger.Error("字体哈希校验失败。");
                    UniversalDialog.ShowMessage("校验Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "校验失败", null, this);
                    await StopInstall();
                    return;
                }
            });
        }

        private async Task<(string, string)> GetFontInfoWithMirrorChyan()
        {
            try
            {
                Log.logger.Info("获取字体MirrorChyan链接。");
                string raw = await GetURLText($"https://mirrorchyan.com/api/resources/LLCCN-Font/latest?user_agent=LLC_MOD_Toolbox&current_version=&cdk={mirrorChyanToken}", parseErrorJson: true);
                if (string.IsNullOrEmpty(raw))
                {
                    Log.logger.Error("获取字体MirrorChyan链接失败。");
                    return (string.Empty, string.Empty);
                }
                var json = ParseMirrorChyanJson(raw);
                string url = json["data"]["url"].Value<string>();
                string sha256 = json["data"]["sha256"].Value<string>();
                return (url, sha256);
            }
            catch (MirrorChyanException ex)
            {
                ErrorReportMirrorChyan(ex, false);
                return (string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                ErrorReport(ex, false);
                return (string.Empty, string.Empty);
            }
        }

        private async Task InstallFontWithoutMirrorChyan()
        {
            await Task.Run(async () =>
            {
                Log.logger.Info("正在安装字体文件。");
                installPhase = 1;
                string fontDir = Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context");
                Directory.CreateDirectory(fontDir);
                string fontZIPFile = Path.Combine(limbusCompanyDir, "LLCCN-Font.7z");
                string fontChinese = Path.Combine(fontDir, "ChineseFont.ttf");
                string fontBackup = Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "BackupFont", "ChineseFont.ttf.bak");
                if (File.Exists(fontChinese) || File.Exists(fontBackup))
                {
                    Log.logger.Info("检测到已安装字体文件。");
                    return;
                }
                if (useGithub)
                {
                    isNewestModVersion = false;
                    await DownloadFileAsync("https://raw.githubusercontent.com/LocalizeLimbusCompany/LocalizeLimbusCompany/refs/heads/main/Fonts/LLCCN-Font.7z", fontZIPFile);
                }
                else
                {
                    isNewestModVersion = false;
                    await DownloadFileAutoAsync("LLCCN-Font.7z", fontZIPFile);
                }
                if (CalculateSHA256(fontZIPFile) == hashCacheObject["font_hash"].Value<string>())
                {
                    Log.logger.Info("解压字体包中。");
                    Unarchive(fontZIPFile, limbusCompanyDir);
                    Log.logger.Info("删除字体包。");
                    File.Delete(fontZIPFile);
                }
                else
                {
                    Log.logger.Error("字体哈希校验失败。");
                    UniversalDialog.ShowMessage("校验Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "校验失败", null, this);
                    await StopInstall();
                    return;
                }
            });
        }
        #endregion
    }
}
