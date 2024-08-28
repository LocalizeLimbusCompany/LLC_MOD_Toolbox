// 此文件用来处理前端样式相关逻辑。
// 我恨XML，这辈子都不想写XML了。
// （而且内存占用好多

using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        public static string nowPage = "install";
        public static string nowInstallPage = "auto";
        public static bool isInstalling = false;
        /// <summary>
        /// 刷新页面状态。
        /// </summary>
        public void RefreshPage()
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
                    MakeGridStatuExceptSelf(AutoInstallPage);
                }
                else if (nowInstallPage == "manual")
                {
                    MakeGridStatuExceptSelf(ManualInstallPage);
                }
                else
                {
                    MakeGridStatuExceptSelf(ReplaceInstallPage);
                }
            }
            // 配置页面相关控件
            else if (nowPage == "config")
            {
                MakeGridStatuExceptSelf(ConfigPage);
            }
            else if (nowPage == "greytest")
            {
                MakeGridStatuExceptSelf(GreytestPage);
            }
            else if (nowPage == "settings")
            {
                MakeGridStatuExceptSelf(SettingsPage);
            }
            else
            {
                MakeGridStatuExceptSelf(AboutPage);
            }
            logger.Info("刷新页面状态完成。");
        }
        /// <summary>
        /// 使输入的Grid可见，隐藏其他Grid。
        /// </summary>
        /// <param name="g"></param>
        public void MakeGridStatuExceptSelf(Grid g)
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
            foreach(Grid grid in gridList)
            {
                MakeGridStatu(grid, false);
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
        private void AutoInstallButtonClick(object sender, RoutedEventArgs e)
        {
            nowInstallPage = "auto";
            RefreshPage();
        }
        private void ManualInstallButtonClick(object sender, RoutedEventArgs e)
        {
            nowInstallPage = "manual";
            RefreshPage();
        }
        private void ReplaceInstallButtonClick(object sender, RoutedEventArgs e)
        {
            nowInstallPage = "replace";
            RefreshPage();
        }
        /// <summary>
        /// 处理安装选项按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InstallOptionClick(object sender, RoutedEventArgs e)
        {
            nowPage = "install";
            nowInstallPage = "auto";
            RefreshPage();
        }
        /// <summary>
        /// 处理配置选项按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigOptionClick(object sender, RoutedEventArgs e)
        {
            nowPage = "config";
            RefreshPage();
        }
        private void GreytestOptionClick(object sender, RoutedEventArgs e)
        {
            nowPage = "greytest";
            RefreshPage();
        }
        private void SettingsOptionClick(object sender, RoutedEventArgs e)
        {
            nowPage = "settings";
            RefreshPage();
        }
        private void AboutOptionClick(object sender, RoutedEventArgs e)
        {
            nowPage = "about";
            RefreshPage();
        }
        /// <summary>
        /// 处理自动安装页面的安装按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void InstallButtonClick(object sender, RoutedEventArgs e)
        {
            isInstalling = true;
            RefreshPage();
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
                await InstallMod();
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
            RefreshPage();
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
    }
}
