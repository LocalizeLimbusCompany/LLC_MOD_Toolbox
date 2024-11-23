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
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger<App> logger;
        private readonly ServiceProvider serviceProvider;

        public App()
        {
            loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddNLog("nlog.config");
            });
            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory>(loggerFactory);
            serviceProvider = services.BuildServiceProvider();

            logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<App>();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            logger.LogInformation("—————新日志分割线—————");
            logger.LogInformation("工具箱已进入加载流程。");
            logger.LogInformation("We have a lift off.");
            SevenZipBase.SetLibraryPath("7z.dll");
            logger.LogInformation("加载流程完成。");
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
            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
