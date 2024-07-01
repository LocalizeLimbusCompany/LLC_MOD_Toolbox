using Downloader;
using log4net;
using Microsoft.Win32;
using Newtonsoft.Json;
using SevenZipNET;
using SimpleJSON;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LLC_MOD_Toolbox_MeowVer
{
    public partial class Main : UIForm
    {
        public const string VERSION = "0.6.8";
        public string CurrentDir = Environment.CurrentDirectory;
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public string LimbusCompanyDir = string.Empty;
        public string LimbusCompanyGameDir = string.Empty;
        public string UseEndPoint = string.Empty;
        public string DefaultEndPoint = "https://node.zeroasso.top/d/od/";
        public bool UseMirrorGithub = false;
        public int InstallPhase = 0;
        public List<Node> NodeList;
        public Main()
        {
            InitializeComponent();
        }
        private void Main_Load(object sender, EventArgs e)
        {
            logger.Info("————NEW LOG————");
            logger.Info("工具箱版本：" + VERSION);
            InitToolBox();
            logger.Info("初始化完成。");
        }
        private void InitToolBox()
        {
            CheckLimbusCompanyPath();
            CheckToolboxUpdate(VERSION);
            InitNode();
            // this.TabControlMenu.Controls.Add(this.DebugPage);
        }
        #region 安装功能
        private async void InstallButton_Click(object sender, EventArgs e)
        {
            logger.Info("开始安装。");
            InstallPhase = 0;
            Process[] limbusProcess = Process.GetProcessesByName("LimbusCompany");
            if (limbusProcess.Length > 0)
            {
                logger.Warn("LimbusCompany仍然开启。");
                DialogResult DialogResult = MessageBox.Show("检测到 Limbus Company 仍然处于开启状态！\n建议您关闭游戏后继续安装模组。\n若您已经关闭了 Limbus Company，请点击确定，否则请点击取消返回。", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Hand);
                if (DialogResult == DialogResult.Cancel)
                {
                    return;
                }
                logger.Warn("用户选择无视警告。");
            }
            try
            {
                await InstallBepInEx();
                await InstallTMP();
                await InstallMod();
            }
            catch (Exception ex)
            {
                ErrorReport(ex, true);
            }
            InstallPhase = 0;
            logger.Info("安装完成。");
            DialogResult RunResult = MessageBox.Show("安装已完成！\n点击“确定”立刻运行边狱公司。\n点击“取消”关闭弹窗。\n加载时请耐心等待。", "完成", MessageBoxButtons.OKCancel);
            if (RunResult == DialogResult.OK)
            {
                try
                {
                    Process.Start("steam://rungameid/1973530");
                }
                catch (Exception ex)
                {
                    logger.Error("出现了问题： " + ex.ToString());
                    MessageBox.Show("出现了问题。\n" + ex.ToString());
                }
            }
            DownloadBar.Value = 0;
            TotalProcessBar.Value = 0;
        }
        private async Task InstallBepInEx()
        {
            logger.Info("已进入安装BepInEx流程。");
            InstallPhase = 1;
            if (!File.Exists(LimbusCompanyDir + "/BepInEx/core/BepInEx.Core.dll"))
            {
                if (!UseMirrorGithub)
                {
                    MessageBox.Show("如果你安装了杀毒软件，接下来可能会提示工具箱正在修改关键dll。\n允许即可。如果不信任汉化补丁，可以退出本程序。", "警告");
                    string BepInExZipPath = Path.Combine(LimbusCompanyDir, "BepInEx-IL2CPP-x64.7z");
                    logger.Info("BepInEx Zip目录： " + BepInExZipPath);
                    await DownloadFileAutoAsync("BepInEx-IL2CPP-x64.7z", BepInExZipPath);
                    logger.Info("开始解压 BepInEx zip。");
                    new SevenZipExtractor(BepInExZipPath).ExtractAll(LimbusCompanyDir, true);
                    logger.Info("解压完成。删除 BepInEx zip。");
                    File.Delete(BepInExZipPath);
                }
                if (UseMirrorGithub)
                {
                    MessageBox.Show("如果你安装了杀毒软件，接下来可能会提示工具箱正在修改关键dll。\n允许即可。如果不信任汉化补丁，可以退出本程序。", "警告");
                    string BepInExZipPath = Path.Combine(LimbusCompanyDir, "BepInEx-IL2CPP-x64.7z");
                    logger.Info("BepInEx Zip目录： " + BepInExZipPath);
                    await DownloadFileAsync("https://mirror.ghproxy.com/https://github.com/LocalizeLimbusCompany/BepInEx_For_LLC/releases/download/v6.0.1-LLC/BepInEx-IL2CPP-x64-6.0.1.7z", BepInExZipPath);
                    logger.Info("开始解压 BepInEx zip。");
                    new SevenZipExtractor(BepInExZipPath).ExtractAll(LimbusCompanyDir, true);
                    logger.Info("解压完成。删除 BepInEx zip。");
                    File.Delete(BepInExZipPath);
                }
            }
            else
            {
                logger.Info("检测到正确BepInEx。");
            }
        }
        private async Task InstallTMP()
        {
            logger.Info("已进入TMP流程。");
            InstallPhase = 2;
            string modsDir = LimbusCompanyDir + "/BepInEx/plugins/LLC";
            Directory.CreateDirectory(modsDir);
            string tmpchineseZipPath = Path.Combine(LimbusCompanyDir, "tmpchinese_BIE.7z");
            string tmpchinese = modsDir + "/tmpchinesefont";
            var LastWriteTime = File.Exists(tmpchinese) ? new FileInfo(tmpchinese).LastWriteTime.ToString("yyMMdd") : string.Empty;
            if (!UseMirrorGithub)
            {
                if (CheckChineseFontAssetUpdate(LastWriteTime, false, out _))
                {
                    await DownloadFileAutoAsync("tmpchinesefont_BIE.7z", tmpchineseZipPath);
                    logger.Info("解压 tmp zip 中。");
                    new SevenZipExtractor(tmpchineseZipPath).ExtractAll(LimbusCompanyDir, true);
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
                    new SevenZipExtractor(tmpchineseZipPath).ExtractAll(LimbusCompanyDir, true);
                    logger.Info("删除 tmp zip 。");
                    File.Delete(tmpchineseZipPath);
                }
            }
        }
        private async Task InstallMod()
        {
            logger.Info("开始安装模组。");
            InstallPhase = 3;
            string modsDir = LimbusCompanyDir + "/BepInEx/plugins/LLC";
            string limbusLocalizeDllPath = modsDir + "/LimbusLocalize_BIE.dll";
            string limbusLocalizeZipPath = Path.Combine(LimbusCompanyDir, "LimbusLocalize_BIE.7z");
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
                            MessageBox.Show("校验Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "校验失败");
                            Close();
                        }
                        logger.Info("解压模组本体 zip 中。");
                        new SevenZipExtractor(limbusLocalizeZipPath).ExtractAll(LimbusCompanyDir, true);
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
                        MessageBox.Show("校验Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "校验失败");
                        Close();
                    }
                    else
                    {
                        logger.Info("校验Hash成功。");
                    }
                    logger.Info("解压模组本体 zip 中。");
                    new SevenZipExtractor(limbusLocalizeZipPath).ExtractAll(LimbusCompanyDir, true);
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
                        new SevenZipExtractor(limbusLocalizeZipPath).ExtractAll(LimbusCompanyDir, true);
                        logger.Info("删除模组本体 zip 。");
                        File.Delete(limbusLocalizeZipPath);
                    }
                }
                else
                {
                    await DownloadFileAutoAsync("https://mirror.ghproxy.com/https://github.com/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/download/" + currentVersion + "/LimbusLocalize_BIE_" + currentVersion + ".7z", limbusLocalizeZipPath);
                    logger.Info("解压模组本体 zip 中。");
                    new SevenZipExtractor(limbusLocalizeZipPath).ExtractAll(LimbusCompanyDir, true);
                    logger.Info("删除模组本体 zip 。");
                    File.Delete(limbusLocalizeZipPath);
                }
            }
        }
        #endregion
        #region 卸载模组
        private void DeleteModButton_Click(object sender, EventArgs e)
        {
            logger.Info("点击删除模组");
            DialogResult result = MessageBox.Show("删除后你需要重新安装汉化补丁。\n确定继续吗？", "警告", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                logger.Info("确定删除模组。");
                try
                {
                    DeleteBepInEx();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("删除过程中出现了一些问题： " + ex.ToString(), "警告");
                    logger.Info("删除过程中出现了一些问题： " + ex.ToString());
                }
                MessageBox.Show("删除完成。", "提示");
                logger.Info("删除完成。");
            }
        }
        /// <summary>
        /// 删除目录。
        /// </summary>
        /// <param name="path"></param>
        private void DeleteDir(string path)
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
        private void DeleteFile(string path)
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
        private void DeleteBepInEx()
        {
            DeleteDir(LimbusCompanyDir + "/BepInEx");
            DeleteDir(LimbusCompanyDir + "/dotnet");
            DeleteFile(LimbusCompanyDir + "/doorstop_config.ini");
            DeleteFile(LimbusCompanyDir + "/Latest(框架日志).log");
            DeleteFile(LimbusCompanyDir + "/Player(游戏日志).log");
            DeleteFile(LimbusCompanyDir + "/winhttp.dll");
            DeleteFile(LimbusCompanyDir + "/changelog.txt");
            DeleteFile(LimbusCompanyDir + "/BepInEx-IL2CPP-x64.7z");
            DeleteFile(LimbusCompanyDir + "/LimbusLocalize_BIE.7z");
            DeleteFile(LimbusCompanyDir + "/tmpchinese_BIE.7z");
        }
        #endregion
        #region 常用方法
        private void CheckLimbusCompanyPath()
        {
            LimbusCompanyDir = FindLimbusCompanyDirectory();
            if (string.IsNullOrEmpty(LimbusCompanyDir))
            {
                logger.Error("未能找到 Limbus Company 目录，手动选择模式。");
                MessageBox.Show("未能找到 Limbus Company 目录。请手动选择。");
                FolderBrowserDialog dialog = new()
                {
                    Description = "请选择你的边狱公司游戏路径（steam目录/steamapps/common/Limbus Company）请不要选择LimbusCompany_Data！"
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LimbusCompanyDir = dialog.SelectedPath;
                    LimbusCompanyGameDir = LimbusCompanyDir + "/LimbusCompany.exe";
                    if (File.Exists(LimbusCompanyGameDir) != true)
                    {
                        logger.Error("选择了错误目录，关闭游戏。");
                        MessageBox.Show("选择目录有误，没有在当前目录找到游戏。", "错误", MessageBoxButtons.OK);
                        Close();
                    }
                    logger.Info("找到了正确目录。");
                    File.WriteAllText("LimbusCompanyPath.txt", LimbusCompanyDir);
                }
            }
            LimbusCompanyGameDir = LimbusCompanyDir + "/LimbusCompany.exe";
            logger.Info("边狱公司路径：" + LimbusCompanyDir);
        }
        /// <summary>
        /// 获取最新版汉化模组哈希
        /// </summary>
        /// <returns>返回Sha256</returns>
        private string GetLimbusLocalizeHash()
        {
            string HashRaw;
            if (UseEndPoint != string.Empty)
            {
                HashRaw = GetURLText(UseEndPoint + "LimbusLocalizeHash.json");
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
        private string FindLimbusCompanyDirectory()
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
            string steamPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null);
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
        /// 处理使用WebClient下载文件的事件。（过时）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LegacyOnDownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            logger.Debug("LEGACY ProgressPercentage: " + e.ProgressPercentage + " ProgressPercentage(Int): " + (int)(e.ProgressPercentage));
            DownloadBar.Value = (int)(e.ProgressPercentage);
            if (InstallPhase != 0)
            {
                TotalProcessBar.Value = (int)((InstallPhase - 1) * 33 + e.ProgressPercentage * 0.33);
            }
        }
        /// <summary>
        /// 处理使用Downloader下载文件的事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewOnDownloadProgressChanged(object sender, Downloader.DownloadProgressChangedEventArgs e)
        {
            logger.Debug("ProgressPercentage: " + e.ProgressPercentage + " ProgressPercentage(Int): " + (int)(e.ProgressPercentage));
            DownloadBar.Value = (int)(e.ProgressPercentage);
            if (InstallPhase != 0)
            {
                TotalProcessBar.Value = (int)((InstallPhase - 1) * 33 + e.ProgressPercentage * 0.33);
            }
        }
        private void LegacyOnDownloadProgressCompleted(object sender, AsyncCompletedEventArgs e)
        {
            DownloadBar.Value = 100;
            if (InstallPhase != 0)
            {
                TotalProcessBar.Value = (int)(InstallPhase * 33);
            }
        }
        private void NewOnDownloadProgressCompleted(object sender, AsyncCompletedEventArgs e)
        {
            DownloadBar.Value = 100;
            if (InstallPhase != 0)
            {
                TotalProcessBar.Value = (int)(InstallPhase * 33);
            }
        }
        /// <summary>
        /// 使用WebClient下载文件。（过时）
        /// </summary>
        /// <param name="Url">网址</param>
        /// <param name="Path">下载到的地址</param>
        public async Task LegacyDownloadFileAsync(string Url, string Path)
        {
            using var web = new WebClient();
            web.DownloadProgressChanged += LegacyOnDownloadProgressChanged;
            web.DownloadFileCompleted += LegacyOnDownloadProgressCompleted;
            await web.DownloadFileTaskAsync(Url, Path);
        }
        /// <summary>
        /// 使用Downloader多线程下载文件。
        /// </summary>
        /// <param name="Url">网址</param>
        /// <param name="Path">下载到的地址</param>
        /// <returns></returns>
        public async Task NewDownloadFileAsync(string Url, string Path)
        {
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
        /// <summary>
        /// 自动选择方式下载文件。
        /// </summary>
        /// <param name="Url">网址</param>
        /// <param name="Path">下载到的地址</param>
        /// <returns></returns>
        public async Task DownloadFileAsync(string Url, string Path)
        {
            logger.Info("正在DownloadFileAsync方法。");
            bool LegacyDownload = UseLegacyDownloadSwitch.Active;
            if (LegacyDownload)
            {
                logger.Info("使用Legacy下载方式。从" + Url + "下载到" + Path);
                await LegacyDownloadFileAsync(Url, Path);
            }
            else
            {
                logger.Info("使用New下载方式。从" + Url + "下载到" + Path);
                await NewDownloadFileAsync(Url, Path);
            }
        }
        public async Task DownloadFileAutoAsync(string File, string Path)
        {
            logger.Info("DownloadFileAutoAsync File: " + File + "Path: " + Path);
            if (UseEndPoint != string.Empty)
            {
                string DownloadUrl = UseEndPoint + File;
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
            MessageBox.Show("运行中出现了问题，若要反馈，请带上链接或日志。\n——————————\n" + ex.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        /// <summary>
        /// 用于错误处理。
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="CloseWindow">是否关闭窗体。</param>
        public void ErrorReport(Exception ex, bool CloseWindow)
        {
            logger.Error("出现了问题：\n" + ex.ToString());
            MessageBox.Show("运行中出现了问题，若要反馈，请带上链接或日志。\n——————————\n" + ex.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            logger.Info("正在检查工具箱更新。");
            using WebClient client = new();
            client.Headers.Add("User-Agent", "request");
            string raw = string.Empty;
            logger.Info("从镜像检查。");
            raw = new StreamReader(client.OpenRead(new Uri("https://api.kr.zeroasso.top/Toolbox_Release.json")), Encoding.UTF8).ReadToEnd();
            dynamic JsonObject = JsonConvert.DeserializeObject(raw);
            string latestReleaseTagRaw = JsonObject.tag_name;
            string latestReleaseTag = latestReleaseTagRaw.Remove(0, 1);
            logger.Info("最新安装器tag：" + latestReleaseTag);
            if (new System.Version(latestReleaseTag) > new System.Version(version))
            {
                logger.Info("有更新。");
                logger.Info("安装器存在更新。");
                MessageBox.Show("安装器存在更新。\n点击确定进入官网下载最新版本工具箱", "更新提醒");
                OpenUrl("https://www.zeroasso.top/docs/install/autoinstall");
                Close();
            }
            logger.Info("没有更新。");
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
                dynamic JsonObject = JsonConvert.DeserializeObject(raw);
                string latestReleaseTag = JsonObject.tag_name;
                if (latestReleaseTag != version)
                {
                    tag = latestReleaseTag;
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现了问题。\n" + ex.ToString());
                logger.Error("出现了问题。\n" + ex.ToString());
            }
            return false;
        }
        #endregion
        #region 读取节点
        public class Node
        {
            public string Name { get; set; }
            public string Endpoint { get; set; }
            public bool IsDefault { get; set; }
        }
        private void InitNode()
        {
            ReadNodeJsonFile();
            foreach (var Node in NodeList)
            {
                NodeComboBox.Items.Add(Node.Name);
                if (Node.IsDefault == true)
                {
                    DefaultEndPoint = Node.Endpoint;
                }
            }
            NodeComboBox.Items.Add("Mirror Ghproxy（第三方）");
            NodeComboBox.Items.Add("恢复默认");
        }
        public void ReadNodeJsonFile()
        {
            string NodeListRaw = File.ReadAllText(CurrentDir + "/Configs/NodeList.json");
            NodeList = JsonConvert.DeserializeObject<List<Node>>(NodeListRaw);
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
        private void NodeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NodeComboBox.Text == "恢复默认")
            {
                UseEndPoint = string.Empty;
                NodeComboBox.Text = "手动选择节点";
                logger.Info("节点恢复默认。");
                MessageBox.Show("已恢复默认节点！", "提示");
            }
            else if (NodeComboBox.Text == "Mirror Ghproxy（第三方）")
            {
                UseMirrorGithub = true;
                UseEndPoint = string.Empty;
                logger.Info("用户切换至从MirrorGHProxy下载。");
                MessageBox.Show("本镜像源来自第三方，不保证安全性和可用性。");
            }
            else
            {
                try
                {
                    UseEndPoint = FindEndpoint(NodeComboBox.Text);
                    logger.Info("手动选择节点：" + UseEndPoint);
                    MessageBox.Show("已选择 " + NodeComboBox.Text + " 节点。\n该节点EndPoint为： " + UseEndPoint, "提示");
                }
                catch (Exception ex)
                {
                    ErrorReport(ex);
                }
            }
        }
        #endregion
        #region Debug区域
        private async void DebugButton1_Click(object sender, EventArgs e)
        {
            try
            {
                logger.Info("下载测试文件。");
                string CachePath = Path.Combine(CurrentDir, "100MB.TEST");
                await DownloadFileAsync("http://hkg.download.datapacket.com/100mb.bin", CachePath);
            }
            catch (Exception ex)
            {
                ErrorReport(ex);
            }
        }
        #endregion
        #region 关于区域
        private void AboutMemberButton_Click(object sender, EventArgs e)
        {
            OpenUrl("https://paratranz.cn/projects/6860/leaderboard");
        }
        private void AboutAfdianButton_Click(object sender, EventArgs e)
        {
            OpenUrl("https://afdian.net/a/Limbus_zero");
        }
        private void AboutHuijiWikiButton_Click(object sender, EventArgs e)
        {
            OpenUrl("https://limbuscompany.huijiwiki.com");
        }
        #endregion
    }
}
