using System.IO;
using Downloader;
using LLC_MOD_Toolbox.Helpers;
using Microsoft.Extensions.Logging;

namespace LLC_MOD_Toolbox.Services;

public class RegularFileDownloadService : IFileDownloadService
{
    public DownloadService DownloadService { get; }
    private static readonly DownloadConfiguration downloadOpt =
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
    private readonly ILogger<RegularFileDownloadService> _logger;

    public async Task<string> GetJsonAsync(string url)
    {
        DownloadService.AddLogger(_logger);
        Stream stream = await DownloadService.DownloadFileTaskAsync(url);
        using StreamReader reader = new(stream);
        return await reader.ReadToEndAsync();
    }

    public RegularFileDownloadService(ILogger<RegularFileDownloadService> logger)
    {
        _logger = logger;
        DownloadService = new DownloadService(downloadOpt);
    }
}
