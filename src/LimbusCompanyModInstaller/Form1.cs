using Microsoft.Win32;
using SevenZipNET;
using SimpleJSON;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using File = System.IO.File;

namespace LimbusCompanyModInstaller
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            if (CheckCanUse())
                Form1_Load();
        }
        private bool CheckCanUse()
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    return CheckCanUse((int)ndpKey.GetValue("Release"));
                }
            }
            return false;
        }
        private bool CheckCanUse(int releaseKey)
        {
            if (releaseKey >= 528040)
            {
                Version win10version = new Version(10, 0);
                Version currentVersion = Environment.OSVersion.Version;
                if (currentVersion >= win10version)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("该模组需要WIN 10运行环境", "致命错误", MessageBoxButtons.OK);
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("该模组需要.NET 6.0运行环境", "致命错误", MessageBoxButtons.OK);
                this.Close();
            }
            return false;
        }
        private async void Form1_Load()
        {
            // Step 1: Find Limbus Company directory
            label1.Text = "正在查找Limbus Company目录...";
            progressBar1.Value = 0;
            string limbusCompanyDir = FindLimbusCompanyDirectory();
            if (string.IsNullOrEmpty(limbusCompanyDir))
            {
                MessageBox.Show("未能找到Limbus Company目录。");
                return;
            }
            progressBar1.Value = 25;

            // Step 2: Download and extract MelonLoader
            label1.Text = "正在下载并解压MelonLoader...";
            if (!File.Exists(limbusCompanyDir + "/version.dll"))
            {
                string melonLoaderUrl = "https://github.com/LavaGang/MelonLoader/releases/download/v0.6.0/MelonLoader.x64.zip";
                string melonLoaderZipPath = Path.Combine(limbusCompanyDir, "MelonLoader.x64.zip");
                await DownloadFileAsync(melonLoaderUrl, melonLoaderZipPath);
                ZipFile.ExtractToDirectory(melonLoaderZipPath, limbusCompanyDir);
                File.Delete(melonLoaderZipPath);
            }
            progressBar1.Value = 50;

            // Step 3: Download and extract tmpchinese
            label1.Text = "正在下载并解压tmpchinese...";
            string modsDir = Path.Combine(limbusCompanyDir, "Mods");
            Directory.CreateDirectory(modsDir);
            string tmpchinese = modsDir + "/tmpchinesefont";
            if (!File.Exists(tmpchinese))
            {
                string tmpchineseUrl = "https://github.com/Bright1192/LimbusLocalize/releases/download/v0.1.3/tmpchinesefont.7z";
                string tmpchineseZipPath = Path.Combine(limbusCompanyDir, "tmpchinese.zip");
                await DownloadFileAsync(tmpchineseUrl, tmpchineseZipPath);
                ZipFile.ExtractToDirectory(tmpchineseZipPath, limbusCompanyDir);
                File.Delete(modsDir);
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
                if (currentVersion == latestVersion)
                {
                    MessageBox.Show("一切都是最新的。");
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
            this.Close();
            progressBar1.Value = 100;
        }

        private string FindLimbusCompanyDirectory()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType == DriveType.Fixed)
                {
                    DirectoryInfo driveRoot = drive.RootDirectory;

                    DirectoryInfo[] ProgramFiles = driveRoot.GetDirectories("Program Files (x86)");
                    if (ProgramFiles.Length > 0)
                    {
                        DirectoryInfo[] Steam = ProgramFiles[0].GetDirectories("Steam");
                        if (Steam.Length > 0)
                        {
                            DirectoryInfo[] steamapps = Steam[0].GetDirectories("steamapps");
                            if (steamapps.Length > 0)
                            {
                                string commonDir = Path.Combine(steamapps[0].FullName, "common");
                                if (Directory.Exists(commonDir))
                                {
                                    DirectoryInfo[] gameDirs = new DirectoryInfo(commonDir).GetDirectories("Limbus Company");
                                    if (gameDirs.Length > 0)
                                    {
                                        return gameDirs[0].FullName;
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
                string raw = new StreamReader(client.OpenRead(new Uri("https://api.github.com/repos/Bright1192/LimbusLocalize/releases")), Encoding.UTF8).ReadToEnd();
                JSONArray releases = JSONNode.Parse(raw).AsArray;

                string latestReleaseTag = releases[0]["tag_name"].Value;
                latest2ReleaseTag = releases.Count > 1 ? releases[1]["tag_name"].Value : string.Empty;
                return latestReleaseTag;
            }
        }

        private string GetLatestLimbusLocalizeDownloadUrl(string version, bool isota)
        {
            return "https://github.com/Bright1192/LimbusLocalize/releases/download/" + version + "/LimbusLocalize_" + (isota ? "OTA_" : string.Empty) + version + ".7z";
        }
    }
}
