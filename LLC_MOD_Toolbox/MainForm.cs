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

namespace LLC_MOD_Toolbox
{
    public partial class MainForm : UIForm
    {
        public const string VERSION = "0.3.5";
        private bool Has_NET_6_0;
        private bool isWindows10;
        private string limbusCompanyDir;
        private string limbusCompanyGameDir;
        private static string fastestNode;
        private string melonLoaderUrl;
        private string melonLoaderZipPath;

        logger logger = new logger("LLCToolBox_log.txt");

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            useMFL.Active = true;
            useGithub.Active = false;
            string logFilePath = Path.Combine(Application.StartupPath, "LLCToolBox_log.txt");
            if (File.Exists(logFilePath))
            {
                try
                {
                    File.Delete(logFilePath);
                }
                catch
                {
                }
            }
            logger.Log("LLCToolbox is loading.");

            // 关闭按钮
            ControlButton(false);

            logger.Log("Getting Fastest Node...");
            // 获取最快节点
            fastestNode = GetFastnetNode();
            if (string.IsNullOrEmpty(fastestNode))
            {
                MessageBox.Show("网络错误,你无法访问任何站点。请检查 网络或代理", "错误", MessageBoxButtons.OK);
                Close();
                return;
            }
            logger.Log("Done. The Fastest Node is: " + fastestNode);
            // 检查更新
            logger.Log("Check the Installer's Update.");
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("User-Agent", "request");
                    logger.Log("Open Github API.");
                    bool isgit = fastestNode == "github.com";
                    string raw = new StreamReader(client.OpenRead(new Uri(isgit ? "https://api.github.com/repos/LocalizeLimbusCompany/LLC_MOD_Installer/releases/latest" : "https://json.zxp123.eu.org/Toolbox_Release.json")), Encoding.UTF8).ReadToEnd();
                    var latest = JSONNode.Parse(raw).AsObject;
                    string latestReleaseTag = latest["tag_name"].Value.Remove(0, 1);
                    if (new Version(latestReleaseTag) > new Version(FileVersionInfo.GetVersionInfo("./LLC_MOD_Toolbox.exe").ProductVersion))
                    {
                        logger.Log("Find the installer's new version. version: " + latestReleaseTag);
                        MessageBox.Show("安装器存在更新");
                        Openuri(isgit ? "https://github.com/LocalizeLimbusCompany/LLC_MOD_Installer/releases/latest" : "https://www.zeroasso.top/docs/install/autoinstall");
                        Close();
                        return;
                    }
                    logger.Log("Check Installer's Update is done.");
                }
            }
            catch
            {
                logger.Log("Crash - Check Installer's Update");
            }

            // 检查.NET6.0
            Check_DotnetVer();
            if (Has_NET_6_0 != true)
            {
                Install_Dotnet();
            }

            // 实验性的检查windows10
            CheckWindows10();
            if (isWindows10 != true)
            {
                logger.Log("Crash - isn't windows10 or windows11");
                MessageBox.Show("汉化补丁不支持Windows7及以下版本的Windows！", "错误", MessageBoxButtons.OK);
                Close();
            }

            // 查找边狱公司dir
            logger.Log("Find Limbus Company Dir.");

            limbusCompanyDir = FindLimbusCompanyDirectory();
            if (string.IsNullOrEmpty(limbusCompanyDir))
            {
                logger.Log("Can't auto find the limbus company dir. Use the another way.");
                MessageBox.Show("未能找到Limbus Company目录。请手动选择。");
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
                        logger.Log("Select wrong dir.Close.");
                        MessageBox.Show("选择目录有误，没有在当前目录找到游戏。", "错误", MessageBoxButtons.OK);
                        Close();
                    }
                    logger.Log("Find the true dir.");
                    File.WriteAllText("LimbusCompanyPath.txt", limbusCompanyDir);
                }
            }
            limbusCompanyGameDir = limbusCompanyDir + "/LimbusCompany.exe";

            logger.Log("Find Limbus Company Dir is done.");

            // 控制打开Button
            ControlButton(true);
        }

        // 安装
        private async void installButton_Click(object sender, EventArgs e)
        {
            logger.Log("Start Install Mod.");

            logger.Log("Download MelonLoader.");

            ControlButton(false);

            // 下载MelonLoader
            if (useMFL.Active == true)
            {
                logger.Log("Use MFL.");
                try
                {
                    logger.Log("Downloading MFL...");
                    status.Text = "正在下载并解压MelonLoader...";
                    logger.Log("LimbusCompanyDir: " + limbusCompanyDir);
                    bool MelonLoaderVersion = !File.Exists(limbusCompanyDir + "/MelonLoader/net6/MelonLoader.dll") || new Version(FileVersionInfo.GetVersionInfo(limbusCompanyDir + "/MelonLoader/net6/MelonLoader.dll").ProductVersion) < new Version("0.6.3");
                    if (useGithub.Active != true)
                    {
                        if (MelonLoaderVersion)
                        {
                            logger.Log("cant find melonloader. start installing");
                            MessageBox.Show("如果你安装了杀毒软件，接下来可能会提示工具箱正在修改关键dll。\n允许即可。如果不信任汉化补丁，可以退出本程序。", "警告");
                            if (Directory.Exists(limbusCompanyDir + "/MelonLoader"))
                            {
                                Directory.Delete(limbusCompanyDir + "/MelonLoader", true);
                            }
                            melonLoaderUrl = "https://" + fastestNode + "/files/ML_LLC_v0.6.3.zip";
                            melonLoaderZipPath = Path.Combine(limbusCompanyDir, "ML_LLC_v0.6.3.zip");
                            logger.Log("MelonLoaderZipPath: " + melonLoaderZipPath);
                            await DownloadFileAsync(melonLoaderUrl, melonLoaderZipPath);
                            logger.Log("start Extract zip.");
                            new SevenZipExtractor(melonLoaderZipPath).ExtractAll(limbusCompanyDir, true);
                            logger.Log("Extract zip...");
                            File.Delete(melonLoaderZipPath);
                        }
                        else
                        {
                            logger.Log("Find MelonLoader. Stop.");
                        }
                    }
                    else
                    {
                        if (MelonLoaderVersion)
                        {
                            melonLoaderUrl = "https://github.com/LocalizeLimbusCompany/MelonLoader-LLC/releases/download/v0.6.3/ML_LLC_v0.6.3.zip";
                            melonLoaderZipPath = Path.Combine(limbusCompanyDir, "ML_LLC_v0.6.3.zip");
                            logger.Log("MelonLoaderZipPath: " + melonLoaderZipPath);
                            await DownloadFileAsync(melonLoaderUrl, melonLoaderZipPath);
                        }
                        else
                        {
                            logger.Log("Find MelonLoader. Stop.");
                        }
                    }
                    logger.Log("MFL is done.");
                    done_Bar.Value = 33;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("出现了问题。\n" + ex.ToString());
                    logger.Log("have some problem: " + ex.ToString());
                    Close();
                    return;
                }
            }
            else
            {
                logger.Log("Use Offical MelonLoader.");
                try
                {
                    logger.Log("Downloading Offical Melonloader...");
                    status.Text = "正在下载并解压MelonLoader...";
                    MessageBox.Show("如果你安装了杀毒软件，接下来可能会提示工具箱正在修改关键dll。\n允许即可。如果不信任汉化补丁，可以退出本程序。", "警告");
                    if (Directory.Exists(limbusCompanyDir + "/MelonLoader"))
                    {
                        Directory.Delete(limbusCompanyDir + "/MelonLoader", true);
                    }
                    if (useGithub.Active != true)
                    {
                        melonLoaderUrl = "https://" + fastestNode + "/files/MelonLoader.x64.zip";
                    }
                    else
                    {
                        melonLoaderUrl = "https://github.com/LavaGang/MelonLoader/releases/download/v0.6.1/MelonLoader.x64.zip";
                    }
                    string melonLoaderZipPath = Path.Combine(limbusCompanyDir, "MelonLoader.x64.zip");
                    await DownloadFileAsync(melonLoaderUrl, melonLoaderZipPath);
                    new SevenZipExtractor(melonLoaderZipPath).ExtractAll(limbusCompanyDir, true);
                    logger.Log("Extract zip...");
                    File.Delete(melonLoaderZipPath);
                    logger.Log("Offical MelonLoader is done.");
                    done_Bar.Value = 33;
                }
                catch (Exception ex)
                {
                    logger.Log("have some problem: " + ex.ToString());
                    MessageBox.Show("出现了问题。\n" + ex.ToString());
                    Close();
                    return;
                }
            }

            // 下载 tmp
            logger.Log("Downloading tmpchinese...");
            status.Text = "正在下载并解压tmpchinese...";
            string modsDir = Path.Combine(limbusCompanyDir, "Mods");
            logger.Log("create moddir.");
            Directory.CreateDirectory(modsDir);
            string tmpchineseZipPath = Path.Combine(limbusCompanyDir, "tmpchinese.7z");
            string tmpchinese = modsDir + "/tmpchinesefont";
            var LastWriteTime = File.Exists(tmpchinese) ? new FileInfo(tmpchinese).LastWriteTime.ToString("yyMMdd") : string.Empty;

            try
            {
                if (useGithub.Active != true)
                {
                    if (CheckChineseFontAssetUpdate(LastWriteTime, false, out var latestTag))
                    {
                        await DownloadFileAsync("https://" + fastestNode + "/files/tmpchinesefont.7z", tmpchineseZipPath);
                        logger.Log("Extract zip...");
                        new SevenZipExtractor(tmpchineseZipPath).ExtractAll(limbusCompanyDir, true);
                        File.Delete(tmpchineseZipPath);
                    }
                }
                else
                {
                    if (CheckChineseFontAssetUpdate(LastWriteTime, true, out var latestTag))
                    {
                        string downloadTMP = "https://github.com/LocalizeLimbusCompany/LLC_ChineseFontAsset/releases/download/" + latestTag + "/tmpchinesefont_" + latestTag + ".7z";
                        tmpchineseZipPath = Path.Combine(limbusCompanyDir, "tmpchinese.7z");
                        await DownloadFileAsync(downloadTMP, tmpchineseZipPath);
                        logger.Log("Extract zip...");
                        new SevenZipExtractor(tmpchineseZipPath).ExtractAll(limbusCompanyDir, true);
                        File.Delete(tmpchineseZipPath);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log("have some problem: " + ex.ToString());
                MessageBox.Show("出现了问题。\n" + ex.ToString());
                Close();
                return;
            }

            logger.Log("download tmpchinese is done.");
            done_Bar.Value = 66;

            // 下载本体
            status.Text = "正在下载并解压模组本体...";
            logger.Log("download mod...");

            string limbusLocalizeDllPath = modsDir + "/LimbusLocalize.dll";
            string limbusLocalizeZipPath = Path.Combine(limbusCompanyDir, "LimbusLocalize.7z");
            string latestLLCVersion;
            string currentVersion = null;
            try
            {
                if (useGithub.Active != true)
                {
                    latestLLCVersion = GetLatestLimbusLocalizeVersion(false, out string latest2ReleaseTag);
                    if (File.Exists(limbusLocalizeDllPath))
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(limbusLocalizeDllPath);
                        currentVersion = versionInfo.ProductVersion;
                        if (new Version(versionInfo.ProductVersion) < new Version(latestLLCVersion.Remove(0, 1)))
                        {
                            await DownloadFileAsync("https://" + fastestNode + "/files/LimbusLocalize_FullPack.7z", limbusLocalizeZipPath);
                            logger.Log("Extract zip...");
                            new SevenZipExtractor(limbusLocalizeZipPath).ExtractAll(limbusCompanyDir, true);
                            File.Delete(limbusLocalizeZipPath);
                        }
                    }
                    else
                    {
                        await DownloadFileAsync("https://" + fastestNode + "/files/LimbusLocalize_FullPack.7z", limbusLocalizeZipPath);
                        logger.Log("Extract zip...");
                        new SevenZipExtractor(limbusLocalizeZipPath).ExtractAll(limbusCompanyDir, true);
                        File.Delete(limbusLocalizeZipPath);
                    }
                }
                else
                {
                    latestLLCVersion = GetLatestLimbusLocalizeVersion(true, out string latest2ReleaseTag);
                    string limbusLocalizeUrl = GetLatestLimbusLocalizeDownloadUrl(latestLLCVersion, false);
                    if (File.Exists(limbusLocalizeDllPath))
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(limbusLocalizeDllPath);
                        currentVersion = versionInfo.ProductVersion;
                        if (new Version(versionInfo.ProductVersion) < new Version(latestLLCVersion.Remove(0, 1)))
                        {
                            await DownloadFileAsync(limbusLocalizeUrl, limbusLocalizeZipPath);
                            logger.Log("Extract zip...");
                            new SevenZipExtractor(limbusLocalizeZipPath).ExtractAll(limbusCompanyDir, true);
                            File.Delete(limbusLocalizeZipPath);
                        }
                    }
                    else
                    {
                        await DownloadFileAsync(limbusLocalizeUrl, limbusLocalizeZipPath);
                        logger.Log("Extract zip...");
                        new SevenZipExtractor(limbusLocalizeZipPath).ExtractAll(limbusCompanyDir, true);
                        File.Delete(limbusLocalizeZipPath);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log("have some problem: " + ex.ToString());
                MessageBox.Show("出现了问题。\n" + ex.ToString());
                Close();
                return;
            }
            done_Bar.Value = 100;
            logger.Log("Install is done!");
            var version = FileVersionInfo.GetVersionInfo(limbusLocalizeDllPath);
            Version new_version = new Version(version.ProductVersion);
            if (string.IsNullOrEmpty(currentVersion) || new_version > new Version(currentVersion) && new_version >= new Version(latestLLCVersion.Remove(0, 1)))
                MessageBox.Show("安装已完成！\n你现在可以运行游戏了。", "完成", MessageBoxButtons.OK);
            else
                MessageBox.Show("模组没有更新", "完成?", MessageBoxButtons.OK);
            ControlButton(true);
            done_Bar.Value = 0;
            down_Bar.Value = 0;
            status.Text = "空闲中！";
        }

        private void disable_Click(object sender, EventArgs e)
        {
            logger.Log("Disable Mod...");
            string melonfileName = "version.dll";
            string filePath = Path.Combine(limbusCompanyDir, melonfileName);
            status.Text = "禁用模组中……";
            if (File.Exists(filePath))
            {
                try
                {
                    File.Move(filePath, Path.Combine(limbusCompanyDir, melonfileName + ".disable"));
                    logger.Log("Disable is done.");
                    MessageBox.Show("禁用成功！", "提示", MessageBoxButtons.OK);
                }
                catch (Exception ex)
                {
                    logger.Log("have some problem: " + ex.Message);
                    MessageBox.Show("禁用失败！", "提示", MessageBoxButtons.OK);
                }
            }
            else
            {
                logger.Log("have some problem: cant find file.");
                MessageBox.Show("禁用失败！是不是已经禁用了？", "提示", MessageBoxButtons.OK);
            }
            status.Text = "空闲中！";
        }

        // 启用
        private void canable_Click(object sender, EventArgs e)
        {
            logger.Log("Enable mod...");
            string disablefileName = "version.dll.disable";
            string filePath = Path.Combine(limbusCompanyDir, disablefileName);
            status.Text = "启用模组中……";
            MessageBox.Show("如果你安装了杀毒软件，接下来可能会提示工具箱正在修改关键dll。\n允许即可。如果不信任汉化补丁，可以退出本程序。", "警告");
            if (File.Exists(filePath))
            {
                try
                {
                    File.Move(filePath, Path.Combine(limbusCompanyDir, "version.dll"));
                    logger.Log("Enable is done.");
                    MessageBox.Show("启用成功！", "提示", MessageBoxButtons.OK);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("启用失败！", "提示", MessageBoxButtons.OK);
                    logger.Log("have some problem: " + ex.Message);
                }
            }
            else
            {
                logger.Log("have some problem: cant find file.");
                Console.WriteLine("文件不存在：" + filePath);
                MessageBox.Show("启用失败！是不是已经启用了？", "提示", MessageBoxButtons.OK);
            }
            status.Text = "空闲中！";
        }

        // Utils
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
            logger.Log(System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
            logger.Log("Check the Dotnet Ver.");
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
                logger.Log("User have dotnet 6.0.");
                Has_NET_6_0 = true;
            }
            else
            {
                logger.Log("User doesn't have dotnet 6.0?");
            }
        }

        private async void Install_Dotnet()
        {
            logger.Log("Installing the Dotnet...");
            status.Text = "正在下载并打开安装NET 6.0流程";
            logger.Log("Downloading the Dotnet...");
            await DownloadFileAsync("https://download.visualstudio.microsoft.com/download/pr/4a725ea4-cd2c-4383-9b63-263156d5f042/d973777b32563272b85617105a06d272/dotnet-sdk-6.0.406-win-x64.exe", "./!!!先安装我!!!NET 6.0.exe");
            var NET = new FileInfo("./!!!先安装我!!!NET 6.0.exe");
            logger.Log("Start Install the Dotnet.");
            Process.Start(NET.FullName).WaitForExit();
            NET.Delete();
            logger.Log("Installing the Dotnet is done.");
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
            logger.Log("Check User System.");
            RtlGetVersion(out var osVersion);
            if (osVersion.MajorVersion > 9U)
            {
                isWindows10 = true;
                logger.Log("User system is Windows10 or Windows11.");
            }
            else
            {
                isWindows10 = false;
                logger.Log("User system isn't Windows10 or Windows11.");
            }
        }
        // 下载文件任务
        private async Task DownloadFileAsync(string url, string filePath)
        {
            logger.Log("Download the File from: " + url);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (WebClient client = new WebClient())
            {
                client.DownloadProgressChanged += (s, e) =>
                {
                    down_Bar.Value = e.ProgressPercentage;
                };
                client.DownloadFileCompleted += (s, e) =>
                {
                    down_Bar.Value = 100;
                };
                await client.DownloadFileTaskAsync(new Uri(url), filePath);
            }
        }

        // 关闭/打开按钮
        private void ControlButton(bool yon)
        {
            if (yon == true)
            {
                installButton.Enabled = true;
                useMFL.ReadOnly = false;
                useGithub.ReadOnly = false;
                if (fastestNode == "github.com")
                    useGithub.Active = true;
                enterAfdian.Enabled = true;
                enterGithub.Enabled = true;
                enterWiki.Enabled = true;
                enterDoc.Enabled = true;
                logger.Log("Button is Enabled.");
            }
            else
            {
                installButton.Enabled = false;
                useMFL.ReadOnly = true;
                useGithub.ReadOnly = true;
                enterAfdian.Enabled = false;
                enterGithub.Enabled = false;
                enterWiki.Enabled = false;
                enterDoc.Enabled = false;
                logger.Log("Button is Disabled.");
            }
        }

        // 获取最快节点
        private string GetFastnetNode()
        {
            logger.Log("Ping the Node...");

            Dictionary<string, long> pingTimes = new Dictionary<string, long>() { { "github.com", 9999L }, { "limbus.determination.top", 9999L }, { "llc.determination.top", 9999L }, { "dl.determination.top", 9999L } };

            foreach (string url in pingTimes.Keys.ToArray())
            {
                Ping ping = new Ping();
                logger.Log("Ping: " + url + " ...");
                try
                {
                    PingReply reply = ping.Send(url);
                    if (reply.Status == IPStatus.Success)
                    {
                        logger.Log(url + " 's Roundtriptime is: " + reply.RoundtripTime + "ms.");
                        pingTimes[url] = reply.RoundtripTime;
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    logger.Log("ping: " + url + "has some problem. " + ex.ToString());
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
                logger.Log("network error");
                return null;
            }
            logger.Log("Done. The Fastest Node is: " + Fastest + ". Roundtriptime is: " + Roundtriptime + "ms.");
            return Fastest;
        }


        // 获取LimbusCompany路径
        private string FindLimbusCompanyDirectory()
        {
            try
            {
                logger.Log("Use the auto select way.");
                string LimbusCompanyPath = "./LimbusCompanyPath.txt";
                if (File.Exists(LimbusCompanyPath))
                {
                    logger.Log("Find the legacy txt. return.");
                    string LimbusCompany = File.ReadAllText(LimbusCompanyPath);
                    if (File.Exists(LimbusCompany + "/LimbusCompany.exe"))
                    {
                        return LimbusCompany;
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
                                                logger.Log("Find Limbus Company Dir.");
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
                logger.Log("Auto Find Limbus Company Failed!");
                return null;
            }
        }


        private void enterAfdian_Click(object sender, EventArgs e)
        {
            Openuri("https://afdian.net/a/Limbus_zero");
        }

        private void enterGithub_Click(object sender, EventArgs e)
        {
            Openuri("https://github.com/LocalizeLimbusCompany/LLC_MOD_Installer");
        }

        private void enterWiki_Click(object sender, EventArgs e)
        {
            Openuri("https://limbuscompany.huijiwiki.com");
        }
        private void enterDoc_Click(object sender, EventArgs e)
        {
            Openuri("https://www.zeroasso.top");
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
                return latestReleaseTag;
            }
        }

        private string GetLatestLimbusLocalizeDownloadUrl(string version, bool isota)
        {
            return "https://github.com/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/download/" + version + "/LimbusLocalize_" + (isota ? "OTA_" : string.Empty) + version + ".7z";
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
    }
}
