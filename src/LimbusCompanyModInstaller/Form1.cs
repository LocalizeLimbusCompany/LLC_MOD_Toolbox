using Microsoft.Win32;
using SevenZipNET;
using SimpleJSON;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using File = System.IO.File;

namespace LimbusCompanyModInstaller
{
    public partial class Form1 : Form
    {
        public const string VERSION = "0.2.1";
        public Form1()
        {
            try
            {
                InitializeComponent();
                if (CheckCanUse())
                    Form1_Load();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                Openuri("https://github.com/LocalizeLimbusCompany/LLC_MOD_Installer/releases/latest");
                Close();
            }
        }
        static bool Has_NET_6_0;
        private bool CheckCanUse()
        {
            Version win10version = new Version(10, 0);
            Version currentVersion = Environment.OSVersion.Version;
            if (currentVersion < win10version)
            {
                MessageBox.Show("该模组需要WIN 10运行环境", "致命错误", MessageBoxButtons.OK);
                Close();
                return false;
            }
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                    Check_NET_Version((int)ndpKey.GetValue("Release"));
            }
            return true;
        }
        private void Check_NET_Version(int releaseKey)
        {
            if (releaseKey >= 528040)
                Has_NET_6_0 = true;
        }
        static void Openuri(string uri)
        {
            ProcessStartInfo psi = new ProcessStartInfo(uri)
            {
                UseShellExecute = true
            };
            Process.Start(psi);
        }
        private async void Form1_Load()
        {
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
            catch (Exception ex)
            {
                MessageBox.Show("看起来你无法访问Github\n" + ex.ToString());
                Openuri("https://github.com/LocalizeLimbusCompany/LLC_MOD_Installer/releases/latest");
                Close();
                return;
            }
            if (!Has_NET_6_0)
            {
                label1.Text = "正在下载并打开NET 6.0流程";
                await DownloadFileAsync("https://download.visualstudio.microsoft.com/download/pr/4a725ea4-cd2c-4383-9b63-263156d5f042/d973777b32563272b85617105a06d272/dotnet-sdk-6.0.406-win-x64.exe", "./!!!先安装我!!!NET 6.0.exe");
                var NET = new FileInfo("./!!!先安装我!!!NET 6.0.exe");
                Process.Start(NET.FullName).WaitForExit();
                NET.Delete();
            }
            // Step 1: Find Limbus Company directory
            label1.Text = "正在查找Limbus Company目录...";
            progressBar1.Value = 0;
            string limbusCompanyDir = FindLimbusCompanyDirectory();
            if (string.IsNullOrEmpty(limbusCompanyDir))
            {
                MessageBox.Show("未能找到Limbus Company目录。");
                Close();
                return;
            }
            progressBar1.Value = 25;

            // Step 2: Download and extract MelonLoader
            label1.Text = "正在下载并解压MelonLoader...";
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
            progressBar1.Value = 50;

            // Step 3: Download and extract tmpchinese
            label1.Text = "正在下载并解压tmpchinese...";
            string modsDir = Path.Combine(limbusCompanyDir, "Mods");
            Directory.CreateDirectory(modsDir);
            string tmpchinese = modsDir + "/tmpchinesefont";

            var LastWriteTime = File.Exists(tmpchinese) ? new FileInfo(tmpchinese).LastWriteTime.ToString("yyMMdd") : string.Empty;

            if (CheckChineseFontAssetUpdate(LastWriteTime, out var tmpchineseUrl))
            {
                string tmpchineseZipPath = Path.Combine(limbusCompanyDir, "tmpchinese.7z");
                await DownloadFileAsync(tmpchineseUrl, tmpchineseZipPath);
                new SevenZipExtractor(tmpchineseZipPath).ExtractAll(limbusCompanyDir, true);
                File.Delete(tmpchineseZipPath);
            }

             tmpchinese = modsDir + "/chinese_dante_notes_font";

             LastWriteTime = File.Exists(tmpchinese) ? new FileInfo(tmpchinese).LastWriteTime.ToString("yyMMdd") : string.Empty;

            if (Check_Dante_Notes_FontUpdate(LastWriteTime, out tmpchineseUrl))
            {
                string tmpchineseZipPath = Path.Combine(limbusCompanyDir, "tmpchinese2.7z");
                await DownloadFileAsync(tmpchineseUrl, tmpchineseZipPath);
                new SevenZipExtractor(tmpchineseZipPath).ExtractAll(limbusCompanyDir, true);
                File.Delete(tmpchineseZipPath);
            }
            
            progressBar1.Value = 75;

            // Step 4: Check and update LimbusLocalize
            label1.Text = "正在检查并更新LimbusLocalize...";
            string limbusLocalizeDllPath = modsDir + "/LimbusLocalize.dll";
            string latestVersion = GetLatestLimbusLocalizeVersion(out string latest2ReleaseTag);
            if (File.Exists(limbusLocalizeDllPath))
            {
                // Check version
                var versionInfo = FileVersionInfo.GetVersionInfo(limbusLocalizeDllPath);
                string currentVersion = "v" + versionInfo.ProductVersion;
                if (new Version(versionInfo.ProductVersion) >= new Version(latestVersion.Remove(0, 1)))
                {
                    MessageBox.Show("一切都是最新的。");
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
            MessageBox.Show("完成");
            Close();
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
        static bool Check_Dante_Notes_FontUpdate(string LastWriteTime, out string download)
        {
            download = string.Empty;
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent", "request");
                string raw = new StreamReader(client.OpenRead(new Uri("https://api.github.com/repos/LocalizeLimbusCompany/LLC_Dante_Notes_Font/releases/latest")), Encoding.UTF8).ReadToEnd();
                var latest = JSONNode.Parse(raw).AsObject;
                string latestReleaseTag = latest["tag_name"].Value;
                if (latestReleaseTag != LastWriteTime)
                {
                    download = "https://github.com/LocalizeLimbusCompany/LLC_Dante_Notes_Font/releases/download/" + latestReleaseTag + "/chinese_dante_notes_font_" + latestReleaseTag + ".7z";
                    return true;
                }
            }
            return false;
        }
        private string GetLatestLimbusLocalizeDownloadUrl(string version, bool isota)
        {
            return "https://github.com/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/download/" + version + "/LimbusLocalize_" + (isota ? "OTA_" : string.Empty) + version + ".7z";
        }
    }
}
