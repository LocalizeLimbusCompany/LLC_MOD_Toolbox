using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using LLC_MOD_Toolbox.Helpers;
using LLC_MOD_Toolbox.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        private readonly ILogger<MainWindow> logger;

        public MainWindow(
            ILogger<MainWindow> logger,
            MainViewModel mainViewModel,
            AutoInstallerViewModel autoInstallerViewModel,
            SettingsViewModel settingsViewModel,
            GachaViewModel gachaViewModel,
            LinkViewModel linkViewModel
        )
        {
            InitializeComponent();
            this.logger = logger;
            DataContext = mainViewModel;
            AutoInstallerPage.DataContext = autoInstallerViewModel;
            SettingsPage.DataContext = settingsViewModel;
            GachaPage.DataContext = gachaViewModel;
            LinkPage.DataContext = linkViewModel;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await ChangeEEPic("https://dl.kr.zeroasso.top/ee_pic/public/public.png");

            logger.LogInformation("加载流程完成。");
        }

        private void ManualInstallPage_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var file = files.FirstOrDefault(f =>
                    f.EndsWith(".7z", StringComparison.OrdinalIgnoreCase)
                    || f.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
                );
                if (!string.IsNullOrEmpty(file))
                {
                    logger.LogInformation("拖拽文件路径: {filePath}", file);
                    var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    var limbusCompanyPath =
                        PathHelper.DetectedLimbusCompanyPath ?? PathHelper.SelectPath();
                    try
                    {
                        FileHelper.ExtractLanguagePackage(fileStream, limbusCompanyPath);
                        logger.LogInformation(
                            "成功安装语言包到边狱公司目录：{limbusCompanyPath}",
                            limbusCompanyPath
                        );
                    }
                    finally
                    {
                        fileStream.Dispose();
                    }
                }
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

        //private void GreytestInfoButtonClick(object sender, RoutedEventArgs e)
        //{
        //    HttpHelper.LaunchUrl("https://www.zeroasso.top/docs/community/llcdev");
        //}

        #region 彩蛋
        /*        private async void WhiteBlackClickDouble(object sender, MouseButtonEventArgs e)
                {
                    if (!eeOpening && !eeEntered)
                    {
                        _logger.Info("不要点了>_<");
                        eeOpening = true;
                        eeEntered = false;
                        await ChangeEEVB(true);
                    }
                }*/
        public async Task ChangeEEPic(string url)
        {
            logger.LogDebug("更改彩蛋图片为： {url}", url);
            await this.Dispatcher.BeginInvoke(method: () =>
            {
                EEPageImage.Source = BitmapFrame.Create(
                    new Uri(url),
                    BitmapCreateOptions.None,
                    BitmapCacheOption.Default
                );
            });
        }
        #endregion

#if false
        private async void StartGreytestButtonClick(object sender, RoutedEventArgs e)
        {
            logger.LogInformation("Z-TECH 灰度测试客户端程序 v2.0 启动。（并不是");
            if (!greytestStatus)
            {
                string token = await GetGreytestBoxText();
                if (token == string.Empty || token == "请输入秘钥")
                {
                    logger.LogInformation("Token为空。");
                    System.Windows.MessageBox.Show(
                        "请输入有效的Token。",
                        "提示",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    return;
                }
                logger.LogInformation("Token为：" + token);
                string tokenUrl = $"https://dev.zeroasso.top/api/{token}.json";
                using (HttpClient client = new())
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(tokenUrl);
                        if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                        {
                            logger.LogInformation("秘钥有效。");
                        }
                        else
                        {
                            logger.LogInformation("秘钥无效。");
                            System.Windows.MessageBox.Show(
                                "请输入有效的Token。",
                                "提示",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error
                            );
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorReport(ex, false);
                        return;
                    }
                }
                try
                {
                    string tokenJson = await GetURLText(tokenUrl);
                    var tokenObject = JObject.Parse(tokenJson);
                    string runStatus = tokenObject["status"].Value<string>();
                    if (runStatus == "test")
                    {
                        logger.LogInformation("Token状态正常。");
                    }
                    else
                    {
                        logger.LogInformation("Token已停止测试。");
                        System.Windows.MessageBox.Show(
                            "Token已停止测试。",
                            "提示",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                        return;
                    }
                    string fileName = tokenObject["file_name"].Value<string>();
                    string note = tokenObject["note"].Value<string>();
                    logger.LogInformation($"Token信息：{token}\n混淆文件名：{fileName}\n备注：{note}");
                    await ChangeLogoToTest();
                    System.Windows.MessageBox.Show(
                        $"目前Token有效。\n-------------\nToken信息：\n秘钥：{token}\n混淆文件名：{fileName}\n备注：{note}\n-------------\n灰度测试模式已开启。\n请在自动安装安装此秘钥对应版本汉化。\n秘钥信息请勿外传。",
                        "提示",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                    greytestStatus = true;
                    greytestUrl =
                        "https://dev.zeroasso.top/files/LimbusLocalize_Dev_" + fileName + ".7z";
                }
                catch (Exception ex)
                {
                    ErrorReport(ex, false);
                    return;
                }
            }
            else
            {
                System.Windows.MessageBox.Show(
                    "灰度测试模式已开启。\n请在自动安装安装此秘钥对应版本汉化。\n若需要正常使用或更换秘钥，请重启工具箱。",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }
        }
#endif
    }
}
