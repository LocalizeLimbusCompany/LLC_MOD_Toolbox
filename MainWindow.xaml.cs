// 用于处理后端逻辑。
/*
 * You may think you know what the following code does.
 * But you dont. Trust me.
 * Fiddle with it, and youll spend many a sleepless
 * night cursing the moment you thought youd be clever
 * enough to "optimize" the code below.
 * Now close this file and go play with something else.
 * 你可能会认为你读得懂以下的代码。但是你不会懂的，相信我吧。
 * 要是你尝试玩弄这段代码的话，你将会在无尽的通宵中不断地咒骂自己为什么会认为自己聪明到可以优化这段代码。
 * 现在请关闭这个文件去玩点别的吧。
*/
using log4net;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        public static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        public const string VERSION = "0.7.0";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            logger.Info("——————————");
            logger.Info("工具箱已进入加载流程。");
            logger.Info("We have a lift off.");
            logger.Info("WPF架构工具箱 版本：" + VERSION + " 。");
            RefreshPage();
        }
    }
}