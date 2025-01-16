using System.Windows;
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
        public static new App Current => (App)Application.Current;
        public IServiceProvider Services { get; }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<PrimaryNodeList>();
            services.AddSingleton<IFileDownloadService, GrayFileDownloadService>();
            services.AddSingleton<IFileDownloadService, RegularFileDownloadService>();
            services.AddTransient<MainWindow>();
            services.AddTransient(sp => new AutoInstaller
            {
                DataContext = sp.GetRequiredService<AutoInstallerViewModel>()
            });
            services.AddTransient(sp => new Settings
            {
                DataContext = sp.GetRequiredService<SettingsViewModel>()
            });
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
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var logger = Services.GetRequiredService<ILogger<App>>();
            logger.LogInformation("—————新日志分割线—————");
            logger.LogInformation("工具箱已进入加载流程。");
            logger.LogInformation("We have a lift off.");
            logger.LogInformation("当前版本：{version}", GetType().Assembly.GetName().Version);
            SevenZipBase.SetLibraryPath("7z.dll");
            if (e.Args.Length > 0)
            {
                logger.LogInformation("检测到启动参数。");
                throw new NotImplementedException("暂不支持启动参数。");
            }
            try
            {
                PrimaryNodeList primaryNodeList = Services.GetRequiredService<PrimaryNodeList>();
                primaryNodeList = await PrimaryNodeList.CreateAsync("NodeList.json");
                logger.LogInformation("节点初始化完成。");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "节点初始化失败。");
            }
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow?.Show();
        }
    }
}
