using System.Net.Http;
using System.Windows;
using LLC_MOD_Toolbox.Helpers;
using LLC_MOD_Toolbox.Models;
using LLC_MOD_Toolbox.Services;
using LLC_MOD_Toolbox.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
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
            var services = new ServiceCollection();

            // Models
            services.AddSingleton(_ => PrimaryNodeList.Create("NodeList.json"));

            // Services
            services.AddTransient<IFileDownloadService, RegularFileDownloadService>();

            // Views
            services.AddTransient<MainWindow>();

            // ViewModels
            services.AddTransient<AutoInstallerViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<GachaViewModel>();

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddNLog("Nlog.config");
            });
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
            _logger.LogInformation("—————新日志分割线—————");
            _logger.LogInformation("工具箱已进入加载流程。");
            _logger.LogInformation("We have a lift off.");

            SevenZipBase.SetLibraryPath("7z.dll");
            if (e.Args.Length > 0)
            {
                _logger.LogInformation("检测到启动参数。");
                throw new NotImplementedException("暂不支持启动参数。");
            }

            _logger.LogInformation("当前版本：{}", VersionHelper.LocalVersion);
            // 检查更新
            try
            {
                var http = Services.GetRequiredService<IFileDownloadService>();
                var NodeList = Services.GetRequiredService<PrimaryNodeList>();
                // TODO: 优化节点选择
                NodeInformation nodeInformation = NodeList.ApiNode.Last(n => n.IsDefault);
                var jsonPayload = await http.GetJsonAsync(
                    string.Format(nodeInformation.Endpoint, "Toolbox_Release.json")
                );
                _logger.LogInformation("API 节点连接成功。");
                var announcement = JsonHelper.DeserializeValue("body", jsonPayload);
                var latestVersion = JsonHelper.DeserializeValue("tag_name", jsonPayload);
                _logger.LogInformation("当前网络版本：{latestVersion}", latestVersion);
                if (VersionHelper.CheckForUpdate(latestVersion))
                {
                    _logger.LogInformation("检测到新版本，打开链接。");
                    PathHelper.LaunchUrl("https://www.zeroasso.top/docs/install/autoinstall");
                    throw new NotImplementedException("暂不支持自动更新。");
                }
                MessageBox.Show(announcement);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "网络不通畅，无法获取联网版本");
            }
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void Application_HandleException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.LogError(e.ExceptionObject as Exception, "未处理异常");
            MessageBox.Show($"出现未处理的异常，请截图留存，否则可能无法定位：{e.ExceptionObject}");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _logger.LogInformation("工具箱已退出。");
        }
    }
}
