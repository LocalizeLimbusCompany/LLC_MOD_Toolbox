// 用于处理后端逻辑。
/*
 * You may think you know what the following code does.
 * But you dont. Trust me.
 * Fiddle with it, and youll spend many a sleepless
 * night cursing the moment you thought youd be clever
 * enough to "optimize" the code below.
 * Now close this file and go play with something else.
 * 你可能会认为你读得懂以下的代码。但是你不会懂的，相信我吧。
 * 要是你尝试玩弄这段代码的话，你将会在无尽的通宵中不断地咒骂自己为什么会认为自己聪明到可以优化这段代码。
 * 现在请关闭这个文件去玩点别的吧。
*/
using LLC_MOD_Toolbox.Models;
using log4net.Config;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using SevenZip;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using static LLC_MOD_Toolbox.SimpleDnsChecker;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        private static string? useEndPoint;
        private static string? useAPIEndPoint;
        private static bool useGithub = false;
        private static string limbusCompanyDir = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 1973530", "InstallLocation", null) as string
            ?? string.Empty;
        private static string limbusCompanyGameDir = Path.Combine(limbusCompanyDir, "LimbusCompany.exe");
        private static readonly string currentDir = AppDomain.CurrentDomain.BaseDirectory;
        private static List<Node> nodeList = [];
        private static List<Node> apiList = [];
        private static string defaultEndPoint = "";
        private static string defaultAPIEndPoint = "";
        private static int installPhase = 0;
        private readonly DispatcherTimer progressTimer;
        private float progressPercentage = 0;
        private bool isNewestModVersion = true;
        private static string greytestUrl = string.Empty;
        private static bool greytestStatus = false;
        private readonly string VERSION = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
        private JObject hashCacheObject = null;
        private static ConfigurationManager configuation = new ConfigurationManager(Path.Combine(currentDir, "config.json"));
        private static bool isLauncherMode = Environment.GetCommandLineArgs().Contains("-launcher");
        internal static bool isMirrorChyanMode = false;
        internal static string mirrorChyanToken = "";

        internal bool isLaunching = false;

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            XmlConfigurator.Configure();
            isLaunching = true;
            Log.logger.Info("—————新日志分割线—————");
            Log.logger.Info("工具箱已进入加载流程。");
            Log.logger.Info("We have a lift off.");
            Log.logger.Info($"WPF架构工具箱 版本：{VERSION} 。");

            LoadAndApplySkin();

            await DisableGlobalOperations();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            using HttpClient client = new HttpClient
            {
                DefaultRequestVersion = HttpVersion.Version11,
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact
            };
            CheckMirrorChyan();
            await CheckLoadingText();
            InitNode();
            await InitializeSkinComboBoxAsync();
            if (isMirrorChyanMode)
            {
                await this.Dispatcher.BeginInvoke(() =>
                {
                    ViewModel.SetNodeOptions(["已使用Mirror酱"]);
                    ViewModel.UpdateSelectedNodeOption("已使用Mirror酱");
                    ViewModel.SetApiOptions(["已使用Mirror酱"]);
                    ViewModel.UpdateSelectedApiOption("已使用Mirror酱");
                });
            }
            else
            {
                ReadConfigNode();
            }
            await RefreshPage();
            await CheckToolboxUpdate(isMirrorChyanMode);
            CheckLimbusCompanyPath();
            SevenZipBase.SetLibraryPath(Path.Combine(currentDir, "7z.dll"));
            await CheckAnno();
            CheckLCBPath();
            bool needUpdate = await ChangeHomePageVersion();
            AdaptFuckingPM.CheckAdapt(limbusCompanyDir);
            if (!isLauncherMode)
            {
                LaunchUpdateLoadingThread();
                await ChangeEEPic();
                await CheckModInstalled();
                await CheckDNS();
            }
            if (isLauncherMode && !hasNewAnno && !needUpdate)
            {
                try
                {
                    OpenUrl("steam://rungameid/1973530");
                }
                catch (Exception ex)
                {
                    Log.logger.Error("出现了问题： ", ex);
                    UniversalDialog.ShowMessage("出现了问题。\n" + ex.ToString(), "提示", null, this);
                }
                Environment.Exit(0);
            }
            if ((configuation.Settings.install.installWhenLaunch || isLauncherMode) && !hasNewAnno && needUpdate)
            {
                await StartAutoInstallAsync();
            }
            await EnableGlobalOperations();
            isLaunching = false;
            Log.logger.Info("加载流程完成。");
        }
    }
}
