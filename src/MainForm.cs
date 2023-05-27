using Microsoft.Win32;
using SevenZipNET;
using SimpleJSON;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LimbusCompanyModInstaller
{
    public partial class MainForm : Form
    {
        public const string VERSION = "0.2.3";
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 禁用安装按钮
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("User-Agent", "request");
                    string raw = new StreamReader(client.OpenRead(new Uri("https://api.github.com/repos/LocalizeLimbusCompany/LLC_MOD_Installer/releases/latest")), Encoding.UTF8).ReadToEnd();
                    var latest = JSONNode.Parse(raw).AsObject;
                    string latestReleaseTag = latest["tag_name"].Value.Remove(0, 1);
                    if (new Version(latestReleaseTag) > new Version(FileVersionInfo.GetVersionInfo("./LimbusCompanyModInstaller.exe").ProductVersion))
                    {
                        MessageBox.Show("安装器存在更新");
                        Openuri("https://github.com/LocalizeLimbusCompany/LLC_MOD_Installer/releases/latest");
                        Close();
                        return;
                    }
                }
            }
            catch
            {
                
            }
            // 提示
            MessageBox.Show("如果出现故障，请前往GitHub仓库提交issue。\n程序拥有三个节点，如果你没有网络工具，请优先选择镜像节点。\n与此同时,镜像节点可能不是最新的", "注意事项", MessageBoxButtons.OK);

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
                MessageBox.Show("汉化补丁不支持Windows7及以下版本的Windows！", "错误", MessageBoxButtons.OK);
                Close();
            }

            // 检查完成
            noteLabel.Text = "环境检查完成，您可以运行本程序了！";

            // 启用安装按钮
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }
        // GITHUB
        private async void Button1_Click(object sender, EventArgs e)
        {
            // 禁用安装按钮
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;

            // 寻找文件夹
            findLCC();

            // 下载MelonLoader
            try
            {
                noteLabel.Text = "正在下载并解压MelonLoader...";
                bool MelonLoaderVersion = !File.Exists(limbusCompanyDir + "/MelonLoader/net6/MelonLoader.dll") || new Version(FileVersionInfo.GetVersionInfo(limbusCompanyDir + "/MelonLoader/net6/MelonLoader.dll").ProductVersion) < new Version("0.6.1");
                if (MelonLoaderVersion)
                {
                    if (Directory.Exists(limbusCompanyDir + "/MelonLoader"))
                        Directory.Delete(limbusCompanyDir + "/MelonLoader", true);
                    string melonLoaderUrl = "https://github.com/LavaGang/MelonLoader/releases/download/v0.6.1/MelonLoader.x64.zip";
                    string melonLoaderZipPath = Path.Combine(limbusCompanyDir, "MelonLoader.x64.zip");
                    await DownloadFileAsync(melonLoaderUrl, melonLoaderZipPath);
                    new SevenZipExtractor(melonLoaderZipPath).ExtractAll(limbusCompanyDir, true);
                    File.Delete(melonLoaderZipPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现了问题。\n" + ex.ToString());
                Close();
                return;
            }
            progressBar1.Value = 50;

            // 下载Font
            noteLabel.Text = "正在下载并解压tmpchinese...";
            string modsDir = Path.Combine(limbusCompanyDir, "Mods");
            Directory.CreateDirectory(modsDir);
            string tmpchinese = modsDir + "/tmpchinesefont";

            var LastWriteTime = File.Exists(tmpchinese) ? new FileInfo(tmpchinese).LastWriteTime.ToString("yyMMdd") : string.Empty;
            try
            {
                if (CheckChineseFontAssetUpdate(LastWriteTime, out var tmpchineseUrl))
                {
                    string tmpchineseZipPath = Path.Combine(limbusCompanyDir, "tmpchinese.7z");
                    await DownloadFileAsync(tmpchineseUrl, tmpchineseZipPath);
                    new SevenZipExtractor(tmpchineseZipPath).ExtractAll(limbusCompanyDir, true);
                    File.Delete(tmpchineseZipPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现了问题。\n" + ex.ToString());
                Close();
                return;
            }
            progressBar1.Value = 75;

            // 下载本体
            noteLabel.Text = "正在检查并更新LimbusLocalize...";
            string limbusLocalizeDllPath = modsDir + "/LimbusLocalize.dll";
            string latestVersion = GetLatestLimbusLocalizeVersion(out string latest2ReleaseTag);
            if (File.Exists(limbusLocalizeDllPath))
            {
                // Check version
                var versionInfo = FileVersionInfo.GetVersionInfo(limbusLocalizeDllPath);
                string currentVersion = "v" + versionInfo.ProductVersion;
                if (new Version(versionInfo.ProductVersion) >= new Version(latestVersion.Remove(0, 1)))
                {
                    MessageBox.Show("一切都是最新的。已完成。", "完成", MessageBoxButtons.OK);
                    Close();
                    return;
                }
                else
                {
                    // Download and extract latest version
                    string limbusLocalizeUrl = GetLatestLimbusLocalizeDownloadUrl(latestVersion, !string.IsNullOrEmpty(latest2ReleaseTag) && currentVersion == latest2ReleaseTag);
                    string limbusLocalizeZipPath = Path.Combine(limbusCompanyDir, "LimbusLocalize.7z");
                    await DownloadFileAsync(limbusLocalizeUrl, limbusLocalizeZipPath);
                    new SevenZipExtractor(limbusLocalizeZipPath).ExtractAll(limbusCompanyDir, true);
                    File.Delete(limbusLocalizeZipPath);
                }
            }
            else
            {
                // Download and extract latest version
                string limbusLocalizeUrl = GetLatestLimbusLocalizeDownloadUrl(latestVersion, false);
                string limbusLocalizeZipPath = Path.Combine(limbusCompanyDir, "LimbusLocalize.7z");
                await DownloadFileAsync(limbusLocalizeUrl, limbusLocalizeZipPath);
                new SevenZipExtractor(limbusLocalizeZipPath).ExtractAll(limbusCompanyDir, true);
                File.Delete(limbusLocalizeZipPath);
            }
            progressBar1.Value = 100;
            MessageBox.Show("安装已完成！", "完成", MessageBoxButtons.OK);
            Close();
        }
        private async void Button2_Click(object sender, EventArgs e)
        {
            // 禁用安装按钮
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;

            // 寻找文件夹
            findLCC();

            // 下载MelonLoader
            try
            {
                noteLabel.Text = "正在下载并解压MelonLoader...";
                if (Directory.Exists(limbusCompanyDir + "/MelonLoader"))
                {
                    Directory.Delete(limbusCompanyDir + "/MelonLoader", true);
                }
                string melonLoaderUrl = "https://limbus.determination.top/files/MelonLoader.x64.zip";
                string melonLoaderZipPath = Path.Combine(limbusCompanyDir, "MelonLoader.x64.zip");
                await DownloadFileAsync(melonLoaderUrl, melonLoaderZipPath);
                new SevenZipExtractor(melonLoaderZipPath).ExtractAll(limbusCompanyDir, true);
                File.Delete(melonLoaderZipPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现了问题。\n" + ex.ToString());
                Close();
                return;
            }
            progressBar1.Value = 50;

            // 下载Font
            noteLabel.Text = "正在下载并解压tmpchinese...";
            string modsDir = Path.Combine(limbusCompanyDir, "Mods");
            Directory.CreateDirectory(modsDir);

            try
            {
                string tmpchineseZipPath = Path.Combine(limbusCompanyDir, "tmpchinese.7z");
                await DownloadFileAsync("https://limbus.determination.top/files/tmpchinesefont.7z", tmpchineseZipPath);
                new SevenZipExtractor(tmpchineseZipPath).ExtractAll(limbusCompanyDir, true);
                File.Delete(tmpchineseZipPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现了问题。\n" + ex.ToString());
                Close();
                return;
            }
            progressBar1.Value = 75;

            // 下载本体
            noteLabel.Text = "正在下载并解压模组本体...";
            string limbusLocalizeZipPath = Path.Combine(limbusCompanyDir, "LimbusLocalize.7z");
            await DownloadFileAsync("https://limbus.determination.top/files/LimbusLocalize_FullPack.7z", limbusLocalizeZipPath);
            new SevenZipExtractor(limbusLocalizeZipPath).ExtractAll(limbusCompanyDir, true);
            File.Delete(limbusLocalizeZipPath);
            progressBar1.Value = 100;
            MessageBox.Show("安装已完成！", "完成", MessageBoxButtons.OK);
            Close();
        }

        private async void Button3_Click(object sender, EventArgs e)
        {
            // 禁用安装按钮
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;

            // 寻找文件夹
            findLCC();

            // 下载MelonLoader
            try
            {
                noteLabel.Text = "正在下载并解压MelonLoader...";
                if (Directory.Exists(limbusCompanyDir + "/MelonLoader"))
                {
                    Directory.Delete(limbusCompanyDir + "/MelonLoader", true);
                }
                string melonLoaderUrl = "https://llc.determination.top/files/MelonLoader.x64.zip";
                string melonLoaderZipPath = Path.Combine(limbusCompanyDir, "MelonLoader.x64.zip");
                await DownloadFileAsync(melonLoaderUrl, melonLoaderZipPath);
                new SevenZipExtractor(melonLoaderZipPath).ExtractAll(limbusCompanyDir, true);
                File.Delete(melonLoaderZipPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现了问题。\n" + ex.ToString());
                Close();
                return;
            }
            progressBar1.Value = 50;

            // 下载Font
            noteLabel.Text = "正在下载并解压tmpchinese...";
            string modsDir = Path.Combine(limbusCompanyDir, "Mods");
            Directory.CreateDirectory(modsDir);

            try
            {
                string tmpchineseZipPath = Path.Combine(limbusCompanyDir, "tmpchinese.7z");
                await DownloadFileAsync("https://llc.determination.top/files/tmpchinesefont.7z", tmpchineseZipPath);
                new SevenZipExtractor(tmpchineseZipPath).ExtractAll(limbusCompanyDir, true);
                File.Delete(tmpchineseZipPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现了问题。\n" + ex.ToString());
                Close();
                return;
            }
            progressBar1.Value = 75;

            // 下载本体
            noteLabel.Text = "正在下载并解压模组本体...";
            string limbusLocalizeZipPath = Path.Combine(limbusCompanyDir, "LimbusLocalize.7z");
            await DownloadFileAsync("https://llc.determination.top/files/LimbusLocalize_FullPack.7z", limbusLocalizeZipPath);
            new SevenZipExtractor(limbusLocalizeZipPath).ExtractAll(limbusCompanyDir, true);
            File.Delete(limbusLocalizeZipPath);
            progressBar1.Value = 100;
            MessageBox.Show("安装已完成！", "完成", MessageBoxButtons.OK);
            Close();
        }

        private void Check_DotnetVer()
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                    Check_NET_Version((int)ndpKey.GetValue("Release"));
            }
        }
        private void Check_NET_Version(int releaseKey)
        {
            if (releaseKey >= 528040)
                Has_NET_6_0 = true;
        }

        private async void Install_Dotnet()
        {
            noteLabel.Text = "正在下载并打开安装NET 6.0流程";
            await DownloadFileAsync("https://download.visualstudio.microsoft.com/download/pr/4a725ea4-cd2c-4383-9b63-263156d5f042/d973777b32563272b85617105a06d272/dotnet-sdk-6.0.406-win-x64.exe", "./!!!先安装我!!!NET 6.0.exe");
            var NET = new FileInfo("./!!!先安装我!!!NET 6.0.exe");
            Process.Start(NET.FullName).WaitForExit();
            NET.Delete();
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
            RtlGetVersion(out var osVersion);
            if (osVersion.MajorVersion > 9U)
            {
                isWindows10 = true;
            }
            else
            {
                isWindows10 = false;
            }
        }


        // 下载文件任务
        private async Task DownloadFileAsync(string url, string filePath)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadProgressChanged += (s, e) =>
                {
                    progressBar2.Value = e.ProgressPercentage;
                };
                client.DownloadFileCompleted += (s, e) =>
                {
                    progressBar2.Value = 100;
                };
                await client.DownloadFileTaskAsync(new Uri(url), filePath);
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

        private void findLCC()
        {
            noteLabel.Text = "正在查找Limbus Company目录...";
            progressBar1.Value = 0;
            limbusCompanyDir = FindLimbusCompanyDirectory();
            if (string.IsNullOrEmpty(limbusCompanyDir))
            {
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
                        MessageBox.Show("选择目录有误，没有在当前目录找到游戏。", "错误", MessageBoxButtons.OK);
                        Close();
                    }
                    Console.WriteLine(limbusCompanyDir);
                }
            }
            progressBar1.Value = 25;
        }
        private string FindLimbusCompanyDirectory()
        {
            string LimbusCompanyPath = "./LimbusCompanyPath.txt";
            if (File.Exists(LimbusCompanyPath))
            {
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
        static bool CheckChineseFontAssetUpdate(string LastWriteTime, out string download)
        {
            download = string.Empty;
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent", "request");
                string raw = new StreamReader(client.OpenRead(new Uri("https://api.github.com/repos/LocalizeLimbusCompany/LLC_ChineseFontAsset/releases/latest")), Encoding.UTF8).ReadToEnd();
                var latest = JSONNode.Parse(raw).AsObject;
                string latestReleaseTag = latest["tag_name"].Value;
                if (latestReleaseTag != LastWriteTime)
                {
                    download = "https://github.com/LocalizeLimbusCompany/LLC_ChineseFontAsset/releases/download/" + latestReleaseTag + "/tmpchinesefont_" + latestReleaseTag + ".7z";
                    return true;
                }
            }
            return false;
        }

        private string GetLatestLimbusLocalizeDownloadUrl(string version, bool isota)
        {
            return "https://github.com/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/download/" + version + "/LimbusLocalize_" + (isota ? "OTA_" : string.Empty) + version + ".7z";
        }
        private string GetLatestLimbusLocalizeVersion(out string latest2ReleaseTag)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent", "request");
                string raw = new StreamReader(client.OpenRead(new Uri("https://api.github.com/repos/LocalizeLimbusCompany/LocalizeLimbusCompany/releases")), Encoding.UTF8).ReadToEnd();
                JSONArray releases = JSONNode.Parse(raw).AsArray;

                string latestReleaseTag = releases[0]["tag_name"].Value;
                latest2ReleaseTag = releases.Count > 1 ? releases[1]["tag_name"].Value : string.Empty;
                return latestReleaseTag;
            }
        }
        // 变量区
        private bool Has_NET_6_0;
        private bool isWindows10;
        private string limbusCompanyDir;
        private string limbusCompanyGameDir;
    }
}
