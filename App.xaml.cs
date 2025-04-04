using System.Configuration;
using System.Data;
using System.Windows;
using static LLC_MOD_Toolbox.MainWindow;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "App.config", ConfigFileExtension = "config", Watch = true)]
namespace LLC_MOD_Toolbox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.DispatcherUnhandledException += App_DispatcherUnhandledException; ;
        }
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            logger.Error("出现了问题：\n", ex);
            MessageBox.Show($"运行中出现了未经处理的严重问题，且在这个错误发生后，工具箱将关闭。\n若要反馈，请带上链接或日志。\n反馈请勿！请勿截图此页面！\n错误分析原因：\n{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            Application.Current.Shutdown();
        }
    }
}
