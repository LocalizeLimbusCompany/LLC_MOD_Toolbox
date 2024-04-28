/*

                    警告：
        本安装器代码可能带有以下成分：
        1. 瞎命名函数
        2. 瞎放函数
        3. 睿智实现功能
        4. 可读性为零的结构
        5. 垃圾性能
        6. 没有注释！
        如果感到不适，请立刻退出本文件！

*/
using log4net;
using Microsoft.Win32;
using SevenZipNET;
using SharpConfig;
using SimpleJSON;
using Sunny.UI;
using System;
using System.Collections.Generic;
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
using static System.Net.Mime.MediaTypeNames;

namespace LLC_MOD_Toolbox
{

    public partial class MainPage : UIForm
    {
        public const string VERSION = "0.6.3";
        private string tipTexts;
        private string personalTexts;
        private readonly string TipsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tips.txt");
        // 注册日志系统
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗口加载事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 初始化，大部分初始化工作由此完成。
        /// </summary>
        private void Init_Toolbox()
        {
            useGithub = false;
            config_has_open = false;
            devmode = false;
            mirrorGithub = false;

            // ChangeStatu("获取最快节点。");
            // fastestNode = GetFastnetNode();

            if (CheckToolboxUpdate(VERSION, false))
            {
                logger.Info("安装器存在更新。");
                MessageBox.Show("安装器存在更新。\n点击确定进入官网下载最新版本工具箱", "更新提醒");
                Openuri("https://www.zeroasso.top/docs/install/autoinstall");
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
            System.Windows.Forms.Application.DoEvents();
            TipTimer.Enabled = true;
            VERY_SECRET_APRIL_FOOL_METHOD();
            personalTexts = GetWikiPersonalText();
            if (personalTexts == null)
            {
                PersonalButton.Visible = false;
            }
        }

        /// <summary>
        /// 点击安装按钮。安装操作在这里运行。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            // 看这段代码我想吐
            try
            {
                logger.Info("下载 BepInEx For LLC 中。");
                ChangeStatu("正在下载并解压BepInEx...");
                logger.Info("Limbus Company 目录： " + limbusCompanyDir);
                if (!useGithub && !mirrorGithub)
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
                else if (mirrorGithub)
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
                    BepInExUrl = "https://mirror.ghproxy.com/https://github.com/LocalizeLimbusCompany/BepInEx_For_LLC/releases/download/v6.0.1-LLC/BepInEx-IL2CPP-x64-6.0.1.7z";
                    if (!File.Exists(limbusCompanyDir + "/BepInEx/core/BepInEx.Core.dll"))
                    {
                        logger.Info("从 Mirror Github 下载 BepInEx 。");
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
                            MessageBox.Show("如果你安装了杀毒软件，接下来可能会提示工具箱正在修改关键dll。\n允许即可。如果不信任汉化补丁，可以退出本程序。", "警告"); BepInExUrl = "https://github.com/LocalizeLimbusCompany/BepInEx_For_LLC/releases/download/v6.0.1-LLC/BepInEx-IL2CPP-x64-6.0.1.7z";
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
                else
                {
                    BepInExUrl = "https://github.com/LocalizeLimbusCompany/BepInEx_For_LLC/releases/download/v6.0.1-LLC/BepInEx-IL2CPP-x64-6.0.1.7z";
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
                            MessageBox.Show("如果你安装了杀毒软件，接下来可能会提示工具箱正在修改关键dll。\n允许即可。如果不信任汉化补丁，可以退出本程序。", "警告");BepInExUrl = "https://github.com/LocalizeLimbusCompany/BepInEx_For_LLC/releases/download/v6.0.1-LLC/BepInEx-IL2CPP-x64-6.0.1.7z";
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
                if (!useGithub && !mirrorGithub)
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
                else if (mirrorGithub)
                {
                    if (CheckChineseFontAssetUpdate(LastWriteTime, true, out var latestTag))
                    {
                        string downloadTMP = "https://mirror.ghproxy.com/https://github.com/LocalizeLimbusCompany/LLC_ChineseFontAsset/releases/download/" + latestTag + "/tmpchinesefont_BIE_" + latestTag + ".7z";
                        tmpchineseZipPath = Path.Combine(limbusCompanyDir, "tmpchinese_BIE.7z");
                        await DownloadFileAsync(downloadTMP, tmpchineseZipPath);
                        logger.Info("解压 tmp zip 。");
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
            string currentVersion;
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
                    if (!useGithub && !mirrorGithub)
                    {
                        latestLLCVersion = GetLatestLimbusLocalizeVersion();
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
                    else if (mirrorGithub)
                    {
                        latestLLCVersion = GetLatestLimbusLocalizeVersion();
                        logger.Info("最后模组版本： " + latestLLCVersion);
                        string limbusLocalizeUrl = GetLatestLimbusLocalizeDownloadUrl(latestLLCVersion);
                        limbusLocalizeUrl = "https://mirror.ghproxy.com/" + limbusLocalizeUrl;
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
                    else
                    {
                        latestLLCVersion = GetLatestLimbusLocalizeVersion();
                        logger.Info("最后模组版本： " + latestLLCVersion);
                        string limbusLocalizeUrl = GetLatestLimbusLocalizeDownloadUrl(latestLLCVersion);
                        if (mirrorGithub)
                        {
                            limbusLocalizeUrl = "https://mirror.ghproxy.com/" + limbusLocalizeUrl;
                        }
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

        /// <summary>
        /// 控制全局按钮可用性。
        /// </summary>
        /// <param name="CanUse">true为可用，false为不可用</param>
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
                deleteButton.Enabled = true;
                PersonalButton.Enabled = true;
                logger.Info("开启完成。");
            }
            else
            {
                logger.Info("正在关闭按钮。");
                uiTabControl.Enabled = false;
                installButton.Enabled = false;
                deleteButton.Enabled = false;
                PersonalButton.Enabled = false;
                logger.Info("关闭完成。");
            }
        }
        /// <summary>
        /// 打开指定链接。
        /// </summary>
        /// <param name="uri">链接</param>
        static void Openuri(string uri)
        {
            ProcessStartInfo psi = new(uri)
            {
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        /// <summary>
        /// 使用HttpClient异步下载文件。
        /// </summary>
        /// <param name="url">文件链接</param>
        /// <param name="filePath">文件保存地址</param>
        /// <returns></returns>
        private async Task DownloadFileAsync(string url, string filePath)
        {
            logger.Info("从: " + url + "下载文件。");
            HttpClient httpClient = new();
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            long totalSize = response.Content.Headers.ContentLength ?? -1;

            using Stream contentStream = await response.Content.ReadAsStreamAsync();
            using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
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

        /// <summary>
        /// 自动选择下载节点。
        /// </summary>
        /// <param name="file">文件名</param>
        /// <param name="filePath">文件保存地址</param>
        /// <returns></returns>
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
                await DownloadFileAsync(ofb, filePath);
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

        /// <summary>
        /// 获取LCB路径，同时在工具箱目录保存LimbusCompanyPath.txt
        /// </summary>
        /// <returns>返回路径。</returns>
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

        /// <summary>
        /// 更改自动安装界面状态文本。
        /// </summary>
        /// <param name="txt">文本</param>
        private void ChangeStatu(string txt)
        {
            statu.Text = txt;
        }

        /// <summary>
        /// 获取最新汉化模组标签。
        /// </summary>
        /// <returns>返回模组标签</returns>
        private string GetLatestLimbusLocalizeVersion()
        {
            using WebClient client = new();
            client.Headers.Add("User-Agent", "request");
            string raw = string.Empty;
            if (useGithub == true)
            {
                raw = new StreamReader(client.OpenRead(new Uri("https://api.github.com/repos/LocalizeLimbusCompany/LocalizeLimbusCompany/releases")), Encoding.UTF8).ReadToEnd();
            }
            else
            {
                raw = new StreamReader(client.OpenRead(new Uri("https://api.kr.zeroasso.top/Mod_Release.json")), Encoding.UTF8).ReadToEnd();
            }
            JSONArray releases = JSONNode.Parse(raw).AsArray;

            string latestReleaseTag = releases[0]["tag_name"].Value;
            logger.Info("汉化模组最后标签为： " + latestReleaseTag);
            return latestReleaseTag;
        }
        /// <summary>
        /// 获取Github汉化模组下载链接
        /// </summary>
        /// <param name="version">版本号</param>
        /// <returns></returns>
        private string GetLatestLimbusLocalizeDownloadUrl(string version)
        {
            return "https://github.com/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/download/" + version + "/LimbusLocalize_BIE_" + version + ".7z";
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
        /// <summary>
        /// 获取最新版汉化模组哈希
        /// </summary>
        /// <returns>返回Sha256</returns>
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
                raw = node switch
                {
                    "unicom" => new StreamReader(client.OpenRead(new Uri(unicom)), Encoding.UTF8).ReadToEnd(),
                    "tianyi" => new StreamReader(client.OpenRead(new Uri(tianyi)), Encoding.UTF8).ReadToEnd(),
                    "ofb" => new StreamReader(client.OpenRead(new Uri(ofb)), Encoding.UTF8).ReadToEnd(),
                    "kr" => new StreamReader(client.OpenRead(new Uri(kr)), Encoding.UTF8).ReadToEnd(),
                    _ => new StreamReader(client.OpenRead(new Uri(ofb)), Encoding.UTF8).ReadToEnd(),
                };
            }
            var hashObject = JSONNode.Parse(raw).AsObject;
            string hash = hashObject["hash"].Value;
            logger.Info("获取到的最新Hash为：" + hash);
            return hash;
        }
        /// <summary>
        /// 检查工具箱更新
        /// </summary>
        /// <param name="version">当前版本</param>
        /// <param name="IsGithub">是否使用Github</param>
        /// <returns>是否存在更新</returns>
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
        /// 点击删除模组后的逻辑。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            OpenFileDialog openFileDialog = new()
            {
                Filter = "BepInEx压缩包 (*.7z)|*.7z"
            };

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
            OpenFileDialog openFileDialog = new()
            {
                Filter = "TMP压缩包 (*.7z)|*.7z"
            };

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
            OpenFileDialog openFileDialog = new()
            {
                Filter = "汉化补丁压缩包 (*.7z)|*.7z"
            };

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
            OpenFileDialog openFileDialog = new()
            {
                Filter = "文件覆盖压缩包 (*.zip)|*.zip"
            };

            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                logger.Info("选择的文件路径： " + filePath);
                filereplace_file.Text = filePath;
            }
        }
        /// <summary>
        /// 文件覆盖逻辑。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// 移动某个文件夹。
        /// </summary>
        /// <param name="sourceFolderPath">源文件夹</param>
        /// <param name="destinationFolderPath">目标地址</param>
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
        /// <summary>
        /// 节点选择逻辑。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (NodeComboBox.Text)
            {
                case "镜像节点-1-天翼网盘":
                    node = "tianyi";
                    mirrorGithub = false;
                    useGithub = false;
                    break;
                case "镜像节点-2-联通网盘":
                    node = "unicom";
                    mirrorGithub = false;
                    useGithub = false;
                    break;
                case "镜像节点-3-Onedrive For Business":
                    mirrorGithub = false;
                    useGithub = false;
                    node = "ofb";
                    break;
                case "镜像节点-4-韩国首尔":
                    mirrorGithub = false;
                    useGithub = false;
                    node = "kr";
                    break;
                case "镜像节点-5-GhProxy":
                    mirrorGithub = false;
                    useGithub = false;
                    mirrorGithub = true;
                    MessageBox.Show("此服务不由零协会运营。\n安全性和可用性不作保证。", "提示");
                    break;
                case "直接从Github下载":
                    useGithub = true;
                    MessageBox.Show("若您目前为国内网络环境则无法使用。\n请使用全局代理，Watt Toolkit等工具加速Github。");
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 重置节点为默认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetNode_Click(object sender, EventArgs e)
        {
            NodeComboBox.Text = "手动选择节点（点击右方箭头选择）";
            node = String.Empty;
        }
        /// <summary>
        /// 节点信息。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("镜像节点1：从天翼网盘下载。\n" +
                "镜像节点2：从联通云盘下载。\n" +
                "镜像节点3：从零协会Onedrive For Business下载。此为默认下载方式，速度较慢，但可以为零协会续费Office E5开发者账号。\n" +
                "镜像节点4：从零协会韩国首尔服务器下载。\n" +
                "镜像节点5：从mirror.ghproxy.com下载。注意，由第三方提供，零协会不保证安全性和可用性。\n" +
                "直接从Github下载：从github下载。需要代理服务。", "节点说明", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }
        /// <summary>
        /// 手动安装逻辑。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// 删除目录。
        /// </summary>
        /// <param name="path"></param>
        private void deleteDir(string path)
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
        private void deleteFile(string path)
        {
            if (File.Exists(path))
            {
                logger.Info("删除文件： " + path);
                File.Delete(path);
            }
        }
        /// <summary>
        /// 删除Melonloader版本汉化补丁。
        /// </summary>
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
        /// <summary>
        /// 删除BepInEx版本汉化补丁。
        /// </summary>
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
        /// <summary>
        /// 更改汉化补丁配置文件中的Bool值。
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="change">更改为的bool值</param>
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
        /// <summary>
        /// 更改汉化补丁配置文件中的string值
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="change">更改为的string值</param>
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
        /// <summary>
        /// 读取配置并反馈到界面。
        /// </summary>
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
            Openuri("https://pd.qq.com/s/gqpsr265g");
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
        /// <summary>
        /// 检查测试秘钥是否正确，并进行节点锁定，安装方式锁定，获取dev文件链接。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                this.uiTabControl1.Controls.Remove(this.tabPage5);
                this.uiTabControl1.Controls.Remove(this.tabPage7);
                if (!devmode)
                {
                    this.Text += "[测试模式]";
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
        /// <summary>
        /// 获取token对应json链接。
        /// </summary>
        /// <param name="token">测试秘钥</param>
        /// <param name="gettype">要获取的内容类型</param>
        /// <returns></returns>
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

        #region April Fool
        /// <summary>
        /// 愚人节逻辑。
        /// </summary>
        private void VERY_SECRET_APRIL_FOOL_METHOD()
        {
            DateTime currentDate = DateTime.Now;
            if (currentDate.Month == 4 && currentDate.Day == 1)
            {
                logger.Info("您猜怎么着？今儿四月一！");
                AprilFoolMode = true;
                MessageBox.Show("抽卡模拟器的概率发生了一些微妙的变化……", "提示...?", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }

        #endregion

        #region 抽卡模拟器
        /// <summary>
        /// 从Wiki获得人格数据库Json
        /// </summary>
        /// <returns>页面原始数据</returns>
        public static string GetWikiPersonalText()
        {
            logger.Info("获取Wiki人格数据。");
            try
            {
                using WebClient client = new();
                client.Headers.Add("User-Agent", "request");
                string raw = string.Empty;
                logger.Info("从Wiki获取原始文本");
                raw = new StreamReader(client.OpenRead(new Uri("https://limbuscompany.huijiwiki.com/w/api.php?action=query&format=json&prop=revisions&rvprop=content&titles=Data:Identitychoose.tabx")), Encoding.UTF8).ReadToEnd();
                var wikitext = JSONNode.Parse(raw).AsObject;
                string originText = wikitext["query"]["pages"]["4338"]["revisions"][0]["*"].Value;
                string resultText = originText.Replace("\\", "");
                return resultText;
            }
            catch (Exception ex)
            {
                logger.Error("出现了问题。\n" + ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// 点击抽卡模拟器逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PersonalButton_Click(object sender, EventArgs e)
        {
            int[] PersonalData = PersonalDataGen(AprilFoolMode);
            var PersonalObject = JSONNode.Parse(personalTexts).AsObject;
            int PersonalCount = PersonalObject["data"].Count;
            string[] PersonalList = new string[10];
            int CachePersonal;
            Random rand = new Random();
            // 高危代码。如果有问题先看看这里。
            try
            {
                for (int i = 0; i < PersonalData.Length; i++)
                {
                    while (true)
                    {
                        CachePersonal = rand.Next(0, PersonalCount - 1);
                        if (PersonalObject["data"][CachePersonal][6] == PersonalData[i])
                        {
                            PersonalList[i] = PersonalObject["data"][CachePersonal][6] + "★ | " + PersonalObject["data"][CachePersonal][0];
                            break;
                        }
                    }
                }
                string message = "抽卡结果：\n";
                for (int i = 0; i < PersonalData.Length; i++)
                {
                    message += PersonalList[i] + "\n";
                }
                message += "下次还抽吗？";
                MessageBox.Show(message, "结果");
            }
            catch(Exception ex)
            {
                logger.Error("出现了问题。\n" + ex.ToString());
                MessageBox.Show("出现了问题\n" + ex.ToString());
            }
        }
        /// <summary>
        /// 生成人格品质
        /// </summary>
        /// <param name="AprilMode">是否为愚人节，若为愚人节，只会生成1。</param>
        /// <returns>一个int[]，内含有10个1,2,3，代表人格品质</returns>
        public static int[] PersonalDataGen(bool AprilMode)
        {
            Random random = new Random();
            int[] numbers = new int[10];

            for (int i = 0; i < numbers.Length; i++)
            {
                int randomNumber = random.Next(1, 101);
                if (!AprilMode)
                {
                    if (i == 9)
                    {
                        if (randomNumber <= 3)
                        {
                            numbers[i] = 3;
                        }
                        else
                        {
                            numbers[i] = 2;
                        }
                    }
                    else
                    {
                        if (randomNumber <= 13)
                        {
                            numbers[i] = 2;
                        }
                        else
                        {
                            numbers[i] = 1;
                        }
                    }
                }
                else
                {
                    numbers[i] = 1;
                }
            }
            return numbers;
        }
        #endregion
        private void downloadFile_Click(object sender, EventArgs e)
        {
            logger.Info("进入下载手动安装文件的链接（0协会下载站点）。");
            Openuri("http://alist.zeroasso.top:5244/od/sharefile");
        }

        private void download_filereplace_Click(object sender, EventArgs e)
        {
            logger.Info("进入下载文件覆盖文件的链接。");
            Openuri("https://n07w1-my.sharepoint.com/:f:/g/personal/northwind_n07w1_onmicrosoft_com/ElVIKQVcHqtCj3a4NJjLdDUBMkVxSQ5S6TGQ0MzZlU1nBw");
        }
        /// <summary>
        /// 开关模组逻辑。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        private string node = string.Empty;

        private string limbusCompanyDir;
        private string limbusCompanyGameDir;

        private string BepInExUrl;
        private string BepInExZipPath;
        private bool config_has_open;

        private bool alreadyLoaded;
        private bool devmode;
        private string devfilename;

        private bool mirrorGithub = false;
        private bool useGithub;
        private bool AprilFoolMode = false;

        private void NetDiskDownload_Click(object sender, EventArgs e)
        {
            logger.Info("进入下载手动安装文件的链接（天翼云盘）。");
            Openuri("https://cloud.189.cn/web/share?code=7Jryq22yuQny");
        }
    }
}