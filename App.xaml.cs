using System.Net.Http;
using System.Windows;
using LLC_MOD_Toolbox.Helpers;
using LLC_MOD_Toolbox.Models;
using LLC_MOD_Toolbox.Services;
using LLC_MOD_Toolbox.ViewModels;
using LLC_MOD_Toolbox.Views;
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

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Models
            services.AddSingleton<PrimaryNodeList>();

            // Services
            //services.AddKeyedTransient<IFileDownloadService, GrayFileDownloadService>("Gray");
            services.AddTransient<IFileDownloadService, RegularFileDownloadService>();

            // Views
            services.AddTransient<MainWindow>();
            services.AddTransient(sp => new AutoInstaller
            {
                DataContext = sp.GetRequiredService<AutoInstallerViewModel>()
            });
            services.AddTransient(sp => new Settings
            {
                DataContext = sp.GetRequiredService<SettingsViewModel>()
            });

            // ViewModels
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<AutoInstallerViewModel>();
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
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var _logger = Services.GetRequiredService<ILogger<App>>();
                _logger.LogError(e.ExceptionObject as Exception, "未处理异常");
            };
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var _logger = Services.GetRequiredService<ILogger<App>>();
            _logger.LogInformation("—————新日志分割线—————");
            _logger.LogInformation("工具箱已进入加载流程。");
            _logger.LogInformation("We have a lift off.");

            SevenZipBase.SetLibraryPath("7z.dll");
            if (e.Args.Length > 0)
            {
                _logger.LogInformation("检测到启动参数。");
                throw new NotImplementedException("暂不支持启动参数。");
            }
            try
            {
                PrimaryNodeList primaryNodeList = Services.GetRequiredService<PrimaryNodeList>();
                primaryNodeList = await PrimaryNodeList.CreateAsync("NodeList.json");
                _logger.LogInformation("节点初始化完成。");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "节点初始化失败。");
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
                    new Uri(nodeInformation.Endpoint, "Toolbox_Release.json")
                );
                _logger.LogInformation("API 节点连接成功。");
                var latestVersion = JsonHelper.DeserializeTagName(jsonPayload);
                _logger.LogInformation("当前网络版本：{latestVersion}", latestVersion);
                if (VersionHelper.CheckForUpdate(latestVersion))
                {
                    _logger.LogInformation("检测到新版本，打开链接。");
                    PathHelper.LaunchUrl("https://www.zeroasso.top/docs/install/autoinstall");
                    throw new NotImplementedException("暂不支持自动更新。");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "网络不通畅");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "未处理异常");
            }
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow?.Show();
        }
    }
}
