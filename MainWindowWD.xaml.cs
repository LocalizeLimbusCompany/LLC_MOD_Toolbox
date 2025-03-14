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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Downloader;
using LLC_MOD_Toolbox.Helpers;
using LLC_MOD_Toolbox.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SevenZip;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        private static string? useEndPoint = null;
        private static string? useAPIEndPoint = null;
        private static bool useGithub = false;
        private static bool useMirrorGithub = false;

        private readonly DispatcherTimer progressTimer;

        // GreyTest 灰度测试2.0
        private static string greytestUrl = string.Empty;
        private static bool greytestStatus = false;

        #region 常用方法
        public static void Unarchive(string archivePath, string output)
        {
            using SevenZipExtractor extractor = new(archivePath);
            extractor.ExtractArchive(output);
        }

        /// <summary>
        /// 处理使用Downloader下载文件的事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewOnDownloadProgressChanged(
            object? sender,
            DownloadProgressChangedEventArgs e
        )
        {
            logger.LogDebug(
                "ProgressPercentage: {ProgressPercentage} ProgressPercentage(Int): {ProgressPercentageInt}",
                e.ProgressPercentage,
                (int)(e.ProgressPercentage)
            );
        }

        public void ErrorReport(Exception ex, bool CloseWindow, string advice = "")
        {
            logger.LogError(ex, "出现了问题：\n{Advice}", advice);
            System.Windows.MessageBox.Show(
                $"运行中出现了问题。\n{advice}若要反馈，请带上链接或日志。\n——————————\n{ex}",
                "错误",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            if (CloseWindow)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        #endregion
        #region 进度条系统
        public void ProgressTime_Tick(object? sender, EventArgs e)
        {
            //await ChangeProgressValue(progressPercentage);
        }

        public void StartProgressTimer()
        {
            progressTimer.Start();
        }

        public void StopProgressTimer()
        {
            progressTimer.Stop();
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
        private async Task<string> GetGreytestBoxText()
        {
            string? text = string.Empty;
            await this.Dispatcher.BeginInvoke(() =>
            {
                text = GreytestTokenBox.Text;
            });
            return text;
        }

        private async Task ChangeLogoToTest()
        {
            await this.Dispatcher.BeginInvoke(() =>
            {
                ZALogo.Visibility = Visibility.Visible;
            });
        }

        #region 开关模组
        // 我也不知道为什么这个功能这么多人想要，不过既然那么多人要，那我就写了
        private void ChangeStatuButtonClick(object sender, RoutedEventArgs e)
        {
            logger.LogInformation("点击开关模组按钮。");
            if (
                !File.Exists(limbusCompanyDir + "/winhttp.dll")
                && !File.Exists(limbusCompanyDir + "/winhttp.dll.disabled")
            )
            {
                logger.LogInformation("模组未安装。");
                System.Windows.MessageBox.Show(
                    "模组未安装。",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }
            if (File.Exists(limbusCompanyDir + "/winhttp.dll.disabled"))
            {
                logger.LogInformation("检测到 winhttp.dll.disabled，进行开启。");
                File.Move(
                    limbusCompanyDir + "/winhttp.dll.disabled",
                    limbusCompanyDir + "/winhttp.dll"
                );
                System.Windows.MessageBox.Show(
                    "模组已开启。",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            else if (File.Exists(limbusCompanyDir + "/winhttp.dll"))
            {
                logger.LogInformation("检测到 winhttp.dll，进行关闭。");
                File.Move(
                    limbusCompanyDir + "/winhttp.dll",
                    limbusCompanyDir + "/winhttp.dll.disabled"
                );
                System.Windows.MessageBox.Show(
                    "模组已关闭。",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
        #endregion

        private string limbusCompanyDir;
    }
}
