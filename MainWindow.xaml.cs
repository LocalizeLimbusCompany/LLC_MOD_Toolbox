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
using log4net;
using Microsoft.Win32;
using Newtonsoft.Json;
using SevenZip;
using SimpleJSON;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        public static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        public static string useEndPoint = string.Empty;
        public static bool UseMirrorGithub = false;
        public static string limbusCompanyDir = string.Empty;
        public static string limbusCompanyGameDir = string.Empty;
        public static string currentDir = AppDomain.CurrentDomain.BaseDirectory;
        public static List<Node> NodeList = new();
        public static string DefaultEndPoint = "https://node.zeroasso.top/d/od/";
        public static int InstallPhase = 0;
        private DispatcherTimer progressTimer;
        private float progressPercentage = 0;
        public const string VERSION = "0.7.0";
        public MainWindow() => InitializeComponent();

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            logger.Info("——————————");
            logger.Info("工具箱已进入加载流程。");
            logger.Info("We have a lift off.");
            logger.Info("WPF架构工具箱 版本：" + VERSION + " 。");
            RefreshPage();
            CheckToolboxUpdate(VERSION);
            InitNode();
            CheckLimbusCompanyPath();
            progressTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.05)
            };
            progressTimer.Tick += ProgressTime_Tick;
            SevenZipBase.SetLibraryPath(Path.Combine(currentDir, "7z.dll"));
            logger.Info("加载流程完成。");
        }
        #region 安装功能
        private async Task StopInstall()
        {
            isInstalling = false;
            InstallPhase = 0;
            progressPercentage = 0;
            await ChangeProgressValue(progressPercentage);
            RefreshPage();
        }
        private async Task InstallBepInEx()
        {
            await Task.Run(async () =>
            {
                logger.Info("已进入安装BepInEx流程。");
                InstallPhase = 1;
                if (!File.Exists(limbusCompanyDir + "/BepInEx/core/BepInEx.Core.dll"))
                {
                    if (!UseMirrorGithub)
                    {
                        System.Windows.MessageBox.Show("如果你安装了杀毒软件，接下来可能会提示工具箱正在修改关键dll。\n允许即可。如果不信任汉化补丁，可以退出本程序。", "警告");
                        string BepInExZipPath = Path.Combine(limbusCompanyDir, "BepInEx-IL2CPP-x64.7z");
                        logger.Info("BepInEx Zip目录： " + BepInExZipPath);
                        await DownloadFileAutoAsync("BepInEx-IL2CPP-x64.7z", BepInExZipPath);
                        logger.Info("开始解压 BepInEx zip。");
                        Unarchive(BepInExZipPath, limbusCompanyDir);
                        logger.Info("解压完成。删除 BepInEx zip。");
                        File.Delete(BepInExZipPath);
                    }
                    if (UseMirrorGithub)
                    {
                        System.Windows.MessageBox.Show("如果你安装了杀毒软件，接下来可能会提示工具箱正在修改关键dll。\n允许即可。如果不信任汉化补丁，可以退出本程序。", "警告");
                        string BepInExZipPath = Path.Combine(limbusCompanyDir, "BepInEx-IL2CPP-x64.7z");
                        logger.Info("BepInEx Zip目录： " + BepInExZipPath);
                        await DownloadFileAsync("https://mirror.ghproxy.com/https://github.com/LocalizeLimbusCompany/BepInEx_For_LLC/releases/download/v6.0.1-LLC/BepInEx-IL2CPP-x64-6.0.1.7z", BepInExZipPath);
                        logger.Info("开始解压 BepInEx zip。");
                        Unarchive(BepInExZipPath, limbusCompanyDir);
                        logger.Info("解压完成。删除 BepInEx zip。");
                        File.Delete(BepInExZipPath);
                    }
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
                InstallPhase = 2;
                string modsDir = limbusCompanyDir + "/BepInEx/plugins/LLC";
                Directory.CreateDirectory(modsDir);
                string tmpchineseZipPath = Path.Combine(limbusCompanyDir, "tmpchinese_BIE.7z");
                string tmpchinese = modsDir + "/tmpchinesefont";
                var LastWriteTime = File.Exists(tmpchinese) ? new FileInfo(tmpchinese).LastWriteTime.ToString("yyMMdd") : string.Empty;
                if (!UseMirrorGithub)
                {
                    if (CheckChineseFontAssetUpdate(LastWriteTime, false, out _))
                    {
                        await DownloadFileAutoAsync("tmpchinesefont_BIE.7z", tmpchineseZipPath);
                        logger.Info("解压 tmp zip 中。");
                        Unarchive(tmpchineseZipPath, limbusCompanyDir);
                        logger.Info("删除 tmp zip 。");
                        File.Delete(tmpchineseZipPath);
                    }
                }
                else
                {
                    if (CheckChineseFontAssetUpdate(LastWriteTime, false, out var latestTag))
                    {
                        await DownloadFileAsync("https://mirror.ghproxy.com/https://github.com/LocalizeLimbusCompany/LLC_ChineseFontAsset/releases/download/" + latestTag + "/tmpchinesefont_BIE_" + latestTag + ".7z", tmpchineseZipPath);
                        logger.Info("解压 tmp zip 中。");
                        Unarchive(tmpchineseZipPath, limbusCompanyDir);
                        logger.Info("删除 tmp zip 。");
                        File.Delete(tmpchineseZipPath);
                    }
                }
            });
        }
        private async Task InstallMod()
        {
            await Task.Run(async () =>
            {
                logger.Info("开始安装模组。");
                InstallPhase = 3;
                string modsDir = limbusCompanyDir + "/BepInEx/plugins/LLC";
                string limbusLocalizeDllPath = modsDir + "/LimbusLocalize_BIE.dll";
                string limbusLocalizeZipPath = Path.Combine(limbusCompanyDir, "LimbusLocalize_BIE.7z");
                string latestLLCVersion = string.Empty;
                string currentVersion = string.Empty;
                if (!UseMirrorGithub)
                {
                    latestLLCVersion = GetLatestLimbusLocalizeVersion();
                    latestLLCVersion = GetLatestLimbusLocalizeVersion();
                    logger.Info("最后模组版本： " + latestLLCVersion);
                    if (File.Exists(limbusLocalizeDllPath))
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(limbusLocalizeDllPath);
                        currentVersion = versionInfo.ProductVersion;
                        if (new System.Version(currentVersion) < new System.Version(latestLLCVersion.Remove(0, 1)))
                        {
                            await DownloadFileAutoAsync("LimbusLocalize_BIE_" + latestLLCVersion + ".7z", limbusLocalizeZipPath);
                            if (GetLimbusLocalizeHash() != CalculateSHA256(limbusLocalizeZipPath))
                            {
                                logger.Error("校验Hash失败。");
                                System.Windows.MessageBox.Show("校验Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "校验失败");
                                Close();
                            }
                            logger.Info("解压模组本体 zip 中。");
                            Unarchive(limbusLocalizeZipPath, limbusCompanyDir);
                            logger.Info("删除模组本体 zip 。");
                            File.Delete(limbusLocalizeZipPath);
                        }
                    }
                    else
                    {
                        await DownloadFileAutoAsync("LimbusLocalize_BIE_" + latestLLCVersion + ".7z", limbusLocalizeZipPath);
                        if (GetLimbusLocalizeHash() != CalculateSHA256(limbusLocalizeZipPath))
                        {
                            logger.Error("校验Hash失败。");
                            System.Windows.MessageBox.Show("校验Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "校验失败");
                            Close();
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
                }
                if (UseMirrorGithub)
                {
                    latestLLCVersion = GetLatestLimbusLocalizeVersion();
                    latestLLCVersion = GetLatestLimbusLocalizeVersion();
                    logger.Info("最后模组版本： " + latestLLCVersion);
                    if (File.Exists(limbusLocalizeDllPath))
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(limbusLocalizeDllPath);
                        currentVersion = versionInfo.ProductVersion;
                        if (new System.Version(currentVersion) < new System.Version(latestLLCVersion.Remove(0, 1)))
                        {
                            await DownloadFileAutoAsync("https://mirror.ghproxy.com/https://github.com/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/download/" + currentVersion + "/LimbusLocalize_BIE_" + currentVersion + ".7z", limbusLocalizeZipPath);
                            logger.Info("解压模组本体 zip 中。");
                            Unarchive(limbusLocalizeZipPath, limbusCompanyDir);
                            logger.Info("删除模组本体 zip 。");
                            File.Delete(limbusLocalizeZipPath);
                        }
                    }
                    else
                    {
                        await DownloadFileAutoAsync("https://mirror.ghproxy.com/https://github.com/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/download/" + currentVersion + "/LimbusLocalize_BIE_" + currentVersion + ".7z", limbusLocalizeZipPath);
                        logger.Info("解压模组本体 zip 中。");
                        Unarchive(limbusLocalizeZipPath, limbusCompanyDir);
                        logger.Info("删除模组本体 zip 。");
                        File.Delete(limbusLocalizeZipPath);
                    }
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
            foreach (var Node in NodeList)
            {
                if (Node.IsDefault == true)
                {
                    DefaultEndPoint = Node.Endpoint;
                }
                NodeCombobox.Items.Add(Node.Name);
            }
        }
        public void ReadNodeJsonFile()
        {
            string NodeListRaw = File.ReadAllText(currentDir + "/NodeList.json");
            NodeList = JsonConvert.DeserializeObject<List<Node>>(NodeListRaw) ?? new List<Node>();
        }
        private string FindEndpoint(string Name)
        {
            foreach (var Node in NodeList)
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
                    logger.Info("已恢复默认Endpoint。");
                }
                else
                {
                    useEndPoint = FindEndpoint(nodeComboboxText);
                    logger.Info("当前Endpoint：" + useEndPoint);
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
        private void CheckLimbusCompanyPath()
        {
#pragma warning disable CS8601 // 引用类型赋值可能为 null。
            limbusCompanyDir = FindlimbusCompanyDirectory();
#pragma warning restore CS8601 // 引用类型赋值可能为 null。
            string CheckFile = Path.Combine(currentDir, "SkipLimbusPathCheck");
            MessageBoxResult CheckLCBPathResult = MessageBoxResult.OK;
            if (!File.Exists(CheckFile))
            {
                if (!string.IsNullOrEmpty(limbusCompanyDir))
                {
                    CheckLCBPathResult = System.Windows.MessageBox.Show("这是您的边狱公司地址吗？\n" + limbusCompanyDir, "检查路径", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
                    if (File.Exists(limbusCompanyGameDir) != true)
                    {
                        logger.Error("选择了错误目录，关闭游戏。");
                        System.Windows.MessageBox.Show("选择目录有误，没有在当前目录找到游戏。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        System.Windows.Application.Current.Shutdown();
                    }
                    else
                    {
                        logger.Info("找到了正确目录。");
                        File.Create(CheckFile);
                        File.WriteAllText("LimbusCompanyPath.txt", limbusCompanyDir);
                    }
                }
            }
            else
            {
                logger.Info("检测到跳过检查文件。");
            }
            limbusCompanyGameDir = limbusCompanyDir + "/LimbusCompany.exe";
            logger.Info("边狱公司路径：" + limbusCompanyDir);
        }
        /// <summary>
        /// 获取最新版汉化模组哈希
        /// </summary>
        /// <returns>返回Sha256</returns>
        private string GetLimbusLocalizeHash()
        {
            string HashRaw;
            if (useEndPoint != string.Empty)
            {
                HashRaw = GetURLText(useEndPoint + "LimbusLocalizeHash.json");
            }
            else
            {
                HashRaw = GetURLText(DefaultEndPoint + "LimbusLocalizeHash.json");
            }
            dynamic JsonObject = JsonConvert.DeserializeObject(HashRaw);
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
        /// 获取LCB路径，同时在工具箱目录保存LimbusCompanyPath.txt
        /// </summary>
        /// <returns>返回路径。</returns>
        private string? FindlimbusCompanyDirectory()
        {
            logger.Info("使用自动查找边狱公司方法。");
            string LimbusCompanyPath = "./LimbusCompanyPath.txt";
            if (File.Exists(LimbusCompanyPath))
            {
                logger.Info("在根目录找到了之前获取的路径，检查可用性。");
                string LimbusCompany = File.ReadAllText(LimbusCompanyPath);
                if (File.Exists(LimbusCompany + "/LimbusCompany.exe"))
                {
                    logger.Info("路径可用，返回路径。");
                    return LimbusCompany;
                }
                else
                {
                    logger.Info("路径不可用，重新进行查找。");
                }
            }
            string? steamPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null) as string;
            if (steamPath != null)
            {
                string libraryFoldersPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
                if (File.Exists(libraryFoldersPath))
                {
                    string[] lines = File.ReadAllLines(libraryFoldersPath);
                    foreach (string line in lines)
                    {
                        if (line.Contains("\t\"path\"\t\t"))
                        {
                            string libraryPath = line.Split('\t')[4].Trim('\"');

                            DirectoryInfo[] steamapps = new DirectoryInfo(libraryPath).GetDirectories("steamapps");
                            if (steamapps.Length > 0)
                            {
                                string commonDir = Path.Combine(steamapps[0].FullName, "common");
                                if (Directory.Exists(commonDir))
                                {
                                    DirectoryInfo[] gameDirs = new DirectoryInfo(commonDir).GetDirectories("Limbus Company");
                                    if (gameDirs.Length > 0)
                                    {
                                        var FullName = gameDirs[0].FullName;
                                        if (File.Exists(FullName + "/LimbusCompany.exe"))
                                        {
                                            logger.Info("已自动查找到边狱公司路径，返回路径并保存。");
                                            File.WriteAllText("LimbusCompanyPath.txt", FullName);
                                            return FullName;
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 处理使用Downloader下载文件的事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewOnDownloadProgressChanged(object sender, Downloader.DownloadProgressChangedEventArgs e)
        {
            logger.Debug("ProgressPercentage: " + e.ProgressPercentage + " ProgressPercentage(Int): " + (int)(e.ProgressPercentage));
            if (InstallPhase != 0)
            {
                progressPercentage = (float)((InstallPhase - 1) * 33 + e.ProgressPercentage * 0.33);
            }
        }
        private void NewOnDownloadProgressCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (InstallPhase != 0)
            {
                progressPercentage = InstallPhase * 33;
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
            logger.Debug("Thread.CurrentThread.ManagedThreadId: " + Thread.CurrentThread.ManagedThreadId);
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
            logger.Info("DownloadFileAutoAsync File: " + File + "Path: " + Path);
            if (useEndPoint != string.Empty)
            {
                string DownloadUrl = useEndPoint + File;
                await DownloadFileAsync(DownloadUrl, Path);
            }
            else
            {
                string DownloadUrl = DefaultEndPoint + File;
                await DownloadFileAsync(DownloadUrl, Path);
            }
        }
        /// <summary>
        /// 获取最新汉化模组标签。
        /// </summary>
        /// <returns>返回模组标签</returns>
        private string GetLatestLimbusLocalizeVersion()
        {
            logger.Info("获取模组标签。");
            string raw = string.Empty;
            raw = GetURLText("https://api.kr.zeroasso.top/Mod_Release.json");
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
        public string GetURLText(string Url)
        {
            try
            {
                using WebClient client = new();
                string raw = string.Empty;
                raw = new StreamReader(client.OpenRead(new Uri(Url)), Encoding.UTF8).ReadToEnd();
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
        public void OpenUrl(string Url)
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
        public void ErrorReport(Exception ex)
        {
            logger.Error("出现了问题：\n" + ex.ToString());
            System.Windows.MessageBox.Show("运行中出现了问题，若要反馈，请带上链接或日志。\n——————————\n" + ex.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        /// <summary>
        /// 用于错误处理。
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="CloseWindow">是否关闭窗体。</param>
        public void ErrorReport(Exception ex, bool CloseWindow)
        {
            logger.Error("出现了问题：\n" + ex.ToString());
            System.Windows.MessageBox.Show("运行中出现了问题，若要反馈，请带上链接或日志。\n——————————\n" + ex.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            if (CloseWindow)
            {
                Close();
            }
        }
        /// <summary>
        /// 检查工具箱更新
        /// </summary>
        /// <param name="version">当前版本</param>
        /// <param name="IsGithub">是否使用Github</param>
        /// <returns>是否存在更新</returns>
        private void CheckToolboxUpdate(string version)
        {
            try
            {
                logger.Info("正在检查工具箱更新。");
                using WebClient client = new();
                client.Headers.Add("User-Agent", "request");
                string raw = string.Empty;
                logger.Info("从镜像检查。");
                raw = new StreamReader(client.OpenRead(new Uri("https://api.kr.zeroasso.top/Toolbox_Release.json")), Encoding.UTF8).ReadToEnd();
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
                    Close();
                }
                logger.Info("没有更新。");
            }
            catch (Exception ex)
            {
                logger.Error("检查安装器更新出现问题。" + ex.ToString());
            }
        }
        /// <summary>
        /// 获取tmp字体最新标签以及是否为最新版
        /// </summary>
        /// <param name="version">当前版本</param>
        /// <param name="IsGithub">是否使用Github</param>
        /// <param name="tag">返回tmp字体tag</param>
        /// <returns>是否不是最新版</returns>
        static bool CheckChineseFontAssetUpdate(string version, bool IsGithub, out string tag)
        {
            tag = string.Empty;
            try
            {
                using WebClient client = new();
                client.Headers.Add("User-Agent", "request");
                string raw = string.Empty;
                if (IsGithub == true)
                {
                    raw = new StreamReader(client.OpenRead(new Uri("https://api.github.com/repos/LocalizeLimbusCompany/LLC_ChineseFontAsset/releases/latest")), Encoding.UTF8).ReadToEnd();
                }
                else
                {
                    raw = new StreamReader(client.OpenRead(new Uri("https://api.kr.zeroasso.top/LatestTmp_Release.json")), Encoding.UTF8).ReadToEnd();
                }
                var JsonObject = JSONNode.Parse(raw).AsObject;
                string latestReleaseTag = JsonObject["tag_name"].Value;
                if (latestReleaseTag != version)
                {
                    tag = latestReleaseTag;
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("出现了问题。\n" + ex.ToString());
                logger.Error("出现了问题。\n" + ex.ToString());
            }
            return false;
        }
        #endregion
        #region 进度条系统
        public async void ProgressTime_Tick(object sender, EventArgs e)
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
    }
}