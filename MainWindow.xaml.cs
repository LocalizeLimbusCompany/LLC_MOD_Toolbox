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
using Downloader;
using LLC_MOD_Toolbox.Datas;
using log4net;
using Microsoft.Win32;
using Newtonsoft.Json;
using SevenZip;
using SimpleJSON;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Threading;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        private static string useEndPoint = string.Empty;
        private static bool useGithub = false;
        private static bool useMirrorGithub = false;
        private static string limbusCompanyDir = string.Empty;
        private static string limbusCompanyGameDir = string.Empty;
        private static string currentDir = AppDomain.CurrentDomain.BaseDirectory;
        private static List<Node> nodeList = [];
        private static string defaultEndPoint = "https://node.zeroasso.top/d/od/";
        private static int installPhase = 0;
        private readonly DispatcherTimer progressTimer;
        private float progressPercentage = 0;
        // GreyTest 灰度测试2.0
        private static string greytestUrl = string.Empty;
        private static bool greytestStatus = false;
        private const string VERSION = "1.0.0";
        public MainWindow()
        {
            InitializeComponent();
            progressTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.05)
            };
            progressTimer.Tick += ProgressTime_Tick;
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            logger.Info("—————新日志分割线—————");
            logger.Info("工具箱已进入加载流程。");
            logger.Info("We have a lift off.");
            logger.Info("WPF架构工具箱 版本：" + VERSION + " 。");
            await RefreshPage();
            await ChangeEEPic("https://dl.kr.zeroasso.top/ee_pic/public/public.png");
            CheckToolboxUpdate(VERSION);
            InitNode();
            LoadConfig();
            InitLink();
            CheckLimbusCompanyPath();
            SevenZipBase.SetLibraryPath(Path.Combine(currentDir, "7z.dll"));
            logger.Info("加载流程完成。");
        }
        private static void PrintInstallInfo(string promptInfo, int intObject)
        {
            if (useEndPoint != null)
            {
                logger.Info(promptInfo + "：" + intObject);
            }
            else
            {
                logger.Info(promptInfo + "：空");
            }
        }
        private static void PrintInstallInfo(string promptInfo, string stringObject)
        {
            if (!string.IsNullOrEmpty(useEndPoint))
            {
                logger.Info(promptInfo + "：" + stringObject);
            }
            else
            {
                logger.Info(promptInfo + "：空");
            }
        }
        private static void PrintInstallInfo(string promptInfo, bool boolObject)
        {
            if (boolObject)
            {
                logger.Info(promptInfo + "：True");
            }
            else
            {
                logger.Info(promptInfo + "：False");
            }
        }
        #region 安装功能
        /// <summary>
        /// 处理自动安装页面的安装按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void InstallButtonClick(object sender, RoutedEventArgs e)
        {
            isInstalling = true;
            await RefreshPage();
            logger.Info("开始安装。");
            logger.Info("**********安装信息打印**********");
            logger.Info("本次安装信息：");
            PrintInstallInfo("是否使用Github：", useGithub);
            PrintInstallInfo("是否使用Mirror Github：", useMirrorGithub);
            PrintInstallInfo("Limbus公司目录：", limbusCompanyDir);
            PrintInstallInfo("Limbus公司游戏目录：", limbusCompanyGameDir);
            PrintInstallInfo("节点列表数量：", nodeList.Count);
            PrintInstallInfo("使用节点：", useEndPoint);
            PrintInstallInfo("灰度测试状态：", greytestStatus);
            logger.Info("**********安装信息打印**********");
            installPhase = 0;
            Process[] limbusProcess = Process.GetProcessesByName("LimbusCompany");
            if (limbusProcess.Length > 0)
            {
                logger.Warn("LimbusCompany仍然开启。");
                MessageBoxResult DialogResult = System.Windows.MessageBox.Show("检测到 Limbus Company 仍然处于开启状态！\n建议您关闭游戏后继续安装模组。\n若您已经关闭了 Limbus Company，请点击确定，否则请点击取消返回。", "警告", MessageBoxButton.OKCancel, MessageBoxImage.Hand);
                if (DialogResult == MessageBoxResult.Cancel)
                {
                    return;
                }
                logger.Warn("用户选择无视警告。");
            }
            try
            {
                StartProgressTimer();
                await InstallBepInEx();
                if (!File.Exists(Path.Combine(limbusCompanyDir, "winhttp.dll")))
                {
                    logger.Error("winhttp.dll不存在。");
                    System.Windows.MessageBox.Show("winhttp.dll不存在。\n请尝试关闭杀毒软件后再次安装。");
                    await StopInstall();
                    return;
                }
                await InstallTMP();
                if (!greytestStatus)
                {
                    await InstallMod();
                }
                else
                {
                    await InstallGreytestMod();
                }
            }
            catch (Exception ex)
            {
                ErrorReport(ex, true);
            }
            installPhase = 0;
            logger.Info("安装完成。");
            MessageBoxResult RunResult = System.Windows.MessageBox.Show("安装已完成！\n点击“确定”立刻运行边狱公司。\n点击“取消”关闭弹窗。\n加载时请耐心等待。", "完成", MessageBoxButton.OKCancel);
            if (RunResult == MessageBoxResult.OK)
            {
                try
                {
                    OpenUrl("steam://rungameid/1973530");
                }
                catch (Exception ex)
                {
                    logger.Error("出现了问题： " + ex.ToString());
                    System.Windows.MessageBox.Show("出现了问题。\n" + ex.ToString());
                }
            }
            isInstalling = false;
            progressPercentage = 0;
            await ChangeProgressValue(0);
            await RefreshPage();
        }
        private async Task StopInstall()
        {
            isInstalling = false;
            installPhase = 0;
            progressPercentage = 0;
            await ChangeProgressValue(progressPercentage);
            await RefreshPage();
        }
        private async Task InstallBepInEx()
        {
            await Task.Run(async () =>
            {
                logger.Info("已进入安装BepInEx流程。");
                installPhase = 1;
                string BepInExZipPath = Path.Combine(limbusCompanyDir, "BepInEx-IL2CPP-x64.7z");
                logger.Info("BepInEx Zip目录： " + BepInExZipPath);
                if (!File.Exists(limbusCompanyDir + "/BepInEx/core/BepInEx.Core.dll"))
                {
                    System.Windows.MessageBox.Show("如果你安装了杀毒软件，接下来可能会提示工具箱正在修改关键dll。\n允许即可。如果不信任汉化补丁，可以退出本程序。", "警告");
                    if (useGithub)
                    {
                        await DownloadFileAsync("https://github.com/LocalizeLimbusCompany/BepInEx_For_LLC/releases/download/v6.0.1-LLC/BepInEx-IL2CPP-x64-6.0.1.7z", BepInExZipPath);
                    }
                    else if (!useMirrorGithub)
                    {
                        await DownloadFileAutoAsync("BepInEx-IL2CPP-x64.7z", BepInExZipPath);
                    }
                    else if (useMirrorGithub)
                    {
                        await DownloadFileAsync("https://mirror.ghproxy.com/https://github.com/LocalizeLimbusCompany/BepInEx_For_LLC/releases/download/v6.0.1-LLC/BepInEx-IL2CPP-x64-6.0.1.7z", BepInExZipPath);
                    }
                    logger.Info("开始解压 BepInEx zip。");
                    Unarchive(BepInExZipPath, limbusCompanyDir);
                    logger.Info("解压完成。删除 BepInEx zip。");
                    File.Delete(BepInExZipPath);
                }
                else
                {
                    logger.Info("检测到正确BepInEx。");
                }
            });
        }
        private async Task InstallTMP()
        {
            await Task.Run(async () =>
            {
                logger.Info("已进入TMP流程。");
                installPhase = 2;
                string modsDir = limbusCompanyDir + "/BepInEx/plugins/LLC";
                Directory.CreateDirectory(modsDir);
                string tmpchineseZipPath = Path.Combine(limbusCompanyDir, "tmpchinese_BIE.7z");
                string tmpchinese = modsDir + "/tmpchinesefont";
                var LastWriteTime = File.Exists(tmpchinese) ? new FileInfo(tmpchinese).LastWriteTime.ToString("yyMMdd") : string.Empty;
                FontUpdateResult result = new();
                if (useGithub)
                {
                    result = await CheckChineseFontAssetUpdate(LastWriteTime, true);
                    await DownloadFileAsync("https://github.com/LocalizeLimbusCompany/LLC_ChineseFontAsset/releases/download/" + result.Tag + "/tmpchinesefont_BIE_" + result.Tag + ".7z", tmpchineseZipPath);
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
                    await DownloadFileAsync("https://mirror.ghproxy.com/https://github.com/LocalizeLimbusCompany/LLC_ChineseFontAsset/releases/download/" + result.Tag + "/tmpchinesefont_BIE_" + result.Tag + ".7z", tmpchineseZipPath);
                }
                logger.Info("解压 tmp zip 中。");
                Unarchive(tmpchineseZipPath, limbusCompanyDir);
                logger.Info("删除 tmp zip 。");
                File.Delete(tmpchineseZipPath);
            });
        }
        private async Task InstallMod()
        {
            await Task.Run(async () =>
            {
                logger.Info("开始安装模组。");
                installPhase = 3;
                string modsDir = limbusCompanyDir + "/BepInEx/plugins/LLC";
                string limbusLocalizeDllPath = modsDir + "/LimbusLocalize_BIE.dll";
                string limbusLocalizeZipPath = Path.Combine(limbusCompanyDir, "LimbusLocalize_BIE.7z");
                string latestLLCVersion = string.Empty;
                string currentVersion = string.Empty;
                if (useGithub)
                {
                    latestLLCVersion = await GetLatestLimbusLocalizeVersion(true);
                    logger.Info("最后模组版本： " + latestLLCVersion);
                    if (File.Exists(limbusLocalizeDllPath))
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(limbusLocalizeDllPath);
                        currentVersion = versionInfo.ProductVersion;
                        if (string.IsNullOrEmpty(currentVersion))
                        {
                            logger.Info("模组版本获取失败");
                            System.Windows.MessageBox.Show("模组版本获取失败，请尝试卸载后重新安装。\n如果问题仍然出现，请进行反馈。", "获取失败");
                            await StopInstall();
                        }
                        else if (new Version(currentVersion) >= new Version(latestLLCVersion.Remove(0, 1)))
                        {
                            logger.Info("模组无需更新。");
                            return;
                        }
                    }
                    else
                    {
                        logger.Info("模组不存在。进行安装。");
                    }
                    await DownloadFileAsync("https://github.com/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/download/" + latestLLCVersion + "/LimbusLocalize_BIE_" + latestLLCVersion + ".7z", limbusLocalizeZipPath);
                    logger.Info("解压模组本体 zip 中。");
                    Unarchive(limbusLocalizeZipPath, limbusCompanyDir);
                    logger.Info("删除模组本体 zip 。");
                    File.Delete(limbusLocalizeZipPath);
                    return;
                }
                if (!useMirrorGithub)
                {
                    latestLLCVersion = await GetLatestLimbusLocalizeVersion(false);
                    logger.Info("最后模组版本： " + latestLLCVersion);
                    if (File.Exists(limbusLocalizeDllPath))
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(limbusLocalizeDllPath);
                        currentVersion = versionInfo.ProductVersion;
                        if (string.IsNullOrEmpty(currentVersion))
                        {
                            logger.Info("模组版本获取失败");
                            System.Windows.MessageBox.Show("模组版本获取失败，请尝试卸载后重新安装。\n如果问题仍然出现，请进行反馈。", "获取失败");
                            await StopInstall();
                        }
                        else if (new System.Version(currentVersion) >= new System.Version(latestLLCVersion.Remove(0, 1)))
                        {
                            logger.Info("模组无需更新。");
                            return;
                        }
                    }
                    else
                    {
                        logger.Info("模组不存在。进行安装。");
                    }
                    await DownloadFileAutoAsync("LimbusLocalize_BIE_" + latestLLCVersion + ".7z", limbusLocalizeZipPath);
                    if (await GetLimbusLocalizeHash() != CalculateSHA256(limbusLocalizeZipPath))
                    {
                        logger.Error("校验Hash失败。");
                        System.Windows.MessageBox.Show("校验Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "校验失败");
                        await StopInstall();
                    }
                    else
                    {
                        logger.Info("校验Hash成功。");
                    }
                    logger.Info("解压模组本体 zip 中。");
                    Unarchive(limbusLocalizeZipPath, limbusCompanyDir);
                    logger.Info("删除模组本体 zip 。");
                    File.Delete(limbusLocalizeZipPath);
                }
                else if (useMirrorGithub)
                {
                    latestLLCVersion = await GetLatestLimbusLocalizeVersion(false);
                    logger.Info("最后模组版本： " + latestLLCVersion);
                    if (File.Exists(limbusLocalizeDllPath))
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(limbusLocalizeDllPath);
                        currentVersion = versionInfo.ProductVersion;
                        if (string.IsNullOrEmpty(currentVersion))
                        {
                            logger.Info("模组版本获取失败");
                            System.Windows.MessageBox.Show("模组版本获取失败，请尝试卸载后重新安装。\n如果问题仍然出现，请进行反馈。", "获取失败");
                            await StopInstall();
                        }
                        else if (new System.Version(currentVersion) >= new System.Version(latestLLCVersion.Remove(0, 1)))
                        {
                            logger.Info("无需更新。");
                            return;
                        }
                    }
                    await DownloadFileAsync("https://mirror.ghproxy.com/https://github.com/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/download/" + latestLLCVersion + "/LimbusLocalize_BIE_" + latestLLCVersion + ".7z", limbusLocalizeZipPath);
                    logger.Info("解压模组本体 zip 中。");
                    Unarchive(limbusLocalizeZipPath, limbusCompanyDir);
                    logger.Info("删除模组本体 zip 。");
                    File.Delete(limbusLocalizeZipPath);
                }
            });
        }
        #endregion
        #region 读取节点
        public class Node
        {
            public required string Name { get; set; }
            public required string Endpoint { get; set; }
            public required bool IsDefault { get; set; }
        }

        public void InitNode()
        {
            ReadNodeJsonFile();
            NodeCombobox.Items.Add("恢复默认");
            foreach (var Node in nodeList)
            {
                if (Node.IsDefault == true)
                {
                    defaultEndPoint = Node.Endpoint;
                }
                NodeCombobox.Items.Add(Node.Name);
            }
            NodeCombobox.Items.Add("Github直连");
            NodeCombobox.Items.Add("Mirror Github");
        }
        public static void ReadNodeJsonFile()
        {
            string nodeListRaw = File.ReadAllText(currentDir + "/nodeList.json");
            nodeList = JsonConvert.DeserializeObject<List<Node>>(nodeListRaw) ?? new List<Node>();
        }
        private static string FindEndpoint(string Name)
        {
            foreach (var Node in nodeList)
            {
                if (Node.Name == Name)
                {
                    return Node.Endpoint;
                }
            }
            return string.Empty;
        }
        public async Task<string> GetNodeComboboxText()
        {
            string combotext = string.Empty;
            await this.Dispatcher.BeginInvoke(() =>
            {
                combotext = NodeCombobox.SelectedItem.ToString();
            });
            return combotext;
        }
        private async void NodeComboboxSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string nodeComboboxText = await GetNodeComboboxText();
            logger.Info("选择节点。");
            if (nodeComboboxText != string.Empty)
            {
                if (nodeComboboxText == "恢复默认")
                {
                    useEndPoint = string.Empty;
                    useMirrorGithub = false;
                    useGithub = false;
                    logger.Info("已恢复默认Endpoint。");
                }
                else if (nodeComboboxText == "Github直连")
                {
                    logger.Info("选择Github节点。");
                    System.Windows.MessageBox.Show("如果您没有使用代理软件（包括Watt Toolkit）\n请不要使用此节点。\nGithub由于不可抗力因素，对国内网络十分不友好。\n如果您是国外用户，才应该使用此选项。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    useEndPoint = string.Empty;
                    useGithub = true;
                    useMirrorGithub = false;
                }
                else if (nodeComboboxText == "Mirror Github")
                {
                    logger.Info("选择镜像Github节点。");
                    System.Windows.MessageBox.Show("Mirror Github服务由【mirror.ghproxy.com】提供。\n零协会不对其可能造成的任何问题（包括不可使用，安全性相关）负责。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    useMirrorGithub = true;
                    useGithub = false;
                }
                else
                {
                    useEndPoint = FindEndpoint(nodeComboboxText);
                    useMirrorGithub = false;
                    useGithub = false;
                    logger.Info("当前Endpoint：" + useEndPoint);
                    System.Windows.MessageBox.Show("切换成功。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                logger.Info("NodeComboboxText 为 null。");
            }
        }
        #endregion
        #region 常用方法
        public static void Unarchive(string archivePath, string output)
        {
            using (SevenZipExtractor extractor = new SevenZipExtractor(archivePath))
            {
                extractor.ExtractArchive(output);
            }
        }
        private static void CheckLimbusCompanyPath()
        {
            if (skipLCBPathCheck && !string.IsNullOrEmpty(LCBPath))
            {
                limbusCompanyDir = LCBPath;
                logger.Info("跳过检查路径。");
            }
            else
            {
#pragma warning disable CS8601 // 引用类型赋值可能为 null。
                limbusCompanyDir = FindlimbusCompanyDirectory();
#pragma warning restore CS8601 // 引用类型赋值可能为 null。
                MessageBoxResult CheckLCBPathResult = MessageBoxResult.OK;
                if (!string.IsNullOrEmpty(limbusCompanyDir))
                {
                    CheckLCBPathResult = System.Windows.MessageBox.Show("这是您的边狱公司地址吗？\n" + limbusCompanyDir, "检查路径", MessageBoxButton.YesNo, MessageBoxImage.Question);
                }
                if (CheckLCBPathResult == MessageBoxResult.Yes)
                {
                    logger.Info("用户确认路径。");
                    ChangeLCBPathConfig(limbusCompanyDir);
                    ChangeSkipPathCheckConfig(true);
                }
                if (string.IsNullOrEmpty(limbusCompanyDir) || CheckLCBPathResult == MessageBoxResult.No)
                {
                    if (string.IsNullOrEmpty(limbusCompanyDir))
                    {
                        logger.Error("未能找到 Limbus Company 目录，手动选择模式。");
                        System.Windows.MessageBox.Show("未能找到 Limbus Company 目录。请手动选择。", "提示");
                    }
                    else
                    {
                        logger.Error("用户否认 Limbus Company 目录正确性。");
                    }
                    using (var folderDialog = new FolderBrowserDialog())
                    {
                        folderDialog.Description = "请选择你的边狱公司游戏路径（steam目录/steamapps/common/Limbus Company）请不要选择LimbusCompany_Data！";
                        folderDialog.ShowNewFolderButton = true;

                        DialogResult result = folderDialog.ShowDialog();
                        if (result == System.Windows.Forms.DialogResult.OK)
                        {
                            string selectedPath = folderDialog.SelectedPath;
                            limbusCompanyDir = selectedPath;
                        }
                    }
                    limbusCompanyGameDir = limbusCompanyDir + "/LimbusCompany.exe";
                    if (!File.Exists(limbusCompanyGameDir))
                    {
                        logger.Error("选择了错误目录，关闭游戏。");
                        System.Windows.MessageBox.Show("选择目录有误，没有在当前目录找到游戏。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        System.Windows.Application.Current.Shutdown();
                    }
                    else
                    {
                        logger.Info("找到了正确目录。");
                        ChangeSkipPathCheckConfig(true);
                        ChangeLCBPathConfig(limbusCompanyDir);
                    }
                }
            }
            limbusCompanyGameDir = limbusCompanyDir + "/LimbusCompany.exe";
            logger.Info("边狱公司路径：" + limbusCompanyDir);
        }
        /// <summary>
        /// 获取最新版汉化模组哈希
        /// </summary>
        /// <returns>返回Sha256</returns>
        private static async Task<string> GetLimbusLocalizeHash()
        {
            string HashRaw;
            if (useEndPoint != string.Empty)
            {
                HashRaw = await GetURLText(useEndPoint + "LimbusLocalizeHash.json");
            }
            else
            {
                HashRaw = await GetURLText(defaultEndPoint + "LimbusLocalizeHash.json");
            }
            dynamic JsonObject = JsonConvert.DeserializeObject(HashRaw);
            if (JsonObject == null)
            {
                logger.Error("获取模组Hash失败。");
                throw new Exception("获取Hash失败。");
            }
            string Hash = JsonObject.hash;
            logger.Info("获取到的最新Hash为：" + Hash);
            return Hash;
        }
        /// <summary>
        /// 计算文件Sha256
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <returns>返回Sha256</returns>
        public static string CalculateSHA256(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var fileStream = File.OpenRead(filePath);
            byte[] hashBytes = sha256.ComputeHash(fileStream);
            logger.Info("计算位置为 " + filePath + " 的文件的Hash结果为：" + BitConverter.ToString(hashBytes).Replace("-", "").ToLower());
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
        /// <summary>
        /// 获取LCB路径
        /// </summary>
        /// <returns>String 路径</returns>
        private static string? FindlimbusCompanyDirectory()
        {
            logger.Info("使用自动查找边狱公司方法。");
            if (!string.IsNullOrEmpty(LCBPath))
            {
                logger.Info("找到了之前获取的路径，检查可用性。");
                if (File.Exists(Path.Combine(LCBPath + @"\LimbusCompany.exe")))
                {
                    logger.Info("路径可用，返回路径。");
                    return LCBPath;
                }
                else
                {
                    logger.Info("路径不可用，重新进行查找。");
                }
            }
            string? steamPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null) as string;
            if (steamPath == null) return null;
            string libraryFoldersPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
            //查找libraryfolders.vdf，中""1973530"\t\t"，并向上找到最近的path
            try {
                return Path.Combine(
                    File.ReadAllLines(libraryFoldersPath).Reverse()
                    .SkipWhile(n => !n.Contains($"\"{Constants.GAME_APPID}\"\t\t"))
                    .First(n => n.Contains("path"))
                    .Split('"')[^2].Replace(@"\\", @"\"),
                "steamapps","common", "Limbus Company");
            }
            catch (Exception ex)
            {
                logger.Warn(ex+"：游戏未下载或未Steam");
                return null;
            }
        }
        /// <summary>
        /// 处理使用Downloader下载文件的事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewOnDownloadProgressChanged(object? sender, Downloader.DownloadProgressChangedEventArgs e)
        {
            logger.Debug("ProgressPercentage: " + e.ProgressPercentage + " ProgressPercentage(Int): " + (int)(e.ProgressPercentage));
            if (installPhase != 0)
            {
                progressPercentage = (float)((installPhase - 1) * 33 + e.ProgressPercentage * 0.33);
            }
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
            logger.Info("下载 " + Url + " 到 " + Path);
            var downloadOpt = new DownloadConfiguration()
            {
                BufferBlockSize = 10240,
                ChunkCount = 8,
                MaxTryAgainOnFailover = 5,
            };
            var downloader = new DownloadService(downloadOpt);
            downloader.DownloadProgressChanged += NewOnDownloadProgressChanged;
            downloader.DownloadFileCompleted += NewOnDownloadProgressCompleted;
            await downloader.DownloadFileTaskAsync(Url, Path);
        }
        public async Task DownloadFileAutoAsync(string File, string Path)
        {
            logger.Info("自动选择下载节点式下载文件 文件: " + File + "  路径: " + Path);
            if (useEndPoint != string.Empty)
            {
                string DownloadUrl = useEndPoint + File;
                await DownloadFileAsync(DownloadUrl, Path);
            }
            else
            {
                string DownloadUrl = defaultEndPoint + File;
                await DownloadFileAsync(DownloadUrl, Path);
            }
        }
        /// <summary>
        /// 获取最新汉化模组标签。
        /// </summary>
        /// <returns>返回模组标签</returns>
        private static async Task<string> GetLatestLimbusLocalizeVersion(bool useGithub)
        {
            logger.Info("获取模组标签。");
            string raw;
            if (!useGithub)
            {
                raw = await GetURLText("https://api.kr.zeroasso.top/Mod_Release.json");
            }
            else
            {
                raw = await GetURLText("https://api.github.com/repos/LocalizeLimbusCompany/LocalizeLimbusCompany/releases");
            }
            JSONArray releases = JSONNode.Parse(raw).AsArray;
            string latestReleaseTag = releases[0]["tag_name"].Value;
            logger.Info("汉化模组最后标签为： " + latestReleaseTag);
            return latestReleaseTag;
        }
        /// <summary>
        /// 获取该网址的文本，通常用于API。
        /// </summary>
        /// <param name="Url">网址</param>
        /// <returns></returns>
        public static async Task<string> GetURLText(string Url)
        {
            try
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.Add("User-Agent", "LLC_MOD_Toolbox");
                string raw = string.Empty;
                raw = await client.GetStringAsync(Url);
                return raw;
            }
            catch (Exception ex)
            {
                ErrorReport(ex);
                return string.Empty;
            }
        }
        /// <summary>
        /// 打开指定网址。
        /// </summary>
        /// <param name="Url">网址</param>
        public static void OpenUrl(string Url)
        {
            logger.Info("打开了网址：" + Url);
            ProcessStartInfo psi = new(Url)
            {
                UseShellExecute = true
            };
            Process.Start(psi);
        }
        /// <summary>
        /// 用于错误处理。
        /// </summary>
        /// <param name="ex"></param>
        public static void ErrorReport(Exception ex)
        {
            logger.Error("出现了问题：\n" + ex.ToString());
            System.Windows.MessageBox.Show("运行中出现了问题，若要反馈，请带上链接或日志。\n——————————\n" + ex.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        /// <summary>
        /// 用于错误处理。
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="CloseWindow">是否关闭窗体。</param>
        public static void ErrorReport(Exception ex, bool CloseWindow)
        {
            logger.Error("出现了问题：\n" + ex.ToString());
            System.Windows.MessageBox.Show("运行中出现了问题，若要反馈，请带上链接或日志。\n——————————\n" + ex.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private static async void CheckToolboxUpdate(string version)
        {
            try
            {
                logger.Info("正在检查工具箱更新。");
                string raw = await GetURLText("https://api.kr.zeroasso.top/Toolbox_Release.json");
                var JsonObject = JSONNode.Parse(raw).AsObject;
                string latestReleaseTagRaw = JsonObject["tag_name"].Value;
                string latestReleaseTag = latestReleaseTagRaw.Remove(0, 1);
                logger.Info("最新安装器tag：" + latestReleaseTag);
                if (new System.Version(latestReleaseTag) > new System.Version(version))
                {
                    logger.Info("有更新。");
                    logger.Info("安装器存在更新。");
                    System.Windows.MessageBox.Show("安装器存在更新。\n点击确定进入官网下载最新版本工具箱", "更新提醒", MessageBoxButton.OK, MessageBoxImage.Warning);
                    OpenUrl("https://www.zeroasso.top/docs/install/autoinstall");
                    System.Windows.Application.Current.Shutdown();
                }
                logger.Info("没有更新。");
            }
            catch (Exception ex)
            {
                logger.Error("检查安装器更新出现问题。" + ex.ToString());
                return;
            }
        }
        public class FontUpdateResult
        {
            public string? Tag { get; set; }
            public bool IsNotLatestVersion { get; set; }
        }
        /// <summary>
        /// 获取tmp字体最新标签以及是否为最新版
        /// </summary>
        /// <param name="version">当前版本</param>
        /// <param name="IsGithub">是否使用Github</param>
        /// <param name="tag">返回tmp字体tag</param>
        /// <returns>是否不是最新版</returns>
        public static async Task<FontUpdateResult> CheckChineseFontAssetUpdate(string version, bool IsGithub)
        {
            string tag;
            try
            {
                string raw = string.Empty;
                if (IsGithub == true)
                {
                    raw = await GetURLText("https://api.github.com/repos/LocalizeLimbusCompany/LLC_ChineseFontAsset/releases/latest");
                }
                else
                {
                    raw = await GetURLText("https://api.kr.zeroasso.top/LatestTmp_Release.json");
                }
                var JsonObject = JSONNode.Parse(raw).AsObject;
                string latestReleaseTag = JsonObject["tag_name"].Value;
                if (latestReleaseTag != version)
                {
                    tag = latestReleaseTag;
                    return new FontUpdateResult
                    {
                        Tag = tag,
                        IsNotLatestVersion = true
                    };
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("出现了问题。\n" + ex.ToString());
                logger.Error("出现了问题。\n" + ex.ToString());
            }
            return new FontUpdateResult
            {
                Tag = null,
                IsNotLatestVersion = false
            };
        }
        #endregion
        #region 进度条系统
        public async void ProgressTime_Tick(object? sender, EventArgs e)
        {
            await ChangeProgressValue(progressPercentage);
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
            logger.Info("点击删除模组");
            MessageBoxResult result = System.Windows.MessageBox.Show("删除后你需要重新安装汉化补丁。\n确定继续吗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                logger.Info("确定删除模组。");
                try
                {
                    DeleteBepInEx();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("删除过程中出现了一些问题： " + ex.ToString(), "警告");
                    logger.Error("删除过程中出现了一些问题： " + ex.ToString());
                }
                System.Windows.MessageBox.Show("删除完成。", "提示");
                logger.Info("删除完成。");
            }
        }
        /// <summary>
        /// 删除目录。
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteDir(string path)
        {
            if (Directory.Exists(path))
            {
                logger.Info("删除目录： " + path);
                Directory.Delete(path, true);
            }
        }
        /// <summary>
        /// 删除文件。
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                logger.Info("删除文件： " + path);
                File.Delete(path);
            }
        }
        /// <summary>
        /// 删除BepInEx版本汉化补丁。
        /// </summary>
        public static void DeleteBepInEx()
        {
            DeleteDir(limbusCompanyDir + "/BepInEx");
            DeleteDir(limbusCompanyDir + "/dotnet");
            DeleteFile(limbusCompanyDir + "/doorstop_config.ini");
            DeleteFile(limbusCompanyDir + "/Latest(框架日志).log");
            DeleteFile(limbusCompanyDir + "/Player(游戏日志).log");
            DeleteFile(limbusCompanyDir + "/winhttp.dll");
            DeleteFile(limbusCompanyDir + "/changelog.txt");
            DeleteFile(limbusCompanyDir + "/BepInEx-IL2CPP-x64.7z");
            DeleteFile(limbusCompanyDir + "/LimbusLocalize_BIE.7z");
            DeleteFile(limbusCompanyDir + "/tmpchinese_BIE.7z");
        }
        #endregion
        #region 灰度测试
        private async void StartGreytestButtonClick(object sender, RoutedEventArgs e)
        {
            logger.Info("Z-TECH 灰度测试客户端程序 v2.0 启动。（并不是");
            if (!greytestStatus)
            {
                string token = await GetGreytestBoxText();
                if (token == string.Empty || token == "请输入秘钥")
                {
                    logger.Info("Token为空。");
                    System.Windows.MessageBox.Show("请输入有效的Token。", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                logger.Info("Token为：" + token);
                string tokenUrl = "https://dev.zeroasso.top/api/" + token + ".json";
                using (HttpClient client = new())
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(tokenUrl);
                        if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                        {
                            logger.Info("秘钥有效。");
                        }
                        else
                        {
                            logger.Info("秘钥无效。");
                            System.Windows.MessageBox.Show("请输入有效的Token。", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    var tokenObject = JSONNode.Parse(tokenJson).AsObject;
                    string runStatus = tokenObject["status"].Value;
                    if (runStatus == "test")
                    {
                        logger.Info("Token状态正常。");
                    }
                    else
                    {
                        logger.Info("Token已停止测试。");
                        System.Windows.MessageBox.Show("Token已停止测试。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    string fileName = tokenObject["file_name"].Value;
                    string note = tokenObject["note"].Value;
                    logger.Info("Token信息：" + token + "混淆文件名：" + fileName + "备注：" + note);
                    await ChangeLogoToTest();
                    System.Windows.MessageBox.Show("目前Token有效。\n-------------\nToken信息：\n秘钥：" + token + "\n混淆文件名：" + fileName + "\n备注：" + note + "\n-------------\n灰度测试模式已开启。\n请在自动安装安装此秘钥对应版本汉化。\n秘钥信息请勿外传。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    greytestStatus = true;
                    greytestUrl = "https://dev.zeroasso.top/files/LimbusLocalize_Dev_" + fileName + ".7z";
                }
                catch (Exception ex)
                {
                    ErrorReport(ex, false);
                    return;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("灰度测试模式已开启。\n请在自动安装安装此秘钥对应版本汉化。\n若需要正常使用或更换秘钥，请重启工具箱。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
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
                logger.Info("灰度测试模式已开启。开始安装灰度模组。");
                installPhase = 3;
                await DownloadFileAsync(greytestUrl, limbusCompanyDir + "/LimbusLocalize_Dev.7z");
                Unarchive(limbusCompanyDir + "/LimbusLocalize_Dev.7z", limbusCompanyDir);
                logger.Info("灰度模组安装完成。");
            });
        }
        #endregion
        #region 程序配置
        public class LLCMTConfig
        {
            public bool CskipLCBPathCheck { get; set; }
            public string? CLCBPath { get; set; }
        }
        private static bool skipLCBPathCheck = false;
        private static string? LCBPath = string.Empty;
        private static string configPath = Path.Combine(currentDir, "config.json");
        private static void LoadConfig()
        {
            logger.Info("加载程序配置。");
            try
            {
                if (File.Exists(configPath))
                {
                    string configContent = File.ReadAllText(configPath);
                    LLCMTConfig LLCMTconfig = JsonConvert.DeserializeObject<LLCMTConfig>(configContent);
                    if (LLCMTconfig == null)
                    {
                        throw new FileNotFoundException("配置文件未找到。");
                    }
                    skipLCBPathCheck = LLCMTconfig.CskipLCBPathCheck;
                    LCBPath = LLCMTconfig.CLCBPath;
                    logger.Info("跳过路径检查：" + skipLCBPathCheck);
                    logger.Info("路径：" + LCBPath);
                }
            }
            catch (Exception ex)
            {
                ErrorReport(ex, false);
            }
        }
        private static void ChangeSkipPathCheckConfig(bool boolValue)
        {
            logger.Info("改变跳过路径检查配置，Value：" + boolValue);
            try
            {
                if (File.Exists(configPath))
                {
                    string configContent = File.ReadAllText(configPath);
                    LLCMTConfig LLCMTconfig = JsonConvert.DeserializeObject<LLCMTConfig>(configContent);
                    if (LLCMTconfig == null)
                    {
                        throw new FileNotFoundException("配置文件未找到。");
                    }
                    LLCMTconfig.CskipLCBPathCheck = boolValue;
                    string updatedConfigContent = JsonConvert.SerializeObject(LLCMTconfig, Formatting.Indented);
                    logger.Debug("更新后的配置文件：" + updatedConfigContent);
                    File.WriteAllText(configPath, updatedConfigContent);
                    logger.Info("配置文件更新完成。");
                }
            }
            catch (Exception ex)
            {
                ErrorReport(ex, false);
            }
        }
        private static void ChangeLCBPathConfig(string? stringValue)
        {
            logger.Info("改变边狱公司路径配置，Value：" + stringValue);
            try
            {
                if (string.IsNullOrEmpty(stringValue))
                {
                    logger.Error("修改的值为Null。");
                    return;
                }
                if (File.Exists(configPath))
                {
                    string configContent = File.ReadAllText(configPath);
                    LLCMTConfig LLCMTconfig = JsonConvert.DeserializeObject<LLCMTConfig>(configContent);
                    if (LLCMTconfig == null)
                    {
                        throw new FileNotFoundException("配置文件未找到。");
                    }
                    LLCMTconfig.CLCBPath = stringValue;
                    string updatedConfigContent = JsonConvert.SerializeObject(LLCMTconfig, Formatting.Indented);
                    logger.Debug("更新后的配置文件：" + updatedConfigContent);
                    File.WriteAllText(configPath, updatedConfigContent);
                    logger.Info("配置文件更新完成。");
                }
            }
            catch (Exception ex)
            {
                ErrorReport(ex, false);
            }
        }
        #endregion
    }
}