// 此文件用来处理前端样式相关逻辑。
// 我恨XML，这辈子都不想写XML了。
// （而且内存占用好多

using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using LLC_MOD_Toolbox.ViewModels;

namespace LLC_MOD_Toolbox
{

    public partial class MainWindow : Window
    {
        private static bool eeOpening = false;
        private static bool eeEntered = false;

        public MainWindowViewModel ViewModel { get; }

        private bool IsEasterEggPageActive => ViewModel.CurrentPage == MainPage.EasterEgg;

        private bool IsAnnouncementPageActive => ViewModel.CurrentPage == MainPage.Announcement;

        public MainWindow()
        {
            ViewModel = new MainWindowViewModel();
            InitializeComponent();
            DataContext = ViewModel;
            progressTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.05)
            };
            progressTimer.Tick += ProgressTime_Tick;
            ViewModel.CloseRequested += () => Application.Current.Shutdown();
            ViewModel.MinimizeRequested += () => WindowState = WindowState.Minimized;
            ViewModel.LinkRequested += OpenUrl;
            ViewModel.GachaRequested += HandleGachaNavigationAsync;
            ViewModel.AutoInstallRequested += StartAutoInstallAsync;
            ViewModel.GachaRollRequested += StartGachaRollAsync;
            ViewModel.AnnouncementAcknowledged += AlreadyReadAnno;
            ViewModel.UninstallRequested += HandleUninstallAsync;
            ViewModel.LauncherShortcutRequested += HandleLauncherShortcutRequested;
            ViewModel.LauncherHelpRequested += HandleLauncherHelpRequested;
            ViewModel.NodeHelpRequested += HandleNodeHelpRequested;
            ViewModel.HotUpdateHelpRequested += HandleHotUpdateHelpRequested;
            ViewModel.MirrorChyanConfigRequested += HandleMirrorChyanConfigRequested;
            ViewModel.NodeSelectionChanged += HandleNodeSelectionChanged;
            ViewModel.ApiSelectionChanged += HandleApiSelectionChanged;
            ViewModel.SkinSelectionChanged += HandleSkinSelectionChanged;
            ViewModel.ExploreFontRequested += HandleExploreFontRequested;
            ViewModel.PreviewFontRequested += HandlePreviewFontAsync;
            ViewModel.ApplyFontRequested += HandleApplyFontRequested;
            ViewModel.RestoreFontRequested += HandleRestoreFontRequested;
            ViewModel.GreytestStartRequested += HandleGreytestStartAsync;
            ViewModel.GreytestInfoRequested += HandleGreytestInfoRequested;
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        /// <summary>
        /// 刷新页面状态。
        /// </summary>
        public async Task RefreshPage()
        {
            Log.logger.Info("刷新页面状态中。");
            // 隐藏按钮Hover
            CloseHover.Opacity = 0;
            MinimizeHover.Opacity = 0;
            // 隐藏自动安装页面Hover
            AutoInstallPage.ResetHoverOpacity();
            await UpdateEasterEggStateAsync();
            Log.logger.Info("刷新页面状态完成。");
        }
        /// <summary>
        /// 使输入的Grid可见，隐藏其他Grid。
        /// </summary>
        /// <param name="g"></param>
        public async Task MakeGridStatuExceptSelf(FrameworkElement g)
        {
            if (g == AutoInstallPage)
            {
                ViewModel.SelectInstallSubPage(InstallSubPage.Auto);
            }
            else if (g == FontReplacePage)
            {
                ViewModel.SelectInstallSubPage(InstallSubPage.Font);
            }
            else if (g == GachaPage)
            {
                ViewModel.SelectInstallSubPage(InstallSubPage.Gacha);
            }
            else if (g == SkinPage)
            {
                ViewModel.SelectInstallSubPage(InstallSubPage.Skin);
            }
            else if (g == LinkPage)
            {
                ViewModel.SelectMainPage(MainPage.Link);
            }
            else if (g == GreytestPage)
            {
                ViewModel.SelectMainPage(MainPage.Greytest);
            }
            else if (g == SettingsPage)
            {
                ViewModel.SelectMainPage(MainPage.Settings);
            }
            else if (g == AboutPage)
            {
                ViewModel.SelectMainPage(MainPage.About);
            }
            else if (g == AnnouncementPage)
            {
                ViewModel.SelectMainPage(MainPage.Announcement);
            }
            else if (g == EEPage)
            {
                ViewModel.SelectMainPage(MainPage.EasterEgg);
            }

            await UpdateEasterEggStateAsync();
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
        private async Task HandleGachaNavigationAsync()
        {
            if (!isInitGacha)
            {
                bool messageBoxResult = UniversalDialog.ShowConfirm("本抽卡模拟器资源来源自维基，可能信息更新不准时。\n本模拟器 不 会 对您的游戏数据造成任何影响。\n若您已知悉，请点击“确定”进行初始化。", "提示", this);
                if (messageBoxResult)
                {
                    ViewModel.SelectInstallSubPage(InstallSubPage.Gacha);
                    await InitGacha();
                    await RefreshPage();
                }
            }
            else
            {
                ViewModel.SelectInstallSubPage(InstallSubPage.Gacha);
                await RefreshPage();
            }
        }

        public async Task ChangeProgressValue(float value)
        {
            value = (float)Math.Round(value, 1);
            Log.logger.Debug("安装进度：" + value + "%");
            RectangleGeometry rectGeometry = new()
            {
                Rect = new Rect(0, 0, 6.24 * value, 50)
            };
            await this.Dispatcher.BeginInvoke(() =>
            {
                AutoInstallPage.ProgressClip = rectGeometry;
            });
            Log.logger.Debug("更改进度完成。");
        }
        #region 彩蛋
        private async void WhiteBlackClickDouble(object sender, MouseButtonEventArgs e)
        {
            if (!eeOpening && !eeEntered && !isInAnno && !IsAnnouncementPageActive)
            {
                Log.logger.Info("不要点了>_<");
                eeOpening = true;
                eeEntered = false;
                await ChangeEEVB(true);
            }
        }
        public async Task ChangeEEVB(bool b)
        {
            await Dispatcher.BeginInvoke(() => ViewModel.IsEasterEggUnlocked = b);
        }
        public async Task ChangeEEPic()
        {
            string url = "https://api.zeroasso.top/v2/eepic/get_image";
            if (configuation.Settings.general.internationalMode)
            {
                url = "https://cdn-api.zeroasso.top/v2/eepic/get_image";
            }
            try
            {
                using (var client = new HttpClient())
                {
                    var bytes = await client.GetByteArrayAsync(url);
                    using (var stream = new MemoryStream(bytes))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                        EEPage.PageImageSource = bitmap;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.logger.Error("更改彩蛋图片失败：" + ex.Message);
            }
        }
        #endregion
        public async Task ChangeAutoInstallButton()
        {
            Log.logger.Debug("更改自动安装模组。");
            await this.Dispatcher.BeginInvoke(() =>
            {
                AutoInstallPage.ButtonImageSource = BitmapFrame.Create(new Uri("pack://siteoforigin:,,,/Picture/Update.png"), BitmapCreateOptions.None, BitmapCacheOption.Default);
                AutoInstallPage.ButtonHoverImageSource = BitmapFrame.Create(new Uri("pack://siteoforigin:,,,/Picture/UpdateHover.png"), BitmapCreateOptions.None, BitmapCacheOption.Default);
            });
        }
        public async Task DisableGlobalOperations()
        {
            await Dispatcher.BeginInvoke(() => ViewModel.IsGlobalOperationsEnabled = false);
        }

        public async Task EnableGlobalOperations()
        {
            await Dispatcher.BeginInvoke(() => ViewModel.IsGlobalOperationsEnabled = true);
        }
        public async Task ChangeLeftButtonStatu(bool statu)
        {
            await Dispatcher.BeginInvoke(() => ViewModel.ArePrimaryActionsEnabled = statu);
        }
        public async Task ChangeAnnoTip(int num)
        {
            await Dispatcher.BeginInvoke(() => ViewModel.AnnouncementTip = "由于本次公告较为重要，您需要继续阅读" + num + "秒。");
        }
        public async Task ChangeAnnoText(string text)
        {
            await Dispatcher.BeginInvoke(() => ViewModel.AnnouncementText = text);
        }
        public async Task AnnoCountEnd()
        {
            await Dispatcher.BeginInvoke(() =>
            {
                ViewModel.IsAnnouncementButtonEnabled = true;
                ViewModel.ShowAnnouncementTip = false;
            });
        }
        public async Task AlreadyReadAnno()
        {
            ViewModel.SelectMainPage(MainPage.Install, InstallSubPage.Auto);
            await ChangeLeftButtonStatu(true);
            await RefreshPage();
            if (configuation.Settings.install.installWhenLaunch || isLauncherMode)
            {
                await StartAutoInstallAsync();
            }
        }
        public async Task ChangeLoadingText(string text)
        {
            await Dispatcher.BeginInvoke(() => ViewModel.LoadingText = text);
        }

        private Task StartAutoInstallAsync()
        {
            InstallButtonClick();
            return Task.CompletedTask;
        }

        private Task StartGachaRollAsync()
        {
            InGachaButtonClick();
            return Task.CompletedTask;
        }

        private async Task UpdateEasterEggStateAsync()
        {
            if (!IsEasterEggPageActive && !eeOpening && eeEntered)
            {
                await ChangeEEVB(false);
                eeEntered = false;
            }

            if (IsEasterEggPageActive)
            {
                eeOpening = false;
                eeEntered = true;
            }
            else
            {
                eeEntered = false;
            }
        }

        private async void ViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.CurrentPage))
            {
                await UpdateEasterEggStateAsync();
            }
        }
    }
}
