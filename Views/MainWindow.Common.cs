using Downloader;
using LLC_MOD_Toolbox.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SevenZip;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        #region 常用方法
        public static void Unarchive(string archivePath, string output)
        {
            using SevenZipExtractor extractor = new(archivePath);
            extractor.ExtractArchive(output);
        }

        private static void PrintInstallInfo<T>(string promptInfo, T someObject)
        {
            if (someObject == null)
            {
                Log.logger.Info($"{promptInfo}：空");
            }
            else
            {
                Log.logger.Info($"{promptInfo}{someObject}");
            }
        }

        private static void CheckLimbusCompanyPath()
        {
            if (configuation.Settings.general.skipLCBPathCheck && !string.IsNullOrEmpty(configuation.Settings.general.LCBPath))
            {
                limbusCompanyDir = configuation.Settings.general.LCBPath;
                Log.logger.Info("跳过检查路径。");
            }
            else
            {
                bool CheckLCBPathResult = false;
                if (string.IsNullOrEmpty(limbusCompanyDir))
                {
                    try
                    {
                        limbusCompanyDir = SteamLocator.FindLimbusCompanyPath(
                            appId: "1973530",
                            executableName: "LimbusCompany.exe"
                        );

                        Log.logger.Info($"找到 Limbus Company 安装路径：{limbusCompanyDir}");
                    }
                    catch (Exception ex)
                    {
                        Log.logger.Info($"未找到：{ex.Message}");
                    }
                }
                if (!string.IsNullOrEmpty(limbusCompanyDir))
                {
                    CheckLCBPathResult = UniversalDialog.ShowConfirm($"这是您的边狱公司地址吗？\n{limbusCompanyDir}", "检查路径");
                }
                if (CheckLCBPathResult)
                {
                    Log.logger.Info("用户确认路径。");
                    configuation.Settings.general.LCBPath = limbusCompanyDir;
                    configuation.Settings.general.skipLCBPathCheck = true;
                    configuation.SaveConfig();
                }
                if (string.IsNullOrEmpty(limbusCompanyDir) || !CheckLCBPathResult)
                {
                    if (string.IsNullOrEmpty(limbusCompanyDir))
                    {
                        Log.logger.Warn("未能找到 Limbus Company 目录，手动选择模式。");
                        UniversalDialog.ShowMessage("未能找到 Limbus Company 目录。请手动选择。", "提示", null, null);
                    }
                    else
                    {
                        Log.logger.Warn("用户否认 Limbus Company 目录正确性。");
                    }
                    var fileDialog = new OpenFileDialog
                    {
                        Title = "请选择你的边狱公司游戏文件",
                        Multiselect = false,
                        InitialDirectory = limbusCompanyDir,
                        Filter = "LimbusCompany.exe|LimbusCompany.exe",
                        FileName = "LimbusCompany.exe"
                    };
                    if (fileDialog.ShowDialog() == true)
                    {
                        limbusCompanyDir = Path.GetDirectoryName(fileDialog.FileName) ?? limbusCompanyDir;
                        limbusCompanyGameDir = Path.GetFullPath(fileDialog.FileName);
                    }

                    if (!File.Exists(limbusCompanyGameDir))
                    {
                        Log.logger.Error("选择了错误目录，关闭。");
                        UniversalDialog.ShowMessage("选择目录有误，没有在当前目录找到游戏。", "错误", null, null);
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        Log.logger.Info("找到了正确目录。");
                        configuation.Settings.general.LCBPath = limbusCompanyDir;
                        configuation.Settings.general.skipLCBPathCheck = true;
                        configuation.SaveConfig();
                    }
                }
            }
            limbusCompanyGameDir = Path.Combine(limbusCompanyDir, "LimbusCompany.exe");
            Log.logger.Info("边狱公司路径：" + limbusCompanyDir);
        }

        public static string CalculateSHA256(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var fileStream = File.OpenRead(filePath);
            byte[] hashBytes = sha256.ComputeHash(fileStream);
            Log.logger.Info($"计算位置为 {filePath} 的文件的Hash结果为：{BitConverter.ToString(hashBytes).Replace("-", "").ToLower()}");
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private void NewOnDownloadProgressChanged(object? sender, Downloader.DownloadProgressChangedEventArgs e)
        {
            if (installPhase != 0)
            {
                progressPercentage = (float)((installPhase - 1) * 50 + e.ProgressPercentage * 0.5);
            }
        }

        private void NewOnDownloadProgressCompleted(object? sender, AsyncCompletedEventArgs e)
        {
            if (installPhase != 0)
            {
                progressPercentage = installPhase * 50;
            }
        }

        public async Task DownloadFileAsync(string Url, string Path)
        {
            Log.logger.Info(ProcessLogText($"下载 {Url} 到 {Path}"));
            var downloadOpt = new DownloadConfiguration()
            {
                BufferBlockSize = 10240,
                ChunkCount = 8,
                MaxTryAgainOnFailover = 5,
            };
            var downloader = new DownloadService(downloadOpt);
            downloader.DownloadProgressChanged += NewOnDownloadProgressChanged;
            downloader.DownloadFileCompleted += NewOnDownloadProgressCompleted;
            await downloader.DownloadFileTaskAsync(Url, Path);
        }

        public async Task DownloadFileAsyncWithoutProgress(string Url, string Path)
        {
            Log.logger.Info(ProcessLogText($"下载 {Url} 到 {Path}"));
            var downloadOpt = new DownloadConfiguration()
            {
                BufferBlockSize = 10240,
                ChunkCount = 8,
                MaxTryAgainOnFailover = 5,
            };
            var downloader = new DownloadService(downloadOpt);
            await downloader.DownloadFileTaskAsync(Url, Path);
        }

        public async Task DownloadFileAutoAsync(string File, string Path)
        {
            Log.logger.Info(ProcessLogText($"自动选择下载节点式下载文件 文件: {File}  路径: {Path}"));
            if (!string.IsNullOrEmpty(useEndPoint))
            {
                string DownloadUrl = string.Format(useEndPoint, File);
                await DownloadFileAsync(DownloadUrl, Path);
            }
            else
            {
                string DownloadUrl = string.Format(defaultEndPoint, File);
                await DownloadFileAsync(DownloadUrl, Path);
            }
        }

        private async Task<int> GetLatestLimbusLocalizeVersion(bool useGithub)
        {
            try
            {
                Log.logger.Info("获取模组标签。");
                string version;
                if (!useGithub)
                {
                    string raw = await GetURLText(string.Format(useAPIEndPoint, "v2/resource/get_version"));
                    var json = JObject.Parse(raw);
                    version = json["version"].Value<string>();
                }
                else
                {
                    string raw = await GetURLText("https://api.github.com/repos/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/latest");
                    var json = JObject.Parse(raw);
                    version = json["tag_name"].Value<string>();
                }
                Log.logger.Info($"汉化模组最后标签为： {version}");
                int parseVersion = int.Parse(version);
                return parseVersion;
            }
            catch (Exception ex)
            {
                Log.logger.Error("获取模组标签失败。", ex);
                return -100;
            }
        }

        public async Task<string> GetURLText(string url, bool reportError = true, int maxRetries = 3, int delayMs = 300, bool parseErrorJson = false)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                Log.logger.Error("URL不能为空");
                return string.Empty;
            }

            Log.logger.Info(ProcessLogText($"获取 {url} 文本内容。"));

            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("User-Agent", "LLC_MOD_Toolbox");

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    if (attempt > 1)
                    {
                        Log.logger.Info(ProcessLogText($"第 {attempt} 次尝试获取 {url}"));
                    }

                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.StatusCode == HttpStatusCode.Forbidden && parseErrorJson)
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        await HandleErrorJson(errorContent, url);
                        return string.Empty;
                    }

                    response.EnsureSuccessStatusCode();

                    string result = await response.Content.ReadAsStringAsync();

                    if (attempt > 1)
                    {
                        Log.logger.Info($"第 {attempt} 次尝试成功获取内容");
                    }

                    return result;
                }
                catch (MirrorChyanException ex)
                {
                    ErrorReportMirrorChyan(ex, false);
                    return string.Empty;
                }
                catch (HttpRequestException ex) when (ex.Data.Contains("StatusCode") &&
                                                   (HttpStatusCode)ex.Data["StatusCode"] == HttpStatusCode.Forbidden &&
                                                   parseErrorJson)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    bool isLastAttempt = attempt == maxRetries;

                    if (isLastAttempt)
                    {
                        if (reportError)
                        {
                            ErrorReport(ex, false);
                        }
                        else
                        {
                            Log.logger.Error($"获取网址文本内容失败，已重试 {maxRetries} 次。", ex);
                        }
                    }
                    else
                    {
                        Log.logger.Warn($"第 {attempt} 次获取失败，{delayMs}ms 后重试");
                        await Task.Delay(delayMs);
                    }
                }
            }

            return string.Empty;
        }

        private async Task HandleErrorJson(string jsonContent, string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonContent) ||
                    (!jsonContent.TrimStart().StartsWith("{") && !jsonContent.TrimStart().StartsWith("[")))
                {
                    Log.logger.Warn($"403响应内容不是有效的JSON格式: {url}");
                    return;
                }

                JObject jsonObject = JObject.Parse(jsonContent);
                JToken codeToken = jsonObject["code"];
                if (codeToken != null)
                {
                    int errorCode = codeToken.Value<int>();
                    Log.logger.Error($"MirrorChyan API返回了错误码: {errorCode}");
                    throw new MirrorChyanException(errorCode);
                }
                else
                {
                    Log.logger.Warn($"403响应的JSON中未找到code字段: {url}");
                }
            }
            catch (JsonReaderException ex)
            {
                Log.logger.Warn($"解析403响应JSON失败: {url}, Error: {ex.Message}");
            }
        }

        public static void OpenUrl(string Url)
        {
            Log.logger.Info("打开了网址：" + Url);
            ProcessStartInfo psi = new(Url)
            {
                UseShellExecute = true
            };
            Process.Start(psi);
        }
        #endregion

        #region 进度条系统
        public async void ProgressTime_Tick(object? sender, EventArgs e)
        {
            await ChangeProgressValue(progressPercentage);
        }

        public void StartProgressTimer()
        {
            progressPercentage = 0;
            progressTimer.Start();
        }

        public void StopProgressTimer()
        {
            progressTimer.Stop();
        }
        #endregion
    }
}
