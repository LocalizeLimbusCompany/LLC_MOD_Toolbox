using Downloader;
using LLC_MOD_Toolbox.Models;
using LLC_MOD_Toolbox.Services.UI;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;

namespace LLC_MOD_Toolbox.Services.Network
{
    public sealed class HttpService : IHttpService
    {
        private static readonly HttpClient SharedHttpClient = CreateSharedHttpClient();
        private readonly AppState _appState;
        private readonly IDialogService _dialogService;

        public HttpService(AppState appState, IDialogService dialogService)
        {
            _appState = appState;
            _dialogService = dialogService;
        }

        private static HttpClient CreateSharedHttpClient()
        {
            var client = new HttpClient
            {
                DefaultRequestVersion = HttpVersion.Version11,
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact
            };
            client.DefaultRequestHeaders.Add("User-Agent", "LLC_MOD_Toolbox");
            return client;
        }

        public async Task<string> GetTextAsync(string url, bool reportError = true, int maxRetries = 3, int delayMs = 300, bool parseErrorJson = false)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                Log.logger.Error("URL不能为空");
                return string.Empty;
            }

            Log.logger.Info(MaskToken($"获取 {url} 文本内容。"));

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    if (attempt > 1)
                    {
                        Log.logger.Info(MaskToken($"第 {attempt} 次尝试获取 {url}"));
                    }

                    using HttpResponseMessage response = await SharedHttpClient.GetAsync(url);

                    if (response.StatusCode == HttpStatusCode.Forbidden && parseErrorJson)
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        HandleErrorJson(errorContent, url);
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
                catch (MirrorChyanException)
                {
                    throw;
                }
                catch (HttpRequestException ex) when (ex.Data.Contains("StatusCode") &&
                                                       (HttpStatusCode)ex.Data["StatusCode"]! == HttpStatusCode.Forbidden &&
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
                            ReportError(ex, false);
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

        public Task DownloadFileAsync(string url, string path, IProgress<float>? progress = null) =>
            DownloadCoreAsync(url, path, progress);

        public Task DownloadFileWithoutProgressAsync(string url, string path) =>
            DownloadCoreAsync(url, path, null);

        private async Task DownloadCoreAsync(string url, string path, IProgress<float>? progress)
        {
            Log.logger.Info(MaskToken($"下载 {url} 到 {path}"));
            const int maxAttempts = 3;
            Exception? lastError = null;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                var downloadOpt = new DownloadConfiguration()
                {
                    BufferBlockSize = 10240,
                    ChunkCount = 8,
                    MaxTryAgainOnFailure = 5,
                    BlockTimeout = 30000,
                };
                using var downloader = new DownloadService(downloadOpt);
                if (progress != null)
                {
                    downloader.DownloadProgressChanged += (sender, e) =>
                    {
                        progress.Report((float)e.ProgressPercentage);
                    };
                }
                Exception? downloadError = null;
                downloader.DownloadFileCompleted += (sender, e) =>
                {
                    if (e.Cancelled)
                        downloadError = new OperationCanceledException("下载被取消。");
                    else if (e.Error != null)
                        downloadError = e.Error;
                };

                await downloader.DownloadFileTaskAsync(url, path);

                if (downloadError == null && File.Exists(path))
                    return;

                lastError = downloadError;
                Log.logger.Warn(MaskToken($"第 {attempt}/{maxAttempts} 次下载失败：{url} - {downloadError?.Message ?? "文件未生成"}"));
                TryDeletePartialFile(path);
                if (attempt < maxAttempts)
                    await Task.Delay(1000 * attempt);
            }

            ThrowDownloadFailed(url, path, lastError);
        }

        private static void TryDeletePartialFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch (Exception ex)
            {
                Log.logger.Warn($"清理未完成的下载文件失败：{path} - {ex.Message}");
            }
        }

        private void ThrowDownloadFailed(string url, string path, Exception? downloadError)
        {
            if (downloadError != null)
            {
                Log.logger.Error(MaskToken($"下载失败：{url} - {downloadError.Message}"));
                throw new IOException(MaskToken($"下载 {url} 失败：{downloadError.Message}"), downloadError);
            }
            Log.logger.Error(MaskToken($"下载完成但文件不存在：{path}（URL：{url}）"));
            throw new FileNotFoundException(MaskToken($"下载完成但未找到文件：{path}（URL：{url}）"), path);
        }

        private void ThrowIfDownloadFailed(string url, string path, Exception? downloadError)
        {
            if (downloadError != null)
            {
                Log.logger.Error(MaskToken($"下载失败：{url} - {downloadError.Message}"));
                throw new IOException(MaskToken($"下载 {url} 失败：{downloadError.Message}"), downloadError);
            }
            if (!File.Exists(path))
            {
                Log.logger.Error(MaskToken($"下载完成但文件不存在：{path}（URL：{url}）"));
                throw new FileNotFoundException(MaskToken($"下载完成但未找到文件：{path}（URL：{url}）"), path);
            }
        }

        internal HttpClient GetRawClient() => SharedHttpClient;

        private void HandleErrorJson(string jsonContent, string url)
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
                JToken? codeToken = jsonObject["code"];
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
            catch (Newtonsoft.Json.JsonReaderException ex)
            {
                Log.logger.Warn($"解析403响应JSON失败: {url}, Error: {ex.Message}");
            }
        }

        private void ReportError(Exception ex, bool closeWindow)
        {
            Log.logger.Error("出现了问题：\n", ex);
            string errorMessage = GetExceptionText(ex);
            _dialogService.ShowMessage($"运行中出现了问题。但你仍然能够使用工具箱（大概）。\n若要反馈，请带上链接或日志。\n反馈请勿！请勿截图此页面！\n——————————\n错误分析原因：\n{errorMessage}", "错误");
        }

        private string MaskToken(string raw)
        {
            if (string.IsNullOrWhiteSpace(_appState.MirrorChyanToken))
                return raw;
            if (raw.Contains(_appState.MirrorChyanToken))
                return raw.Replace(_appState.MirrorChyanToken, new string('*', _appState.MirrorChyanToken.Length));
            return raw;
        }

        public static string GetExceptionText(Exception ex)
        {
            if (ex is System.Net.WebException or HttpRequestException or HttpProtocolException or System.Net.Sockets.SocketException or System.Net.HttpListenerException or HttpIOException)
                return "网络链接错误，请尝试更换节点，关闭加速器或代理后再试。\n您也可以尝试在官网\"常见问题\"进行排查。";
            if (ex is SevenZip.SevenZipException)
                return "解压出现问题，大概率为网络问题。\n请尝试更换节点，关闭加速器或代理后再试。\n您也可以尝试在官网\"常见问题\"进行排查。";
            if (ex is System.IO.FileNotFoundException)
                return "无法找到文件，可能是网络问题，也可能是边狱公司路径出现错误。\n请尝试更换节点，关闭加速器或代理后再试。\n您也可以尝试在官网\"常见问题\"进行排查。";
            if (ex is UnauthorizedAccessException)
                return "无权限访问文件，请尝试以管理员身份启动，也可能是你打开了边狱公司？";
            if (ex is System.IO.IOException)
                return "文件访问出现问题。\n可能是文件已被边狱公司占用？\n您可以尝试关闭边狱公司。";
            if (ex is HashException)
                return "文件损坏。\n大概率为网络问题，请尝试更换节点，关闭加速器或代理后再试。\n您也可以尝试在官网\"常见问题\"进行排查。";
            return "未知错误原因，错误已记录至日志，请查看官网\"常见问题\"进行排查。\n如果没有解决，请尝试进行反馈。";
        }
    }
}
