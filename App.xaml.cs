using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows;
using LLC_MOD_Toolbox.Helpers;
using LLC_MOD_Toolbox.Models;
using LLC_MOD_Toolbox.Services;
using LLC_MOD_Toolbox.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Targets;
using SevenZip;

namespace LLC_MOD_Toolbox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Gets the current instance of the application.
        /// </summary>
        public static new App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        private readonly ILogger<App> _logger;

        private static ServiceProvider ConfigureServices()
        {
            ServiceCollection services = new();

            // Models
            services.AddSingleton(PrimaryNodeList.Create("NodeList.json"));
            services.AddSingleton<Config>();

            // Services
            services.AddTransient<IFileDownloadService, FileDownloadService>();
            services.AddTransient<IDialogDisplayService, DialogDisplayService>();

            // TODO 尚未实装，因此在中运行时永远忽略
            if (DateTime.Now > DateTime.MinValue)
            {
                services.AddTransient<ILoadingTextService, FileLoadingTextService>();
            }
            else
            {
#pragma warning disable Api // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
                services.AddTransient<ILoadingTextService, ApiLoadingTextService>();
#pragma warning restore Api // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
            }

            // Views
            services.AddTransient<MainWindow>();

            // ViewModels
            services.AddTransient<AutoInstallerViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<GachaViewModel>();
            services.AddTransient<LinkViewModel>();

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
#if DEBUG
                var config = new NLog.Config.LoggingConfiguration();
                config.AddTarget("console", new ConsoleTarget());
                builder.AddNLog(config).SetMinimumLevel(LogLevel.Trace);
#else
                builder.AddNLog("Nlog.config");
#endif
            });

            services.AddHttpClient();

            return services.BuildServiceProvider();
        }

        public App()
        {
            Services = ConfigureServices();
            _logger = Services.GetRequiredService<ILogger<App>>();
            AppDomain.CurrentDomain.UnhandledException += Application_HandleException;
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
#if DEBUG
            AllocConsole();
#endif
            _logger.LogInformation("—————新日志分割线—————");
            _logger.LogInformation("工具箱已进入加载流程。");
            _logger.LogInformation("We have a lift off.");

            if (e.Args.Contains("--cli"))
            {
                _logger.LogInformation("检测到控制台模式参数。");
                RunAsConsole();
                return;
            }

            _logger.LogInformation("当前版本：{}", VersionHelper.LocalVersion);
            // 检查更新
            try
            {
                SevenZipBase.SetLibraryPath("7z.dll");
                IFileDownloadService http = Services.GetRequiredService<IFileDownloadService>();

                PrimaryNodeList NodeList = Services.GetRequiredService<PrimaryNodeList>();
                // TODO: 优化节点选择
                NodeInformation nodeInformation = NodeList.ApiNode.Last(n => n.IsDefault);
                string jsonPayload = await http.GetJsonAsync(
                    UrlHelper.GetReleaseUrl(nodeInformation.Endpoint)
                );
                _logger.LogInformation("API 节点连接成功。");
                string announcement = JsonHelper.DeserializeValue("body", jsonPayload);
                string latestVersion = JsonHelper.DeserializeValue("tag_name", jsonPayload);
                _logger.LogInformation("当前网络版本：{latestVersion}", latestVersion);
                if (VersionHelper.CheckForUpdate(latestVersion))
                {
                    MessageBox.Show(announcement);
                    _logger.LogInformation("检测到新版本，打开链接。");
                    UrlHelper.LaunchUrl("https://www.zeroasso.top/docs/install/autoinstall");
                    throw new NotImplementedException("暂不支持自动更新。");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "网络不通畅，无法获取联网版本");
            }
            catch (NotImplementedException)
            {
                _logger.LogInformation("暂不支持自动更新");
                Current.Shutdown();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查更新时出现异常");
            }
            MainWindow mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void Application_HandleException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
                _logger.LogError(exception, "未处理异常：{}", GetExceptionMessage(exception));
            MessageBox.Show($"出现未处理的异常，请截图留存，否则可能无法定位：{e.ExceptionObject}");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _logger.LogInformation("工具箱已退出。");
        }

        /// <summary>
        /// 获取异常信息
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>获取异常对象的内部信息</returns>
        private static string GetExceptionMessage(Exception ex)
        {
            return ex.InnerException == null
                ? ex.Message
                : $"{ex.Message} -> {GetExceptionMessage(ex.InnerException)}";
        }

        private void RunAsConsole()
        {
            _logger.LogInformation("工具箱已以控制台模式运行。");

            Console.WriteLine("欢迎使用 LLC_MOD_Toolbox！");
            _logger.LogInformation("以控制台方式启动");
            Config config = Services.GetRequiredService<Config>();
            IFileDownloadService fileDownloadService =
                Services.GetRequiredService<IFileDownloadService>();
            List<string> paths = UrlHelper.GetCustumApiUrls(config.ApiNode.Endpoint, config.Token);
            foreach (string path in paths)
            {
                fileDownloadService.GetJsonAsync(path);
            }
            Console.ReadKey();
            Current.Shutdown();
        }

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32", CharSet = CharSet.Auto)]
#pragma warning disable SYSLIB1054 // 使用 “LibraryImportAttribute” 而不是 “DllImportAttribute” 在编译时生成 P/Invoke 封送代码
        internal static extern bool AllocConsole();
#pragma warning restore SYSLIB1054 // 使用 “LibraryImportAttribute” 而不是 “DllImportAttribute” 在编译时生成 P/Invoke 封送代码
    }
}
