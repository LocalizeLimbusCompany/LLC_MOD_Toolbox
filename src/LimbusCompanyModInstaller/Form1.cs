using Microsoft.Win32;
using SevenZipNET;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using File = System.IO.File;

namespace LimbusCompanyModInstaller
{
    public partial class Form1 : Form
    {
        public const string VERSION = "0.1.1";
        static Form1 __instance;
        public Form1()
        {
            __instance = this;
            try
            {
                InitializeComponent();
                if (CheckCanUse())
                    Form1_Load();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                System.Diagnostics.Process.Start("https://github.com/LocalizeLimbusCompany/LLC_MOD_Installer/releases/latest");
                this.Close();
            }
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
            MessageBox.Show("该模组需要.NET 6.0运行环境", "致命错误", MessageBoxButtons.OK);
            System.Diagnostics.Process.Start("https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/sdk-6.0.406-windows-x64-installer");
            this.Close();
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
                System.Diagnostics.Process.Start("https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/sdk-6.0.406-windows-x64-installer");
                this.Close();
            }
            return false;
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
                        System.Diagnostics.Process.Start("https://github.com/LocalizeLimbusCompany/LLC_MOD_Installer/releases/latest");
                        this.Close();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("看起来你无法访问Github\n" + ex.ToString());
                System.Diagnostics.Process.Start("https://github.com/LocalizeLimbusCompany/LLC_MOD_Installer/releases/latest");
                this.Close();
                return;
            }
            // Step 1: Find Limbus Company directory
            label1.Text = "正在查找Limbus Company目录...";
            progressBar1.Value = 0;
            string limbusCompanyDir = FindLimbusCompanyDirectory();
            if (string.IsNullOrEmpty(limbusCompanyDir))
            {
                MessageBox.Show("未能找到Limbus Company目录。");
                this.Close();
                return;
            }
            progressBar1.Value = 25;

            // Step 2: Download and extract MelonLoader
            label1.Text = "正在下载并解压MelonLoader...";
            bool MelonLoaderVersion = File.Exists(limbusCompanyDir + "/MelonLoader/net6/MelonLoader.dll") ? new Version(FileVersionInfo.GetVersionInfo(limbusCompanyDir + "/MelonLoader/net6/MelonLoader.dll").ProductVersion) < new Version("0.6.0") : true;
            if (MelonLoaderVersion)
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

            var LastWriteTime = File.Exists(tmpchinese) ? new FileInfo(tmpchinese).LastWriteTime.ToString("yyMMdd") : string.Empty;

            if (CheckChineseFontAssetUpdate(LastWriteTime, out var tmpchineseUrl))
            {
                string tmpchineseZipPath = Path.Combine(limbusCompanyDir, "tmpchinese.zip");
                await DownloadFileAsync(tmpchineseUrl, tmpchineseZipPath);
                new SevenZipExtractor(tmpchineseZipPath).ExtractAll(limbusCompanyDir, true);
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
                if (new Version(versionInfo.ProductVersion) >= new Version(latestVersion.Remove(0, 1)))
                {
                    MessageBox.Show("一切都是最新的。");
                    this.Close();
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
            this.Close();
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
        private string GetLatestLimbusLocalizeDownloadUrl(string version, bool isota)
        {
            return "https://github.com/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/download/" + version + "/LimbusLocalize_" + (isota ? "OTA_" : string.Empty) + version + ".7z";
        }
    }
}
