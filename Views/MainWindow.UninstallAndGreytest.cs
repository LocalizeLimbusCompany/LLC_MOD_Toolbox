using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        private static readonly Regex MirrorChyanKeyPattern = new("^[0-9a-z]{24}$", RegexOptions.Compiled);

        #region 卸载功能
        private async Task HandleUninstallAsync()
        {
            Log.logger.Info("点击删除模组");
            bool result = UniversalDialog.ShowConfirm("删除后你需要重新安装汉化补丁。\n确定继续吗？", "警告", this);
            if (result)
            {
                Log.logger.Info("确定删除模组。");
                try
                {
                    await DisableGlobalOperations();
                    DeleteLanguagePack();
                    DeleteBepInEx();
                    DeleteMelonLoader();
                    await EnableGlobalOperations();
                }
                catch (Exception ex)
                {
                    UniversalDialog.ShowMessage("删除过程中出现了一些问题： " + ex.ToString(), "警告", null, this);
                    Log.logger.Error("删除过程中出现了一些问题： ", ex);
                }
                UniversalDialog.ShowMessage("删除完成。", "提示", null, this);
                await CHangeFkingHomeVersion("未安装");
                Log.logger.Info("删除完成。");
            }
        }

        public static void DeleteDir(string path)
        {
            if (Directory.Exists(path))
            {
                Log.logger.Info("删除目录： " + path);
                Directory.Delete(path, true);
            }
            else
            {
                Log.logger.Info("目录不存在： " + path);
            }
        }

        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                Log.logger.Info("删除文件： " + path);
                File.Delete(path);
            }
            else
            {
                Log.logger.Info("文件不存在： " + path);
            }
        }

        public void DeleteLanguagePack()
        {
            DeleteDir(Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN"));
            ChangeLCBLangConfig("");
        }

        public static void DeleteBepInEx()
        {
            DeleteDir(limbusCompanyDir + "/BepInEx");
            DeleteDir(limbusCompanyDir + "/dotnet");
            DeleteFile(limbusCompanyDir + "/doorstop_config.ini");
            DeleteFile(limbusCompanyDir + "/Latest(框架日志).log");
            DeleteFile(limbusCompanyDir + "/Player(游戏日志).log");
            DeleteFile(limbusCompanyDir + "/winhttp.dll");
            DeleteFile(limbusCompanyDir + "/winhttp.dll.disabled");
            DeleteFile(limbusCompanyDir + "/changelog.txt");
            DeleteFile(limbusCompanyDir + "/BepInEx-IL2CPP-x64.7z");
            DeleteFile(limbusCompanyDir + "/LimbusLocalize_BIE.7z");
            DeleteFile(limbusCompanyDir + "/tmpchinese_BIE.7z");
        }

        public static void DeleteMelonLoader()
        {
            DeleteDir(limbusCompanyDir + "/MelonLoader");
            DeleteDir(limbusCompanyDir + "/Mods");
            DeleteDir(limbusCompanyDir + "/Plugins");
            DeleteDir(limbusCompanyDir + "/UserData");
            DeleteDir(limbusCompanyDir + "/UserLibs");
            DeleteFile(limbusCompanyDir + "/dobby.dll");
            DeleteFile(limbusCompanyDir + "/version.dll");
        }
        #endregion

        #region 灰度测试
        private async Task HandleGreytestStartAsync()
        {
            Log.logger.Info("Z-TECH 灰度测试客户端程序 v3.0 启动。");
            await DisableGlobalOperations();
            if (!greytestStatus)
            {
                string token = GetGreytestTokenText();
                if (token == string.Empty || token == "请输入秘钥")
                {
                    Log.logger.Info("Token为空。");
                    UniversalDialog.ShowMessage("请输入有效的Token。", "提示", null, this);
                    await EnableGlobalOperations();
                    return;
                }
                if (MirrorChyanKeyPattern.IsMatch(token))
                {
                    Log.logger.Info("检测到疑似 Mirror 酱秘钥格式。");
                    UniversalDialog.ShowMessage("不要输入你的Mirror酱秘钥。", "提示", null, this);
                    await EnableGlobalOperations();
                    return;
                }
                Log.logger.Info("Token为：" + token);
                string tokenUrl = string.Format(useAPIEndPoint, $"v2/grey_test/get_token?code={token}");
                using (HttpClient client = new())
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(tokenUrl);
                        if (response.StatusCode != HttpStatusCode.NotFound)
                        {
                            Log.logger.Info("秘钥有效。");
                        }
                        else
                        {
                            Log.logger.Info("秘钥无效。");
                            UniversalDialog.ShowMessage("请输入有效的Token。", "提示", null, this);
                            await EnableGlobalOperations();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorReport(ex, false);
                        await EnableGlobalOperations();
                        return;
                    }
                }
                try
                {
                    string tokenJson = await GetURLText(tokenUrl);
                    var tokenObject = JObject.Parse(tokenJson);
                    string runStatus = tokenObject["status"].Value<string>();
                    if (runStatus == "test")
                    {
                        Log.logger.Info("Token状态正常。");
                    }
                    else
                    {
                        Log.logger.Info("Token已停止测试。");
                        UniversalDialog.ShowMessage("Token已停止测试。", "提示", null, this);
                        await EnableGlobalOperations();
                        return;
                    }
                    string note = tokenObject["note"].Value<string>();
                    Log.logger.Info($"Token：{token}\n备注：{note}");
                    await ChangeLogoToTest();
                    UniversalDialog.ShowMessage($"目前Token有效。\n-------------\nToken信息：\n秘钥：{token}\n备注：{note}\n-------------\n灰度测试模式已开启。\n请在自动安装安装此秘钥对应版本汉化。\n秘钥信息请勿外传。", "提示", null, this);
                    greytestStatus = true;
                    greytestUrl = string.Format(useAPIEndPoint, $"v2/grey_test/get_file?code={token}");
                    await EnableGlobalOperations();
                }
                catch (Exception ex)
                {
                    ErrorReport(ex, false);
                    await EnableGlobalOperations();
                    return;
                }
            }
            else
            {
                UniversalDialog.ShowMessage("灰度测试模式已开启。\n请在自动安装安装此秘钥对应版本汉化。\n若需要正常使用或更换秘钥，请重启工具箱。", "提示", null, this);
                await EnableGlobalOperations();
                return;
            }
        }

        private string GetGreytestTokenText() => ViewModel.GreytestToken;

        private void HandleGreytestInfoRequested()
        {
            OpenUrl("https://www.zeroasso.top/docs/community/llcdev");
        }

        private async Task ChangeLogoToTest()
        {
            await this.Dispatcher.BeginInvoke(() =>
            {
                ZALogo.Visibility = Visibility.Visible;
            });
        }

        private async Task InstallGreytestMod()
        {
            await Task.Run(async () =>
            {
                Log.logger.Info("灰度测试模式已开启。开始安装灰度模组。");
                installPhase = 2;
                isNewestModVersion = false;
                await DownloadFileAsync(greytestUrl, limbusCompanyDir + "/LimbusLocalize_Dev.7z");
                Unarchive(limbusCompanyDir + "/LimbusLocalize_Dev.7z", limbusCompanyDir);
                File.Delete(limbusCompanyDir + "/LimbusLocalize_Dev.7z");
                Log.logger.Info("灰度模组安装完成。");
            });
        }
        #endregion
    }
}
