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
        private static string nowPage = "install";
        private static string nowInstallPage = "auto";
        private static bool isInstalling = false;
        private static bool eeOpening = false;
        private static bool eeEntered = false;
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
                GachaSimDisabled.Visibility = Visibility.Hidden;
                AutoInstallButton.Visibility = Visibility.Visible;
                ManualInstallButton.Visibility = Visibility.Visible;
                ReplaceInstallButton.Visibility = Visibility.Visible;
                GachaSimInstallButton.Visibility = Visibility.Visible;
                AutoInstallButton.IsHitTestVisible = true;
                ManualInstallButton.IsHitTestVisible = true;
                ReplaceInstallButton.IsHitTestVisible = true;
            }
            else
            {
                AutoInstallDisabled.Visibility = Visibility.Visible;
                ManualInstallDisabled.Visibility = Visibility.Visible;
                ReplaceInstallDisabled.Visibility = Visibility.Visible;
                GachaSimDisabled.Visibility = Visibility.Visible;
                AutoInstallButton.Visibility = Visibility.Hidden;
                ManualInstallButton.Visibility = Visibility.Hidden;
                ReplaceInstallButton.Visibility = Visibility.Hidden;
                GachaSimInstallButton.Visibility = Visibility.Hidden;
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
                switch (nowInstallPage)
                {
                    case "auto":
                        await MakeGridStatuExceptSelf(AutoInstallPage);
                        break;
                    case "manual":
                        await MakeGridStatuExceptSelf(ManualInstallPage);
                        break;
                    case "replace":
                        await MakeGridStatuExceptSelf(ReplaceInstallPage);
                        break;
                    case "gacha":
                        await MakeGridStatuExceptSelf(GachaPage);
                        break;
                }
            }
            // 配置页面相关控件
            else if (nowPage == "link")
            {
                await MakeGridStatuExceptSelf(LinkPage);
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
                LinkPage,
                GreytestPage,
                SettingsPage,
                AboutPage,
                EEPage,
                GachaPage
            ];
            gridList.Remove(g);
            MakeGridStatu(g, true);
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
        private async void GachaSimButtonClick(object sender, RoutedEventArgs e)
        {
            if (!isInitGacha)
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("本抽卡模拟器资源来源自维基，可能信息更新不准时。\n本模拟器 不 会 对您的游戏数据造成任何影响。\n若您已知悉，请点击“确定”进行初始化。", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                if (messageBoxResult == MessageBoxResult.OK)
                {
                    nowInstallPage = "gacha";
                    await InitGacha();
                    await RefreshPage();
                }
            }
            else
            {
                nowInstallPage = "gacha";
                await RefreshPage();
            }
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
        private async void LinkOptionClick(object sender, RoutedEventArgs e)
        {
            nowPage = "link";
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
        public async Task ChangeProgressValue(float value)
        {
            value = (float)Math.Round(value, 1);
            logger.Debug("安装进度：" + value + "%");
            RectangleGeometry rectGeometry = new()
            {
                Rect = new Rect(0, 0, 6.24 * value, 50)
            };
            await this.Dispatcher.BeginInvoke(() =>
            {
                FullProgress.Clip = rectGeometry;
            });
            logger.Debug("更改进度完成。");
        }
        private void GreytestInfoButtonClick(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://www.zeroasso.top/docs/community/llcdev");
        }
        #region 彩蛋
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
            logger.Debug("更改彩蛋图片为： " + url);
            await this.Dispatcher.BeginInvoke(() =>
            {
                EEPageImage.Source = BitmapFrame.Create(new Uri(url), BitmapCreateOptions.None, BitmapCacheOption.Default);
            });
        }
        #endregion
        #region 链接
        public Dictionary<string, string> linkDictionary = [];
        private void InitLink()
        {
            linkDictionary.Add("LinkButton1", "https://www.zeroasso.top");
            linkDictionary.Add("LinkButton2", "https://space.bilibili.com/1247764479");
            linkDictionary.Add("LinkButton3", "https://github.com/LocalizeLimbusCompany");
            linkDictionary.Add("LinkButton4", "https://afdian.com/a/Limbus_zero");
            linkDictionary.Add("LinkButton5", "https://paratranz.cn/projects/6860/leaderboard");
            linkDictionary.Add("LinkButton6", "https://paratranz.cn");
            linkDictionary.Add("LinkButton7", "https://weidian.com/?userid=1655827241");
            linkDictionary.Add("LinkButton8", "https://limbuscompany.huijiwiki.com");
        }
        private async Task<string?> GetSenderName(System.Windows.Controls.Control? control)
        {
            if (control != null)
            {
                string name = string.Empty;
                await this.Dispatcher.BeginInvoke(() =>
                {
                    name = control.Name;
                });
                return name;
            }
            else
            {
                return string.Empty;
            }
        }
        private async void LinkButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                string name = await GetSenderName(sender as System.Windows.Controls.Control);
                if (!string.IsNullOrEmpty(name) && linkDictionary.TryGetValue(name, out string url))
                {

                    OpenUrl(url);
                }
            }
        }
        #endregion
    }
}
