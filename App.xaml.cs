using LLC_MOD_Toolbox.Services;
using LLC_MOD_Toolbox.Services.Configuration;
using LLC_MOD_Toolbox.Services.Content;
using LLC_MOD_Toolbox.Services.Font;
using LLC_MOD_Toolbox.Services.Gacha;
using LLC_MOD_Toolbox.Services.Greytest;
using LLC_MOD_Toolbox.Services.Installation;
using LLC_MOD_Toolbox.Services.IO;
using LLC_MOD_Toolbox.Services.Network;
using LLC_MOD_Toolbox.Services.Skin;
using LLC_MOD_Toolbox.Services.UI;
using LLC_MOD_Toolbox.Services.Update;
using LLC_MOD_Toolbox.Services.Version;
using LLC_MOD_Toolbox.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "App.config", ConfigFileExtension = "config", Watch = true)]
namespace LLC_MOD_Toolbox
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; private set; } = null!;
        private static Mutex? _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;
            _mutex = new Mutex(true, "LLC_MOD_TOOLBOX", out createdNew);
            if (!createdNew)
            {
                UniversalDialog.ShowMessage("已有工具箱在运行中！", "提示", null, null);
                Current.Shutdown();
                return;
            }

            var services = new ServiceCollection();
            ConfigureServices(services);
            Services = services.BuildServiceProvider();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;

            services.AddSingleton<AppState>();
            services.AddSingleton(new ConfigurationManager(Path.Combine(currentDir, "config.json")));

            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IHttpService, HttpService>();
            services.AddSingleton<INodeService, NodeService>();
            services.AddSingleton<IMirrorChyanService, MirrorChyanService>();
            services.AddSingleton<IInstallService, InstallService>();
            services.AddSingleton<IUninstallService, UninstallService>();
            services.AddSingleton<IToolboxUpdateService, ToolboxUpdateService>();
            services.AddSingleton<IVersionService, VersionService>();
            services.AddSingleton<IAnnouncementService, AnnouncementService>();
            services.AddSingleton<ILoadingTextService, LoadingTextService>();
            services.AddSingleton<IGachaService, GachaService>();
            services.AddSingleton<IGreytestService, GreytestService>();
            services.AddSingleton<IFontService, FontService>();
            services.AddSingleton<ISkinService, SkinService>();
            services.AddSingleton<ISkinMusicService, SkinMusicService>();

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainWindow>();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            Log.logger.Error("出现了问题：\n", ex);
            UniversalDialog.ShowMessage($"运行中出现了未经处理的严重问题，且在这个错误发生后，工具箱将关闭。\n若要反馈，请带上链接或日志。\n反馈请勿！请勿截图此页面！\n错误分析原因：\n{ex.Message}", "错误", null, null);
            e.Handled = true;
            Current.Shutdown();
        }
    }
}
