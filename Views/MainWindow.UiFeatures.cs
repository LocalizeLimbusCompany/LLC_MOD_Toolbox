using LLC_MOD_Toolbox.Models;
using Newtonsoft.Json.Linq;
using SevenZip;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        #region 错误处理
        public void ErrorReport(Exception ex, bool CloseWindow, string advice = "")
        {
            Log.logger.Error("出现了问题：\n", ex);
            string errorMessage = ReturnExceptionText(ex);
            if (CloseWindow)
            {
                UniversalDialog.ShowMessage($"运行中出现了问题，且在这个错误发生后，工具箱将关闭。\n{advice}若要反馈，请带上链接或日志。\n反馈请勿！请勿截图此页面！\n错误分析原因：\n{errorMessage}", "错误", null, this);
            }
            else
            {
                UniversalDialog.ShowMessage($"运行中出现了问题。但你仍然能够使用工具箱（大概）。\n{advice}若要反馈，请带上链接或日志。\n反馈请勿！请勿截图此页面！\n——————————\n错误分析原因：\n{errorMessage}", "错误", null, this);
            }
            if (CloseWindow)
            {
                Application.Current.Shutdown();
            }
        }

        public void ErrorReportMirrorChyan(MirrorChyanException ex, bool CloseWindow)
        {
            Log.logger.Error("访问 Mirror 酱服务中出现了错误\n", ex);
            if (CloseWindow)
            {
                UniversalDialog.ShowMessage($"访问 Mirror 酱服务出现了问题，且在这个错误发生后，工具箱将关闭。\n出现该问题原因：{ex.Message}\n若要反馈，请带上链接或日志。\n反馈请勿！请勿截图此页面！", "错误", null, this);
            }
            else
            {
                UniversalDialog.ShowMessage($"访问 Mirror 酱服务出现了问题。但你仍然能够使用工具箱（大概）。\n出现该问题原因：{ex.Message}\n若要反馈，请带上链接或日志。\n反馈请勿！请勿截图此页面！", "错误", null, this);
            }
            if (CloseWindow)
            {
                Application.Current.Shutdown();
            }
        }

        public static string ReturnExceptionText(Exception ex)
        {
            if (ex is (System.Net.WebException) || (ex is HttpRequestException) || (ex is HttpProtocolException) || (ex is System.Net.Sockets.SocketException) || (ex is System.Net.HttpListenerException) || (ex is HttpIOException))
            {
                return "网络链接错误，请尝试更换节点，关闭加速器或代理后再试。\n您也可以尝试在官网“常见问题”进行排查。";
            }
            else if (ex is SevenZipException)
            {
                return "解压出现问题，大概率为网络问题。\n请尝试更换节点，关闭加速器或代理后再试。\n您也可以尝试在官网“常见问题”进行排查。";
            }
            else if (ex is FileNotFoundException)
            {
                return "无法找到文件，可能是网络问题，也可能是边狱公司路径出现错误。\n请尝试更换节点，关闭加速器或代理后再试。\n您也可以尝试在官网“常见问题”进行排查。";
            }
            else if (ex is UnauthorizedAccessException)
            {
                return "无权限访问文件，请尝试以管理员身份启动，也可能是你打开了边狱公司？";
            }
            else if (ex is IOException)
            {
                return "文件访问出现问题。\n可能是文件已被边狱公司占用？\n您可以尝试关闭边狱公司。";
            }
            else if (ex is HashException)
            {
                return "文件损坏。\n大概率为网络问题，请尝试更换节点，关闭加速器或代理后再试。\n您也可以尝试在官网“常见问题”进行排查。";
            }
            return "未知错误原因，错误已记录至日志，请查看官网“常见问题”进行排查。\n如果没有解决，请尝试进行反馈。";
        }
        #endregion

        #region 公告系统
        private DispatcherTimer _AnnoTimer;
        private int annoLastTime = 0;
        private bool isInAnno = false;
        private bool hasNewAnno = false;

        private async Task CheckAnno()
        {
            if (!configuation.Settings.announcement.getAnno)
            {
                return;
            }
            try
            {
                string annoText = await GetURLText(string.Format(useAPIEndPoint, "v2/announcement/get_anno"));
                if (string.IsNullOrEmpty(annoText))
                {
                    return;
                }
                var annoObject = JObject.Parse(annoText);
                if (annoObject["version"]!.Value<int>() <= configuation.Settings.announcement.annoVersion)
                {
                    Log.logger.Info("无新公告。");
                    return;
                }
                else
                {
                    Log.logger.Info("有新公告。");
                }
                string annoContent = annoObject["anno"]!.Value<string>();
                annoContent = annoContent.Replace("\\n", "\n");
                string annoLevel = annoObject["level"]!.Value<string>();
                int annoVersionNew = annoObject["version"]!.Value<int>();
                await ChangeLeftButtonStatu(false);
                await ChangeAnnoText(annoContent);
                configuation.Settings.announcement.annoVersion = annoVersionNew;
                configuation.SaveConfig();
                isInAnno = true;
                hasNewAnno = true;
                ViewModel.IsAnnouncementButtonEnabled = false;
                ViewModel.ShowAnnouncementTip = true;
                await MakeGridStatuExceptSelf(AnnouncementPage);
                if (annoLevel == "normal")
                {
                    await AnnoCountEnd();
                    return;
                }
                else if (annoLevel == "important")
                {
                    annoLastTime = 5;
                }
                else if (annoLevel == "special")
                {
                    annoLastTime = 15;
                }
                ViewModel.AnnouncementTip = "由于本次公告较为重要，您需要继续阅读" + annoLastTime + "秒。";
                _AnnoTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                _AnnoTimer.Tick += AnnoTimer_Tick;
                _AnnoTimer.Start();
            }
            catch (Exception ex)
            {
                Log.logger.Error("检查公告失败。", ex);
            }
        }

        private async void AnnoTimer_Tick(object? sender, EventArgs e)
        {
            if (annoLastTime > 0)
            {
                annoLastTime -= 1;
                await ChangeAnnoTip(annoLastTime);
            }
            else
            {
                isInAnno = false;
                await AnnoCountEnd();
                _AnnoTimer.Stop();
            }
        }
        #endregion

        #region 启动器发生器
        private void HandleLauncherShortcutRequested()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string shortcutPath = Path.Combine(desktopPath, "LimbusCompany with LLC.lnk");
            IWshRuntimeLibrary.WshShell wsh = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)wsh.CreateShortcut(shortcutPath);
            shortcut.TargetPath = Path.Combine(currentDir, "LLC_MOD_Toolbox.exe");
            shortcut.Arguments = "-launcher";
            shortcut.WorkingDirectory = currentDir;
            shortcut.Description = "启动边狱公司并检查汉化更新";
            shortcut.IconLocation = Path.Combine(currentDir, "PublicResource", "favicon.ico");
            shortcut.Save();
            Log.logger.Info($"快捷方式已创建: {shortcutPath}");
            UniversalDialog.ShowMessage("快捷方式已创建。\n可在桌面上找到“LimbusCompany with LLC”启动。", "提示", null, this);
        }

        private void HandleLauncherHelpRequested()
        {
            OpenUrl("https://www.zeroasso.top/docs/install/hotupdate");
        }

        internal void HandleHotUpdateHelpRequested()
        {
            UniversalDialog.ShowMessage("你想要知道怎么用热更新？那你可找对地方了兄弟！" +
                "\n你现在有两种方式，随你便，你想用哪个就用哪个：" +
                "\n1. 从快捷方式启动" +
                "\n - 点击旁边的发送按钮" +
                "\n - 后续在桌面点击 LimbusCompany with LLC即可在启动游戏前检查是否更新汉化并自动安装" +
                "\n2. 从Steam启动" +
                "\n - 打开安装器的所在文件夹，选中LLC_MOD_Toolbox.exe，并复制其地址" +
                "\n * 怎么复制地址：右键LLC_MOD_Toolbox-复制文件地址" +
                "\n - 打开您的Steam库页面，在最左下角唤起“添加非Steam游戏”菜单" +
                "\n - 在该菜单中选择并打开LLC_MOD_Toolbox.exe，直接在文件名那里粘贴你刚刚复制的地址然后回车就可以了" +
                "\n - LLC_MOD_Toolbox将会出现在选单内，确认其选中状态并确认添加" +
                "\n - 在您的Steam库中找到LLC_MOD_Toolbox，在启动选项内填入-launcher(全小写)" +
                "\n这两种方法效果相同，根据自己喜好选择。", "热更新教程", null, this);
        }
        #endregion

        #region Loading文本
        private JArray CachedLoadingTexts;

        private async Task CheckLoadingText()
        {
            JObject loadingObject = JObject.Parse(await File.ReadAllTextAsync(Path.Combine(currentDir, "loadingText.json")));
            if (DateTime.TryParseExact(loadingObject["loadingDate"].Value<string>(), "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                DateTime today = DateTime.Now;
                TimeSpan difference = today - parsedDate;
                if (Math.Abs(difference.TotalDays) >= 14)
                {
                    Log.logger.Info("Loading文本需要更新。");
                    var newLoadingObject = await DownloadNewLoadingText();
                    if (newLoadingObject != null)
                    {
                        loadingObject = newLoadingObject;
                    }
                }
            }
            else
            {
                Log.logger.Error("读取Loading文本日期失败。");
            }
            Random random = new();
            JArray loadingTexts = loadingObject["loadingTexts"] as JArray;
            CachedLoadingTexts = loadingTexts;
            int choice = random.Next(0, 100);
            string loadingText = "出现这个文本绝不是因为出了什么问题...";
            if (CachedLoadingTexts == null || CachedLoadingTexts.Count == 0)
            {
                Log.logger.Error("Loading文本为空。");
            }
            else
            {
                if (choice < 25)
                {
                    loadingText = CachedLoadingTexts[1].Value<string>();
                }
                else if (choice < 35)
                {
                    loadingText = CachedLoadingTexts[0].Value<string>();
                }
                else
                {
                    loadingText = CachedLoadingTexts[random.Next(0, CachedLoadingTexts.Count)].Value<string>();
                }
                Log.logger.Info("Loading文本：" + loadingText);
            }
            Log.logger.Info("Loading文本：" + loadingText);
            await ChangeLoadingText(loadingText);
        }

        private void LaunchUpdateLoadingThread()
        {
            Thread updateLoadingThread = new Thread(async () =>
            {
                while (true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(30));
                    await TurnAnotherLoadingText();
                }
            });
            updateLoadingThread.Start();
        }

        private async Task TurnAnotherLoadingText()
        {
            Random random = new();
            int choice = random.Next(0, 100);
            string loadingText = "出现这个文本绝不是因为出了什么问题...";
            if (CachedLoadingTexts == null || CachedLoadingTexts.Count == 0)
            {
                Log.logger.Error("Loading文本为空。");
            }
            else
            {
                if (choice < 15)
                {
                    loadingText = CachedLoadingTexts[1].Value<string>();
                }
                else if (choice < 25)
                {
                    loadingText = CachedLoadingTexts[0].Value<string>();
                }
                else
                {
                    loadingText = CachedLoadingTexts[random.Next(0, CachedLoadingTexts.Count)].Value<string>();
                }
            }
            Log.logger.Info("Loading文本：" + loadingText);
            await ChangeLoadingText(loadingText);
        }

        private async Task<JObject?> DownloadNewLoadingText()
        {
            string loadingText = "";
            if (!configuation.Settings.general.internationalMode)
            {
                loadingText = await GetURLText("https://api.zeroasso.top/v2/loading/get_loading", false);
            }
            else
            {
                loadingText = await GetURLText("https://cdn-api.zeroasso.top/v2/loading/get_loading", false);
            }
            if (string.IsNullOrEmpty(loadingText))
            {
                return null;
            }
            JArray loadingArray = JArray.Parse(loadingText);
            var loadingObject = JObject.Parse(await File.ReadAllTextAsync(Path.Combine(currentDir, "loadingText.json")));
            DateTime today = DateTime.Now;
            loadingObject["loadingDate"] = today.ToString("yyyy-MM-dd HH:mm");
            loadingObject["loadingTexts"] = loadingArray;
            File.WriteAllText(Path.Combine(currentDir, "loadingText.json"), loadingObject.ToString());
            return loadingObject;
        }
        #endregion

        #region 字体替换
        private void HandleExploreFontRequested()
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "请选择你的字体",
                Filter = "字体文件 (*.ttf;*.otf)|*.ttf;*.otf|所有文件 (*.*)|*.*",
                Multiselect = false,
            };
            if (fileDialog.ShowDialog() == true)
            {
                ViewModel.FontReplacePath = Path.GetFullPath(fileDialog.FileName);
            }
        }

        private async Task HandlePreviewFontAsync()
        {
            double size;
            if (double.TryParse(ViewModel.FontSizeText, out double fontSize))
            {
                size = fontSize;
            }
            else
            {
                UniversalDialog.ShowMessage("请输入正确的字体大小。", "提示", null, this);
                return;
            }
            if (IsValidFontFile(ViewModel.FontReplacePath))
            {
                Uri fontUri = new Uri(ViewModel.FontReplacePath);
                FontFamily customFont = new FontFamily(fontUri.AbsoluteUri + "#" + GetFontFamilyName(ViewModel.FontReplacePath));
                await this.Dispatcher.BeginInvoke(() =>
                {
                    this.Resources["GlobalPreviewFont"] = customFont;
                    this.Resources["GlobalPreviewFontSize"] = size;
                    this.Resources["GlobalPreviewSmallFontSize"] = size / 16 * 12;
                });
                UniversalDialog.ShowMessage("已将预览文本切换为自定义字体。\n如果出现部分字显示为默认字体，可能影响游戏内显示。", "提示", null, this);
            }
            else
            {
                UniversalDialog.ShowMessage("请选择正确的字体文件。", "提示", null, this);
            }
        }

        private void HandleApplyFontRequested()
        {
            string oldFontPath = Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context", "ChineseFont.ttf");
            string oldOTFFontPath = Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context", "ChineseFont.otf");
            string backupFontPath = Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "BackupFont", "ChineseFont.ttf.bak");
            if (!File.Exists(oldFontPath) && !File.Exists(backupFontPath))
            {
                UniversalDialog.ShowMessage("请先安装汉化，然后再进行字体替换。", "提示", null, this);
                return;
            }
            if (!IsValidFontFile(ViewModel.FontReplacePath))
            {
                Log.logger.Info("字体文件无效。");
                UniversalDialog.ShowMessage("字体文件无效。", "提示", null, this);
                return;
            }
            if (File.Exists(oldFontPath) && !File.Exists(backupFontPath))
            {
                Log.logger.Info("正在备份原字体文件。");
                Directory.CreateDirectory(Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "BackupFont"));
                File.Move(oldFontPath, backupFontPath);
            }
            if (File.Exists(oldFontPath))
            {
                Log.logger.Info("正在删除原TTF字体文件。");
                File.Delete(oldFontPath);
            }
            if (File.Exists(oldOTFFontPath))
            {
                Log.logger.Info("正在删除原OTF字体文件。");
                File.Delete(oldOTFFontPath);
            }
            Log.logger.Info("正在替换字体文件。");
            string extension = new FileInfo(ViewModel.FontReplacePath).Extension;
            File.Copy(ViewModel.FontReplacePath, Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context", $"ChineseFont{extension}"), true);
            UniversalDialog.ShowMessage("字体替换成功。\n启动游戏以应用更改。", "提示", null, this);
            Log.logger.Info("字体替换成功。");
        }

        private void HandleRestoreFontRequested()
        {
            string backupFontPath = Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "BackupFont", "ChineseFont.ttf.bak");
            if (File.Exists(backupFontPath))
            {
                string oldFontTTFPath = Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context", "ChineseFont.ttf");
                string oldFontOTFPath = Path.Combine(limbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context", "ChineseFont.otf");
                if (File.Exists(oldFontTTFPath))
                {
                    File.Delete(oldFontTTFPath);
                }
                if (File.Exists(oldFontOTFPath))
                {
                    File.Delete(oldFontOTFPath);
                }
                File.Move(backupFontPath, oldFontTTFPath);
                UniversalDialog.ShowMessage("字体还原成功。\n启动游戏以应用更改。", "提示", null, this);
            }
            else
            {
                Log.logger.Info("没有找到备份字体文件。");
                UniversalDialog.ShowMessage("没有找到备份字体文件。", "提示", null, this);
            }
        }

        private bool IsValidFontFile(string filePath)
        {
            if (filePath == "输入字体路径")
            {
                return false;
            }
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            if (extension != ".ttf" && extension != ".otf" && extension != ".ttc")
            {
                return false;
            }
            if (!File.Exists(filePath))
            {
                return false;
            }
            try
            {
                using (System.Drawing.Text.PrivateFontCollection fontCollection = new System.Drawing.Text.PrivateFontCollection())
                {
                    fontCollection.AddFontFile(filePath);
                    return fontCollection.Families.Length > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private string GetFontFamilyName(string filePath)
        {
            try
            {
                using (System.Drawing.Text.PrivateFontCollection fontCollection = new System.Drawing.Text.PrivateFontCollection())
                {
                    fontCollection.AddFontFile(filePath);
                    if (fontCollection.Families.Length > 0)
                    {
                        return fontCollection.Families[0].Name;
                    }
                }
            }
            catch { }

            return Path.GetFileNameWithoutExtension(filePath);
        }
        #endregion
    }
}
