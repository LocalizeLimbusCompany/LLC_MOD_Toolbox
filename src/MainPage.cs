/*

                    警告：
        本安装器代码可能带有以下成分：
        1. 瞎命名函数
        2. 瞎放函数
        3. 睿智实现功能
        4. 可读性为零的结构
        5. 垃圾性能
        如果感到不适，请立刻退出本文件！

*/
// 我的价值终会实现的。
using log4net;
using Microsoft.Win32;
using SevenZipNET;
using SharpConfig;
using SimpleJSON;
using Sunny.UI;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LLC_MOD_Toolbox
{

    public partial class MainPage : UIForm
    {
        public const string VERSION = "0.6.1";
        private string tipTexts;
        private string TipsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tips.txt");
        // 注册日志系统
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MainPage()
        {
            InitializeComponent();
        }

        // 窗体加载事件
        private void MainPage_Load(object sender, EventArgs e)
        {
            logger.Info("-----------------------");
            alreadyLoaded = false;
            ControlButton(false);
            logger.Info("正在初始化窗体。");
            Init_Toolbox();
            alreadyLoaded = true;
            logger.Info("窗体已完成加载。");
            ChangeStatu("空闲中！");
            logger.Info("安装器版本：" + VERSION);
            ControlButton(true);
        }

        // 初始化
        private void Init_Toolbox()
        {
            manual_has_open = false;
            config_has_open = false;
            filereplace_has_open = false;
            devmode = false;

            // ChangeStatu("获取最快节点。");
            // fastestNode = GetFastnetNode();

            if (CheckToolboxUpdate(VERSION, false))
            {
                logger.Info("安装器存在更新。");
                MessageBox.Show("安装器存在更新。\n点击确定进入官网下载最新版本工具箱", "更新提醒");
                Openuri("https://www.zeroasso.top/docs/install/autoinstall");
                Close();
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
                FolderBrowserDialog dialog = new()
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

            readConfig();

            if (File.Exists(TipsPath))
            {
                var tips = JSONNode.Parse(File.ReadAllText(TipsPath)).AsObject;
                string tipsversion = tips["version"].Value;
                logger.Info(tipsversion);
                CheckTipUpdate(tipsversion);
            }
            else
            {
                using WebClient client = new();
                string raw = string.Empty;
                try
                {
                    raw = new StreamReader(client.OpenRead(new Uri("https://dl.kr.zeroasso.top/api/tip.json")), Encoding.UTF8).ReadToEnd();
                    File.WriteAllText(TipsPath, raw);
                }
                catch (Exception ex)
                {
                    logger.Error("出现问题： " + ex.ToString());
                }
            }
            tipTexts = File.ReadAllText(TipsPath);
            Application.DoEvents();
            TipTimer.Enabled = true;
        }

        // 安装
        private async void installButton_Click(object sender, EventArgs e)
        {
            logger.Info("开始安装。");

            logger.Info("安装 Bepinex 中。");

            ControlButton(false);

            logger.Info("检查某些可能出现的问题。");

            Process[] limbusProcess = Process.GetProcessesByName("LimbusCompany");

            if (limbusProcess.Length > 0)
            {
                logger.Warn("LimbusCompany仍然开启。");
                MessageBox.Show("Limbus Company 仍然处于开启状态！\n您需要在关闭游戏后才能安装模组。", "警告");
                ControlButton(true);
                return;
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
                        BepInExZipPath = Path.Combine(limbusCompanyDir, "BepInEx-IL2CPP-x64.7z");
                        logger.Info("BepInEx Zip目录： " + BepInExZipPath);
                        await DownloadFileAutoSelect("BepInEx-IL2CPP-x64.7z", BepInExZipPath);
                        logger.Info("开始解压 BepInEx zip。");
                        new SevenZipExtractor(BepInExZipPath).ExtractAll(limbusCompanyDir, true);
                        logger.Info("解压完成。删除 BepInEx zip。");
                        File.Delete(BepInExZipPath);
                    }
                    else
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(limbusCompanyDir + "/BepInEx/core/BepInEx.Core.dll");
                        if (new Version(versionInfo.FileVersion.Remove(5, 2)) < new Version("6.0.1"))
                        {
                            logger.Info("未检测到正确Bepinex。");
                            MessageBox.Show("如果你安装了杀毒软件，接下来可能会提示工具箱正在修改关键dll。\n允许即可。如果不信任汉化补丁，可以退出本程序。", "警告");
                            BepInExZipPath = Path.Combine(limbusCompanyDir, "BepInEx-IL2CPP-x64.7z");
                            logger.Info("BepInEx Zip目录： " + BepInExZipPath);
                            await DownloadFileAutoSelect("BepInEx-IL2CPP-x64.7z", BepInExZipPath);
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
                    if (!File.Exists(limbusCompanyDir + "/BepInEx/core/BepInEx.Core.dll"))
                    {
                        logger.Info("从 Github 下载 BepInEx 。");
                        BepInExUrl = "https://github.com/LocalizeLimbusCompany/BepInEx_For_LLC/releases/download/v6.0.1-LLC/BepInEx-IL2CPP-x64-6.0.1.7z";
                        BepInExZipPath = Path.Combine(limbusCompanyDir, "BepInEx-IL2CPP-x64-6.0.1.7z");
                        logger.Info("BepInEx Zip路径： " + BepInExZipPath);
                        await DownloadFileAsync(BepInExUrl, BepInExZipPath);
                        logger.Info("开始解压 BepInEx zip。");
                        new SevenZipExtractor(BepInExZipPath).ExtractAll(limbusCompanyDir, true);
                        logger.Info("解压完成。删除 BepInEx zip。");
                        File.Delete(BepInExZipPath);
                    }
                    else
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(limbusCompanyDir + "/BepInEx/core/BepInEx.Core.dll");
                        if (new Version(versionInfo.FileVersion.Remove(5, 2)) < new Version("6.0.1"))
                        {
                            logger.Info("未检测到正确Bepinex。");
                            MessageBox.Show("如果你安装了杀毒软件，接下来可能会提示工具箱正在修改关键dll。\n允许即可。如果不信任汉化补丁，可以退出本程序。", "警告");
                            BepInExUrl = "https://github.com/LocalizeLimbusCompany/BepInEx_For_LLC/releases/download/v6.0.1-LLC/BepInEx-IL2CPP-x64-6.0.1.7z";
                            BepInExZipPath = Path.Combine(limbusCompanyDir, "BepInEx-IL2CPP-x64-6.0.1.7z");
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
                        await DownloadFileAutoSelect("tmpchinesefont_BIE.7z", tmpchineseZipPath);
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
                if (devmode)
                {
                    logger.Info("开发者模式。下载测试文件：" + devfilename);
                    await DownloadFileAsync("https://dev.zeroasso.top/files/LimbusLocalize_Dev_" + devfilename + ".7z", limbusLocalizeZipPath);
                    logger.Info("解压模组本体 zip 中。");
                    new SevenZipExtractor(limbusLocalizeZipPath).ExtractAll(limbusCompanyDir, true);
                    logger.Info("删除模组本体 zip 。");
                    File.Delete(limbusLocalizeZipPath);
                }
                else
                {
                    if (useGithub.Active != true)
                    {
                        latestLLCVersion = GetLatestLimbusLocalizeVersion(false, out string latest2ReleaseTag);
                        logger.Info("最后模组版本： " + latestLLCVersion);
                        if (File.Exists(limbusLocalizeDllPath))
                        {
                            var versionInfo = FileVersionInfo.GetVersionInfo(limbusLocalizeDllPath);
                            currentVersion = versionInfo.ProductVersion;
                            if (new Version(currentVersion) < new Version(latestLLCVersion.Remove(0, 1)))
                            {
                                await DownloadFileAutoSelect("LimbusLocalize_BIE_" + latestLLCVersion + ".7z", limbusLocalizeZipPath);
                                if (GetLimbusLocalizeHash() != CalculateSHA256(limbusLocalizeZipPath))
                                {
                                    logger.Error("校验Hash失败。");
                                    MessageBox.Show("校验Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "校验失败");
                                    Close();
                                }
                                logger.Info("解压模组本体 zip 中。");
                                new SevenZipExtractor(limbusLocalizeZipPath).ExtractAll(limbusCompanyDir, true);
                                logger.Info("删除模组本体 zip 。");
                                File.Delete(limbusLocalizeZipPath);
                            }
                        }
                        else
                        {
                            await DownloadFileAutoSelect("LimbusLocalize_BIE_" + latestLLCVersion + ".7z", limbusLocalizeZipPath);
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
                            if (new Version(currentVersion) < new Version(latestLLCVersion.Remove(0, 1)))
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
                installButton.Enabled = true;
                useGithub.ReadOnly = false;
                deleteButton.Enabled = true;
                logger.Info("开启完成。");
            }
            else
            {
                logger.Info("正在关闭按钮。");
                uiTabControl.Enabled = false;
                installButton.Enabled = false;
                deleteButton.Enabled = false;
                useGithub.ReadOnly = true;
                logger.Info("关闭完成。");
            }
        }

        static void Openuri(string uri)
        {
            ProcessStartInfo psi = new(uri)
            {
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void Check_DotnetVer()
        {
            logger.Info("检查 .NET FrameWork 版本。");
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            using RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey);
            if (ndpKey != null && ndpKey.GetValue("Release") != null)
            {
                Check_NET_Version((int)ndpKey.GetValue("Release"));
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
            HttpClient httpClient = new HttpClient();
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            long totalSize = response.Content.Headers.ContentLength ?? -1;

            using (Stream contentStream = await response.Content.ReadAsStreamAsync())
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    long totalBytesRead = 0;

                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        totalBytesRead += bytesRead;

                        int progressPercentage = (int)((double)totalBytesRead / totalSize * 100);

                        Invoke(new Action(() =>
                        {
                            DownloadBar.Value = progressPercentage;
                        }));
                    }
                }
            }
        }

        // 自适应下载文件
        private async Task DownloadFileAutoSelect(string file, string filePath)
        {
            string unicom = "http://alist.zeroasso.top:5244/d/unicom/" + file;
            string tianyi = "http://alist.zeroasso.top:5244/d/tianyi/" + file;
            string ofb = "http://alist.zeroasso.top:5244/d/od/" + file;
            string kr = "https://dl.kr.zeroasso.top/files/" + file;
            // 是，我知道这段代码和if else一样，很屎。
            // 体谅一下。没办法。
            if (node == String.Empty)
            {
                await DownloadFileAsync(kr, filePath);
            }
            else
            {
                // 你看这个，简洁大方。
                switch (node)
                {
                    case "unicom":
                        await DownloadFileAsync(unicom, filePath);
                        break;
                    case "tianyi":
                        await DownloadFileAsync(tianyi, filePath);
                        break;
                    case "ofb":
                        await DownloadFileAsync(ofb, filePath);
                        break;
                    case "kr":
                        await DownloadFileAsync(kr, filePath);
                        break;
                    default:
                        break;
                }
            }
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
            using WebClient client = new();
            client.Headers.Add("User-Agent", "request");
            string raw = string.Empty;
            if (IsGithub == true)
            {
                raw = new StreamReader(client.OpenRead(new Uri("https://api.github.com/repos/LocalizeLimbusCompany/LocalizeLimbusCompany/releases")), Encoding.UTF8).ReadToEnd();
            }
            else
            {
                raw = new StreamReader(client.OpenRead(new Uri("https://api.kr.zeroasso.top/Mod_Release.json")), Encoding.UTF8).ReadToEnd();
            }
            JSONArray releases = JSONNode.Parse(raw).AsArray;

            string latestReleaseTag = releases[0]["tag_name"].Value;
            latest2ReleaseTag = releases.Count > 1 ? releases[1]["tag_name"].Value : string.Empty;
            logger.Info("TMP 字体最后标签为： " + latestReleaseTag);
            return latestReleaseTag;
        }

        private string GetLatestLimbusLocalizeDownloadUrl(string version, bool isota)
        {
            return "https://github.com/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/download/" + version + "/LimbusLocalize_BIE_" + (isota ? "OTA_" : string.Empty) + version + ".7z";
        }

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
                var latest = JSONNode.Parse(raw).AsObject;
                string latestReleaseTag = latest["tag_name"].Value;
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

        private string GetLimbusLocalizeHash()
        {
            string unicom = "http://alist.zeroasso.top:5244/d/unicom/LimbusLocalizeHash.json";
            string tianyi = "http://alist.zeroasso.top:5244/d/tianyi/LimbusLocalizeHash.json";
            string ofb = "http://alist.zeroasso.top:5244/d/od/LimbusLocalizeHash.json";
            string kr = "https://dl.kr.zeroasso.top/files/LimbusLocalizeHash.json";
            using WebClient client = new();
            client.Headers.Add("User-Agent", "request");
            string raw = string.Empty;
            if (node == String.Empty)
            {
                raw = new StreamReader(client.OpenRead(new Uri(ofb)), Encoding.UTF8).ReadToEnd();
            }
            else
            {
                switch (node)
                {
                    case "unicom":
                        raw = new StreamReader(client.OpenRead(new Uri(unicom)), Encoding.UTF8).ReadToEnd();
                        break;
                    case "tianyi":
                        raw = new StreamReader(client.OpenRead(new Uri(tianyi)), Encoding.UTF8).ReadToEnd();
                        break;
                    case "ofb":
                        raw = new StreamReader(client.OpenRead(new Uri(ofb)), Encoding.UTF8).ReadToEnd();
                        break;
                    case "kr":
                        raw = new StreamReader(client.OpenRead(new Uri(kr)), Encoding.UTF8).ReadToEnd();
                        break;
                    default:
                        raw = new StreamReader(client.OpenRead(new Uri(ofb)), Encoding.UTF8).ReadToEnd();
                        break;
                }
            }
            var hashObject = JSONNode.Parse(raw).AsObject;
            string hash = hashObject["hash"].Value;
            logger.Info("获取到的最新Hash为：" + hash);
            return hash;
        }

        static bool CheckToolboxUpdate(string version, bool IsGithub)
        {
            logger.Info("正在检查工具箱更新。");
            try
            {
                using WebClient client = new();
                client.Headers.Add("User-Agent", "request");
                string raw = string.Empty;
                if (IsGithub == true)
                {
                    logger.Info("从Github检查。");
                    raw = new StreamReader(client.OpenRead(new Uri("https://api.github.com/repos/LocalizeLimbusCompany/LLC_MOD_Toolbox/releases/latest")), Encoding.UTF8).ReadToEnd();
                }
                else
                {
                    logger.Info("从镜像检查。");
                    raw = new StreamReader(client.OpenRead(new Uri("https://api.kr.zeroasso.top/Toolbox_Release.json")), Encoding.UTF8).ReadToEnd();
                }
                var latest = JSONNode.Parse(raw).AsObject;
                string latestReleaseTag = latest["tag_name"].Value.Remove(0, 1);
                logger.Info("最新安装器tag：" + latestReleaseTag);
                if (new Version(latestReleaseTag) > new Version(version))
                {
                    logger.Info("有更新。");
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error("出现了问题。\n" + ex.ToString());
            }
            logger.Info("没有更新。");
            return false;
        }

        static bool CheckBepInExUpdate(string version, bool IsGithub, out string tag)
        {
            tag = string.Empty;
            try
            {
                using WebClient client = new();
                client.Headers.Add("User-Agent", "request");
                string raw = string.Empty;
                if (IsGithub == true)
                {
                    raw = new StreamReader(client.OpenRead(new Uri("https://api.github.com/repos/LocalizeLimbusCompany/BepInEx_For_LLC/releases/latest")), Encoding.UTF8).ReadToEnd();
                }
                else
                {
                    raw = new StreamReader(client.OpenRead(new Uri("https://api.kr.zeroasso.top/BepInEx_For_LLC_Release.json")), Encoding.UTF8).ReadToEnd();
                }
                var latest = JSONNode.Parse(raw).AsObject;
                string latestReleaseTag = latest["tag_name"].Value;
                if (latestReleaseTag != version)
                {
                    tag = latestReleaseTag;
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现了问题。\n" + ex.ToString());
                logger.Error("出现了问题：\n" + ex.ToString());
            }
            return false;
        }
        public static string CalculateSHA256(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var fileStream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = sha256.ComputeHash(fileStream);
                    logger.Info("计算位置为 " + filePath + " 的文件的Hash结果为：" + BitConverter.ToString(hashBytes).Replace("-", "").ToLower());
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }
            }
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

        private void selectBepinex_Click(object sender, EventArgs e)
        {
            logger.Info("手动选择 BepInEx 。");
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "BepInEx压缩包 (*.7z)|*.7z";

            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                logger.Info("选择的文件路径： " + filePath);
                bepinexfile.Text = filePath;
            }
        }
        private void selectTmp_Click(object sender, EventArgs e)
        {
            logger.Info("手动选择 TMP 。");
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "TMP压缩包 (*.7z)|*.7z";

            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                logger.Info("选择的文件路径： " + filePath);
                tmpfile.Text = filePath;
            }
        }

        private void SelectLLCFile_Click(object sender, EventArgs e)
        {
            logger.Info("手动选择汉化补丁。");
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "汉化补丁压缩包 (*.7z)|*.7z";

            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                logger.Info("选择的文件路径： " + filePath);
                llcfile.Text = filePath;
            }
        }

        private void filereplace_button_Click(object sender, EventArgs e)
        {
            logger.Info("选择文件覆盖文件。");
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "文件覆盖压缩包 (*.zip)|*.zip";

            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                logger.Info("选择的文件路径： " + filePath);
                filereplace_file.Text = filePath;
            }
        }

        private void filereplace_start_Click(object sender, EventArgs e)
        {
            if (filereplace_file.Text == "请点击右侧浏览文件" || filereplace_file.Text == String.Empty)
            {
                MessageBox.Show("未填写文件路径。", "提示");
                logger.Info("未填写文件路径。");
                return;
            }
            filereplace_start.Enabled = false;
            try
            {
                deleteMelonLoader();
                deleteBepInEx();
                MessageBox.Show("如果你安装了杀毒软件，接下来可能会提示工具箱正在修改关键dll。\n允许即可。如果不信任汉化补丁，可以退出本程序。", "警告");
                new SevenZipExtractor(filereplace_file.Text).ExtractAll(limbusCompanyDir, true);
                MoveFolder(limbusCompanyDir + "/Limbus Company", limbusCompanyDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现了问题。\n" + ex.ToString());
                logger.Error("出现了问题：\n" + ex.ToString());
            }
            MessageBox.Show("安装已完成！\n你现在可以运行游戏了。\n加载时请耐心等待。", "完成", MessageBoxButtons.OK);
            filereplace_start.Enabled = true;
        }

        static void MoveFolder(string sourceFolderPath, string destinationFolderPath)
        {
            Directory.CreateDirectory(destinationFolderPath);

            foreach (string file in Directory.GetFiles(sourceFolderPath))
            {
                string fileName = Path.GetFileName(file);
                string destinationFilePath = Path.Combine(destinationFolderPath, fileName);
                File.Move(file, destinationFilePath);
            }

            foreach (string subfolder in Directory.GetDirectories(sourceFolderPath))
            {
                string subfolderName = Path.GetFileName(subfolder);
                string destinationSubfolderPath = Path.Combine(destinationFolderPath, subfolderName);
                MoveFolder(subfolder, destinationSubfolderPath);
            }

            Directory.Delete(sourceFolderPath);
        }

        private void config_open_Click(object sender, EventArgs e)
        {
            if (!config_has_open)
            {
                logger.Info("开启更改配置页面");
                config_open.Text = "关闭";
                config_open_text.Text = "关闭更改配置页面";
                this.uiTabControl.Controls.Add(this.tabPage6);
                config_has_open = true;
                MessageBox.Show("开启成功", "提示");
            }
            else
            {
                logger.Info("关闭更改配置页面");
                config_open.Text = "开启";
                config_open_text.Text = "开启更改配置页面";
                this.uiTabControl.Controls.Remove(this.tabPage6);
                config_has_open = false;
                MessageBox.Show("关闭成功", "提示");
            }
        }

        private void NodeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (NodeComboBox.Text)
            {
                case "镜像节点-1-高速":
                    node = "tianyi";
                    break;
                case "镜像节点-2-高速-联通优化":
                    node = "unicom";
                    break;
                case "镜像节点-3-支持我们":
                    node = "ofb";
                    break;
                case "镜像节点-4-韩国首尔":
                    node = "kr";
                    break;
                default:
                    break;
            }
        }

        private void ResetNode_Click(object sender, EventArgs e)
        {
            NodeComboBox.Text = "手动选择节点（点击右方箭头选择）";
            node = String.Empty;
        }
        private void NodeInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("镜像节点-3为我们的 Onedrive For Business 链接。\n使用此链接下载可帮助开发者续费 Microsft E5 开发者账号。", "节点说明");
        }
        private void manualInstall_Click(object sender, EventArgs e)
        {
            if (bepinexfile.Text == "请点击右侧浏览文件" || bepinexfile.Text == String.Empty)
            {
                MessageBox.Show("未填写 BepInEx 的文件路径。", "提示");
                logger.Info("未填写 BepInEx 的文件路径。");
                return;
            }
            if (tmpfile.Text == "请点击右侧浏览文件" || tmpfile.Text == String.Empty)
            {
                MessageBox.Show("未填写 TMP 的文件路径。", "提示");
                logger.Info("未填写 TMP 的文件路径。");
                return;
            }
            if (llcfile.Text == "请点击右侧浏览文件" || llcfile.Text == String.Empty)
            {
                MessageBox.Show("未填写汉化补丁的文件路径。", "提示");
                logger.Info("未填写汉化补丁的文件路径。");
                return;
            }
            manualInstall.Enabled = false;
            try
            {
                deleteMelonLoader();
                deleteBepInEx();
                MessageBox.Show("如果你安装了杀毒软件，接下来可能会提示工具箱正在修改关键dll。\n允许即可。如果不信任汉化补丁，可以退出本程序。", "警告");
                new SevenZipExtractor(bepinexfile.Text).ExtractAll(limbusCompanyDir, true);
                new SevenZipExtractor(tmpfile.Text).ExtractAll(limbusCompanyDir, true);
                new SevenZipExtractor(llcfile.Text).ExtractAll(limbusCompanyDir, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现了问题。\n" + ex.ToString());
                logger.Error("出现了问题：\n" + ex.ToString());
            }
            MessageBox.Show("安装已完成！\n你现在可以运行游戏了。\n加载时请耐心等待。", "完成", MessageBoxButtons.OK);
            manualInstall.Enabled = true;
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
            deleteFile(limbusCompanyDir + "/框架日志.log");
            deleteFile(limbusCompanyDir + "/游戏日志.log");
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
            deleteFile(limbusCompanyDir + "/changelog.txt");
        }

        #region 配置文件

        private void writeConfigBool(string node, bool change)
        {
            string cfgfile = limbusCompanyDir + "/BepInEx/config/Com.Bright.LocalizeLimbusCompany.cfg";

            logger.Info("更改配置文件（Bool），节点：" + node + "，改为：" + change.ToString());

            if (File.Exists(cfgfile))
            {
                try
                {
                    Configuration config = Configuration.LoadFromFile(cfgfile);
                    foreach (Section item in config)
                    {
                        if (item.Name == "LLC Settings")
                        {
                            item[node].BoolValue = change;
                        }
                    }
                    config.SaveToFile(cfgfile, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("出现了问题。\n" + ex.ToString());
                    logger.Error("出现了问题：\n" + ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("配置文件不存在。\n请尝试进入游戏一次游戏后，退出，再进行修改操作。");
                logger.Error("配置文件不存在。");
            }
        }

        private void writeConfigString(string node, string change)
        {
            string cfgfile = limbusCompanyDir + "/BepInEx/config/Com.Bright.LocalizeLimbusCompany.cfg";

            logger.Info("更改配置文件（String），节点：" + node + "，改为：" + change);

            if (File.Exists(cfgfile))
            {
                try
                {
                    Configuration config = Configuration.LoadFromFile(cfgfile);
                    foreach (Section item in config)
                    {
                        if (item.Name == "LLC Settings")
                        {
                            item[node].StringValue = change;
                        }
                    }
                    config.SaveToFile(cfgfile, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("出现了问题。\n" + ex.ToString());
                    logger.Error("出现了问题：\n" + ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("配置文件不存在。\n请尝试进入游戏一次游戏后，退出，再进行修改操作。");
                logger.Error("配置文件不存在。");
            }
        }

        private void readConfig()
        {
            logger.Info("读取配置。");

            string cfgfile = limbusCompanyDir + "/BepInEx/config/Com.Bright.LocalizeLimbusCompany.cfg";

            if (File.Exists(cfgfile))
            {
                try
                {
                    Configuration config = Configuration.LoadFromFile(cfgfile);

                    foreach (Section item in config)
                    {
                        if (item.Name == "LLC Settings")
                        {
                            randomallcg.Active = item["RandomAllLoadCG"].BoolValue;
                            loadcustomtext.Active = item["RandomLoadText"].BoolValue;
                            usechinese.Active = item["IsUseChinese"].BoolValue;
                            autoupdate.Active = item["AutoUpdate"].BoolValue;
                            if (item["UpdateURI"].StringValue == "Github")
                            {
                                configUseGithub();
                            }
                            else
                            {
                                configUseOfb();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("出现了问题。\n" + ex.ToString());
                    logger.Error("出现了问题：\n" + ex.ToString());
                }
            }
            else
            {
                logger.Error("配置文件不存在。");
            }
        }

        private void configUseGithub()
        {
            update_usegithub.Enabled = false;
            update_useofb.Enabled = true;
            update_usegithub.Text = "(当前)Github";
            update_useofb.Text = "OneDrive For Business";
        }

        private void configUseOfb()
        {
            update_usegithub.Enabled = true;
            update_useofb.Enabled = false;
            update_usegithub.Text = "Github";
            update_useofb.Text = "(当前)OneDrive For Business";
        }

        private void refreshConfig_Click(object sender, EventArgs e)
        {
            logger.Info("点击刷新配置。");
            readConfig();
            MessageBox.Show("读取完成。");
        }

        private void update_usegithub_Click(object sender, EventArgs e)
        {
            logger.Info("自动更新使用Github。");
            writeConfigString("UpdateURI", "Github");
            configUseGithub();
        }

        private void update_useofb_Click(object sender, EventArgs e)
        {
            logger.Info("自动更新使用Ofb。");
            writeConfigString("UpdateURI", "Mirror_OneDrive");
            configUseOfb();
        }

        private void randomallcg_ValueChanged(object sender, bool value)
        {
            if (alreadyLoaded)
            {
                logger.Info("加载页面使用所有已获得CG为：" + randomallcg.Active.ToString());
                writeConfigBool("RandomAllLoadCG", randomallcg.Active);
            }
        }

        private void loadcustomtext_ValueChanged(object sender, bool value)
        {
            if (alreadyLoaded)
            {
                logger.Info("加载页面使用自定义文本为：" + loadcustomtext.Active.ToString());
                writeConfigBool("RandomLoadText", loadcustomtext.Active);
            }
        }

        private void usechinese_ValueChanged(object sender, bool value)
        {
            if (alreadyLoaded)
            {
                logger.Info("是否使用汉化为：" + usechinese.Active.ToString());
                writeConfigBool("IsUseChinese", usechinese.Active);
            }
        }

        private void autoupdate_ValueChanged(object sender, bool value)
        {
            if (alreadyLoaded)
            {
                logger.Info("使用自动更新为：" + autoupdate.Active.ToString());
                writeConfigBool("AutoUpdate", autoupdate.Active);
            }
        }

        #endregion

        #region 链接按钮
        private void EnterToolBoxGithub_Click(object sender, EventArgs e)
        {
            logger.Info("进入工具箱的 Github。");
            Openuri("https://github.com/LocalizeLimbusCompany/LLC_MOD_Toolbox");
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

        #region 灰度测试

        private void get_devjson_Click(object sender, EventArgs e)
        {
            logger.Info("试图获取测试权限。");
            string token = dev_token.Text;
            if (token == null || token == "请在此输入测试秘钥")
            {
                MessageBox.Show("请输入测试秘钥！", "警告");
                logger.Error("测试秘钥为空。");
                return;
            }
            try
            {
                logger.Info("测试秘钥为：" + token);
                string statu = GetDevJson(token, "status");
                if (statu == "stop")
                {
                    MessageBox.Show("测试已停止。", "提示");
                    logger.Info("测试停止。");
                    return;
                }
                test_note.Text = GetDevJson(token, "note");
                devfilename = Regex.Unescape(GetDevJson(token, "file_name"));
                NodeComboBox.Text = "镜像节点-4-韩国首尔";
                NodeComboBox.Enabled = false;
                node = "kr";
                useGithub.Enabled = false;
                this.uiTabControl1.Controls.Remove(this.tabPage5);
                this.uiTabControl1.Controls.Remove(this.tabPage7);
                if (!devmode)
                {
                    this.Text = this.Text + "[测试模式]";
                }
                tabPage8.Text = "自动安装（测试版）";
                devmode = true;
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse httpWebResponse && httpWebResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    MessageBox.Show("您的测试秘钥无效。", "提示");
                    logger.Info("测试秘钥无效。");
                }
                else
                {
                    MessageBox.Show("出现了问题。\n" + ex.ToString());
                    logger.Error("出现了问题：\n" + ex.ToString());
                }
            }
        }

        static string GetDevJson(string token, string gettype)
        {
            using WebClient client = new();
            client.Headers.Add("User-Agent", "request");
            string raw = string.Empty;
            raw = new StreamReader(client.OpenRead(new Uri("https://dev.zeroasso.top/api/" + token + ".json")), Encoding.UTF8).ReadToEnd();
            var rawobject = JSONNode.Parse(raw).AsObject;
            string gettext = rawobject[gettype].Value;
            return gettext;
        }

        #endregion

        #region Tips

        private void tipsTick(object sender, EventArgs e)
        {
            Random random = new();
            var Tip = JSONNode.Parse(tipTexts).AsObject;
            int index = random.Next(Tip["tips"].Count);
            TipText.Text = "Tip: " + Tip["tips"][index].Value;
        }
        private void CheckTipUpdate(string tag)
        {
            logger.Info("检查Tip更新。");
            try
            {
                using WebClient client = new();
                string raw = string.Empty;
                raw = new StreamReader(client.OpenRead(new Uri("https://dl.kr.zeroasso.top/api/tip.json")), Encoding.UTF8).ReadToEnd();
                var json_Object = JSONNode.Parse(raw).AsObject;
                string latestTag = json_Object["version"].Value;
                if (latestTag != tag)
                {
                    logger.Info("版本落后（目前版本：" + tag + "，最新版本：" + latestTag + "），尝试更新。");
                    File.WriteAllText(TipsPath, raw);
                }
                else
                {
                    logger.Info("Tip无需更新。");
                }
            }
            catch (Exception ex)
            {
                logger.Error("出现了问题：\n" + ex.ToString());
            }
        }

        #endregion
        private void downloadFile_Click(object sender, EventArgs e)
        {
            logger.Info("进入下载手动安装文件的链接。");
            Openuri("http://alist.zeroasso.top:5244/od/sharefile");
        }

        private void download_filereplace_Click(object sender, EventArgs e)
        {
            logger.Info("进入下载文件覆盖文件的链接。");
            Openuri("https://n07w1-my.sharepoint.com/:f:/g/personal/northwind_n07w1_onmicrosoft_com/ElVIKQVcHqtCj3a4NJjLdDUBMkVxSQ5S6TGQ0MzZlU1nBw");
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            logger.Info("尝试开关模组。");
            if (File.Exists(limbusCompanyDir + "/winhttp.dll"))
            {
                File.Move(limbusCompanyDir + "/winhttp.dll", limbusCompanyDir + "/winhttp.dll.disable");
                logger.Info("关闭模组成功。");
                MessageBox.Show("关闭模组成功。", "提示");
            }
            else if (File.Exists(limbusCompanyDir + "/winhttp.dll.disable"))
            {
                File.Move(limbusCompanyDir + "/winhttp.dll.disable", limbusCompanyDir + "/winhttp.dll");
                logger.Info("开启模组成功。");
                MessageBox.Show("开启模组成功。", "提示");
            }
            else
            {
                logger.Error("开关失败。");
                MessageBox.Show("开关失败。是否还未安装模组？", "错误");
            }
        }

        private bool Has_NET_6_0 = false;
        private bool isWindows10;

        private string node = string.Empty;

        private string limbusCompanyDir;
        private string limbusCompanyGameDir;

        private string BepInExUrl;
        private string BepInExZipPath;

        private string LimbusLocalizeHash;

        private bool manual_has_open;
        private bool config_has_open;
        private bool filereplace_has_open;

        private bool alreadyLoaded;
        private bool devmode;
        private string devfilename;
    }
}