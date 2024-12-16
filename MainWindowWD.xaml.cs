// 用于处理后端逻辑。
/*
 * You may think you know what the following code does.
 * But you dont. Trust me.
 * Fiddle with it, and youll spend many a sleepless
 * night cursing the moment you thought youd be clever
 * enough to "optimize" the code below.
 * Now close this file and go play with something else.
 * 你可能会认为你读得懂以下的代码。但是你不会懂的，相信我吧。
 * 要是你尝试玩弄这段代码的话，你将会在无尽的通宵中不断地咒骂自己为什么会认为自己聪明到可以优化这段代码。
 * 现在请关闭这个文件去玩点别的吧。
*/
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Downloader;
using LLC_MOD_Toolbox.Helpers;
using LLC_MOD_Toolbox.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SevenZip;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        private static string? useEndPoint = null;
        private static string? useAPIEndPoint = null;
        private static bool useGithub = false;
        private static bool useMirrorGithub = false;
        private static string limbusCompanyDir = FileHelper.LimbusCompanyPath ?? string.Empty;

        private static int installPhase = 0;
        private readonly DispatcherTimer progressTimer;
        private float progressPercentage = 0;

        // GreyTest 灰度测试2.0
        private static string greytestUrl = string.Empty;
        private static bool greytestStatus = false;

        /// <summary>
        /// 按某格式打印log4net，毫无意义的封装，建议弃用，尽快切换写法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="promptInfo"></param>
        /// <param name="someObject"></param>
        [Obsolete]
        private void PrintInstallInfo<T>(string promptInfo, T someObject)
        {
            if (someObject == null)
            {
                logger.LogInformation($"{promptInfo}：空");
            }
            else
            {
                logger.LogInformation($"{promptInfo}{someObject}");
            }
        }

        #region 安装功能
        /// <summary>
        /// 处理自动安装页面的安装按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private async void InstallButtonClick(object sender, RoutedEventArgs e)
        {
            logger.LogInformation("开始安装。");
            logger.LogInformation("**********安装信息打印**********");
            logger.LogInformation("本次安装信息：");
            //请勿抑制警告，尽快设计更好的处理方法
            PrintInstallInfo("是否使用Github：", useGithub);
            PrintInstallInfo("是否使用Mirror Github：", useMirrorGithub);
            PrintInstallInfo("Limbus公司目录：", limbusCompanyDir);
            PrintInstallInfo("节点列表数量：", 0);
            PrintInstallInfo("使用节点：", useEndPoint);
            PrintInstallInfo("灰度测试状态：", greytestStatus);
            logger.LogInformation("**********安装信息打印**********");
            installPhase = 0;
            Process[] limbusProcess = Process.GetProcessesByName("LimbusCompany");
            if (limbusProcess.Length > 0)
            {
                logger.LogWarning("LimbusCompany仍然开启。");
                MessageBoxResult DialogResult = MessageBox.Show(
                    "检测到 Limbus Company 仍然处于开启状态！\n建议您关闭游戏后继续安装模组。\n若您已经关闭了 Limbus Company，请点击确定，否则请点击取消返回。",
                    "警告",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Hand
                );
                if (DialogResult == MessageBoxResult.Cancel)
                {
                    return;
                }
                logger.LogWarning("用户选择无视警告。");
            }
            try
            {
                StartProgressTimer();
                await InstallBepInEx();
                if (!File.Exists(Path.Combine(limbusCompanyDir, "winhttp.dll")))
                {
                    logger.LogError("winhttp.dll不存在。");
                    System.Windows.MessageBox.Show("winhttp.dll不存在。\n请尝试关闭杀毒软件后再次安装。");
                    StopInstall();
                    return;
                }
                await InstallTMP();
                if (!greytestStatus) { }
                else
                {
                    await InstallGreytestMod();
                }
            }
            catch (Exception ex)
            {
                ErrorReport(ex, true, "您可以尝试在设置中切换节点。\n");
            }
            installPhase = 0;
            logger.LogInformation("安装完成。");
            MessageBoxResult RunResult = System.Windows.MessageBox.Show(
                "安装已完成！\n点击“确定”立刻运行边狱公司。\n点击“取消”关闭弹窗。\n加载时请耐心等待。",
                "完成",
                MessageBoxButton.OKCancel
            );
            if (RunResult == MessageBoxResult.OK)
            {
                //try
                //{
                //    HttpHelper.LaunchUrl("steam://rungameid/1973530");
                //}
                //catch (Exception ex)
                //{
                //    logger.LogError("出现了问题： ", ex);
                //    System.Windows.MessageBox.Show("出现了问题。\n" + ex.ToString());
                //}
            }
            /*            isInstalling = false;
                        progressPercentage = 0;
                        await ChangeProgressValue(0);
                        await RefreshPage();*/
        }

        private void StopInstall()
        {
            //isInstalling = false;
            installPhase = 0;
            progressPercentage = 0;
            //await ChangeProgressValue(progressPercentage);
            //await RefreshPage();
        }

        private async Task InstallBepInEx()
        {
            await Task.Run(async () =>
            {
                logger.LogInformation("已进入安装BepInEx流程。");
                installPhase = 1;
                string BepInExZipPath = Path.Combine(limbusCompanyDir, "BepInEx-IL2CPP-x64.7z");
                logger.LogInformation("BepInEx Zip目录： " + BepInExZipPath);
                if (!File.Exists(limbusCompanyDir + "/BepInEx/core/BepInEx.Core.dll"))
                {
                    System.Windows.MessageBox.Show(
                        "如果你安装了杀毒软件，接下来可能会提示工具箱正在修改关键dll。\n允许即可。如果不信任汉化补丁，可以退出本程序。",
                        "警告"
                    );
                    if (useGithub)
                    {
                        await DownloadFileAsync(
                            "https://github.com/LocalizeLimbusCompany/BepInEx_For_LLC/releases/download/v6.0.1-LLC/BepInEx-IL2CPP-x64-6.0.1.7z",
                            BepInExZipPath
                        );
                    }
                    else if (!useMirrorGithub)
                    {
                        await DownloadFileAutoAsync("BepInEx-IL2CPP-x64.7z", BepInExZipPath);
                    }
                    else if (useMirrorGithub)
                    {
                        await DownloadFileAsync(
                            "https://mirror.ghproxy.com/https://github.com/LocalizeLimbusCompany/BepInEx_For_LLC/releases/download/v6.0.1-LLC/BepInEx-IL2CPP-x64-6.0.1.7z",
                            BepInExZipPath
                        );
                    }
                    logger.LogInformation("开始解压 BepInEx zip。");
                    Unarchive(BepInExZipPath, limbusCompanyDir);
                    logger.LogInformation("解压完成。删除 BepInEx zip。");
                    File.Delete(BepInExZipPath);
                }
                else
                {
                    logger.LogInformation("检测到正确BepInEx。");
                }
            });
        }

        private async Task InstallTMP()
        {
            await Task.Run(async () =>
            {
                logger.LogInformation("已进入TMP流程。");
                installPhase = 2;
                string modsDir = limbusCompanyDir + "/BepInEx/plugins/LLC";
                Directory.CreateDirectory(modsDir);
                string tmpchineseZipPath = Path.Combine(limbusCompanyDir, "tmpchinese_BIE.7z");
                string tmpchinese = modsDir + "/tmpchinesefont";
                var LastWriteTime = File.Exists(tmpchinese)
                    ? new FileInfo(tmpchinese).LastWriteTime.ToString("yyMMdd")
                    : string.Empty;
                FontUpdateResult result;
                if (useGithub)
                {
                    result = await CheckChineseFontAssetUpdate(LastWriteTime, true);
                    await DownloadFileAsync(
                        $"https://github.com/LocalizeLimbusCompany/LLC_ChineseFontAsset/releases/download/{result.Tag}/tmpchinesefont_BIE_{result.Tag}.7z",
                        tmpchineseZipPath
                    );
                }
                else
                {
                    result = await CheckChineseFontAssetUpdate(LastWriteTime, false);
                }
                if (!result.IsNotLatestVersion)
                {
                    return;
                }
                if (!useGithub && !useMirrorGithub && result.IsNotLatestVersion)
                {
                    await DownloadFileAutoAsync("tmpchinesefont_BIE.7z", tmpchineseZipPath);
                }
                else if (result.IsNotLatestVersion)
                {
                    await DownloadFileAsync(
                        $"https://mirror.ghproxy.com/https://github.com/LocalizeLimbusCompany/LLC_ChineseFontAsset/releases/download/{result.Tag}/tmpchinesefont_BIE_{result.Tag}.7z",
                        tmpchineseZipPath
                    );
                }
                logger.LogInformation("解压 tmp zip 中。");
                Unarchive(tmpchineseZipPath, limbusCompanyDir);
                logger.LogInformation("删除 tmp zip 。");
                File.Delete(tmpchineseZipPath);
            });
        }
        #endregion
        #region 读取节点

        private void WhyShouldIUseThis(object sender, RoutedEventArgs e)
        {
            //HttpHelper.LaunchUrl("https://www.zeroasso.top/docs/configuration/nodes");
        }
        #endregion
        #region 常用方法
        public void Unarchive(string archivePath, string output)
        {
            using SevenZipExtractor extractor = new(archivePath);
            extractor.ExtractArchive(output);
        }

        /// <summary>
        /// 处理使用Downloader下载文件的事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewOnDownloadProgressChanged(
            object? sender,
            DownloadProgressChangedEventArgs e
        )
        {
            logger.LogDebug(
                "ProgressPercentage: {ProgressPercentage} ProgressPercentage(Int): {ProgressPercentageInt}",
                e.ProgressPercentage,
                (int)(e.ProgressPercentage)
            );
        }

        private void NewOnDownloadProgressCompleted(object? sender, AsyncCompletedEventArgs e)
        {
            if (installPhase != 0)
            {
                progressPercentage = installPhase * 33;
            }
        }

        /// <summary>
        /// 自动下载文件。
        /// </summary>
        /// <param name="Url">网址</param>
        /// <param name="Path">下载到的地址</param>
        /// <returns></returns>
        public async Task DownloadFileAsync(string Url, string Path)
        {
            logger.LogInformation($"下载 {Url} 到 {Path}");
            await FileHelper.DownloadFileAsync(
                Url,
                Path,
                NewOnDownloadProgressChanged,
                NewOnDownloadProgressCompleted
            );
        }

        public async Task DownloadFileAutoAsync(string File, string Path)
        {
            logger.LogInformation($"自动选择下载节点式下载文件 文件: {File}  路径: {Path}");
            if (!string.IsNullOrEmpty(useEndPoint))
            {
                string DownloadUrl = useEndPoint + File;
                await DownloadFileAsync(DownloadUrl, Path);
            }
        }

        /// <summary>
        /// 获取该网址的文本，通常用于API。
        /// </summary>
        /// <param name="Url">网址</param>
        /// <returns></returns>
        public async Task<string> GetURLText(string Url)
        {
            try
            {
                logger.LogInformation($"获取 {Url} 文本内容。");
                using HttpClient client = new();
                client.DefaultRequestHeaders.Add("User-Agent", "LLC_MOD_Toolbox");
                string raw = string.Empty;
                raw = await client.GetStringAsync(Url);
                return raw;
            }
            catch (Exception ex)
            {
                ErrorReport(ex, false);
                return string.Empty;
            }
        }

        public void ErrorReport(Exception ex, bool CloseWindow, string advice = "")
        {
            logger.LogError(ex, "出现了问题：\n{Advice}", advice);
            System.Windows.MessageBox.Show(
                $"运行中出现了问题。\n{advice}若要反馈，请带上链接或日志。\n——————————\n{ex}",
                "错误",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            if (CloseWindow)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// 检查工具箱更新
        /// </summary>
        /// <param name="version">当前版本</param>
        /// <param name="IsGithub">是否使用Github</param>
        /// <returns>是否存在更新</returns>
        [Obsolete]
        private void CheckToolboxUpdate()
        {
            try
            {
                logger.LogInformation("正在检查工具箱更新。");

                var latestReleaseTag = new Version(0, 0, 0);
                logger.LogInformation($"最新安装器tag：{latestReleaseTag}");
                if (latestReleaseTag > Assembly.GetExecutingAssembly().GetName().Version)
                {
                    logger.LogInformation("安装器存在更新。");
                    MessageBox.Show(
                        "安装器存在更新。\n点击确定进入官网下载最新版本工具箱",
                        "更新提醒",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    //HttpHelper.LaunchUrl("https://www.zeroasso.top/docs/install/autoinstall");
                    Application.Current.Shutdown();
                }
                logger.LogInformation("没有更新。");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "检查安装器更新出现问题。");
                return;
            }
        }

        public record FontUpdateResult(string? Tag, bool IsNotLatestVersion);

        /// <summary>
        /// 获取tmp字体最新标签以及是否为最新版
        /// </summary>
        /// <param name="version">当前版本</param>
        /// <param name="IsGithub">是否使用Github</param>
        /// <param name="tag">返回tmp字体tag</param>
        /// <returns>是否不是最新版</returns>
        public async Task<FontUpdateResult> CheckChineseFontAssetUpdate(
            string version,
            bool IsGithub
        )
        {
            string tag;
            try
            {
                string raw = string.Empty;
                if (IsGithub == true)
                {
                    raw = await GetURLText(
                        "https://api.github.com/repos/LocalizeLimbusCompany/LLC_ChineseFontAsset/releases/latest"
                    );
                }
                else
                {
                    raw = await GetURLText(useAPIEndPoint + "LatestTmp_Release.json");
                }
                var JsonObject = JObject.Parse(raw);
                string latestReleaseTag = JsonObject["tag_name"].Value<string>();
                if (latestReleaseTag != version)
                {
                    tag = latestReleaseTag;
                    return new FontUpdateResult(tag, true);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("出现了问题。\n" + ex.ToString());
                logger.LogError(ex, "出现了问题。\n");
            }
            return new FontUpdateResult(null, false);
        }
        #endregion
        #region 进度条系统
        public void ProgressTime_Tick(object? sender, EventArgs e)
        {
            //await ChangeProgressValue(progressPercentage);
        }

        public void StartProgressTimer()
        {
            progressPercentage = 0;
            progressTimer.Start();
        }

        public void StopProgressTimer()
        {
            progressTimer.Stop();
        }
        #endregion
        #region 卸载功能
        private void UninstallButtonClick(object sender, RoutedEventArgs e)
        {
            logger.LogInformation("点击删除模组");
            MessageBoxResult result = System.Windows.MessageBox.Show(
                "删除后你需要重新安装汉化补丁。\n确定继续吗？",
                "警告",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );
            if (result == MessageBoxResult.Yes)
            {
                logger.LogInformation("确定删除模组。");
                try
                {
                    FileHelper.DeleteBepInEx();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("删除过程中出现了一些问题： " + ex.ToString(), "警告");
                    logger.LogError("删除过程中出现了一些问题： ", ex);
                }
                System.Windows.MessageBox.Show("删除完成。", "提示");
                logger.LogInformation("删除完成。");
            }
        }

        /// <summary>
        /// 删除目录。
        /// </summary>
        /// <param name="path"></param>
        public void DeleteDir(string path)
        {
            if (Directory.Exists(path))
            {
                logger.LogInformation("删除目录： " + path);
                Directory.Delete(path, true);
            }
        }

        /// <summary>
        /// 删除文件。
        /// </summary>
        /// <param name="path"></param>
        public void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                logger.LogInformation("删除文件： " + path);
                File.Delete(path);
            }
        }
        #endregion
        #region 灰度测试
        private async void StartGreytestButtonClick(object sender, RoutedEventArgs e)
        {
            logger.LogInformation("Z-TECH 灰度测试客户端程序 v2.0 启动。（并不是");
            if (!greytestStatus)
            {
                string token = await GetGreytestBoxText();
                if (token == string.Empty || token == "请输入秘钥")
                {
                    logger.LogInformation("Token为空。");
                    System.Windows.MessageBox.Show(
                        "请输入有效的Token。",
                        "提示",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    return;
                }
                logger.LogInformation("Token为：" + token);
                string tokenUrl = $"https://dev.zeroasso.top/api/{token}.json";
                using (HttpClient client = new())
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(tokenUrl);
                        if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                        {
                            logger.LogInformation("秘钥有效。");
                        }
                        else
                        {
                            logger.LogInformation("秘钥无效。");
                            System.Windows.MessageBox.Show(
                                "请输入有效的Token。",
                                "提示",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error
                            );
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorReport(ex, false);
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
                        logger.LogInformation("Token状态正常。");
                    }
                    else
                    {
                        logger.LogInformation("Token已停止测试。");
                        System.Windows.MessageBox.Show(
                            "Token已停止测试。",
                            "提示",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                        return;
                    }
                    string fileName = tokenObject["file_name"].Value<string>();
                    string note = tokenObject["note"].Value<string>();
                    logger.LogInformation($"Token信息：{token}\n混淆文件名：{fileName}\n备注：{note}");
                    await ChangeLogoToTest();
                    System.Windows.MessageBox.Show(
                        $"目前Token有效。\n-------------\nToken信息：\n秘钥：{token}\n混淆文件名：{fileName}\n备注：{note}\n-------------\n灰度测试模式已开启。\n请在自动安装安装此秘钥对应版本汉化。\n秘钥信息请勿外传。",
                        "提示",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                    greytestStatus = true;
                    greytestUrl =
                        "https://dev.zeroasso.top/files/LimbusLocalize_Dev_" + fileName + ".7z";
                }
                catch (Exception ex)
                {
                    ErrorReport(ex, false);
                    return;
                }
            }
            else
            {
                System.Windows.MessageBox.Show(
                    "灰度测试模式已开启。\n请在自动安装安装此秘钥对应版本汉化。\n若需要正常使用或更换秘钥，请重启工具箱。",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }
        }

        private async Task<string> GetGreytestBoxText()
        {
            string? text = string.Empty;
            await this.Dispatcher.BeginInvoke(() =>
            {
                text = GreytestTokenBox.Text;
            });
            return text;
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
                logger.LogInformation("灰度测试模式已开启。开始安装灰度模组。");
                installPhase = 3;
                await DownloadFileAsync(greytestUrl, limbusCompanyDir + "/LimbusLocalize_Dev.7z");
                Unarchive($"{limbusCompanyDir}/LimbusLocalize_Dev.7z", limbusCompanyDir);
                logger.LogInformation("灰度模组安装完成。");
            });
        }
        #endregion

        #region 开关模组
        // 我也不知道为什么这个功能这么多人想要，不过既然那么多人要，那我就写了
        private void ChangeStatuButtonClick(object sender, RoutedEventArgs e)
        {
            logger.LogInformation("点击开关模组按钮。");
            if (
                !File.Exists(limbusCompanyDir + "/winhttp.dll")
                && !File.Exists(limbusCompanyDir + "/winhttp.dll.disabled")
            )
            {
                logger.LogInformation("模组未安装。");
                System.Windows.MessageBox.Show(
                    "模组未安装。",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }
            if (File.Exists(limbusCompanyDir + "/winhttp.dll.disabled"))
            {
                logger.LogInformation("检测到 winhttp.dll.disabled，进行开启。");
                File.Move(
                    limbusCompanyDir + "/winhttp.dll.disabled",
                    limbusCompanyDir + "/winhttp.dll"
                );
                System.Windows.MessageBox.Show(
                    "模组已开启。",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            else if (File.Exists(limbusCompanyDir + "/winhttp.dll"))
            {
                logger.LogInformation("检测到 winhttp.dll，进行关闭。");
                File.Move(
                    limbusCompanyDir + "/winhttp.dll",
                    limbusCompanyDir + "/winhttp.dll.disabled"
                );
                System.Windows.MessageBox.Show(
                    "模组已关闭。",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
        #endregion
        #region 抽卡模拟器
        private static bool isInitGacha = false;
        private static bool isInitGachaFailed = false;
        private static int gachaCount = 0;
        private static List<PersonalInfo> personalInfos1star = [];
        private static List<PersonalInfo> personalInfos2star = [];
        private static List<PersonalInfo> personalInfos3star = [];
        private DispatcherTimer? gachaTimer;
        private int _currentIndex = 0;
        private int[]? uniqueCount;

        private async Task InitGacha()
        {
            string gachaText = await GetURLText(
                "https://api.kr.zeroasso.top/wiki/wiki_personal.json"
            );
            if (string.IsNullOrEmpty(gachaText))
            {
                logger.LogError("初始化失败。");
                System.Windows.MessageBox.Show("初始化失败。请检查网络情况。", "提示");
                isInitGachaFailed = true;
                return;
            }

            gachaTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.15) };
            gachaTimer.Tick += GachaTimerTick;
            List<PersonalInfo> personalInfos = TranformTextToList(gachaText);
            logger.LogInformation("人格数量：" + personalInfos.Count);
            personalInfos1star = personalInfos.Where(p => p.Unique == 1).ToList();
            personalInfos2star = personalInfos.Where(p => p.Unique == 2).ToList();
            personalInfos3star = personalInfos.Where(p => p.Unique == 3).ToList();
            // 明明可以用 personalInfos.GroupBy(p => p.Unique)
            System.Windows.MessageBox.Show("初始化完成。", "提示");
            isInitGacha = true;
        }

        private async void InGachaButtonClick(object sender, RoutedEventArgs e)
        {
            logger.LogInformation("点击抽卡。");
            await CollapsedAllGacha();
            if (isInitGachaFailed)
            {
                logger.LogInformation("初始化失败。");
                System.Windows.MessageBox.Show("初始化失败，无法进行抽卡操作。", "提示");
                return;
            }
            try
            {
                List<PersonalInfo> personals = GenPersonalList();
                await StartChangeLabel(personals);
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex, "出现了问题。");
                MessageBox.Show("出了点小问题！\n要不再试一次？\n————————\n" + ex.ToString());
                gachaTimer?.Stop();
                _currentIndex = 0;
                await Dispatcher.BeginInvoke(() =>
                {
                    InGachaButton.IsHitTestVisible = true;
                });
                return;
            }
            if (gachaTimer != null)
            {
                _currentIndex = 0;
                gachaTimer.Start();
            }
        }

        private static int[] GetPersonalUniqueCount(List<PersonalInfo> personals)
        {
            int[] uniqueCount = [0, 0, 0];
            foreach (PersonalInfo personal in personals)
            {
                uniqueCount[personal.Unique - 1] += 1;
            }
            return uniqueCount;
        }

        private async Task StartChangeLabel(List<PersonalInfo> personals)
        {
            await ChangeLabelColorAndPersonal(personals[0], GachaText1);
            await ChangeLabelColorAndPersonal(personals[1], GachaText2);
            await ChangeLabelColorAndPersonal(personals[2], GachaText3);
            await ChangeLabelColorAndPersonal(personals[3], GachaText4);
            await ChangeLabelColorAndPersonal(personals[4], GachaText5);
            await ChangeLabelColorAndPersonal(personals[5], GachaText6);
            await ChangeLabelColorAndPersonal(personals[6], GachaText7);
            await ChangeLabelColorAndPersonal(personals[7], GachaText8);
            await ChangeLabelColorAndPersonal(personals[8], GachaText9);
            await ChangeLabelColorAndPersonal(personals[9], GachaText10);
        }

        private async Task ChangeLabelColorAndPersonal(
            PersonalInfo personal,
            System.Windows.Controls.Label label
        )
        {
            await this.Dispatcher.BeginInvoke(() =>
            {
                if (label.Content is TextBlock textBlock)
                {
                    switch (personal.Unique)
                    {
                        case 1:
                            textBlock.Text = "[★]" + personal.Name;
                            textBlock.Foreground = new SolidColorBrush(
                                (System.Windows.Media.Color)
                                    System.Windows.Media.ColorConverter.ConvertFromString("#B88345")
                            );
                            break;
                        case 2:
                            textBlock.Text = "[★★]" + personal.Name;
                            textBlock.Foreground = new SolidColorBrush(
                                (System.Windows.Media.Color)
                                    System.Windows.Media.ColorConverter.ConvertFromString("#CA1400")
                            );
                            break;
                        case 3:
                            textBlock.Text = "[★★★]" + personal.Name;
                            textBlock.Foreground = new SolidColorBrush(
                                (System.Windows.Media.Color)
                                    System.Windows.Media.ColorConverter.ConvertFromString("#FCC404")
                            );
                            break;
                    }
                }
            });
        }

        private List<PersonalInfo> GenPersonalList()
        {
            Random random = new();
            List<PersonalInfo> genPersonalInfos = [];
            for (int i = 0; i < 10; i++) // 循环十次
            {
                int chance = random.Next(1, 101);
                if (i != 9)
                {
                    if (chance <= 84) // 一星
                    {
                        int randomIndex = random.Next(personalInfos1star.Count);
                        genPersonalInfos.Add(personalInfos1star[randomIndex]);
                    }
                    else if (chance <= 97) // 二星
                    {
                        int randomIndex = random.Next(personalInfos2star.Count);
                        genPersonalInfos.Add(personalInfos2star[randomIndex]);
                    }
                    else // 三星
                    {
                        int randomIndex = random.Next(personalInfos3star.Count);
                        genPersonalInfos.Add(personalInfos3star[randomIndex]);
                    }
                }
                else
                {
                    if (chance <= 84) // 二星
                    {
                        int randomIndex = random.Next(personalInfos2star.Count);
                        genPersonalInfos.Add(personalInfos2star[randomIndex]);
                    }
                    else if (chance <= 97) // 三星
                    {
                        int randomIndex = random.Next(personalInfos3star.Count);
                        genPersonalInfos.Add(personalInfos3star[randomIndex]);
                    }
                }
            }
            uniqueCount = GetPersonalUniqueCount(genPersonalInfos);
            return genPersonalInfos;
        }

        private List<PersonalInfo> TranformTextToList(string gachaText)
        {
            logger.LogInformation("开始转换文本。");
            var gachaObject = JObject.Parse(gachaText);
            List<PersonalInfo> personalInfoList = [];
            for (int i = 0; i < gachaObject["data"].Count(); i++)
            {
                PersonalInfo personalInfo =
                    new(
                        BeautifyText(
                            gachaObject["data"][i][0].Value<string>(),
                            gachaObject["data"][i][1].Value<string>()
                        ),
                        gachaObject["data"][i][7].Value<int>()
                    );
                personalInfoList.Add(personalInfo);
            }
            return personalInfoList;
        }

        private static string BeautifyText(string input, string prefix)
        {
            if (input.StartsWith(prefix))
            {
                string title = input[prefix.Length..];
                return $"{title} {prefix}";
            }
            else
            {
                return input;
            }
        }

        private async void GachaTimerTick(object? sender, EventArgs? e)
        {
            if (_currentIndex < 10)
            {
                var label = (System.Windows.Controls.Label)
                    this.FindName($"GachaText{_currentIndex + 1}");
                await this.Dispatcher.BeginInvoke(() =>
                {
                    label.Visibility = Visibility.Visible;
                });
                _currentIndex++;
            }
            else if (gachaTimer != null)
            {
                gachaTimer.Stop();
                await this.Dispatcher.BeginInvoke(() =>
                {
                    InGachaButton.IsHitTestVisible = true;
                });
                Random random = new();
                gachaCount += 1;
                switch (gachaCount)
                {
                    case 10:
                        System.Windows.MessageBox.Show("你已经抽了100抽了，你上头了？", "提示");
                        return;
                    case 20:
                        System.Windows.MessageBox.Show("恭喜你，你已经抽了一个井了！\n珍爱生命，远离抽卡啊亲！", "提示");
                        return;
                    case 40:
                        System.Windows.MessageBox.Show("两个井了，你算算已经砸了多少狂气了？", "提示");
                        return;
                    case 60:
                        System.Windows.MessageBox.Show(
                            "收手吧！你不算砸了多少狂气我算了！\n你已经砸了60x1300=78000狂气了！",
                            "提示"
                        );
                        return;
                    case 100:
                        System.Windows.MessageBox.Show(
                            "我是来恭喜你，你已经扔进去1000抽，简称130000狂气了。\n你花了多少时间到这里？",
                            "提示"
                        );
                        return;
                }
                if (uniqueCount == null)
                {
                    System.Windows.MessageBox.Show("抽卡完成。", "提示");
                    return;
                }
                else if (uniqueCount[0] == 9 && uniqueCount[1] == 1)
                {
                    int choice = random.Next(1, 4);
                    if (choice == 1)
                    {
                        System.Windows.MessageBox.Show("恭喜九白一红~！", "提示");
                    }
                    else if (choice == 2)
                    {
                        System.Windows.MessageBox.Show("正常发挥正常发挥~", "提示");
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("还好没拿真狂气抽吧！", "提示");
                    }
                }
                else if (uniqueCount[0] == 8 && uniqueCount[1] == 2)
                {
                    int choice = random.Next(1, 4);
                    if (choice == 1)
                    {
                        System.Windows.MessageBox.Show("至少比九白一红好一点，不是么？", "提示");
                    }
                    else if (choice == 2)
                    {
                        System.Windows.MessageBox.Show("你要不先去洗洗手？", "提示");
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("真是可惜，看来这次运气没有站在你这边.jpg", "提示");
                    }
                }
                else if (uniqueCount[0] == 7 && uniqueCount[1] == 3)
                {
                    int choice = random.Next(1, 4);
                    if (choice == 1)
                    {
                        System.Windows.MessageBox.Show("三个二星！这是多少碎片来着？", "提示");
                    }
                    else if (choice == 2)
                    {
                        System.Windows.MessageBox.Show("工具箱的概率可是十分严谨的！\n所以肯定不是工具箱的问题！", "提示");
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("要是抽不中就算了吧，散伙散伙！", "提示");
                    }
                }
                else if (uniqueCount[2] == 1)
                {
                    int choice = random.Next(1, 4);
                    if (choice == 1)
                    {
                        System.Windows.MessageBox.Show("金色传说！虽然说就一个。", "提示");
                    }
                    else if (choice == 2)
                    {
                        System.Windows.MessageBox.Show("恭喜恭喜~不知道抽了多少次了？", "提示");
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("ALL IN！", "提示");
                    }
                }
                else if (uniqueCount[2] == 2)
                {
                    int choice = random.Next(1, 4);
                    if (choice == 1)
                    {
                        System.Windows.MessageBox.Show("双黄蛋？希望你瓦夜的时候也能这样。", "提示");
                    }
                    else if (choice == 2)
                    {
                        System.Windows.MessageBox.Show("100碎片而已，我一点都不羡慕！", "提示");
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("恭喜恭喜~", "提示");
                    }
                }
                else if (uniqueCount[2] == 3)
                {
                    int choice = random.Next(1, 4);
                    if (choice == 1)
                    {
                        System.Windows.MessageBox.Show("真的假的三黄。。？", "提示");
                    }
                    else if (choice == 2)
                    {
                        System.Windows.MessageBox.Show("你平时运气也这么好？！", "提示");
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("爽了，再来再来！", "提示");
                    }
                }
                else if (uniqueCount[2] >= 4)
                {
                    int choice = random.Next(1, 4);
                    switch (choice)
                    {
                        case 1:
                            System.Windows.MessageBox.Show("不可能……不可能啊？！", "提示");
                            break;
                        case 2:
                            System.Windows.MessageBox.Show("欧吃矛！", "提示");
                            break;
                        case 3:
                            System.Windows.MessageBox.Show("再抽池子就要空了！", "提示");
                            break;
                    }
                }
                else
                {
                    int choice = random.Next(1, 4);
                    if (choice == 1)
                    {
                        System.Windows.MessageBox.Show("怎么样？再来一次么？", "提示");
                    }
                    else if (choice == 2)
                    {
                        System.Windows.MessageBox.Show("冷知识：概率真的完全真实。", "提示");
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("你平时抽卡也这个结果吗？", "提示");
                    }
                }
            }
        }

        private async Task CollapsedAllGacha()
        {
            await this.Dispatcher.BeginInvoke(() =>
            {
                GachaText1.Visibility = Visibility.Collapsed;
                GachaText2.Visibility = Visibility.Collapsed;
                GachaText3.Visibility = Visibility.Collapsed;
                GachaText4.Visibility = Visibility.Collapsed;
                GachaText5.Visibility = Visibility.Collapsed;
                GachaText6.Visibility = Visibility.Collapsed;
                GachaText7.Visibility = Visibility.Collapsed;
                GachaText8.Visibility = Visibility.Collapsed;
                GachaText9.Visibility = Visibility.Collapsed;
                GachaText10.Visibility = Visibility.Collapsed;
                InGachaButton.IsHitTestVisible = false;
            });
        }
        #endregion
    }
}
