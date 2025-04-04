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
        //本次运行出现的问题次数
        private static int error_count = 0;
        //程序出错的次数上限
        private static int max_error_count = 5;
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            logger.Error("出现了问题：\n", ex);
            string errorMessage = ReturnExceptionText(ex);
            System.Windows.MessageBox.Show($"出现了未经处理的问题\n也许工具箱能够正常运行，不过建议检查日志并寻求帮助\n错误原因：{errorMessage}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            if (error_count <= max_error_count)
            {
                error_count++;
                e.Handled = true;
            }
            else
            {
                System.Windows.MessageBox.Show("多次出现错误，即将关闭程序", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = false;
            }
        }
    }
}
