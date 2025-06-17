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

            services.AddScoped<ILoadingTextService, FileLoadingTextService>();
            //services.AddTransient<ILoadingTextService, ApiLoadingTextService>();


            // Views
            services.AddTransient<MainWindow>();

            // ViewModels
            services.AddScoped<MainViewModel>();
            services.AddScoped<AutoInstallerViewModel>();
            services.AddScoped<SettingsViewModel>();
            services.AddScoped<GachaViewModel>();
            services.AddScoped<LinkViewModel>();

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
#if DEBUG
                var config = new NLog.Config.LoggingConfiguration();
                var consoleTarget = new ConsoleTarget("console");
                config.AddTarget(consoleTarget);
                // 添加规则，将所有日志级别从Trace到Fatal的日志路由到控制台
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, consoleTarget);
                // 应用NLog配置
                builder.AddNLog().AddConsole();
#else
                builder.AddNLog("Nlog.config");
#endif
            });

            services.AddHttpClient();

            return services.BuildServiceProvider();
        }

        public App()
        {
#if DEBUG
            AllocConsole();
            Console.WriteLine("控制台已生成");
#endif
            Services = ConfigureServices();
            _logger = Services.GetRequiredService<ILogger<App>>();
            AppDomain.CurrentDomain.UnhandledException += Application_HandleException;
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            _logger.LogInformation("—————新日志分割线—————");
            _logger.LogInformation("工具箱已进入加载流程。");
            _logger.LogInformation("We have a lift off.");

            if (e.Args.Contains("-cli"))
            {
                _logger.LogInformation("检测到控制台模式参数。");
                RunAsConsole();
                Shutdown();
            }

            _logger.LogInformation("当前版本：{}", VersionHelper.LocalVersion);
            // 检查更新
            try
            {
                SevenZipBase.SetLibraryPath("7z.dll");
                _logger.LogTrace("7z.dll 路径已设置。");
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

        /// <summary>
        /// 创建控制台窗口
        /// </summary>
        /// <returns>判断是否成功创建控制台窗口</returns>
        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
#pragma warning disable SYSLIB1054 // 使用 “LibraryImportAttribute” 而不是 “DllImportAttribute” 在编译时生成 P/Invoke 封送代码
        internal static extern bool AllocConsole();
#pragma warning restore SYSLIB1054 // 使用 “LibraryImportAttribute” 而不是 “DllImportAttribute” 在编译时生成 P/Invoke 封送代码

        /// <summary>
        /// 仅在控制台模式下运行时调用的方法。
        /// 注意：请在调用此方法后调用 <see cref="Current.Shutdown"/> 以确保应用程序正确退出。
        /// </summary>
        private void RunAsConsole()
        {
            AllocConsole();
            _logger.LogTrace("工具箱已以控制台模式运行。");

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
            _logger.LogTrace("控制台的汉化安装已完成");
            Console.WriteLine("控制台的汉化安装已完成。请按任意键退出。");
            Console.ReadKey();
            UrlHelper.LaunchUrl("steam://rungameid/1973530");
        }
    }
}
