
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
using LLC_MOD_Toolbox.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SevenZip;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static LLC_MOD_Toolbox.SimpleDnsChecker;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        private static string? useEndPoint;
        private static string? useAPIEndPoint;
        private static bool useGithub = false;
        private static string limbusCompanyDir = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 1973530", "InstallLocation", null) as string
            ?? string.Empty;
        private static string limbusCompanyGameDir = Path.Combine(limbusCompanyDir, "LimbusCompany.exe");
        private static readonly string currentDir = AppDomain.CurrentDomain.BaseDirectory;
        private static List<Node> nodeList = [];
        private static List<Node> apiList = [];
        private static string defaultEndPoint = "";
        private static string defaultAPIEndPoint = "";
        private static int installPhase = 0;
        private readonly DispatcherTimer progressTimer;
        private float progressPercentage = 0;
        private bool isNewestModVersion = true;
        // GreyTest 灰度测试2.0
        private static string greytestUrl = string.Empty;
        private static bool greytestStatus = false;
        private readonly string VERSION = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
        // Hash Cache
        private JObject hashCacheObject = null;
        // 配置
        private static ConfigurationManager configuation = new ConfigurationManager(Path.Combine(currentDir, "config.json"));
        // 启动器模式
        private static bool isLauncherMode = Environment.GetCommandLineArgs().Contains("-launcher");
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
            Log.logger.Info("—————新日志分割线—————");
            Log.logger.Info("工具箱已进入加载流程。");
            Log.logger.Info("We have a lift off.");
            Log.logger.Info($"WPF架构工具箱 版本：{VERSION} 。");
            await DisableGlobalOperations();
            // 设置网络协议
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            using HttpClient client = new HttpClient
            {
                DefaultRequestVersion = HttpVersion.Version11,
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact
            };
            await CheckLoadingText();
            InitNode();
            await RefreshPage();
            CheckToolboxUpdate();
            CheckLimbusCompanyPath();
            SevenZipBase.SetLibraryPath(Path.Combine(currentDir, "7z.dll"));
            await CheckAnno();
            CheckLCBPath();
            AdaptFuckingPM.CheckAdapt(limbusCompanyDir);
            if (!isLauncherMode)
            {
                InitLink();
                LaunchUpdateLoadingThread();
                await ChangeEEPic();
                await CheckModInstalled();
                await CheckDNS();
            }
            if ((configuation.Settings.install.installWhenLaunch || isLauncherMode) && !hasNewAnno)
            {
                InstallButtonClick(null, null);
            }
            await EnableGlobalOperations();
            Log.logger.Info("加载流程完成。");
        }
        #region 安装功能
        /// <summary>
        /// 处理自动安装页面的安装按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void InstallButtonClick(object? sender, RoutedEventArgs? e)
        {
            isInstalling = true;
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
                MessageBoxResult DialogResult = System.Windows.MessageBox.Show("检测到MelonLoader框架！\nMelonLoader框架已过时，且其可能导致您的账号遭到封禁，导致您无法进行游戏！\n建议您进行一次卸载后继续安装模组。\n若您**及其确定这是个误判**，请点击确定，否则请点击取消返回，之后您可以在设置中找到卸载，将MelonLoader卸载后重新安装。", "警告", MessageBoxButton.OKCancel, MessageBoxImage.Hand);
                if (DialogResult == MessageBoxResult.Cancel)
                {
                    await StopInstall();
                    return;
                }
                Log.logger.Warn("用户选择无视警告。");
            }
            if (File.Exists(limbusCompanyDir + "/winhttp.dll"))
            {
                Log.logger.Warn("检测到BepInEx框架.");
                MessageBoxResult DialogResult = System.Windows.MessageBox.Show("检测到BepInEx框架（旧版本模组）！\n使用旧版本汉化模组可能遭到月亮计划的封禁！\n建议您进行一次卸载后继续安装模组。\n若您**及其确定这是个误判**，请点击“确定”。\n否则，请点击“取消”停止安装，之后您可以在设置中找到卸载，将BepInEx卸载后重新安装。", "警告", MessageBoxButton.OKCancel, MessageBoxImage.Hand);
                if (DialogResult == MessageBoxResult.Cancel)
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
                MessageBoxResult DialogResult = System.Windows.MessageBox.Show("检测到 Limbus Company 仍然处于开启状态！\n建议您关闭游戏后继续安装模组。\n若您已经关闭了 Limbus Company，请点击确定，否则请点击取消返回。", "警告", MessageBoxButton.OKCancel, MessageBoxImage.Hand);
                if (DialogResult == MessageBoxResult.Cancel)
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
                    await CachedHash();
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
            MessageBoxResult RunResult = new();
            if (isNewestModVersion)
            {
                RunResult = MessageBox.Show("没有检测到新版本模组！\n您的模组已经为最新。\n点击“确定”立刻运行边狱公司。\n点击“取消”关闭弹窗。\n加载时请耐心等待。", "完成", MessageBoxButton.OKCancel);
            }
            else
            {
                RunResult = MessageBox.Show("安装已完成！\n点击“确定”立刻运行边狱公司。\n点击“取消”关闭弹窗。\n加载时请耐心等待。", "完成", MessageBoxButton.OKCancel);
            }
            if (RunResult == MessageBoxResult.OK)
            {
                try
                {
                    OpenUrl("steam://rungameid/1973530");
                }
                catch (Exception ex)
                {
                    Log.logger.Error("出现了问题： ", ex);
                    MessageBox.Show("出现了问题。\n" + ex.ToString());
                }
            }
            hashCacheObject = null;
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
                    MessageBox.Show("校验Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "校验失败");
                    await StopInstall();
                    return;
                }
            });
        }
        private async Task InstallMod()
        {
            await Task.Run(async () =>
            {
                Log.logger.Info("开始安装模组。");
                installPhase = 2;
                string langDir = Path.Combine(limbusCompanyDir, "LimbusCompany_Data/Lang/LLC_zh-CN");
                string versionJsonPath = Path.Combine(langDir, "Info", "version.json");
                string limbusLocalizeZipPath = Path.Combine(limbusCompanyDir, "LimbusLocalize.7z");
                int latestVersion = -1;
                int currentVersion = -1;
                bool needInstall = false;
                JObject versionObj;
                if (!File.Exists(versionJsonPath))
                {
                    Log.logger.Info("模组不存在。开始安装。");
                    needInstall = true;
                    isNewestModVersion = false;
                }
                if (useGithub && !needInstall)
                {
                    latestVersion = await GetLatestLimbusLocalizeVersion(true);
                    Log.logger.Info("最后模组版本： " + latestVersion);
                    versionObj = JObject.Parse(File.ReadAllText(versionJsonPath));
                    currentVersion = versionObj["version"].Value<int>();
                    Log.logger.Info("当前模组版本： " + currentVersion);
                    if (currentVersion >= latestVersion)
                    {
                        Log.logger.Info("模组无需更新。");
                        return;
                    }
                    else
                    {
                        needInstall = true;
                        isNewestModVersion = false;
                        Log.logger.Info("模组需要更新。进行安装。");
                    }
                }
                else if (!useGithub && !needInstall)
                {
                    latestVersion = await GetLatestLimbusLocalizeVersion(true);
                    Log.logger.Info("最后模组版本： " + latestVersion);
                    versionObj = JObject.Parse(File.ReadAllText(versionJsonPath));
                    currentVersion = versionObj["version"].Value<int>();
                    Log.logger.Info("当前模组版本： " + currentVersion);
                    if (currentVersion >= latestVersion)
                    {
                        Log.logger.Info("模组无需更新。");
                        return;
                    }
                    else
                    {
                        needInstall = true;
                        isNewestModVersion = false;
                        Log.logger.Info("模组需要更新。进行安装。");
                    }
                }
                if (useGithub && needInstall)
                {
                    latestVersion = await GetLatestLimbusLocalizeVersion(true);
                    await DownloadFileAsync($"https://github.com/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/download/{latestVersion}/LimbusLocalize_{latestVersion}.7z", limbusLocalizeZipPath);
                    Log.logger.Info("解压模组本体 zip 中。");
                    Unarchive(limbusLocalizeZipPath, limbusCompanyDir);
                    Log.logger.Info("删除模组本体 zip 。");
                    File.Delete(limbusLocalizeZipPath);
                }
                else if (!useGithub && needInstall)
                {
                    latestVersion = await GetLatestLimbusLocalizeVersion(false);
                    await DownloadFileAutoAsync($"LimbusLocalize_{latestVersion}.7z", limbusLocalizeZipPath);
                    if (hashCacheObject["main_hash"].Value<string>() != CalculateSHA256(limbusLocalizeZipPath))
                    {
                        Log.logger.Error("校验Hash失败。");
                        MessageBox.Show("校验Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "校验失败");
                        await StopInstall();
                        return;
                    }
                    else
                    {
                        Log.logger.Info("校验Hash成功。");
                    }
                    Log.logger.Info("解压模组本体 zip 中。");
                    Unarchive(limbusLocalizeZipPath, limbusCompanyDir);
                    Log.logger.Info("删除模组本体 zip 。");
                    File.Delete(limbusLocalizeZipPath);
                }
            });
        }
        private async Task CachedHash()
        {
            string hash = "";
            if (!configuation.Settings.general.internationalMode)
            {
                hash = await GetURLText("https://api.zeroasso.top/v2/hash/get_hash");
            }
            else
            {
                hash = await GetURLText("https://cdn-api.zeroasso.top/v2/hash/get_hash");
            }
            hashCacheObject = JObject.Parse(hash);
            if (hashCacheObject == null)
            {
                Log.logger.Error("获取Hash失败。");
                MessageBox.Show("获取Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "获取Hash失败");
                await StopInstall();
                return;
            }
        }
        #endregion
        #region 读取节点
        private static bool APPChangeAPIUI = false;

        public void InitNode()
        {
            var _jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
            var json = JsonConvert.DeserializeObject<RootModel>(File.ReadAllText($"NodeList.json"), _jsonSettings);
            nodeList = json.DownloadNode;
            apiList = json.ApiNode;
            NodeCombobox.Items.Add("恢复默认");
            foreach (var Node in nodeList)
            {
                if (Node.IsDefault == true && !configuation.Settings.general.internationalMode)
                {
                    defaultEndPoint = Node.Endpoint;
                }
                if (Node.Endpoint == "https://cdn-download.zeroasso.top/files/{0}" && configuation.Settings.general.internationalMode)
                {
                    Log.logger.Info("获取到国际下载节点。");
                    defaultEndPoint = Node.Endpoint;
                }
                NodeCombobox.Items.Add(Node.Name);
            }
            NodeCombobox.Items.Add("Github直连");
            // API
            APICombobox.Items.Add("恢复默认");
            foreach (var api in apiList)
            {
                if (api.IsDefault == true && !configuation.Settings.general.internationalMode)
                {
                    defaultAPIEndPoint = api.Endpoint;
                    useAPIEndPoint = defaultAPIEndPoint;
                }
                if (api.Endpoint == "https://cdn-api.zeroasso.top/{0}" && configuation.Settings.general.internationalMode)
                {
                    Log.logger.Info("获取到国际API节点。");
                    defaultAPIEndPoint = api.Endpoint;
                    useAPIEndPoint = defaultAPIEndPoint;
                }
                APICombobox.Items.Add(api.Name);
            }
            Log.logger.Info("API数量：" + apiList.Count);
            Log.logger.Info("节点数量：" + nodeList.Count);
        }
        private static string FindNodeEndpoint(string Name)
        {
            foreach (var node in nodeList)
            {
                if (node.Name == Name)
                {
                    return node.Endpoint;
                }
            }
            return string.Empty;
        }
        private static string FindAPIEndpoint(string Name)
        {
            foreach (var api in apiList)
            {
                if (api.Name == Name)
                {
                    return api.Endpoint;
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
        public async Task<string> GetAPIComboboxText()
        {
            string combotext = string.Empty;
            await this.Dispatcher.BeginInvoke(() =>
            {
                combotext = APICombobox.SelectedItem.ToString();
            });
            return combotext;
        }
        public async Task<string> SetAPIComboboxText(string text)
        {
            APPChangeAPIUI = true;
            string combotext = string.Empty;
            await this.Dispatcher.BeginInvoke(() =>
            {
                APICombobox.SelectedItem = text;
            });
            return combotext;
        }
        private async void NodeComboboxSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string nodeComboboxText = await GetNodeComboboxText();
            Log.logger.Info("选择节点。");
            if (nodeComboboxText != string.Empty)
            {
                if (nodeComboboxText == "恢复默认")
                {
                    useEndPoint = string.Empty;
                    useGithub = false;
                    Log.logger.Info("已恢复默认Endpoint。");
                }
                else if (nodeComboboxText == "Github直连")
                {
                    Log.logger.Info("选择Github节点。");
                    System.Windows.MessageBox.Show("如果您没有使用代理软件（包括Watt Toolkit）\n请不要使用此节点。\nGithub由于不可抗力因素，对国内网络十分不友好。\n如果您是国外用户，才应该使用此选项。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    useEndPoint = string.Empty;
                    useGithub = true;
                }
                else
                {
                    useEndPoint = FindNodeEndpoint(nodeComboboxText);
                    useGithub = false;
                    Log.logger.Info("当前Endpoint：" + useEndPoint);
                    System.Windows.MessageBox.Show("切换成功。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                Log.logger.Info("NodeComboboxText 为 null。");
            }
        }
        private async void APIComboboxSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!useGithub)
            {
                string apiComboboxText = await GetAPIComboboxText();
                Log.logger.Info("选择API节点。");
                if (apiComboboxText != string.Empty)
                {
                    if (apiComboboxText == "恢复默认")
                    {
                        useAPIEndPoint = defaultAPIEndPoint;
                        Log.logger.Info("已恢复默认API Endpoint。");
                    }
                    else
                    {
                        useAPIEndPoint = FindAPIEndpoint(apiComboboxText);
                        Log.logger.Info("当前API Endpoint：" + useAPIEndPoint);
                        System.Windows.MessageBox.Show("切换成功。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    Log.logger.Info("APIComboboxText 为 null。");
                }
            }
            else if (APPChangeAPIUI == false)
            {
                await SetAPIComboboxText("恢复默认");
                Log.logger.Info("已开启Github。无法切换API。");
                System.Windows.MessageBox.Show("切换失败。\n无法在节点为Github直连的情况下切换API。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            APPChangeAPIUI = false;
        }
        private void WhyShouldIUseThis(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://www.zeroasso.top/docs/configuration/nodes");
        }
        #endregion
        #region 常用方法
        public static void Unarchive(string archivePath, string output)
        {
            using SevenZipExtractor extractor = new(archivePath);
            extractor.ExtractArchive(output);
        }

        /// <summary>
        /// 安装时输出统一格式日志。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="promptInfo"></param>
        /// <param name="someObject"></param>
        private static void PrintInstallInfo<T>(string promptInfo, T someObject)
        {
            if (someObject == null)
            {
                Log.logger.Info($"{promptInfo}：空");
            }
            else
            {
                Log.logger.Info($"{promptInfo}{someObject}");
            }

        }
        private static void CheckLimbusCompanyPath()
        {
            if (configuation.Settings.general.skipLCBPathCheck && !string.IsNullOrEmpty(configuation.Settings.general.LCBPath))
            {
                limbusCompanyDir = configuation.Settings.general.LCBPath;
                Log.logger.Info("跳过检查路径。");
            }
            else
            {
                MessageBoxResult CheckLCBPathResult = MessageBoxResult.OK;
                if (!string.IsNullOrEmpty(limbusCompanyDir))
                {
                    CheckLCBPathResult = System.Windows.MessageBox.Show($"这是您的边狱公司地址吗？\n{limbusCompanyDir}", "检查路径", MessageBoxButton.YesNo, MessageBoxImage.Question);
                }
                if (CheckLCBPathResult == MessageBoxResult.Yes)
                {
                    Log.logger.Info("用户确认路径。");
                    configuation.Settings.general.LCBPath = limbusCompanyDir;
                    configuation.Settings.general.skipLCBPathCheck = true;
                    configuation.SaveConfig();
                }
                if (string.IsNullOrEmpty(limbusCompanyDir) || CheckLCBPathResult == MessageBoxResult.No)
                {
                    if (string.IsNullOrEmpty(limbusCompanyDir))
                    {
                        Log.logger.Warn("未能找到 Limbus Company 目录，手动选择模式。");
                        System.Windows.MessageBox.Show("未能找到 Limbus Company 目录。请手动选择。", "提示");
                    }
                    else
                    {
                        Log.logger.Warn("用户否认 Limbus Company 目录正确性。");
                    }
                    var fileDialog = new OpenFileDialog
                    {
                        Title = "请选择你的边狱公司游戏文件",
                        Multiselect = false,
                        InitialDirectory = limbusCompanyDir,
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
                        Log.logger.Error("选择了错误目录，关闭。");
                        System.Windows.MessageBox.Show("选择目录有误，没有在当前目录找到游戏。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        System.Windows.Application.Current.Shutdown();
                    }
                    else
                    {
                        Log.logger.Info("找到了正确目录。");
                        configuation.Settings.general.LCBPath = limbusCompanyDir;
                        configuation.Settings.general.skipLCBPathCheck = true;
                        configuation.SaveConfig();
                    }
                }
            }
            limbusCompanyGameDir = Path.Combine(limbusCompanyDir, "LimbusCompany.exe");
            Log.logger.Info("边狱公司路径：" + limbusCompanyDir);
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
            Log.logger.Info($"计算位置为 {filePath} 的文件的Hash结果为：{BitConverter.ToString(hashBytes).Replace("-", "").ToLower()}");
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
        /// <summary>
        /// 处理使用Downloader下载文件的事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewOnDownloadProgressChanged(object? sender, Downloader.DownloadProgressChangedEventArgs e)
        {
            Log.logger.Debug("ProgressPercentage: " + e.ProgressPercentage + " ProgressPercentage(Int): " + (int)(e.ProgressPercentage));
            if (installPhase != 0)
            {
                progressPercentage = (float)((installPhase - 1) * 50 + e.ProgressPercentage * 0.5);
            }
        }
        private void NewOnDownloadProgressCompleted(object? sender, AsyncCompletedEventArgs e)
        {
            if (installPhase != 0)
            {
                progressPercentage = installPhase * 50;
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
            Log.logger.Info($"下载 {Url} 到 {Path}");
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
        public static async Task DownloadFileAsyncWithoutProgress(string Url, string Path)
        {
            Log.logger.Info($"下载 {Url} 到 {Path}");
            var downloadOpt = new DownloadConfiguration()
            {
                BufferBlockSize = 10240,
                ChunkCount = 8,
                MaxTryAgainOnFailover = 5,
            };
            var downloader = new DownloadService(downloadOpt);
            await downloader.DownloadFileTaskAsync(Url, Path);
        }
        public async Task DownloadFileAutoAsync(string File, string Path)
        {
            Log.logger.Info($"自动选择下载节点式下载文件 文件: {File}  路径: {Path}");
            if (!string.IsNullOrEmpty(useEndPoint))
            {
                string DownloadUrl = string.Format(useEndPoint, File);
                await DownloadFileAsync(DownloadUrl, Path);
            }
            else
            {
                string DownloadUrl = string.Format(defaultEndPoint, File);
                await DownloadFileAsync(DownloadUrl, Path);
            }
        }
        /// <summary>
        /// 获取最新汉化模组标签。
        /// </summary>
        /// <returns>返回模组标签</returns>
        private static async Task<int> GetLatestLimbusLocalizeVersion(bool useGithub)
        {
            Log.logger.Info("获取模组标签。");
            string version;
            if (!useGithub)
            {
                string raw = await GetURLText(string.Format(useAPIEndPoint, "v2/resource/get_version"));
                var json = JObject.Parse(raw);
                version = json["version"].Value<string>();
            }
            else
            {
                string raw = await GetURLText("https://api.github.com/repos/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/latest");
                var json = JObject.Parse(raw);
                version = json["tag_name"].Value<string>();
            }
            Log.logger.Info($"汉化模组最后标签为： {version}");
            int parseVersion = int.Parse(version);
            return parseVersion;

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
                Log.logger.Info($"获取 {Url} 文本内容。");
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
        /// <summary>
        /// 打开指定网址。
        /// </summary>
        /// <param name="Url">网址</param>
        public static void OpenUrl(string Url)
        {
            Log.logger.Info("打开了网址：" + Url);
            ProcessStartInfo psi = new(Url)
            {
                UseShellExecute = true
            };
            Process.Start(psi);
        }
        /// <summary>
        /// 检查工具箱更新
        /// </summary>
        /// <param name="version">当前版本</param>
        /// <param name="IsGithub">是否使用Github</param>
        /// <returns>是否存在更新</returns>
        private static async void CheckToolboxUpdate()
        {
            try
            {
                Log.logger.Info("正在检查工具箱更新。");
                string raw = await GetURLText(string.Format(useAPIEndPoint, "v2/get_api/get/repos/LocalizeLimbusCompany/LLC_Mod_Toolbox/releases/latest"));
                var JsonObject = JObject.Parse(raw);
                string latestReleaseTagRaw = JsonObject["tag_name"].Value<string>();
                string latestReleaseTag = latestReleaseTagRaw.Remove(0, 1);
                Log.logger.Info("最新安装器tag：" + latestReleaseTag);
                if (new Version(latestReleaseTag) > Assembly.GetExecutingAssembly().GetName().Version)
                {
                    Log.logger.Info("安装器存在更新。");
                    var result = MessageBox.Show("安装器存在更新。\n点击确定下载最新版工具箱安装包并安装。\n你也可以在官网直接下载最新版。", "更新提醒", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.OK)
                    {
                        Log.logger.Info("用户选择下载更新。");
                        string installerEXE = Path.Combine(Path.GetTempPath(), "LLC_Mod_Toolbox_Installer.exe");
                        await DownloadFileAsyncWithoutProgress("https://download.zeroasso.top/files/LLC_MOD_Toolbox_Installer.exe", installerEXE);
                        Log.logger.Info("下载完成。");
                        MessageBox.Show("下载完成，即将启动安装器。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        string batPath = CreateBatchFile(installerEXE);
                        StartBatchProcess(batPath);
                        Application.Current.Shutdown();
                    }
                    Application.Current.Shutdown();
                }
                Log.logger.Info("没有更新。");
            }
            catch (Exception ex)
            {
                Log.logger.Error("检查安装器更新出现问题。", ex);
                return;
            }
        }

        private static string CreateBatchFile(string targetExePath)
        {
            string currentExePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

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
            configuation.Settings.general.skipLCBPathCheck = false;
            var fileDialog = new OpenFileDialog
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
                System.Windows.MessageBox.Show("选择目录有误，没有在当前目录找到游戏。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                Log.logger.Info("找到了正确目录。");
                configuation.Settings.general.LCBPath = limbusCompanyDir;
                configuation.Settings.general.skipLCBPathCheck = true;
                configuation.SaveConfig();
            }
        }
        public static void ChangeLCBLangConfig(string value)
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
                var result = MessageBox.Show("配置文件出现问题，是否尝试进行修复？\n" + ex.Message, "错误", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    File.WriteAllText(Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "config.json"), "{\"lang\": \"\",\"titleFont\": \"\",\"contextFont\": \"\",\"samplingPointSize\": 78,\"padding\": 5}");
                    ChangeLCBLangConfig(value);
                }
            }
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
        private async void UninstallButtonClick(object sender, RoutedEventArgs e)
        {
            Log.logger.Info("点击删除模组");
            MessageBoxResult result = System.Windows.MessageBox.Show("删除后你需要重新安装汉化补丁。\n确定继续吗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
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
                    System.Windows.MessageBox.Show("删除过程中出现了一些问题： " + ex.ToString(), "警告");
                    Log.logger.Error("删除过程中出现了一些问题： ", ex);
                }
                System.Windows.MessageBox.Show("删除完成。", "提示");
                Log.logger.Info("删除完成。");
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
                Log.logger.Info("删除目录： " + path);
                Directory.Delete(path, true);
            }
            else
            {
                Log.logger.Info("目录不存在： " + path);
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
                Log.logger.Info("删除文件： " + path);
                File.Delete(path);
            }
            else
            {
                Log.logger.Info("文件不存在： " + path);
            }
        }
        public static void DeleteLanguagePack()
        {
            DeleteDir(Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN"));
            ChangeLCBLangConfig("");
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
            DeleteFile(limbusCompanyDir + "/winhttp.dll.disabled");
            DeleteFile(limbusCompanyDir + "/changelog.txt");
            DeleteFile(limbusCompanyDir + "/BepInEx-IL2CPP-x64.7z");
            DeleteFile(limbusCompanyDir + "/LimbusLocalize_BIE.7z");
            DeleteFile(limbusCompanyDir + "/tmpchinese_BIE.7z");
        }
        public static void DeleteMelonLoader()
        {
            // 为什么还有人在用Melonloader！！！！
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
        private async void StartGreytestButtonClick(object sender, RoutedEventArgs e)
        {
            Log.logger.Info("Z-TECH 灰度测试客户端程序 v3.0 启动。");
            await DisableGlobalOperations();
            if (!greytestStatus)
            {
                string token = await GetGreytestBoxText();
                if (token == string.Empty || token == "请输入秘钥")
                {
                    Log.logger.Info("Token为空。");
                    System.Windows.MessageBox.Show("请输入有效的Token。", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Log.logger.Info("Token为：" + token);
                string tokenUrl = string.Format(useAPIEndPoint, $"/v2/grey_test/get_token?code={token}");
                using (HttpClient client = new())
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(tokenUrl);
                        if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                        {
                            Log.logger.Info("秘钥有效。");
                        }
                        else
                        {
                            Log.logger.Info("秘钥无效。");
                            System.Windows.MessageBox.Show("请输入有效的Token。", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        System.Windows.MessageBox.Show("Token已停止测试。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        await EnableGlobalOperations();
                        return;
                    }
                    string note = tokenObject["note"].Value<string>();
                    Log.logger.Info($"Token：{token}\n备注：{note}");
                    await ChangeLogoToTest();
                    MessageBox.Show(
                        $"目前Token有效。\n-------------\nToken信息：\n秘钥：{token}\n备注：{note}\n-------------\n灰度测试模式已开启。\n请在自动安装安装此秘钥对应版本汉化。\n秘钥信息请勿外传。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    greytestStatus = true;
                    greytestUrl = string.Format(useAPIEndPoint, $"/v2/grey_test/get_file?code={token}");
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
                System.Windows.MessageBox.Show("灰度测试模式已开启。\n请在自动安装安装此秘钥对应版本汉化。\n若需要正常使用或更换秘钥，请重启工具箱。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                await EnableGlobalOperations();
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
        private bool hasVergil = false;
        private bool alreadyHasVergil = false;
        private async Task InitGacha()
        {
            await DisableGlobalOperations();
            string gachaText = await GetURLText("https://download.zeroasso.top/wiki/wiki_personal.json");
            if (string.IsNullOrEmpty(gachaText))
            {
                Log.logger.Error("初始化失败。");
                System.Windows.MessageBox.Show("初始化失败。请检查网络情况。", "提示");
                isInitGachaFailed = true;
                hasVergil = false;
                await EnableGlobalOperations();
                return;
            }

            gachaTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.15)
            };
            gachaTimer.Tick += GachaTimerTick;
            List<PersonalInfo> personalInfos = TranformTextToList(gachaText);
            Log.logger.Info("人格数量：" + personalInfos.Count);
            personalInfos1star = personalInfos.Where(p => p.Unique == 1).ToList();
            personalInfos2star = personalInfos.Where(p => p.Unique == 2).ToList();
            personalInfos3star = personalInfos.Where(p => p.Unique == 3).ToList();
            // 明明可以用 personalInfos.GroupBy(p => p.Unique)
            System.Windows.MessageBox.Show("初始化完成。", "提示");
            isInitGacha = true;
            await EnableGlobalOperations();
        }
        private async void InGachaButtonClick(object sender, RoutedEventArgs e)
        {
            Log.logger.Info("点击抽卡。");
            await CollapsedAllGacha();
            if (isInitGachaFailed)
            {
                Log.logger.Info("初始化失败。");
                System.Windows.MessageBox.Show("初始化失败，无法进行抽卡操作。", "提示");
                return;
            }
            Random random = new();
            if (random.Next(1, 101) == 100)
            {
                hasVergil = true;
            }
            try
            {
                List<PersonalInfo> personals = GenPersonalList();
                if (personals.Count < 10)
                {
                    Log.logger.Info("人格数量不足。\n尝试重新生成。");
                    personals = GenPersonalList();
                }
                await StartChangeLabel(personals);
            }
            catch (Exception ex)
            {
                Log.logger.Info("出现了问题。", ex);
                System.Windows.MessageBox.Show("出了点小问题！\n要不再试一次？\n————————\n" + ex.ToString());
                gachaTimer?.Stop();
                _currentIndex = 0;
                await this.Dispatcher.BeginInvoke(() =>
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
        private async Task ChangeLabelColorAndPersonal(PersonalInfo personal, System.Windows.Controls.Label label)
        {
            await this.Dispatcher.BeginInvoke(() =>
            {
                if (label.Content is TextBlock textBlock)
                {
                    if (personal.Unique == 1)
                    {
                        textBlock.Text = "[★]" + personal.Name;
                        textBlock.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#B88345"));
                    }
                    if (personal.Unique == 2)
                    {
                        textBlock.Text = "[★★]" + personal.Name;
                        textBlock.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CA1400"));
                    }
                    if (personal.Unique == 3)
                    {
                        textBlock.Text = "[★★★]" + personal.Name;
                        textBlock.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FCC404"));
                    }
                    Random random = new();
                    if (hasVergil && random.Next(1, 10) == 1)
                    {
                        textBlock.Text = "[★★★★★★] 猩红凝视 维吉里乌斯";
                        textBlock.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#9B0101"));
                        hasVergil = false;
                        alreadyHasVergil = true;
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
        private static List<PersonalInfo> TranformTextToList(string gachaText)
        {
            Log.logger.Info("开始转换文本。");
            var gachaObject = JObject.Parse(gachaText);
            List<PersonalInfo> personalInfoList = [];
            for (int i = 0; i < gachaObject["data"].Count(); i++)
            {
                PersonalInfo personalInfo = new()
                {
                    Name = "NullName",
                    Unique = 1,
                };
                personalInfo.Name = BeautifyText(gachaObject["data"][i][0].Value<string>(), gachaObject["data"][i][1].Value<string>());
                personalInfo.Unique = gachaObject["data"][i][7].Value<int>();
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
                var label = (System.Windows.Controls.Label)this.FindName($"GachaText{_currentIndex + 1}");
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
                if (alreadyHasVergil)
                {
                    System.Windows.MessageBox.Show("当你不见前路，不知应去往何方时……\n向导会为你指引方向。\n但丁。", "？？？", MessageBoxButton.OK, MessageBoxImage.Warning);
                    alreadyHasVergil = false;
                    return;
                }
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
                        System.Windows.MessageBox.Show("收手吧！你不算砸了多少狂气我算了！\n你已经砸了60x1300=78000狂气了！", "提示");
                        return;
                    case 100:
                        System.Windows.MessageBox.Show("我是来恭喜你，你已经扔进去1000抽，简称130000狂气了。\n你花了多少时间到这里？", "提示");
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
        #region 错误处理
        /// <summary>
        /// 用于错误处理。
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="CloseWindow">是否关闭窗体。</param>
        /// <param name="advice">提供建议</param>
        public static void ErrorReport(Exception ex, bool CloseWindow, string advice = "")
        {
            Log.logger.Error("出现了问题：\n", ex);
            string errorMessage = ReturnExceptionText(ex);
            if (CloseWindow)
            {
                System.Windows.MessageBox.Show($"运行中出现了问题，且在这个错误发生后，工具箱将关闭。\n{advice}若要反馈，请带上链接或日志。\n反馈请勿！请勿截图此页面！\n错误分析原因：\n{errorMessage}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                System.Windows.MessageBox.Show($"运行中出现了问题。但你仍然能够使用工具箱（大概）。\n{advice}若要反馈，请带上链接或日志。\n反馈请勿！请勿截图此页面！\n——————————\n错误分析原因：\n{errorMessage}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (CloseWindow)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }
        public static string ReturnExceptionText(Exception ex)
        {
            if (ex is (System.Net.WebException) || (ex is HttpRequestException) || (ex is HttpProtocolException) || (ex is System.Net.Sockets.SocketException) || (ex is System.Net.HttpListenerException) || (ex is HttpIOException))
            {
                return "网络链接错误，请尝试更换节点，关闭加速器或代理后再试。\n您也可以尝试在官网“常见问题”进行排查。";
            }
            else if (ex is SevenZipException)
            {
                return "解压出现问题，大概率为网络问题。\n请尝试更换节点，关闭加速器或代理后再试。\n您也可以尝试在官网“常见问题”进行排查。";
            }
            else if (ex is FileNotFoundException)
            {
                return "无法找到文件，可能是网络问题，也可能是边狱公司路径出现错误。\n请尝试更换节点，关闭加速器或代理后再试。\n您也可以尝试在官网“常见问题”进行排查。";
            }
            else if (ex is UnauthorizedAccessException)
            {
                return "无权限访问文件，请尝试以管理员身份启动，也可能是你打开了边狱公司？";
            }
            else if (ex is IOException)
            {
                return "文件访问出现问题。\n可能是文件已被边狱公司占用？\n您可以尝试关闭边狱公司。";
            }
            else if (ex is HashException)
            {
                return "文件损坏。\n大概率为网络问题，请尝试更换节点，关闭加速器或代理后再试。\n您也可以尝试在官网“常见问题”进行排查。";
            }
            return "未知错误原因，错误已记录至日志，请查看官网“常见问题”进行排查。\n如果没有解决，请尝试进行反馈。";
        }
        #endregion
        #region 公告系统
        private DispatcherTimer _AnnoTimer;
        private int annoLastTime = 0;
        private bool isInAnno = false;
        private bool hasNewAnno = false;
        private async Task CheckAnno()
        {
            if (!configuation.Settings.announcement.getAnno)
            {
                return;
            }
            try
            {
                string annoText = await GetURLText(string.Format(useAPIEndPoint, "/v2/announcement/get_anno"));
                if (string.IsNullOrEmpty(annoText))
                {
                    return;
                }
                var annoObject = JObject.Parse(annoText);
                if (annoObject["version"]!.Value<int>() <= configuation.Settings.announcement.annoVersion)
                {
                    Log.logger.Info("无新公告。");
                    return;
                }
                else
                {
                    Log.logger.Info("有新公告。");
                }
                string annoContent = annoObject["anno"]!.Value<string>();
                annoContent = annoContent.Replace("\\n", "\n");
                string annoLevel = annoObject["level"]!.Value<string>();
                int annoVersionNew = annoObject["version"]!.Value<int>();
                await ChangeLeftButtonStatu(false);
                await ChangeAnnoText(annoContent);
                configuation.Settings.announcement.annoVersion = annoVersionNew;
                configuation.SaveConfig();
                isInAnno = true;
                hasNewAnno = true;
                await MakeGridStatuExceptSelf(AnnouncementPage);
                if (annoLevel == "normal")
                {
                    await AnnoCountEnd();
                    return;
                }
                else if (annoLevel == "important")
                {
                    annoLastTime = 5;
                }
                else if (annoLevel == "special")
                {
                    annoLastTime = 15;
                }
                _AnnoTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                _AnnoTimer.Tick += AnnoTimer_Tick;
                _AnnoTimer.Start();
            }
            catch (Exception ex)
            {
                ErrorReport(ex, false);
            }
        }
        private async void AnnoTimer_Tick(object? sender, EventArgs e)
        {
            if (annoLastTime > 0)
            {
                annoLastTime -= 1;
                await ChangeAnnoTip(annoLastTime);
            }
            else
            {
                isInAnno = false;
                await AnnoCountEnd();
                _AnnoTimer.Stop();
            }
        }
        private async void AnnoucementButtonClick(object sender, RoutedEventArgs e)
        {
            await AlreadyReadAnno();
        }
        #endregion
        #region 启动器发生器
        private void LauncherSender(object sender, EventArgs e)
        {
            // 获取桌面路径
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            // 快捷方式完整保存路径
            string shortcutPath = Path.Combine(desktopPath, "LimbusCompany with LLC.lnk");

            // 创建WshShell对象
            IWshRuntimeLibrary.WshShell wsh = new IWshRuntimeLibrary.WshShell();

            // 创建快捷方式对象
            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)wsh.CreateShortcut(shortcutPath);

            // 设置快捷方式属性
            shortcut.TargetPath = Path.Combine(currentDir, "LLC_MOD_Toolbox.exe");      // 程序目标路径
            shortcut.Arguments = "-launcher";        // 参数
            shortcut.WorkingDirectory = currentDir;  // 工作目录
            shortcut.Description = "启动边狱公司并检查汉化更新";
            shortcut.IconLocation = Path.Combine(currentDir, "PublicResource", "favicon.ico");
            shortcut.Save();

            Log.logger.Info($"快捷方式已创建: {shortcutPath}");
            MessageBox.Show("快捷方式已创建。\n可在桌面上找到“LimbusCompany with LLC”启动。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void LauncherHelper(object sender, EventArgs e)
        {
            OpenUrl("https://www.zeroasso.top/docs/install/hotupdate");
        }
        #endregion
        #region Loading文本
        private JArray CachedLoadingTexts;
        private async Task CheckLoadingText()
        {
            JObject loadingObject = JObject.Parse(await File.ReadAllTextAsync(Path.Combine(currentDir, "loadingText.json")));
            if (DateTime.TryParseExact(loadingObject["loadingDate"].Value<string>(), "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                DateTime today = DateTime.Now;
                TimeSpan difference = today - parsedDate;
                if (Math.Abs(difference.TotalDays) >= 14)
                {
                    Log.logger.Info("Loading文本需要更新。");
                    var newLoadingObject = await DownloadNewLoadingText();
                    if (newLoadingObject != null)
                    {
                        loadingObject = newLoadingObject;
                    }
                }
            }
            else
            {
                Log.logger.Error("读取Loading文本日期失败。");
            }
            Random random = new();
            JArray loadingTexts = loadingObject["loadingTexts"] as JArray;
            CachedLoadingTexts = loadingTexts;
            int choice = random.Next(0, 100);
            string loadingText = "出现这个文本绝不是因为出了什么问题...";
            if (CachedLoadingTexts == null || CachedLoadingTexts.Count == 0)
            {
                Log.logger.Error("Loading文本为空。");
            }
            else
            {
                if (choice < 25)
                {
                    loadingText = CachedLoadingTexts[1].Value<string>();
                }
                else if (choice < 35)
                {
                    loadingText = CachedLoadingTexts[0].Value<string>();
                }
                else
                {
                    loadingText = CachedLoadingTexts[random.Next(0, CachedLoadingTexts.Count)].Value<string>();
                }
                Log.logger.Info("Loading文本：" + loadingText);
            }
            Log.logger.Info("Loading文本：" + loadingText);
            await ChangeLoadingText(loadingText);
        }
        private void LaunchUpdateLoadingThread()
        {
            Thread updateLoadingThread = new Thread(async () =>
            {
                while (true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(30));
                    await TurnAnotherLoadingText();
                }
            });
            updateLoadingThread.Start();
        }
        private async Task TurnAnotherLoadingText()
        {
            Random random = new();
            int choice = random.Next(0, 100);
            string loadingText = "出现这个文本绝不是因为出了什么问题...";
            if (CachedLoadingTexts == null || CachedLoadingTexts.Count == 0)
            {
                Log.logger.Error("Loading文本为空。");
            }
            else
            {
                if (choice < 15)
                {
                    loadingText = CachedLoadingTexts[1].Value<string>();
                }
                else if (choice < 25)
                {
                    loadingText = CachedLoadingTexts[0].Value<string>();
                }
                else
                {
                    loadingText = CachedLoadingTexts[random.Next(0, CachedLoadingTexts.Count)].Value<string>();
                }
            }
            Log.logger.Info("Loading文本：" + loadingText);
            await ChangeLoadingText(loadingText);
        }
        private async Task<JObject?> DownloadNewLoadingText()
        {
            string loadingText = "";
            if (!configuation.Settings.general.internationalMode)
            {
                loadingText = await GetURLText("https://api.zeroasso.top/v2/loading/get_loading");
            }
            else
            {
                loadingText = await GetURLText("https://cdn-api.zeroasso.top/v2/loading/get_loading");
            }
            if (string.IsNullOrEmpty(loadingText))
            {
                return null;
            }
            JArray loadingArray = JArray.Parse(loadingText);
            var loadingObject = JObject.Parse(await File.ReadAllTextAsync(Path.Combine(currentDir, "loadingText.json")));
            DateTime today = DateTime.Now;
            loadingObject["loadingDate"] = today.ToString("yyyy-MM-dd HH:mm");
            loadingObject["loadingTexts"] = loadingArray;
            File.WriteAllText(Path.Combine(currentDir, "loadingText.json"), loadingObject.ToString());
            return loadingObject;
        }
        #endregion
        #region 字体替换
        private void ExploreFontButtonClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Title = "请选择你的字体",
                Filter = "字体文件 (*.ttf;*.otf)|*.ttf;*.otf|所有文件 (*.*)|*.*",
                Multiselect = false,
            };
            if (fileDialog.ShowDialog() == true)
            {
                FontReplaceTextBox.Text = Path.GetFullPath(fileDialog.FileName);
            }
        }
        private async void PreviewFontButtonClick(object sender, RoutedEventArgs e)
        {
            double size;
            if (double.TryParse(FontSizeTextBox.Text, out double fontSize))
            {
                size = fontSize;
            }
            else
            {
                MessageBox.Show("请输入正确的字体大小。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (IsValidFontFile(FontReplaceTextBox.Text))
            {
                Uri fontUri = new Uri(FontReplaceTextBox.Text);
                FontFamily customFont = new FontFamily(fontUri.AbsoluteUri + "#" + GetFontFamilyName(FontReplaceTextBox.Text));
                await this.Dispatcher.BeginInvoke(() =>
                {
                    this.Resources["GlobalPreviewFont"] = customFont;
                    this.Resources["GlobalPreviewFontSize"] = size;
                    this.Resources["GlobalPreviewSmallFontSize"] = size / 16 * 12;
                });
                MessageBox.Show("已将预览文本切换为自定义字体。\n如果出现部分字显示为默认字体，可能影响游戏内显示。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("请选择正确的字体文件。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void ChangeFontButtonClick(object sender, RoutedEventArgs e)
        {
            string oldFontPath = Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context", "ChineseFont.ttf");
            string oldOTFFontPath = Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context", "ChineseFont.otf");
            string backupFontPath = Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "BackupFont", "ChineseFont.ttf.bak");
            if (!File.Exists(oldFontPath) && !File.Exists(backupFontPath))
            {
                MessageBox.Show("请先安装汉化，然后再进行字体替换。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!IsValidFontFile(FontReplaceTextBox.Text))
            {
                Log.logger.Info("字体文件无效。");
                MessageBox.Show("字体文件无效。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            if (File.Exists(oldFontPath) && !File.Exists(backupFontPath))
            {
                Log.logger.Info("正在备份原字体文件。");
                Directory.CreateDirectory(Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "BackupFont"));
                File.Move(oldFontPath, backupFontPath);
            }
            if (File.Exists(oldFontPath))
            {
                Log.logger.Info("正在删除原TTF字体文件。");
                File.Delete(oldFontPath);
            }
            if (File.Exists(oldOTFFontPath))
            {
                Log.logger.Info("正在删除原OTF字体文件。");
                File.Delete(oldOTFFontPath);
            }
            Log.logger.Info("正在替换字体文件。");
            string extension = new FileInfo(FontReplaceTextBox.Text).Extension;
            File.Copy(FontReplaceTextBox.Text, Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context", $"ChineseFont{extension}"), true);
            MessageBox.Show("字体替换成功。\n启动游戏以应用更改。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            Log.logger.Info("字体替换成功。");
        }
        private void RestoreFontButtonClick(object sender, RoutedEventArgs e)
        {
            string backupFontPath = Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "BackupFont", "ChineseFont.ttf.bak");
            if (File.Exists(backupFontPath))
            {
                string oldFontTTFPath = Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context", "ChineseFont.ttf");
                string oldFontOTFPath = Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context", "ChineseFont.otf");
                if (File.Exists(oldFontTTFPath))
                {
                    File.Delete(oldFontTTFPath);
                }
                if (File.Exists(oldFontOTFPath))
                {
                    File.Delete(oldFontOTFPath);
                }
                File.Move(backupFontPath, oldFontTTFPath);
                MessageBox.Show("字体还原成功。\n启动游戏以应用更改。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                Log.logger.Info("没有找到备份字体文件。");
                MessageBox.Show("没有找到备份字体文件。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private bool IsValidFontFile(string filePath)
        {
            if (filePath == "输入字体路径")
            {
                return false;
            }
            // 1. 检查文件扩展名
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            if (extension != ".ttf" && extension != ".otf" && extension != ".ttc")
            {
                return false;
            }

            // 2. 检查文件是否存在并且可以访问
            if (!File.Exists(filePath))
            {
                return false;
            }

            // 3. 尝试加载字体来验证它是否真的是字体文件
            try
            {
                // 尝试加载字体，如果不是有效的字体文件，这将引发异常
                using (System.Drawing.Text.PrivateFontCollection fontCollection = new System.Drawing.Text.PrivateFontCollection())
                {
                    fontCollection.AddFontFile(filePath);
                    return fontCollection.Families.Length > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private string GetFontFamilyName(string filePath)
        {
            // 获取字体的家族名称
            try
            {
                using (System.Drawing.Text.PrivateFontCollection fontCollection = new System.Drawing.Text.PrivateFontCollection())
                {
                    fontCollection.AddFontFile(filePath);
                    if (fontCollection.Families.Length > 0)
                    {
                        return fontCollection.Families[0].Name;
                    }
                }
            }
            catch { }

            // 如果无法获取字体名称，返回一个默认值
            return Path.GetFileNameWithoutExtension(filePath);
        }
        #endregion
    }
}