using log4net;
using Microsoft.Win32;
using SevenZipNET;
using SimpleJSON;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LLC_MOD_Toolbox_Remake
{

    public partial class MainPage : UIForm
    {
        public const string VERSION = "0.4.2";

        // 注册日志系统
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MainPage()
        {
            InitializeComponent();
        }

        // 窗体完成加载后事件
        private void MainPage_Load(object sender, EventArgs e)
        {
            logger.Info("-----------------------");
            ControlButton(false);
            logger.Info("正在初始化窗体。");
            Init_Toolbox();
            logger.Info("窗体已完成加载。");
            ChangeStatu("空闲中！");
            logger.Info("安装器版本：" + VERSION);
            ControlButton(true);
        }

        // 初始化
        private void Init_Toolbox()
        {
            ChangeStatu("获取最快节点。");
            fastestNode = GetFastnetNode();

            if (string.IsNullOrEmpty(fastestNode))
            {
                logger.Error("最快节点为空，网络错误。");
                MessageBox.Show("网络错误,你无法访问任何站点。请检查网络或代理是否正常。", "错误", MessageBoxButtons.OK);
                Close();
                return;
            }

            bool isgit = fastestNode == "github.com";
            if (isgit == true)
            {
                useGithub.Active = true;
            }

            ChangeStatu("检查 .NET 。");
            // 检查.NET6.0
            Check_DotnetVer();
            if (Has_NET_6_0 != true)
            {
                Install_Dotnet();
            }

            // 检查windows10
            ChangeStatu("检查 Windows10 。");
            CheckWindows10();
            if (isWindows10 != true)
            {
                logger.Error("汉化补丁不支持 Windows7 及以下版本。已退出。");
                MessageBox.Show("汉化补丁不支持Windows7及以下版本的Windows！", "错误", MessageBoxButtons.OK);
                Close();
            }

            ChangeStatu("获取 Limbus Company 目录。");
            limbusCompanyDir = FindLimbusCompanyDirectory();
            if (string.IsNullOrEmpty(limbusCompanyDir))
            {
                logger.Error("未能找到 Limbus Company 目录，手动选择模式。");
                MessageBox.Show("未能找到 Limbus Company 目录。请手动选择。");
                FolderBrowserDialog dialog = new FolderBrowserDialog
                {
                    Description = "请选择你的边狱公司游戏路径（steam目录/steamapps/common/Limbus Company）请不要选择LimbusCompany_Data！"
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    limbusCompanyDir = dialog.SelectedPath;
                    limbusCompanyGameDir = limbusCompanyDir + "/LimbusCompany.exe";
                    if (File.Exists(limbusCompanyGameDir) != true)
                    {
                        logger.Error("选择了错误目录，关闭游戏。");
                        MessageBox.Show("选择目录有误，没有在当前目录找到游戏。", "错误", MessageBoxButtons.OK);
                        Close();
                    }
                    logger.Info("找到了正确目录。");
                    File.WriteAllText("LimbusCompanyPath.txt", limbusCompanyDir);
                }
            }
            limbusCompanyGameDir = limbusCompanyDir + "/LimbusCompany.exe";

            logger.Info("找到 Limbus Company 目录。");
        }

        // 安装
        private async void installButton_Click(object sender, EventArgs e)
        {
            logger.Info("开始安装。");

            logger.Info("安装 Bepinex 中。");

            ControlButton(false);

            logger.Info("检查某些可能出现的问题。");

            if (useGithub.Active == false && fastestNode == "github.com")
            {
                logger.Warn("在关闭使用 Github 的情况下，最快节点为 Github 。已自动切换至 Onedrive For Business 。");
                fastestNode = "download.zeroasso.top";
            }

            if (downFromOFB == true)
            {
                logger.Info("切换节点至 Onedrive For Business 。");
                fastestNode = "download.zeroasso.top";
            }

            if (downFromLV == true)
            {
                logger.Info("切换节点至 拉斯维加斯服务器。");
                fastestNode = "lv.zeroasso.top";
            }

            if (downFromLVCDN == true)
            {
                logger.Info("切换节点至 拉斯维加斯服务器 with CDN。");
                fastestNode = "lvcdn.zeroasso.top";
            }
            try
            {
                logger.Info("下载 BepInEx For LLC 中。");
                ChangeStatu("正在下载并解压BepInEx...");
                logger.Info("Limbus Company 目录： " + limbusCompanyDir);
                if (useGithub.Active != true)
                {
                    if (File.Exists(limbusCompanyDir + "/MelonLoader/net6/MelonLoader.dll"))
                    {
                        logger.Info("检测到MelonLoader，自动删除");
                        deleteMelonLoader();
                    }
                    else
                    {
                        logger.Info("未检测到MelonLoader");
                    }
                    logger.Info("开始安装。");
                    if (!File.Exists(limbusCompanyDir + "/BepInEx/core/BepInEx.Core.dll"))
                    {
                        logger.Info("未检测到正确Bepinex。");
                        MessageBox.Show("如果你安装了杀毒软件，接下来可能会提示工具箱正在修改关键dll。\n允许即可。如果不信任汉化补丁，可以退出本程序。", "警告");
                        if (Directory.Exists(limbusCompanyDir + "/BepInEx"))
                        {
                            Directory.Delete(limbusCompanyDir + "/BepInEx", true);
                        }
                        BepInExUrl = "https://" + fastestNode + "/files/BepInEx-IL2CPP-x64.7z";
                        BepInExZipPath = Path.Combine(limbusCompanyDir, "BepInEx-IL2CPP-x64.7z");
                        logger.Info("BepInEx Zip目录： " + BepInExZipPath);
                        await DownloadFileAsync(BepInExUrl, BepInExZipPath);
                        logger.Info("开始解压 BepInEx zip。");
                        new SevenZipExtractor(BepInExZipPath).ExtractAll(limbusCompanyDir, true);
                        logger.Info("解压完成。删除 BepInEx zip。");
                        File.Delete(BepInExZipPath);
                    }
                    else
                    {
                        logger.Info("检测到正确BepInEx。");
                    }
                }
                else
                {
                    if (File.Exists(limbusCompanyDir + "/MelonLoader/net6/MelonLoader.dll"))
                    {
                        logger.Info("检测到MelonLoader，自动删除");
                        deleteMelonLoader();
                    }
                    else
                    {
                        logger.Info("未检测到MelonLoader");
                    }
                    if (!File.Exists(limbusCompanyDir + "/BepInEx/core/BepInEx.Core.dll") || new Version(FileVersionInfo.GetVersionInfo(limbusCompanyDir + "/BepInEx/core/BepInEx.Core.dll").ProductVersion) != new Version("6.0.0.0"))
                    {
                        logger.Info("从 Github 下载 BepInEx 。");
                        BepInExUrl = "https://github.com/LocalizeLimbusCompany/BepInEx_For_LLC/releases/download/v6.0.0-LLC/BepInEx-IL2CPP-x64.7z";
                        BepInExZipPath = Path.Combine(limbusCompanyDir, "BepInEx-IL2CPP-x64.7z");
                        logger.Info("BepInEx Zip路径： " + BepInExZipPath);
                        await DownloadFileAsync(BepInExUrl, BepInExZipPath);
                    }
                    else
                    {
                        logger.Info("检测到正确BepInEx。");
                    }
                }
                logger.Info("已完成 BepInEx 的安装。");
                TotalBar.Value = 33;
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现了问题。\n" + ex.ToString());
                logger.Error("出现了问题： " + ex.ToString());
                Close();
                return;
            }
            // 下载 tmp
            logger.Info("下载字体文件……");
            ChangeStatu("正在下载并解压tmpchinese...");
            string modsDir = limbusCompanyDir + "/BepInEx/plugins/LLC";
            logger.Info("创建 Mods 目录。");
            Directory.CreateDirectory(modsDir);
            string tmpchineseZipPath = Path.Combine(limbusCompanyDir, "tmpchinese_BIE.7z");
            string tmpchinese = modsDir + "/tmpchinesefont";
            var LastWriteTime = File.Exists(tmpchinese) ? new FileInfo(tmpchinese).LastWriteTime.ToString("yyMMdd") : string.Empty;

            try
            {
                if (useGithub.Active != true)
                {
                    if (CheckChineseFontAssetUpdate(LastWriteTime, false, out var latestTag))
                    {
                        await DownloadFileAsync("https://" + fastestNode + "/files/tmpchinesefont_BIE.7z", tmpchineseZipPath);
                        logger.Info("解压 tmp zip 中。");
                        new SevenZipExtractor(tmpchineseZipPath).ExtractAll(limbusCompanyDir, true);
                        logger.Info("删除 tmp zip 。");
                        File.Delete(tmpchineseZipPath);
                    }
                }
                else
                {
                    if (CheckChineseFontAssetUpdate(LastWriteTime, true, out var latestTag))
                    {
                        string downloadTMP = "https://github.com/LocalizeLimbusCompany/LLC_ChineseFontAsset/releases/download/" + latestTag + "/tmpchinesefont_BIE_" + latestTag + ".7z";
                        tmpchineseZipPath = Path.Combine(limbusCompanyDir, "tmpchinese_BIE.7z");
                        await DownloadFileAsync(downloadTMP, tmpchineseZipPath);
                        logger.Info("解压 tmp zip 。");
                        new SevenZipExtractor(tmpchineseZipPath).ExtractAll(limbusCompanyDir, true);
                        logger.Info("删除 tmp zip 。");
                        File.Delete(tmpchineseZipPath);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("出现问题： " + ex.ToString());
                MessageBox.Show("出现了问题。\n" + ex.ToString());
                Close();
                return;
            }

            logger.Info("完成安装字体文件。");
            TotalBar.Value = 66;

            // 下载本体
            ChangeStatu("正在下载并解压模组本体...");
            logger.Info("下载模组本体。");

            string limbusLocalizeDllPath = modsDir + "/LimbusLocalize_BIE.dll";
            string limbusLocalizeZipPath = Path.Combine(limbusCompanyDir, "LimbusLocalize_BIE.7z");
            string latestLLCVersion;
            string currentVersion = null;
            try
            {
                if (useGithub.Active != true)
                {
                    latestLLCVersion = GetLatestLimbusLocalizeVersion(false, out string latest2ReleaseTag);
                    logger.Info("最后模组版本： " + latestLLCVersion);
                    if (File.Exists(limbusLocalizeDllPath))
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(limbusLocalizeDllPath);
                        currentVersion = versionInfo.ProductVersion;
                        if (new Version(versionInfo.ProductVersion) < new Version(latestLLCVersion.Remove(0, 1)))
                        {
                            await DownloadFileAsync("https://" + fastestNode + "/files/LimbusLocalize_BIE_FullPack.7z", limbusLocalizeZipPath);
                            logger.Info("解压模组本体 zip 中。");
                            new SevenZipExtractor(limbusLocalizeZipPath).ExtractAll(limbusCompanyDir, true);
                            logger.Info("删除模组本体 zip 。");
                            File.Delete(limbusLocalizeZipPath);
                        }
                    }
                    else
                    {
                        await DownloadFileAsync("https://" + fastestNode + "/files/LimbusLocalize_BIE_FullPack.7z", limbusLocalizeZipPath);
                        logger.Info("解压模组本体 zip 中。");
                        new SevenZipExtractor(limbusLocalizeZipPath).ExtractAll(limbusCompanyDir, true);
                        logger.Info("删除模组本体 zip 。");
                        File.Delete(limbusLocalizeZipPath);
                    }
                }
                else
                {
                    latestLLCVersion = GetLatestLimbusLocalizeVersion(true, out string latest2ReleaseTag);
                    logger.Info("最后模组版本： " + latestLLCVersion);
                    string limbusLocalizeUrl = GetLatestLimbusLocalizeDownloadUrl(latestLLCVersion, false);
                    logger.Info("模组下载链接 " + limbusLocalizeUrl);
                    if (File.Exists(limbusLocalizeDllPath))
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(limbusLocalizeDllPath);
                        currentVersion = versionInfo.ProductVersion;
                        if (new Version(versionInfo.ProductVersion) < new Version(latestLLCVersion.Remove(0, 1)))
                        {
                            await DownloadFileAsync(limbusLocalizeUrl, limbusLocalizeZipPath);
                            logger.Info("解压模组本体 zip 中。");
                            new SevenZipExtractor(limbusLocalizeZipPath).ExtractAll(limbusCompanyDir, true);
                            logger.Info("删除模组本体 zip 。");
                            File.Delete(limbusLocalizeZipPath);
                        }
                    }
                    else
                    {
                        await DownloadFileAsync(limbusLocalizeUrl, limbusLocalizeZipPath);
                        logger.Info("解压模组本体 zip 中。");
                        new SevenZipExtractor(limbusLocalizeZipPath).ExtractAll(limbusCompanyDir, true);
                        logger.Info("删除模组本体 zip 。");
                        File.Delete(limbusLocalizeZipPath);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("出现了问题： " + ex.ToString());
                MessageBox.Show("出现了问题。\n" + ex.ToString());
                Close();
                return;
            }
            TotalBar.Value = 100;
            logger.Info("安装完成。");
            var version = FileVersionInfo.GetVersionInfo(limbusLocalizeDllPath);
            Version new_version = new Version(version.ProductVersion);
            MessageBox.Show("安装已完成！\n你现在可以运行游戏了。\n加载时请耐心等待。", "完成", MessageBoxButtons.OK);
            ControlButton(true);
            TotalBar.Value = 0;
            DownloadBar.Value = 0;
            ChangeStatu("空闲中！");
        }

        // 控制按钮可用性
        private void ControlButton(bool CanUse)
        {
            logger.Info("操作按钮中。");
            // CanUse == true ： 可使用
            // 否则反之
            if (CanUse)
            {
                logger.Info("正在开启按钮。");
                uiTabControl.Enabled = true;
                dlFromDefault.Enabled = true;
                dlFromLV.Enabled = true;
                dlFromLVCDN.Enabled = true;
                dlFromDefault.Enabled = true;
                deleteButton.Enabled = true;
                useGithub.ReadOnly = false;
                logger.Info("开启完成。");
            }
            else
            {
                logger.Info("正在关闭按钮。");
                uiTabControl.Enabled = false;
                dlFromDefault.Enabled = false;
                dlFromLV.Enabled = false;
                dlFromLVCDN.Enabled = false;
                dlFromDefault.Enabled = false;
                deleteButton.Enabled = false;
                useGithub.ReadOnly = true;
                logger.Info("关闭完成。");
            }
        }

        static void Openuri(string uri)
        {
            ProcessStartInfo psi = new ProcessStartInfo(uri)
            {
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void Check_DotnetVer()
        {
            logger.Info("检查 .NET FrameWork 版本。");
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    Check_NET_Version((int)ndpKey.GetValue("Release"));
                }
            }
        }
        private void Check_NET_Version(int releaseKey)
        {
            if (releaseKey >= 528040)
            {
                logger.Info("用户拥有 .NET FrameWork。");
                Has_NET_6_0 = true;
            }
            else
            {
                logger.Info("用户没有 .NET FrameWork。");
                Has_NET_6_0 = false;
            }
        }

        private async void Install_Dotnet()
        {
            logger.Info("安装 .NET FrameWork 6.0 中。");
            ChangeStatu("正在下载并打开安装 .NET 6.0 流程。");
            logger.Info("正在下载 .NET FrameWork 6.0 。");
            await DownloadFileAsync("https://download.visualstudio.microsoft.com/download/pr/4a725ea4-cd2c-4383-9b63-263156d5f042/d973777b32563272b85617105a06d272/dotnet-sdk-6.0.406-win-x64.exe", "./!!!先安装我!!!NET 6.0.exe");
            var NET = new FileInfo("./!!!先安装我!!!NET 6.0.exe");
            logger.Info("开始安装 .NET FrameWork 6.0 。");
            Process.Start(NET.FullName).WaitForExit();
            NET.Delete();
            logger.Info("成功安装 .NET FrameWork 6.0 。");
        }

        // Async 下载文件
        private async Task DownloadFileAsync(string url, string filePath)
        {
            logger.Info("从: " + url + "下载文件。");
            using (WebClient client = new WebClient())
            {
                client.DownloadProgressChanged += (s, e) =>
                {
                    DownloadBar.Value = e.ProgressPercentage;
                };
                client.DownloadFileCompleted += (s, e) =>
                {
                    logger.Info("下载完成。");
                    DownloadBar.Value = 100;
                };
                await client.DownloadFileTaskAsync(new Uri(url), filePath);
            }
        }

        // 获取最快节点
        private string GetFastnetNode()
        {
            logger.Info("正在获取最快的节点……");

            Dictionary<string, long> pingTimes = new Dictionary<string, long>() { { "github.com", 9999L }, { "lv.zeroasso.top", 9999L }, { "lvcdn.zeroasso.top", 9999L }, { "download.zeroasso.top", 9999L } };

            foreach (string url in pingTimes.Keys.ToArray())
            {
                Ping ping = new Ping();
                logger.Info("获取 " + url + " 的延迟中……");
                try
                {
                    PingReply reply = ping.Send(url);
                    if (reply.Status == IPStatus.Success)
                    {
                        logger.Info(url + " 的延迟为 " + reply.RoundtripTime + "ms。");
                        pingTimes[url] = reply.RoundtripTime;
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("在获取 " + url + " 的延迟时遇到了一些错误。" + ex.ToString());
                }
                pingTimes[url] = 9999L;
            }

            List<KeyValuePair<string, long>> pingTimesList = new List<KeyValuePair<string, long>>(pingTimes);
            pingTimesList.Sort(delegate (KeyValuePair<string, long> pair1, KeyValuePair<string, long> pair2)
            {
                return pair1.Value.CompareTo(pair2.Value);
            });
            string Fastest = pingTimesList[0].Key;
            long Roundtriptime = pingTimesList[0].Value;
            if (Roundtriptime > 600L)
            {
                logger.Error("网络错误。");
                return null;
            }
            logger.Info("完成，最快节点为： " + Fastest + " 它的延迟是： " + Roundtriptime + " ms。");
            return Fastest;
        }

        // 获取LimbusCompany路径
        private string FindLimbusCompanyDirectory()
        {
            try
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
            catch
            {
                logger.Error("自动查找边狱公司路径时遇到了一些问题！");
                return null;
            }
        }

        // 实验性的检查windows10
        [DllImport("ntdll.dll", SetLastError = true)]
        internal static extern uint RtlGetVersion(out OsVersionInfo versionInformation);
        internal struct OsVersionInfo
        {
            private readonly uint OsVersionInfoSize;

            internal readonly uint MajorVersion;

            internal readonly uint MinorVersion;

            internal readonly uint BuildNumber;

            private readonly uint PlatformId;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            internal readonly string CSDVersion;
        }
        private void CheckWindows10()
        {
            logger.Info("检查用户系统。");
            RtlGetVersion(out var osVersion);
            if (osVersion.MajorVersion > 9U)
            {
                isWindows10 = true;
                logger.Info("用户系统为 Windows10 或 Windows11。");
            }
            else
            {
                isWindows10 = false;
                logger.Info("用户系统低于 Windows10 。");
            }
        }

        // 更改状态文本
        private void ChangeStatu(string txt)
        {
            statu.Text = txt;
        }

        // For Github Mode
        private string GetLatestLimbusLocalizeVersion(bool IsGithub, out string latest2ReleaseTag)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent", "request");
                string raw = string.Empty;
                if (IsGithub == true)
                {
                    raw = new StreamReader(client.OpenRead(new Uri("https://api.github.com/repos/LocalizeLimbusCompany/LocalizeLimbusCompany/releases")), Encoding.UTF8).ReadToEnd();
                }
                else
                {
                    raw = new StreamReader(client.OpenRead(new Uri("https://json.zxp123.eu.org/Mod_Release.json")), Encoding.UTF8).ReadToEnd();
                }
                JSONArray releases = JSONNode.Parse(raw).AsArray;

                string latestReleaseTag = releases[0]["tag_name"].Value;
                latest2ReleaseTag = releases.Count > 1 ? releases[1]["tag_name"].Value : string.Empty;
                logger.Info("TMP 字体最后标签为： " + latestReleaseTag);
                return latestReleaseTag;
            }
        }

        private string GetLatestLimbusLocalizeDownloadUrl(string version, bool isota)
        {
            return "https://github.com/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/download/" + version + "/LimbusLocalize_BIE_" + (isota ? "OTA_" : string.Empty) + version + ".7z";
        }

        static bool CheckChineseFontAssetUpdate(string LastWriteTime, bool IsGithub, out string tag)
        {
            tag = string.Empty;
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("User-Agent", "request");
                    string raw = string.Empty;
                    if (IsGithub == true)
                    {
                        raw = new StreamReader(client.OpenRead(new Uri("https://api.github.com/repos/LocalizeLimbusCompany/LLC_ChineseFontAsset/releases/latest")), Encoding.UTF8).ReadToEnd();
                    }
                    else
                    {
                        raw = new StreamReader(client.OpenRead(new Uri("https://json.zxp123.eu.org/LatestTmp_Release.json")), Encoding.UTF8).ReadToEnd();
                    }
                    var latest = JSONNode.Parse(raw).AsObject;
                    string latestReleaseTag = latest["tag_name"].Value;
                    if (latestReleaseTag != LastWriteTime)
                    {
                        tag = latestReleaseTag;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现了问题。\n" + ex.ToString());
            }
            return false;
        }

        private void dlFromOFB_Click(object sender, EventArgs e)
        {
            logger.Info("手动切换节点为 Onedrive For Business 。");
            downFromOFB = true;
            downFromLV = false;
            downFromLVCDN = false;
            MessageBox.Show("切换成功。", "提示");
        }

        private void dlFromLV_Click(object sender, EventArgs e)
        {
            logger.Info("手动切换节点为 Onedrive For Business 。");
            downFromOFB = false;
            downFromLV = true;
            downFromLVCDN = false;
            MessageBox.Show("切换成功。", "提示");
        }

        private void dlFromLVCDN_Click(object sender, EventArgs e)
        {
            logger.Info("手动切换节点为 Onedrive For Business 。");
            downFromOFB = false;
            downFromLV = false;
            downFromLVCDN = true;
            MessageBox.Show("切换成功。", "提示");
        }

        private void dlFromDefault_Click(object sender, EventArgs e)
        {
            logger.Info("节点切换为默认情况。");
            downFromOFB = false;
            downFromLV = false;
            downFromLVCDN = false;
            MessageBox.Show("切换成功。", "提示");
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            logger.Info("点击删除模组");
            DialogResult result = MessageBox.Show("删除后你需要重新安装汉化补丁。\n确定继续吗？", "警告", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                logger.Info("确定删除模组。");
                ControlButton(false);
                try
                {
                    deleteMelonLoader();
                    deleteBepInEx();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("删除过程中出现了一些问题： " + ex.ToString(), "警告");
                    logger.Info("删除过程中出现了一些问题： " + ex.ToString());
                }
                MessageBox.Show("删除完成。", "提示");
                logger.Info("删除完成。");
                ControlButton(true);
            }
        }

        private void deleteDir(string path)
        {
            if (Directory.Exists(path))
            {
                logger.Info("删除目录： " + path);
                Directory.Delete(path, true);
            }
        }

        private void deleteFile(string path)
        {
            if (File.Exists(path))
            {
                logger.Info("删除文件： " + path);
                File.Delete(path);
            }
        }

        private void deleteMelonLoader()
        {
            deleteDir(limbusCompanyDir + "/MelonLoader");
            deleteDir(limbusCompanyDir + "/Mods");
            deleteDir(limbusCompanyDir + "/Plugins");
            deleteDir(limbusCompanyDir + "/UserData");
            deleteDir(limbusCompanyDir + "/UserLibs");
            deleteFile(limbusCompanyDir + "/dobby.dll");
            deleteFile(limbusCompanyDir + "/Latest(框架日志).log");
            deleteFile(limbusCompanyDir + "/Player(游戏日志).log");
            deleteFile(limbusCompanyDir + "/version.dll");
            deleteFile(limbusCompanyDir + "/NOTICE.txt");
        }

        private void deleteBepInEx()
        {
            deleteDir(limbusCompanyDir + "/BepInEx");
            deleteDir(limbusCompanyDir + "/dotnet");
            deleteFile(limbusCompanyDir + "/doorstop_config.ini");
            deleteFile(limbusCompanyDir + "/Latest(框架日志).log");
            deleteFile(limbusCompanyDir + "/Player(游戏日志).log");
            deleteFile(limbusCompanyDir + "/winhttp.dll");
        }

        #region 链接按钮
        private void EnterToolBoxGithub_Click(object sender, EventArgs e)
        {
            logger.Info("进入工具箱的 Github。");
            Openuri("https://github.com/LocalizeLimbusCompany/LLC_MOD_Installer");
        }

        private void EnterLLCGithub_Click(object sender, EventArgs e)
        {
            logger.Info("进入汉化补丁的 Github。");
            Openuri("https://github.com/LocalizeLimbusCompany/LocalizeLimbusCompany");
        }

        private void EnterWebsite_Click(object sender, EventArgs e)
        {
            logger.Info("进入官网。");
            Openuri("https://www.zeroasso.top");
        }

        private void EnterBilibili_Click(object sender, EventArgs e)
        {
            logger.Info("进入我们的Bilibili。");
            Openuri("https://space.bilibili.com/1247764479");
        }

        private void EnterWiki_Click(object sender, EventArgs e)
        {
            logger.Info("进入灰机wiki。");
            Openuri("https://limbuscompany.huijiwiki.com");
        }

        private void EnterSteampp_Click(object sender, EventArgs e)
        {
            logger.Info("进入Watt toolkit官网。");
            Openuri("https://steampp.net/");
        }

        private void EnterQuestion_Click(object sender, EventArgs e)
        {
            logger.Info("进入常用问题。");
            Openuri("https://www.zeroasso.top/docs/question");
        }

        private void EnterLLCG_Click(object sender, EventArgs e)
        {
            logger.Info("进入LLCG。");
            Openuri("https://jq.qq.com/?_wv=1027&k=5NE6Kvg2");
        }

        private void EnterParatranz_Click(object sender, EventArgs e)
        {
            logger.Info("进入Paratranz。");
            Openuri("https://paratranz.cn/projects/6860");
        }
        private void EnterAfdian_Click(object sender, EventArgs e)
        {
            logger.Info("进入爱发电。");
            Openuri("https://afdian.net/a/Limbus_zero");
        }

        #endregion

        private bool Has_NET_6_0 = false;
        private bool isWindows10;

        private bool downFromOFB = false;
        private bool downFromLV = false;
        private bool downFromLVCDN = false;

        private string fastestNode;
        private string limbusCompanyDir;
        private string limbusCompanyGameDir;

        private string BepInExUrl;
        private string BepInExZipPath;
    }
}
