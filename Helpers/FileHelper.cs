using System.ComponentModel;
using System.IO;
using Downloader;
using Microsoft.Extensions.Logging;

namespace LLC_MOD_Toolbox.Helpers;

internal static class FileHelper
{
    private static readonly DownloadConfiguration downloadConfig =
        new()
        {
            ChunkCount = 8,
            MaxTryAgainOnFailover = 3,
            ParallelDownload = true,
        };
    private static readonly DownloadService downloader = new(downloadConfig);

    private static readonly List<string> BepInExFiles =
    [
        "doorstop_config.ini",
        "Latest(框架日志).log",
        "Player(游戏日志).log",
        "winhttp.dll",
        "winhttp.dll.disabled",
        "changelog.txt",
        "BepInEx-IL2CPP-x64.7z",
        "LimbusLocalize_BIE.7z",
        "tmpchinese_BIE.7z"
    ];
    private static readonly List<string> BepInExFolders = ["BepInEx", "dotnet",];

    public static async Task DownloadFileAsync(
        string url,
        string path,
        EventHandler<DownloadProgressChangedEventArgs> onDownloadProgressChanged,
        EventHandler<AsyncCompletedEventArgs> onDownloadFileCompleted,
        ILogger logger
    )
    {
        logger.LogInformation("开始下载文件：{url}", url);
        downloader.AddLogger(logger);
        downloader.DownloadProgressChanged += onDownloadProgressChanged;
        downloader.DownloadFileCompleted += onDownloadFileCompleted;
        await downloader.DownloadFileTaskAsync(url, path);
    }

    /// <summary>
    /// 读取节点列表配置文件
    /// </summary>
    /// <returns>节点配置</returns>
    public static Task<string> LoadNodeListConfigAsync => File.ReadAllTextAsync("NodeList.json");

    /// <summary>
    /// 下载边狱公司的 语言包
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public static void ExtractLanguagePackage(Stream stream, string limbusCompanyPath)
    {
        if (Path.Exists(limbusCompanyPath))
            throw new ArgumentException("路径不存在", nameof(limbusCompanyPath));
        using var extractor = new SevenZip.SevenZipExtractor(stream);
        extractor.ExtractArchive(limbusCompanyPath);
    }

    /// <summary>
    /// 删除 Mod，删除内容为 <seealso cref="BepInExFiles"/> 和 <seealso cref="BepInExFolders"/>
    /// </summary>
    /// <exception cref="ArgumentException">路径不存在</exception>
    public static void DeleteBepInEx(string limbusCompanyPath, ILogger logger)
    {
        if (Path.Exists(limbusCompanyPath))
            throw new ArgumentException("路径不存在", nameof(limbusCompanyPath));

        try
        {
            Directory.Delete(Path.Combine(limbusCompanyPath, "LimbusCompany_Data", "Lang"));
        }
        catch (DirectoryNotFoundException)
        {
            logger.LogWarning("语言包已提前被删除。");
        }

        if (!ValidateHelper.CheckBepInEx(limbusCompanyPath))
            return;

        foreach (string file in BepInExFiles)
        {
            File.Delete(Path.Combine(limbusCompanyPath, file));
        }
        foreach (string folder in BepInExFolders)
        {
            try
            {
                Directory.Delete(Path.Combine(limbusCompanyPath, folder), true);
            }
            catch (DirectoryNotFoundException)
            {
                logger.LogInformation("{}已提前被删除。", folder);
            }
        }
    }
}
