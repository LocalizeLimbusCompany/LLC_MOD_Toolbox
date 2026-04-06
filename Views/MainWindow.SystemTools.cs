using LLC_MOD_Toolbox.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        private async Task CheckToolboxUpdate(bool isMirrorChyanMode)
        {
            try
            {
                Log.logger.Info("正在检查工具箱更新。");
                if (isMirrorChyanMode)
                {
                    await CheckToolboxUpdateWithMirrorChyan();
                }
                else
                {
                    await CheckToolboxUpdateWithoutMirrorChyan();
                }
            }
            catch (Exception ex)
            {
                Log.logger.Error("检查安装器更新出现问题。", ex);
                return;
            }
        }

        private async Task<string> GetToolboxMirrorChyanDownloadUrl()
        {
            try
            {
                string withCDKRaw = await GetURLText($"https://mirrorchyan.com/api/resources/LLC_MOD_Toolbox/latest?user_agent=LLC_MOD_Toolbox&current_version=&cdk={mirrorChyanToken}", parseErrorJson: true);
                JObject withCDK = ParseMirrorChyanJson(withCDKRaw);
                string url = withCDK["data"]["url"].Value<string>();
                return url;
            }
            catch (MirrorChyanException ex)
            {
                ErrorReportMirrorChyan(ex, false);
                Log.logger.Error("获取下载链接失败。", ex);
                return string.Empty;
            }
            catch (Exception ex)
            {
                ErrorReport(ex, false);
                Log.logger.Error("获取下载链接失败。", ex);
                return string.Empty;
            }
        }

        private async Task CheckToolboxUpdateWithMirrorChyan()
        {
            string noCDKRaw = await GetURLText("https://mirrorchyan.com/api/resources/LLC_MOD_Toolbox/latest?user_agent=LLC_MOD_Toolbox&current_version=&cdk=");
            JObject noCDKObject = ParseMirrorChyanJson(noCDKRaw);
            if (noCDKObject == null)
            {
                Log.logger.Error("获取最新版本失败。");
                return;
            }
            string latestReleaseTagRaw = noCDKObject["data"]["version_name"].Value<string>();
            string latestReleaseTag = latestReleaseTagRaw.Remove(0, 1);
            Log.logger.Info("最新安装器tag：" + latestReleaseTag);
            if (new Version(latestReleaseTag) > Assembly.GetExecutingAssembly().GetName().Version)
            {
                Log.logger.Info("安装器存在更新。");
                bool result = UniversalDialog.ShowConfirm("安装器存在更新。\n点击是下载最新版工具箱安装包并安装。\n你也可以在官网直接下载最新版。", "安装器更新", this);
                if (result)
                {
                    string installerEXE = Path.Combine(Path.GetTempPath(), "LLC_Mod_Toolbox_Installer.exe");
                    string mirrorChyanUrl = await GetToolboxMirrorChyanDownloadUrl();
                    await DownloadFileAsyncWithoutProgress(mirrorChyanUrl, installerEXE);
                    Log.logger.Info("下载完成。");
                    UniversalDialog.ShowMessage("下载完成，即将启动安装器。", "提示", null, this);
                    string batPath = CreateBatchFile(installerEXE);
                    StartBatchProcess(batPath);
                    Application.Current.Shutdown();
                    return;
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private async Task CheckToolboxUpdateWithoutMirrorChyan()
        {
            string raw = await GetURLText(string.Format(useAPIEndPoint, "v2/get_api/get/repos/LocalizeLimbusCompany/LLC_Mod_Toolbox/releases/latest"));
            var JsonObject = JObject.Parse(raw);
            string latestReleaseTagRaw = JsonObject["tag_name"].Value<string>();
            string latestReleaseTag = latestReleaseTagRaw.Remove(0, 1);
            Log.logger.Info("最新安装器tag：" + latestReleaseTag);
            if (new Version(latestReleaseTag) > Assembly.GetExecutingAssembly().GetName().Version)
            {
                Log.logger.Info("安装器存在更新。");
                bool result = UniversalDialog.ShowConfirm("安装器存在更新。\n点击是下载最新版工具箱安装包并安装。\n你也可以在官网直接下载最新版。", "安装器更新", this);
                if (result)
                {
                    Log.logger.Info("用户选择下载更新。");
                    string installerEXE = Path.Combine(Path.GetTempPath(), "LLC_Mod_Toolbox_Installer.exe");
                    await DownloadFileAsyncWithoutProgress("https://download.zeroasso.top/files/LLC_MOD_Toolbox_Installer.exe", installerEXE);
                    Log.logger.Info("下载完成。");
                    UniversalDialog.ShowMessage("下载完成，即将启动安装器。", "提示", null, this);
                    string batPath = CreateBatchFile(installerEXE);
                    StartBatchProcess(batPath);
                    Application.Current.Shutdown();
                    return;
                }
                Application.Current.Shutdown();
            }
            Log.logger.Info("没有更新。");
        }

        private static string CreateBatchFile(string targetExePath)
        {
            string currentExePath = Assembly.GetExecutingAssembly().Location;
            string batPath = Path.Combine(Path.GetTempPath(), "Cleanup_" + Guid.NewGuid() + ".bat");

            string batContent = $@"
@echo off
timeout /t 1 /nobreak >nul
start /wait """" ""{targetExePath}""
del /f /q ""{targetExePath}""
del /f /q ""{batPath}""
";

            File.WriteAllText(batPath, batContent);
            return batPath;
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

        private async Task CheckModInstalled()
        {
            try
            {
                Log.logger.Info("正在检查模组是否安装。");
                if (File.Exists(Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font")))
                {
                    Log.logger.Info("模组已安装。");
                    await ChangeAutoInstallButton();
                }
                else
                {
                    Log.logger.Info("模组未安装。");
                }
            }
            catch (Exception ex)
            {
                Log.logger.Error("出现问题。" + ex.ToString());
            }
        }

        public void CheckLCBPath()
        {
            Log.logger.Info("检查边狱公司路径。");
            if (!Path.Exists(limbusCompanyDir))
            {
                Log.logger.Error("边狱公司目录不存在。");
                FixLCBPath();
            }
            else
            {
                bool isNormalPath = true;
                if (!File.Exists(limbusCompanyDir + "\\LimbusCompany.exe"))
                {
                    isNormalPath = false;
                }
                if (!File.Exists(limbusCompanyDir + "\\LimbusCompany_Data\\resources.assets"))
                {
                    isNormalPath = false;
                }
                if (!isNormalPath)
                {
                    Log.logger.Error("边狱公司目录不正确。");
                    FixLCBPath();
                }
            }
        }

        public void FixLCBPath()
        {
            configuation.Settings.general.LCBPath = string.Empty;
            var fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "请选择你的边狱公司游戏文件，不要选择快捷方式！！！",
                Multiselect = false,
                Filter = "LimbusCompany.exe|LimbusCompany.exe",
                FileName = "LimbusCompany.exe"
            };
            if (fileDialog.ShowDialog() == true)
            {
                limbusCompanyDir = Path.GetDirectoryName(fileDialog.FileName) ?? limbusCompanyDir;
                limbusCompanyGameDir = Path.GetFullPath(fileDialog.FileName);
            }

            if (!File.Exists(limbusCompanyGameDir))
            {
                Log.logger.Error("选择了错误目录，关闭游戏。");
                UniversalDialog.ShowMessage("选择目录有误，没有在当前目录找到游戏。", "错误", null, this);
                Application.Current.Shutdown();
            }
            else
            {
                Log.logger.Info("找到了正确目录。");
                configuation.Settings.general.LCBPath = limbusCompanyDir;
                configuation.Settings.general.skipLCBPathCheck = true;
                configuation.SaveConfig();
            }
        }

        public void ChangeLCBLangConfig(string value)
        {
            try
            {
                if (File.Exists(Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "config.json")))
                {
                    string configJson = File.ReadAllText(Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "config.json"));
                    var configObject = JObject.Parse(configJson);
                    configObject["lang"] = value;
                    string newConfigJson = configObject.ToString();
                    File.WriteAllText(Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "config.json"), newConfigJson);
                }
            }
            catch (JsonReaderException ex)
            {
                bool result = UniversalDialog.ShowConfirm("配置文件出现问题，是否尝试进行修复？\n" + ex.Message, "确认删除", this);
                if (result)
                {
                    File.WriteAllText(Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "config.json"), "{\"lang\": \"\",\"titleFont\": \"\",\"contextFont\": \"\",\"samplingPointSize\": 78,\"padding\": 5}");
                    ChangeLCBLangConfig(value);
                }
            }
        }

        public string ProcessLogText(string raw)
        {
            if (string.IsNullOrWhiteSpace(mirrorChyanToken))
            {
                return raw;
            }
            string newLog = raw;
            if (raw.Contains(mirrorChyanToken))
            {
                newLog = raw.Replace(mirrorChyanToken, new string('*', mirrorChyanToken.Length));
            }
            return newLog;
        }
    }
}
