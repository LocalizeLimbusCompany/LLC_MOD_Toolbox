using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media.Imaging;
using Downloader;
using LLC_MOD_Toolbox.Helpers;

namespace LLC_MOD_Toolbox.Services;

public enum ServiceState
{
    Regular,
    GrayRelease,
}

public interface IFileDownloadService
{
    public static DownloadService ServiceDownloader { get; } = new(downloadOpt);
    public static readonly DownloadConfiguration downloadOpt =
        new()
        {
            // file parts to download
            ChunkCount = 8,
            // download speed limited to 2MB/s, default values is zero or unlimited
            MaximumBytesPerSecond = 1024 * 1024 * 2,
            // the maximum number of times to fail
            MaxTryAgainOnFailover = 3,
            // release memory buffer after each 50 MB
            MaximumMemoryBufferBytes = 1024 * 1024 * 50,
            // download parts of the file as parallel or not. The default value is false
            ParallelDownload = true,
            // clear package chunks data when download completed with failure, default value is false
            ClearPackageOnCompletionWithFailure = true,
            // minimum size of chunking to download a file in multiple parts, the default value is 512
            MinimumSizeOfChunking = 1024,
            // Before starting the download, reserve the storage space of the file as file size, the default value is false
            ReserveStorageSpaceBeforeStartingDownload = true,
            // Get on demand downloaded data with ReceivedBytes on downloadProgressChanged event
            EnableLiveStreaming = false,
            // config and customize request headers
            RequestConfiguration = { UserAgent = $"LLC_MOD_Toolbox/{VersionHelper.LocalVersion}", }
        };
    private static readonly HttpClient httpClient = new();

    public async Task<Stream> GetAppAsync(string url) =>
        await ServiceDownloader.DownloadFileTaskAsync(url);

    public async Task<Stream> DownloadFileAsync(string url, string path, IProgress<double> progress)
    {
        ServiceDownloader.DownloadProgressChanged += (sender, e) =>
        {
            progress.Report(e.ProgressPercentage);
        };
        return await ServiceDownloader.DownloadFileTaskAsync(url);
    }

    public async Task<string> GetHashAsync(string url)
    {
        var jsonPayload = await GetJsonAsync(UrlHelper.GetHashUrl(url));
        return JsonHelper.DeserializeValue("hash", jsonPayload);
    }

    public async Task<BitmapImage> GetImageAsync(Uri url)
    {
        var stream = await ServiceDownloader.DownloadFileTaskAsync(url.AbsolutePath);
        var image = new BitmapImage();
        image.BeginInit();
        image.StreamSource = stream;
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.EndInit();
        return image;
    }

    /// <summary>
    /// 获取json数据
    /// </summary>
    /// <exception cref="HttpRequestException">当网络连接不良时直接断言</exception>
    /// <param name="url"></param>
    /// <returns></returns>
    public async Task<string> GetJsonAsync(string url)
    {
        var json = await ServiceDownloader.DownloadFileTaskAsync(url);
        using StreamReader reader = new(json);
        return await reader.ReadToEndAsync();
    }

    /// <summary>
    /// Get an http response from sending request to a specific url
    /// 获取从发送请求到特定url的http响应
    /// </summary>
    /// <param name="url"></param>
    /// <param name="method">Http method being used (default: <see cref="HttpMethod.Get"/>)</param>
    /// <exception cref="HttpRequestException">当网络连接不良时直接断言</exception>
    /// <returns></returns>
    public async Task<HttpResponseMessage> GetResponseAsync(Uri url, [Optional] HttpMethod method)
    {
        using var request = new HttpRequestMessage
        {
            RequestUri = url,
            Method = method ?? HttpMethod.Get
        };
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return response;
    }
    public async Task<Stream> GetBepInExAsync(string url)
    {
        Stream stream = await ServiceDownloader.DownloadFileTaskAsync(UrlHelper.GetBepInExUrl(url));
        return stream;
    }
    public async Task<Stream> GetTmpAsync(string url)
    {
        Stream stream = await ServiceDownloader.DownloadFileTaskAsync(UrlHelper.GetTmpUrl(url));
        return stream;
    }
    public Task<Stream> GetModAsync(string url);
}
