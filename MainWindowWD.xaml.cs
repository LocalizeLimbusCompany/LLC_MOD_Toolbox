// 此文件用来处理前端样式相关逻辑。
// 我恨XML，这辈子都不想写XML了。
// （而且内存占用好多

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        public static string nowPage = "install";
        public static string nowInstallPage = "auto";
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
            this.Cursor = Cursors.ScrollAll;
        }
        /// <summary>
        /// 拖拽结束恢复指针样式。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
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
            Application.Current.Shutdown();
        }
        /// <summary>
        /// 处理自动安装按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoInstallButton_Click(object sender, RoutedEventArgs e)
        {
            nowInstallPage = "auto";
            RefreshPage();
        }
        /// <summary>
        /// 处理安装选项按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InstallOption_Click(object sender, RoutedEventArgs e)
        {
            nowPage = "install";
            nowInstallPage = "auto";
            RefreshPage();
        }
    }
}
