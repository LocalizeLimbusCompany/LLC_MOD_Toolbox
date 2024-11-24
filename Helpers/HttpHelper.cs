using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows.Media.Imaging;
using Downloader;
using Microsoft.Extensions.Logging;

namespace LLC_MOD_Toolbox.Helpers;

public class HttpHelper
{
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
            RequestConfiguration =
            {
                UserAgent = $"LLC_MOD_Toolbox/{UpdateHelper.LocalVersion}",
                Proxy = new WebProxy()
                {
                    Address = default,
                    UseDefaultCredentials = false,
                    Credentials = CredentialCache.DefaultNetworkCredentials,
                    BypassProxyOnLocal = true
                }
            }
        };

    private static readonly DownloadService downloader = new(downloadOpt);

    public HttpHelper(LoggerFactory loggerFactory)
    {
        downloader.AddLogger(loggerFactory.CreateLogger<DownloadService>());
    }

    public static async Task<Stream> GetModAsync(Uri url)
    {
        Stream stream = await downloader.DownloadFileTaskAsync(url.AbsolutePath);
        if (await FileHelper.CheckHashAsync(stream, url))
        {
            return stream;
        }
        throw new HttpRequestException("Hash check failed.");
    }

    /// <summary>
    /// 获取json数据
    /// </summary>
    /// <exception cref="HttpRequestException">当网络连接不良时直接断言</exception>
    /// <param name="url"></param>
    /// <returns></returns>
    public static async Task<string> GetJsonAsync(Uri url)
    {
        var stream = await downloader.DownloadFileTaskAsync(url.AbsolutePath);
        using StreamReader reader = new(stream);
        return await reader.ReadToEndAsync();
    }

    public static async Task<string> GetHashAsync(Uri url)
    {
        var stream = await downloader.DownloadFileTaskAsync(url.AbsolutePath);
        using StreamReader reader = new(stream);
        return await reader.ReadToEndAsync();
    }

    public static async Task<BitmapImage> GetImageAsync(Uri url)
    {
        var stream = await downloader.DownloadFileTaskAsync(url.AbsolutePath);
        var image = new BitmapImage();
        image.BeginInit();
        image.StreamSource = stream;
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.EndInit();
        return image;
    }

    /// <summary>
    /// 老实说或许它不应该在这个类里……
    /// </summary>
    /// <param name="url">要打开的文件</param>
    public static void LaunchUrl(string url)
    {
        System.Diagnostics.Process.Start(
            new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true }
        );
    }
}
