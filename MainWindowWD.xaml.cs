// 此文件用来处理前端样式相关逻辑。
// 我恨XML，这辈子都不想写XML了。
// （而且内存占用好多

using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        public static string nowPage = "install";
        public static string nowInstallPage = "auto";
        public static bool isInstalling = false;
        public static bool eeOpening = false;
        public static bool eeEntered = false;
        public static int meow1 = 0;
        /// <summary>
        /// 刷新页面状态。
        /// </summary>
        public async Task RefreshPage()
        {
            logger.Info("刷新页面状态中。");
            // 隐藏按钮Hover
            CloseHover.Opacity = 0;
            MinimizeHover.Opacity = 0;
            // 隐藏面板按钮Hover
            AutoInstallHover.Opacity = 0;
            ManualInstallHover.Opacity = 0;
            ReplaceInstallHover.Opacity = 0;
            // 隐藏自动安装页面Hover
            AutoInstallBTHover.Opacity = 0;
            // 若在安装页面，则隐藏禁用标识
            // 反之，则显示
            if (nowPage == "install")
            {
                AutoInstallDisabled.Visibility = Visibility.Hidden;
                ManualInstallDisabled.Visibility = Visibility.Hidden;
                ReplaceInstallDisabled.Visibility = Visibility.Hidden;
                AutoInstallButton.Visibility = Visibility.Visible;
                ManualInstallButton.Visibility = Visibility.Visible;
                ReplaceInstallButton.Visibility = Visibility.Visible;
                AutoInstallButton.IsHitTestVisible = true;
                ManualInstallButton.IsHitTestVisible = true;
                ReplaceInstallButton.IsHitTestVisible = true;
            }
            else
            {
                AutoInstallDisabled.Visibility = Visibility.Visible;
                ManualInstallDisabled.Visibility = Visibility.Visible;
                ReplaceInstallDisabled.Visibility = Visibility.Visible;
                AutoInstallButton.Visibility = Visibility.Hidden;
                ManualInstallButton.Visibility = Visibility.Hidden;
                ReplaceInstallButton.Visibility = Visibility.Hidden;
                AutoInstallButton.IsHitTestVisible = false;
                ManualInstallButton.IsHitTestVisible = false;
                ReplaceInstallButton.IsHitTestVisible = false;
            }
            // 安装中相关控件
            if (isInstalling)
            {
                AutoInstallStartButton.Visibility = Visibility.Hidden;
                AutoInstallStartButton.IsHitTestVisible = false;
                AutoInstallBTIng.Visibility = Visibility.Visible;
            }
            else
            {
                AutoInstallStartButton.Visibility = Visibility.Visible;
                AutoInstallStartButton.IsHitTestVisible = true;
                AutoInstallBTIng.Visibility = Visibility.Hidden;
            }
            // 安装页面相关控件
            if (nowPage == "install")
            {
                if (nowInstallPage == "auto")
                {
                    await MakeGridStatuExceptSelf(AutoInstallPage);
                }
                else if (nowInstallPage == "manual")
                {
                    await MakeGridStatuExceptSelf(ManualInstallPage);
                }
                else
                {
                    await MakeGridStatuExceptSelf(ReplaceInstallPage);
                }
            }
            // 配置页面相关控件
            else if (nowPage == "config")
            {
                await MakeGridStatuExceptSelf(ConfigPage);
            }
            else if (nowPage == "greytest")
            {
                await MakeGridStatuExceptSelf(GreytestPage);
            }
            else if (nowPage == "settings")
            {
                await MakeGridStatuExceptSelf(SettingsPage);
            }
            else if (nowPage == "about")
            {
                await MakeGridStatuExceptSelf(AboutPage);
            }
            else
            {
                await MakeGridStatuExceptSelf(EEPage);
            }
            logger.Info("刷新页面状态完成。");
        }
        /// <summary>
        /// 使输入的Grid可见，隐藏其他Grid。
        /// </summary>
        /// <param name="g"></param>
        public async Task MakeGridStatuExceptSelf(Grid g)
        {
            List<Grid> gridList =
            [
                AutoInstallPage,
                ManualInstallPage,
                ReplaceInstallPage,
                ConfigPage,
                GreytestPage,
                SettingsPage,
                AboutPage,
                EEPage
            ];
            if (gridList.Contains(g))
            {
                gridList.Remove(g);
                MakeGridStatu(g, true);
            }
            else
            {
                return;
            }
            foreach (Grid grid in gridList)
            {
                MakeGridStatu(grid, false);
            }
            if (g != EEPage && eeOpening == false && eeEntered == true)
            {
                await ChangeEEVB(false);
                eeEntered = false;
            }
            if (g == EEPage)
            {
                eeOpening = false;
                eeEntered = true;
            }
            else
            {
                eeEntered = false;
            }
        }
        /// <summary>
        /// 切换Grid状态
        /// </summary>
        /// <param name="g">要操作的Grid</param>
        /// <param name="statu">状态（True为显示，False为隐藏）</param>
        private static void MakeGridStatu(Grid g, bool statu)
        {
            if (!statu)
            {
                g.Visibility = Visibility.Collapsed;
                g.IsEnabled = false;
            }
            else
            {
                g.Visibility = Visibility.Visible;
                g.IsEnabled = true;
            }
        }
        /// <summary>
        /// 处理窗口拖拽。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowDragMove(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        /// <summary>
        /// 拖拽时更改指针为拖拽样式。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.ScrollAll;
        }
        /// <summary>
        /// 拖拽结束恢复指针样式。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }
        /// <summary>
        /// 处理最小化按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// 处理关闭按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        /// <summary>
        /// 处理自动安装按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AutoInstallButtonClick(object sender, RoutedEventArgs e)
        {
            nowInstallPage = "auto";
            await RefreshPage();
        }
        private async void ManualInstallButtonClick(object sender, RoutedEventArgs e)
        {
            nowInstallPage = "manual";
            await RefreshPage();
        }
        private async void ReplaceInstallButtonClick(object sender, RoutedEventArgs e)
        {
            nowInstallPage = "replace";
            await RefreshPage();
        }
        /// <summary>
        /// 处理安装选项按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void InstallOptionClick(object sender, RoutedEventArgs e)
        {
            nowPage = "install";
            nowInstallPage = "auto";
            await RefreshPage();
        }
        /// <summary>
        /// 处理配置选项按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ConfigOptionClick(object sender, RoutedEventArgs e)
        {
            nowPage = "config";
            await RefreshPage();
        }
        private async void GreytestOptionClick(object sender, RoutedEventArgs e)
        {
            nowPage = "greytest";
            await RefreshPage();
        }
        private async void SettingsOptionClick(object sender, RoutedEventArgs e)
        {
            nowPage = "settings";
            await RefreshPage();
        }
        private async void AboutOptionClick(object sender, RoutedEventArgs e)
        {
            nowPage = "about";
            await RefreshPage();
        }
        private async void EEOptionClick(object sender, RoutedEventArgs e)
        {
            nowPage = "ee";
            await RefreshPage();
        }
        /// <summary>
        /// 处理自动安装页面的安装按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void InstallButtonClick(object sender, RoutedEventArgs e)
        {
            isInstalling = true;
            await RefreshPage();
            logger.Info("开始安装。");
            InstallPhase = 0;
            Process[] limbusProcess = Process.GetProcessesByName("LimbusCompany");
            if (limbusProcess.Length > 0)
            {
                logger.Warn("LimbusCompany仍然开启。");
                MessageBoxResult DialogResult = System.Windows.MessageBox.Show("检测到 Limbus Company 仍然处于开启状态！\n建议您关闭游戏后继续安装模组。\n若您已经关闭了 Limbus Company，请点击确定，否则请点击取消返回。", "警告", MessageBoxButton.OKCancel, MessageBoxImage.Hand);
                if (DialogResult == MessageBoxResult.Cancel)
                {
                    return;
                }
                logger.Warn("用户选择无视警告。");
            }
            try
            {
                StartProgressTimer();
                await InstallBepInEx();
                if (!File.Exists(Path.Combine(limbusCompanyDir, "winhttp.dll")))
                {
                    logger.Error("winhttp.dll不存在。");
                    System.Windows.MessageBox.Show("winhttp.dll不存在。\n请尝试关闭杀毒软件后再次安装。");
                    await StopInstall();
                    return;
                }
                await InstallTMP();
                if (!greytestStatus)
                {
                    await InstallMod();
                }
                else
                {
                    await InstallGreytestMod();
                }
            }
            catch (Exception ex)
            {
                ErrorReport(ex, true);
            }
            InstallPhase = 0;
            logger.Info("安装完成。");
            MessageBoxResult RunResult = System.Windows.MessageBox.Show("安装已完成！\n点击“确定”立刻运行边狱公司。\n点击“取消”关闭弹窗。\n加载时请耐心等待。", "完成", MessageBoxButton.OKCancel);
            if (RunResult == MessageBoxResult.OK)
            {
                try
                {
                    Process.Start("steam://rungameid/1973530");
                }
                catch (Exception ex)
                {
                    logger.Error("出现了问题： " + ex.ToString());
                    System.Windows.MessageBox.Show("出现了问题。\n" + ex.ToString());
                }
            }
            isInstalling = false;
            progressPercentage = 0;
            await ChangeProgressValue(0);
            await RefreshPage();
        }
        public async Task ChangeProgressValue(float value)
        {
            value = (float)Math.Round(value, 1);
            logger.Info("安装进度：" + value + "%");
            RectangleGeometry rectGeometry = new RectangleGeometry();
            rectGeometry.Rect = new Rect(0, 0, 6.24 * value, 50);
            await this.Dispatcher.BeginInvoke(() =>
            {
                FullProgress.Clip = rectGeometry;
            });
            logger.Info("更改进度完成。");
        }
        private void GreytestInfoButtonClick(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://www.zeroasso.top/docs/community/llcdev");
        }
        private async void WhiteBlackClickDouble(object sender, MouseButtonEventArgs e)
        {
            if (!eeOpening && !eeEntered)
            {
                logger.Info("不要点了>_<");
                eeOpening = true;
                eeEntered = false;
                await ChangeEEVB(true);
            }
        }
        public async Task ChangeEEVB(bool b)
        {
            if (b)
            {
                await this.Dispatcher.BeginInvoke(() =>
                {
                    EEOption.Visibility = Visibility.Visible;
                    EEOption.IsHitTestVisible = true;
                });
            }
            else
            {
                await this.Dispatcher.BeginInvoke(() =>
                {
                    EEOption.Visibility = Visibility.Collapsed;
                    EEOption.IsHitTestVisible = false;
                });
            }
        }
        public async Task ChangeEEPic(string url)
        {
            logger.Info("更改彩蛋图片为： " + url);
            await this.Dispatcher.BeginInvoke(() =>
            {
                EEPageImage.Source = BitmapFrame.Create(new Uri(url), BitmapCreateOptions.None, BitmapCacheOption.Default);
            });
        }
    }
}
