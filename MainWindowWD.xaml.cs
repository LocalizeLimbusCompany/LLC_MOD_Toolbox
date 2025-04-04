﻿
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
using log4net;
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
        internal static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(MainWindow));
        private static string? useEndPoint;
        private static string? useAPIEndPoint;
        private static bool useGithub = false;
        //增加对临时节点的适配，后续版本请删除
        private static bool useTempNode = false;
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
            logger.Info($"WPF架构工具箱 版本：{VERSION} 。");
            await DisableGlobalOperations();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            using HttpClient client = new HttpClient
            {
                DefaultRequestVersion = HttpVersion.Version11,
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact
            };
            InitNode();
            await RefreshPage();

            await ChangeEEPic("https://api.zeroasso.top/v2/eepic/get_image");
            CheckToolboxUpdate();
            LoadConfig();
            InitLink();
            CheckLimbusCompanyPath();
            await CheckModInstalled();
            SevenZipBase.SetLibraryPath(Path.Combine(currentDir, "7z.dll"));
            await CheckAnno();
            await EnableGlobalOperations();
            CheckLCBPath();
            await CheckDNS();
            logger.Info("加载流程完成。");
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
            isNewestModVersion = true;
            await RefreshPage();
            logger.Info("开始安装。");
            logger.Info("**********安装信息打印**********");
            logger.Info("本次安装信息：");
            PrintInstallInfo("是否使用Github：", useGithub);
            PrintInstallInfo("Limbus公司目录：", limbusCompanyDir);
            PrintInstallInfo("Limbus公司游戏目录：", limbusCompanyGameDir);
            PrintInstallInfo("节点列表数量：", nodeList.Count);
            PrintInstallInfo("使用节点", useEndPoint);
            PrintInstallInfo("灰度测试状态：", greytestStatus);
            logger.Info("**********安装信息打印**********");
            if (useEndPoint == null)
            {
                logger.Warn("下载节点为空。");
            }
            installPhase = 0;
            //保留检测 Limbus Company 是否开启
            Process[] limbusProcess = Process.GetProcessesByName("LimbusCompany");
            if (limbusProcess.Length > 0)
            {
                logger.Warn("LimbusCompany仍然开启。");
                MessageBoxResult DialogResult = System.Windows.MessageBox.Show("检测到 Limbus Company 仍然处于开启状态！\n建议您关闭游戏后继续安装模组。\n若您已经关闭了 Limbus Company，请点击确定，否则请点击取消返回。", "警告", MessageBoxButton.OKCancel, MessageBoxImage.Hand);
                if (DialogResult == MessageBoxResult.Cancel)
                {
                    //部分情况下用户开启Limbus Company时也可以安装，但是依然推荐关闭后再安装资源
                    //await StopInstall();
                    return;
                }
                logger.Warn("用户选择无视警告。");
            }
            try
            {
                StartProgressTimer();
                if (!greytestStatus)
                {
                    await InstallResource();
                }
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
            logger.Info("安装完成。");
            MessageBoxResult RunResult = new();
            if (isNewestModVersion)
            {
                RunResult = MessageBox.Show("没有检测到新版本汉化资源！\n您的汉化资源已经为最新。\n点击“确定”立刻运行边狱公司。\n点击“取消”关闭弹窗。\n加载时请耐心等待。", "完成", MessageBoxButton.OKCancel);
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
                    logger.Error("出现了问题： ", ex);
                    MessageBox.Show("出现了问题。\n" + ex.ToString());
                }
            }
            isInstalling = false;
            progressPercentage = 0;
            await ChangeProgressValue(0);
            await RefreshPage();
        }
        private async Task InstallResource()
        {
            await Task.Run(async () =>
            {
                logger.Info("开始安装资源。");
                installPhase = 4;
                //使用新的自定义语言地址
                string langDir = limbusCompanyDir + "/LimbusCompany_Data/Lang";
                //确保lang路径是否有效
                Directory.CreateDirectory(langDir);
                //保留版本验证
                //移动原先version.json至LimbusCompany根目录下，更名为LLC_CN_version.json
                string versionJson = limbusCompanyDir + "/LLC_CN_version.json";
                int localVersion = 0;
                int remoteVersion = 0;
                bool needUpdate = false;

                string remoteVersionRaw = "";

                //适配临时节点，后续版本请删除
                if (useTempNode)
                {
                    logger.Info("使用临时节点，取消验证版本信息。");
                    needUpdate = true;
                }
                else if (!File.Exists(versionJson))
                {
                    //version.json不存在，下载新的汉化资源
                    logger.Info("version.json不存在。");
                    localVersion = -1;
                    needUpdate = true;
                }
                //获取最新版本
                if(useTempNode)
                {
                    //适配临时节点，后续版本请删除
                }
                else if (!useGithub)
                {
                    //从零协会官方 API获取版本信息;
                    remoteVersionRaw = await GetURLText("https://api.zeroasso.top/v2/resource/get_version");
                    var remoteVersionObject = JObject.Parse(remoteVersionRaw);
                    remoteVersion = int.Parse(remoteVersionObject["resource_version"].ToString());
                }
                else
                {
                    //从Github获取版本信息;
                    remoteVersionRaw = await GetURLText("https://api.github.com/repos/LocalizeLimbusCompany/LLC_Release/releases/latest");
                    var remoteVersionObject = JObject.Parse(remoteVersionRaw);
                    remoteVersion = int.Parse(remoteVersionObject["tag_name"].ToString());
                }
                //在确保本地拥有版本信息的情况下，获取本地资源版本
                if(File.Exists(versionJson))
                {
                    var versionObject = JObject.Parse(File.ReadAllText(versionJson));
                    localVersion = int.Parse(versionObject["resource_version"].ToString());
                }
                logger.Info("本地资源版本：" + localVersion + "，远程资源版本：" + remoteVersion);
                if (localVersion < remoteVersion)
                {
                    needUpdate = true;
                }

                //判断是否需要更新
                if (needUpdate)
                {
                    logger.Info("资源需要更新。");
                    isNewestModVersion = false;
                    //获取下载文件名称
                    //如果可以请配置github中的资源下载名称与零协会节点中的一致，这样可以删去此处判断
                    string download_file_name = "";
                    if (useTempNode)
                    {
                        //适配临时节点，后续版本请删除
                        download_file_name = "0404Temp.zip";

                    }
                    else if (!useGithub)
                    {
                        //未实现
                        //请在配置新的零协会节点资源下载地址后修改
                        download_file_name = "";
                    }
                    else
                    {
                        //未实现
                        //请在配置新的Github资源下载地址后修改
                        download_file_name = "";

                    }
                    //下载资源
                    try
                    {
                        if (useTempNode)
                        {
                            //临时节点下载逻辑
                            //适配临时节点，后续版本请删除
                            logger.Info("开始下载资源。");
                            await DownloadFileAutoAsync(download_file_name, Path.Combine(limbusCompanyDir, "TempResources.zip"));
                            Unarchive(Path.Combine(limbusCompanyDir, "TempResources.zip"), limbusCompanyDir);
                            File.Delete(Path.Combine(limbusCompanyDir, "TempResources.zip"));
                            logger.Info("资源下载完成。");

                        }
                        else
                        {
                            //修改Github资源的下载逻辑与零协会节点相同
                            //如果使用新的仓库发布资源，请修改nodeLsit.json中的github的endpoint
                            //需要配置下载的资源未LLC_CN资源包，不要附带其他内容

                            //目前解压可能有问题，我以前没用过这个包
                            logger.Info("开始下载资源。");
                            await DownloadFileAutoAsync(download_file_name, Path.Combine(limbusCompanyDir, $"LimbusLocalize_Resource_{remoteVersion}.7z"));
                            Unarchive(Path.Combine(limbusCompanyDir, $"LimbusLocalize_Resource_{remoteVersion}.7z"), langDir);
                            File.Delete(Path.Combine(limbusCompanyDir, $"LimbusLocalize_Resource_{remoteVersion}.7z"));
                            logger.Info("资源下载完成。");

                            //写入config.json，选择下载的资源包
                            string configJson = langDir + "/config.json";
                            JObject config = new JObject()
                            {
                                {"lang","LLC_CN"},
                            };
                            File.WriteAllText(configJson, config.ToString(Formatting.Indented));
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Info("资源出错，信息如下："+ex.ToString());
                        //未实现
                        //处理下载资源时可能遇到的错误
                    }

                    //写入新的版本信息
                    if (useTempNode)
                    {
                        //适配临时节点，后续版本请删除
                        //手动写入版本信息
                        JObject version_data = new JObject()
                        {
                            {"notice", "临时资源，请关注零协会动态更新至最新资源"},
                            {"resource_version", -1},
                            {"version", "0.7.3"},
                        };
                        File.WriteAllText(versionJson, version_data.ToString(Formatting.Indented));

                    }
                    else if (!useGithub)
                    {
                        File.WriteAllText(versionJson, remoteVersionRaw);
                        
                    }
                    else
                    {
                        //未实现，在确定新的github资源格式后实现
                        //手动写入版本信息
                        JObject version_data = new JObject()
                        {
                            {"notice", ""},
                            {"resource_version", remoteVersion},
                            {"version", ""},
                        };
                        File.WriteAllText(versionJson, version_data.ToString(Formatting.Indented));
                    }
                }
            });
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
                if (Node.IsDefault == true)
                {
                    defaultEndPoint = Node.Endpoint;
                    if(Node.Name == "Github直连")
                    {
                        useGithub = true;
                    }
                    else if(Node.Name == "零协会临时节点")
                    {
                        //适配临时节点，后续版本请删除
                        useTempNode = true;
                    }
                }
                NodeCombobox.Items.Add(Node.Name);
            }
            //NodeCombobox.Items.Add("Github直连");
            // API
            APICombobox.Items.Add("恢复默认");
            foreach (var api in apiList)
            {
                if (api.IsDefault == true)
                {
                    defaultAPIEndPoint = api.Endpoint;
                    useAPIEndPoint = defaultAPIEndPoint;
                }
                APICombobox.Items.Add(api.Name);
            }
            logger.Info("API数量：" + apiList.Count);
            logger.Info("节点数量：" + nodeList.Count);
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
            logger.Info("选择节点。");
            if (nodeComboboxText != string.Empty)
            {
                if (nodeComboboxText == "恢复默认")
                {
                    useEndPoint = string.Empty;
                    useGithub = false;
                    useTempNode = false;
                    logger.Info("已恢复默认Endpoint。");
                }
                else if (nodeComboboxText == "Github直连")
                {
                    logger.Info("选择Github节点。");
                    System.Windows.MessageBox.Show("如果您没有使用代理软件（包括Watt Toolkit）\n请不要使用此节点。\nGithub由于不可抗力因素，对国内网络十分不友好。\n如果您是国外用户，才应该使用此选项。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    useEndPoint = string.Empty;
                    useGithub = true;
                }else if (nodeComboboxText == "零协会临时节点")
                {
                    //适配临时节点，后续版本请删除
                    System.Windows.MessageBox.Show("此节点为临时汉化节点，请关注零协会动态获取最新信息", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    useEndPoint = string.Empty;
                    useGithub = false;
                    useTempNode = true;
                }
                else
                {
                    useEndPoint = FindNodeEndpoint(nodeComboboxText);
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
        private async void APIComboboxSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!useGithub)
            {
                string apiComboboxText = await GetAPIComboboxText();
                logger.Info("选择API节点。");
                if (apiComboboxText != string.Empty)
                {
                    if (apiComboboxText == "恢复默认")
                    {
                        useAPIEndPoint = defaultAPIEndPoint;
                        logger.Info("已恢复默认API Endpoint。");
                    }
                    else
                    {
                        useAPIEndPoint = FindAPIEndpoint(apiComboboxText);
                        logger.Info("当前API Endpoint：" + useAPIEndPoint);
                        System.Windows.MessageBox.Show("切换成功。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    logger.Info("APIComboboxText 为 null。");
                }
            }
            else if (APPChangeAPIUI == false)
            {
                await SetAPIComboboxText("恢复默认");
                logger.Info("已开启Github。无法切换API。");
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
                logger.Info($"{promptInfo}：空");
            }
            else
            {
                logger.Info($"{promptInfo}{someObject}");
            }

        }
        private static bool check_path_success(string path)
        {
            string exe_path = Path.Combine(path, "LimbusCompany.exe");
            string data_path = Path.Combine(path, "LimbusCompany_Data");
            return File.Exists(exe_path) && File.Exists(data_path);
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
                //确认路径是否正确
                MessageBoxResult path_check_result = System.Windows.MessageBox.Show($"这是您的边狱公司地址吗？\n{limbusCompanyDir}", "检查路径", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (path_check_result == MessageBoxResult.No)
                {
                    try
                    {
                        //用户否认注册表中的路径，使用libraryfolders进行二次查找
                        string steamPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null) as string ?? string.Empty;
                        string libraryFoldersPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
                        string[] lines = File.ReadAllLines(libraryFoldersPath);
                        foreach (string line in lines)
                        {
                            if (line.Contains("\t\"path\"\t\t"))
                            {
                                string libraryPath = line.Split('\t')[4].Trim('\"').Replace("\\\\","\\");
                                string manifest_path = Path.Combine(libraryPath, "steamapps", "appmanifest_1973530.acf");
                                if (File.Exists(manifest_path))
                                {
                                    //在安装目录中检查到manifes文件，
                                    limbusCompanyDir = Path.Combine(libraryPath, "steamapps", "common", "Limbus Company");
                                    path_check_result = System.Windows.MessageBox.Show($"这是您的边狱公司地址吗？\n{limbusCompanyDir}", "检查路径", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                    //无需继续查找，终止循环
                                    break;
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        //无法通过使用libraryfolders查找路径，提示手动选择
                        path_check_result = MessageBoxResult.No;//以防万一
                        MessageBox.Show("找不到steam路径，请手动选择Limbus Company目录");
                    }

                }
                if (path_check_result != MessageBoxResult.Yes)
                {
                    //手动选择 Limbus Company 目录
                    logger.Warn("未能找到 Limbus Company 目录，手动选择模式。");
                    System.Windows.MessageBox.Show("请选择 Limbus Company 目录", "提示");
                    var fileDialog = new OpenFileDialog
                    {
                        Title = "请选择你的边狱公司游戏文件",
                        Multiselect = false,
                        InitialDirectory = limbusCompanyDir ?? "c://",
                        Filter = "LimbusCompany.exe|LimbusCompany.exe",
                        FileName = "LimbusCompany.exe"
                    };
                    while (true)
                    {

                        if (fileDialog.ShowDialog() == true)
                        {
                            limbusCompanyDir = Path.GetDirectoryName(fileDialog.FileName) ?? limbusCompanyDir;
                            if (check_path_success(limbusCompanyDir))
                            {
                                logger.Error("选择了错误目录，关闭。");
                                System.Windows.MessageBox.Show("选择目录有误，没有在当前目录找到游戏。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                                if(System.Windows.MessageBox.Show("是否再次选择路径，否将会退出程序", "问题", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                                {
                                    System.Windows.Application.Current.Shutdown();
                                }
                            }
                            else
                            {
                                logger.Info("找到了正确目录。");
                                path_check_result = MessageBoxResult.Yes;
                                break;
                            }
                        }
                        else
                        {
                            logger.Error("用户取消选择路径，关闭。");
                            System.Windows.MessageBox.Show("需要选择。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                            System.Windows.Application.Current.Shutdown();
                        }
                    }

                }
                if (path_check_result == MessageBoxResult.Yes)
                {
                    //自动获取路径完成
                    logger.Info("用户确认路径。");
                    ChangeLCBPathConfig(limbusCompanyDir);
                    ChangeSkipPathCheckConfig(true);
                }
                else
                {
                    if (string.IsNullOrEmpty(limbusCompanyDir))
                    {
                        logger.Warn("未能找到 Limbus Company 目录，手动选择模式。");
                        System.Windows.MessageBox.Show("未能找到 Limbus Company 目录。请手动选择。", "提示");
                    }
                    else
                    {
                        logger.Warn("用户否认 Limbus Company 目录正确性。");
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
                        logger.Error("选择了错误目录，关闭。");
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
            limbusCompanyGameDir = Path.Combine(limbusCompanyDir, "LimbusCompany.exe");
            logger.Info("边狱公司路径：" + limbusCompanyDir);
        }
        /// <summary>
        /// 获取最新版汉化模组哈希
        /// </summary>
        /// <returns>返回Sha256</returns>
        private static async Task<string> GetLimbusLocalizeHash()
        {
            string HashRaw = await GetURLText(string.Format(useEndPoint ?? defaultEndPoint, "LimbusLocalizeHash.json"));
            dynamic JsonObject = JsonConvert.DeserializeObject(HashRaw);
            if (JsonObject == null)
            {
                logger.Error("获取模组Hash失败。");
                throw new HashException();
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
            logger.Info($"计算位置为 {filePath} 的文件的Hash结果为：{BitConverter.ToString(hashBytes).Replace("-", "").ToLower()}");
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
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
                progressPercentage = (float)((installPhase - 1) * 25 + e.ProgressPercentage * 0.25);
            }
        }
        private void NewOnDownloadProgressCompleted(object? sender, AsyncCompletedEventArgs e)
        {
            if (installPhase != 0)
            {
                progressPercentage = installPhase * 25;
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
            logger.Info($"下载 {Url} 到 {Path}");
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
            logger.Info($"自动选择下载节点式下载文件 文件: {File}  路径: {Path}");
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
        private static async Task<string> GetLatestLimbusLocalizeVersion(bool useGithub)
        {
            logger.Info("获取模组标签。");
            string raw;
            if (!useGithub)
            {
                raw = await GetURLText(string.Format(useAPIEndPoint, "repos/LocalizeLimbusCompany/LocalizeLimbusCompany/releases"));
            }
            else
            {
                raw = await GetURLText("https://api.github.com/repos/LocalizeLimbusCompany/LocalizeLimbusCompany/releases");
            }
            var output = JArray.Parse(raw)[0].Value<JObject>();
            if (output.TryGetValue("tag_name", out var jtag))
            {
                var latestVersionTag = jtag.Value<string>();
                logger.Info($"汉化模组最后标签为： {latestVersionTag}");
                return latestVersionTag;
            }
            logger.Info("未能获取到汉化模组最后标签。");
            return string.Empty;
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
                logger.Info($"获取 {Url} 文本内容。");
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
            logger.Info("打开了网址：" + Url);
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
                logger.Info("正在检查工具箱更新。");
                string raw = await GetURLText(string.Format(useAPIEndPoint, "repos/LocalizeLimbusCompany/LLC_Mod_Toolbox/releases/latest"));
                var JsonObject = JObject.Parse(raw);
                string latestReleaseTagRaw = JsonObject["tag_name"].Value<string>();
                string latestReleaseTag = latestReleaseTagRaw.Remove(0, 1);
                logger.Info("最新安装器tag：" + latestReleaseTag);
                if (new Version(latestReleaseTag) > Assembly.GetExecutingAssembly().GetName().Version)
                {
                    logger.Info("安装器存在更新。");
                    System.Windows.MessageBox.Show("安装器存在更新。\n点击确定进入官网下载最新版本工具箱", "更新提醒", MessageBoxButton.OK, MessageBoxImage.Warning);
                    OpenUrl("https://www.zeroasso.top/docs/install/autoinstall");
                    System.Windows.Application.Current.Shutdown();
                }
                logger.Info("没有更新。");
            }
            catch (Exception ex)
            {
                logger.Error("检查安装器更新出现问题。", ex);
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
                    raw = await GetURLText(string.Format(useAPIEndPoint, "repos/LocalizeLimbusCompany/LLC_ChineseFontAsset/releases/latest"));
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
                logger.Error("出现了问题。\n" + ex.ToString());
            }
            return new FontUpdateResult(null, false);
        }
        private async Task CheckModInstalled()
        {
            try
            {
                logger.Info("正在检查模组是否安装。");
                if (File.Exists(limbusCompanyDir + "\\BepInEx\\plugins\\LLC\\LimbusLocalize_BIE.dll"))
                {
                    logger.Info("模组已安装。");
                    await ChangeAutoInstallButton();
                }
                else
                {
                    logger.Info("模组未安装。");
                }
            }
            catch (Exception ex)
            {
                logger.Error("出现问题。" + ex.ToString());
            }
        }
        public void CheckLCBPath()
        {
            logger.Info("检查边狱公司路径。");
            if (!Path.Exists(limbusCompanyDir))
            {
                logger.Error("边狱公司目录不存在。");
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
                    logger.Error("边狱公司目录不正确。");
                    FixLCBPath();
                }
            }
        }
        public void FixLCBPath()
        {
            ChangeSkipPathCheckConfig(false);
            var fileDialog = new OpenFileDialog
            {
                Title = "请选择你的边狱公司游戏文件",
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
            logger.Info("点击删除模组");
            MessageBoxResult result = System.Windows.MessageBox.Show("删除后你需要重新安装汉化补丁。\n确定继续吗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                logger.Info("确定删除模组。");
                try
                {
                    await DisableGlobalOperations();
                    DeleteBepInEx();
                    DeleteMelonLoader();
                    await EnableGlobalOperations();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("删除过程中出现了一些问题： " + ex.ToString(), "警告");
                    logger.Error("删除过程中出现了一些问题： ", ex);
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
            logger.Info("Z-TECH 灰度测试客户端程序 v3.0 启动。");
            await DisableGlobalOperations();
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
                string tokenUrl = $"https://api.zeroasso.top/v2/grey_test/get_token?code={token}";
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
                        logger.Info("Token状态正常。");
                    }
                    else
                    {
                        logger.Info("Token已停止测试。");
                        System.Windows.MessageBox.Show("Token已停止测试。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        await EnableGlobalOperations();
                        return;
                    }
                    string note = tokenObject["note"].Value<string>();
                    logger.Info($"Token：{token}\n备注：{note}");
                    await ChangeLogoToTest();
                    MessageBox.Show(
                        $"目前Token有效。\n-------------\nToken信息：\n秘钥：{token}\n备注：{note}\n-------------\n灰度测试模式已开启。\n请在自动安装安装此秘钥对应版本汉化。\n秘钥信息请勿外传。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    greytestStatus = true;
                    greytestUrl = $"https://api.zeroasso.top/v2/grey_test/get_file?code={token}";
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
                //目前实现为直接解压灰度测试资源到lang文件夹
                string langDir = limbusCompanyDir + "/LimbusCompany_Data/Lang";
                logger.Info("灰度测试模式已开启。开始安装灰度模组。");
                installPhase = 3;
                await DownloadFileAsync(greytestUrl, limbusCompanyDir + "/LimbusLocalize_Dev.7z");
                Unarchive(limbusCompanyDir + "/LimbusLocalize_Dev.7z", langDir);
                File.Delete(limbusCompanyDir + "/LimbusLocalize_Dev.7z");
                logger.Info("灰度模组安装完成。");
            });
        }
        #endregion
        #region 程序配置
        public class LLCMTConfig
        {
            public bool CskipLCBPathCheck { get; set; }
            public string? CLCBPath { get; set; }
            public int? CAnnoVersion { get; set; }
        }
        private static bool skipLCBPathCheck = false;
        private static string? LCBPath = string.Empty;
        private static int? AnnoVersion = 0;
        private static readonly string configPath = Path.Combine(currentDir, "config.json");
        private static void LoadConfig()
        {
            logger.Info("加载程序配置。");
            try
            {
                if (File.Exists(configPath))
                {
                    string configContent = File.ReadAllText(configPath);
                    LLCMTConfig LLCMTconfig = JsonConvert.DeserializeObject<LLCMTConfig>(configContent) ?? throw new FileNotFoundException("配置文件未找到。");
                    skipLCBPathCheck = LLCMTconfig.CskipLCBPathCheck;
                    LCBPath = LLCMTconfig.CLCBPath;
                    AnnoVersion = LLCMTconfig.CAnnoVersion;
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
                    LLCMTConfig LLCMTconfig = JsonConvert.DeserializeObject<LLCMTConfig>(configContent) ?? throw new FileNotFoundException("配置文件未找到。");
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
        private static void ChangeLCBPathConfig(string? updatedLCBPath)
        {
            logger.Info($"改变边狱公司路径配置，Value： {updatedLCBPath}");
            try
            {
                if (string.IsNullOrEmpty(updatedLCBPath))
                {
                    logger.Error("修改的值为Null。");
                    return;
                }
                if (File.Exists(configPath))
                {
                    string configContent = File.ReadAllText(configPath);
                    LLCMTConfig LLCMTconfig = JsonConvert.DeserializeObject<LLCMTConfig>(configContent) ?? throw new FileNotFoundException("配置文件未找到。");
                    LLCMTconfig.CLCBPath = updatedLCBPath;
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
        private static void ChangeAnnoVersionConfig(int? updatedVersion)
        {
            logger.Info($"改变公告版本配置，Value： {updatedVersion}");
            try
            {
                if (updatedVersion == null)
                {
                    logger.Error("修改的值为Null。");
                    return;
                }
                if (File.Exists(configPath))
                {
                    string configContent = File.ReadAllText(configPath);
                    LLCMTConfig LLCMTconfig = JsonConvert.DeserializeObject<LLCMTConfig>(configContent) ?? throw new FileNotFoundException("配置文件未找到。");
                    LLCMTconfig.CAnnoVersion = updatedVersion;
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
        #region 开关模组
        // 我也不知道为什么这个功能这么多人想要，不过既然那么多人要，那我就写了
        private void ChangeStatuButtonClick(object sender, RoutedEventArgs e)
        {
            logger.Info("点击开关模组按钮。");
            if (!File.Exists(limbusCompanyDir + "/winhttp.dll") && !File.Exists(limbusCompanyDir + "/winhttp.dll.disabled"))
            {
                logger.Info("模组未安装。");
                System.Windows.MessageBox.Show("模组未安装。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (File.Exists(limbusCompanyDir + "/winhttp.dll.disabled"))
            {
                logger.Info("检测到 winhttp.dll.disabled，进行开启。");
                File.Move(limbusCompanyDir + "/winhttp.dll.disabled", limbusCompanyDir + "/winhttp.dll");
                System.Windows.MessageBox.Show("模组已开启。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (File.Exists(limbusCompanyDir + "/winhttp.dll"))
            {
                logger.Info("检测到 winhttp.dll，进行关闭。");
                File.Move(limbusCompanyDir + "/winhttp.dll", limbusCompanyDir + "/winhttp.dll.disabled");
                System.Windows.MessageBox.Show("模组已关闭。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
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
        private bool hasVergil = false;
        private bool alreadyHasVergil = false;
        private async Task InitGacha()
        {
            await DisableGlobalOperations();
            string gachaText = await GetURLText("https://download.zeroasso.top/wiki/wiki_personal.json");
            if (string.IsNullOrEmpty(gachaText))
            {
                logger.Error("初始化失败。");
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
            logger.Info("人格数量：" + personalInfos.Count);
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
            logger.Info("点击抽卡。");
            await CollapsedAllGacha();
            if (isInitGachaFailed)
            {
                logger.Info("初始化失败。");
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
                    logger.Info("人格数量不足。\n尝试重新生成。");
                    personals = GenPersonalList();
                }
                await StartChangeLabel(personals);
            }
            catch (Exception ex)
            {
                logger.Info("出现了问题。", ex);
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
                    if (hasVergil && random.Next(1,10) == 1)
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
            logger.Info("开始转换文本。");
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
            logger.Error("出现了问题：\n", ex);
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
        private async Task CheckAnno()
        {
            try
            {
                string annoText = await GetURLText("https://api.zeroasso.top/v2/announcement/get_anno");
                if (string.IsNullOrEmpty(annoText))
                {
                    return;
                }
                var annoObject = JObject.Parse(annoText);
                if (annoObject["version"]!.Value<int>() <= AnnoVersion)
                {
                    logger.Info("无新公告。");
                    return;
                }
                else
                {
                    logger.Info("有新公告。");
                }
                string annoContent = annoObject["anno"]!.Value<string>();
                annoContent = annoContent.Replace("\\n", "\n");
                string annoLevel = annoObject["level"]!.Value<string>();
                int annoVersionNew = annoObject["version"]!.Value<int>();
                await ChangeLeftButtonStatu(false);
                await ChangeAnnoText(annoContent);
                ChangeAnnoVersionConfig(annoVersionNew);
                isInAnno = true;
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
    }
}