using System.Windows;
using LLC_MOD_Toolbox.Models;
using LLC_MOD_Toolbox.ViewModels;
using log4net;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.DependencyInjection;
using SevenZip;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "App.config", ConfigFileExtension = "config", Watch = true)]


namespace LLC_MOD_Toolbox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(App));
        private async void Application_Startup(object sender, StartupEventArgs e)
        {

            logger.Info("—————新日志分割线—————");
            logger.Info("工具箱已进入加载流程。");
            logger.Info("We have a lift off.");
            SevenZipBase.SetLibraryPath("7z.dll");
            logger.Info("加载流程完成。");
            try
            {
                AutoInstallerViewModel.PrimaryNodeList = await PrimaryNodeList.CreateAsync("NodeList.json");
                logger.Info("节点初始化完成。");
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("节点初始化失败，原因为：{0}", ex);
            }
            MainWindow mainWindow = new();
            mainWindow.Show();
        }
    }
}
