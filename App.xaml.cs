using System.Windows;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "App.config", ConfigFileExtension = "config", Watch = true)]
namespace LLC_MOD_Toolbox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private static Mutex _mutex = null;
        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;

            _mutex = new Mutex(true, "LLC_MOD_TOOLBOX", out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("已有工具箱在运行中！", "提示");
                Current.Shutdown();
                return;
            }
            base.OnStartup(e);
            this.DispatcherUnhandledException += App_DispatcherUnhandledException; ;
        }
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            Log.logger.Error("出现了问题：\n", ex);
            MessageBox.Show($"运行中出现了未经处理的严重问题，且在这个错误发生后，工具箱将关闭。\n若要反馈，请带上链接或日志。\n反馈请勿！请勿截图此页面！\n错误分析原因：\n{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            Application.Current.Shutdown();
        }
    }
}
