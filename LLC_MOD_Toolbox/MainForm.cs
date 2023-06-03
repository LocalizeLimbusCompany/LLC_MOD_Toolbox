using SimpleJSON;
using Sunny.UI;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System;
using System.Security.Policy;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static LLC_MOD_Toolbox.logger;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using SevenZipNET;

namespace LLC_MOD_Toolbox
{
    public partial class MainForm : UIForm
    {
        public const string VERSION = "0.3.0";
        private bool Has_NET_6_0;
        private bool isWindows10;
        private string limbusCompanyDir;
        private string limbusCompanyGameDir;
        private string fastestNode;

        logger logger = new logger("LLCToolBox_log.txt");

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, System.EventArgs e)
        {
            useMFL.Active = true;
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

            // 检查更新
            logger.Log("Check the Installer's Update.");
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("User-Agent", "request");
                    logger.Log("Open Github API.");
                    string raw = new StreamReader(client.OpenRead(new Uri("https://api.github.com/repos/LocalizeLimbusCompany/LLC_MOD_Installer/releases/latest")), Encoding.UTF8).ReadToEnd();
                    var latest = JSONNode.Parse(raw).AsObject;
                    string latestReleaseTag = latest["tag_name"].Value.Remove(0, 1);
                    if (new Version(latestReleaseTag) > new Version(FileVersionInfo.GetVersionInfo("./LimbusCompanyModInstaller.exe").ProductVersion))
                    {
                        logger.Log("Find the installer's new version. version: "+latestReleaseTag);
                        MessageBox.Show("安装器存在更新");
                        Openuri("https://github.com/LocalizeLimbusCompany/LLC_MOD_Installer/releases/latest");
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
                    limbusCompanyGameDir = limbusCompanyDir + "\\LimbusCompany.exe";
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
            logger.Log("Find Limbus Company Dir is done.");

            logger.Log("Getting Fastest Node...");
            // 获取最快节点
            fastestNode = GetFastnetNode();
            logger.Log("Done. The Fastest Node is: "+fastestNode);

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
                    MessageBox.Show("如果你安装了杀毒软件，接下来可能会提示工具箱正在修改关键dll。\n允许即可。如果不信任汉化补丁，可以退出本程序。", "警告");
                    if (Directory.Exists(limbusCompanyDir + "/MelonLoader"))
                    {
                        Directory.Delete(limbusCompanyDir + "/MelonLoader", true);
                    }
                    string melonLoaderUrl = "https://"+fastestNode+"/files/MelonLoader_ForLLC.zip";
                    string melonLoaderZipPath = Path.Combine(limbusCompanyDir, "MelonLoader_ForLLC.zip");
                    await DownloadFileAsync(melonLoaderUrl, melonLoaderZipPath);
                    new SevenZipExtractor(melonLoaderZipPath).ExtractAll(limbusCompanyDir, true);
                    logger.Log("Extract zip...");
                    File.Delete(melonLoaderZipPath);
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
                    MessageBox.Show("如果你安装了杀毒软件，接下来可能会提示工具箱正在修改关键dll。\n允许即可。如果不信任汉化补丁，可以退出本程序。","警告");
                    if (Directory.Exists(limbusCompanyDir + "/MelonLoader"))
                    {
                        Directory.Delete(limbusCompanyDir + "/MelonLoader", true);
                    }
                    string melonLoaderUrl = "https://" + fastestNode + "/files/MelonLoader.x64.zip";
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
                    logger.Log("have some problem: "+ex.ToString());
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

            try
            {
                string tmpchineseZipPath = Path.Combine(limbusCompanyDir, "tmpchinese.7z");
                await DownloadFileAsync("https://"+fastestNode+"/files/tmpchinesefont.7z", tmpchineseZipPath);
                logger.Log("Extract zip...");
                new SevenZipExtractor(tmpchineseZipPath).ExtractAll(limbusCompanyDir, true);
                File.Delete(tmpchineseZipPath);
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
            string limbusLocalizeZipPath = Path.Combine(limbusCompanyDir, "LimbusLocalize.7z");
            await DownloadFileAsync("https://"+fastestNode+"/files/LimbusLocalize_FullPack.7z", limbusLocalizeZipPath);
            logger.Log("Extract zip...");
            new SevenZipExtractor(limbusLocalizeZipPath).ExtractAll(limbusCompanyDir, true);
            File.Delete(limbusLocalizeZipPath);
            done_Bar.Value = 100;
            logger.Log("Install is done!");
            MessageBox.Show("安装已完成！", "完成", MessageBoxButtons.OK);
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
                    logger.Log("have some problem: "+ ex.Message);
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
            noteLabel.Text = "正在下载并打开安装NET 6.0流程";
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
            logger.Log("Download the File from: "+url);
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
            if(yon == true)
            {
                installButton.Enabled = true;
                disable.Enabled = true;
                canable.Enabled = true;
                useMFL.ReadOnly = false;
                enterAfdian.Enabled = true;
                enterGithub.Enabled = true;
                enterWiki.Enabled = true;
                logger.Log("Button is Enabled.");
            }
            else
            {
                installButton.Enabled = false;
                disable.Enabled = false;
                canable.Enabled = false;
                useMFL.ReadOnly = true;
                enterAfdian.Enabled = false;
                enterGithub.Enabled = false;
                enterWiki.Enabled = false;
                logger.Log("Button is Disabled.");
            }
        }
        
        // 获取最快节点
        private string GetFastnetNode()
        {
            logger.Log("Ping the Node...");
            string[] urls = {"limbus.determination.top", "llc.determination.top", "dl.determination.top" };

            Dictionary<string, long> pingTimes = new Dictionary<string, long>();

            foreach (string url in urls)
            {
                Ping ping = new Ping();
                logger.Log("Ping: "+url+" ...");
                try
                {
                    PingReply reply = ping.Send(url);
                    if (reply.Status == IPStatus.Success)
                    {
                        logger.Log(url + " 's Roundtriptime is: " + reply.RoundtripTime + "ms.");
                        pingTimes.Add(url, reply.RoundtripTime);
                    }
                }
                catch(Exception ex)
                {
                    logger.Log("ping: "+url+"has some problem. "+ex.ToString());
                }
            }

            List<KeyValuePair<string, long>> pingTimesList = new List<KeyValuePair<string, long>>(pingTimes);
            pingTimesList.Sort(delegate (KeyValuePair<string, long> pair1, KeyValuePair<string, long> pair2)
            {
                return pair1.Value.CompareTo(pair2.Value);
            });
            logger.Log("Done. The Fastest Node is: "+ pingTimesList[0].Key+". Roundtriptime is: "+ pingTimesList[0].Value + "ms.");
            return pingTimesList[0].Key;
        }

       
        // 获取LimbusCompany路径
        private string FindLimbusCompanyDirectory()
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

        private void enterAfdian_Click(object sender, EventArgs e)
        {
            Process.Start("https://afdian.net/a/Limbus_zero");
        }

        private void enterGithub_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/LocalizeLimbusCompany/LLC_MOD_Installer");
        }

        private void enterWiki_Click(object sender, EventArgs e)
        {
            Process.Start("https://limbuscompany.huijiwiki.com");
        }
    }
}
