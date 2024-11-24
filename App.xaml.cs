using System.Windows;
using LLC_MOD_Toolbox.Models;
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
        public static new App Current => (App)Application.Current;
        public IServiceProvider Services { get; }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory>(_ =>
                LoggerFactory.Create(builder =>
                {
                    builder.ClearProviders();
                    builder.AddNLog("nlog.config");
                })
            );
            services.AddTransient<MainWindow>();
            services.AddTransient<AutoInstallerViewModel>();
            services.AddTransient<GachaViewModel>();
            return services.BuildServiceProvider();
        }

        public App()
        {
            Services = ConfigureServices();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var logger = Services.GetService<ILoggerFactory>()!.CreateLogger<App>();
            logger.LogInformation("—————新日志分割线—————");
            logger.LogInformation("工具箱已进入加载流程。");
            logger.LogInformation("We have a lift off.");
            SevenZipBase.SetLibraryPath("7z.dll");
            try
            {
                AutoInstallerViewModel.PrimaryNodeList = await PrimaryNodeList.CreateAsync(
                    "NodeList.json"
                );
                logger.LogInformation("节点初始化完成。");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "节点初始化失败。");
            }
            var mainWindow = Services.GetService<MainWindow>();
            mainWindow.DataContext = Services.GetService<AutoInstallerViewModel>();
            mainWindow?.Show();
        }
    }
}
