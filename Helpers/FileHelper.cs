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
    /// 下载边狱公司的 Mod
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public static void InstallPackage(Stream stream, string limbusCompanyPath)
    {
        if (string.IsNullOrEmpty(limbusCompanyPath))
        {
            throw new Exception("未找到边狱公司路径。可能是注册表被恶意修改了！");
        }
        using var extractor = new SevenZip.SevenZipExtractor(stream);
        extractor.ExtractArchive(limbusCompanyPath);
    }

    /// <summary>
    /// 删除 Mod，删除内容为 <seealso cref="BepInExFiles"/> 和 <seealso cref="BepInExFolders"/>
    /// </summary>
    public static void DeleteBepInEx(string limbusCompanyPath, ILogger logger)
    {
        if (string.IsNullOrEmpty(limbusCompanyPath))
        {
            throw new Exception("未找到边狱公司路径。可能是注册表被恶意修改了！");
        }
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
            catch (DirectoryNotFoundException ex)
            {
                logger.LogInformation(ex, "{}已提前被删除。", folder);
            }
        }
    }

    /// <summary>
    /// [未测]
    /// 添加到 <seealso href="https://learn.microsoft.com/zh-cn/powershell/module/defender/add-mppreference">Windows Defender</seealso> 的排除列表<br/>
    /// <b>*需要管理员权限</b><br/>
    /// <b>*危险操作请勿自动进行</b>
    /// </summary>
    /// <param name="path"></param>
    public static void AddToExcludeList(string path)
    {
        var processInfo = new System.Diagnostics.ProcessStartInfo("powershell")
        {
            Arguments = $"Add-MpPreference -ExclusionPath \"{path}\"",
            UseShellExecute = true,
            Verb = "RunAs"
        };
        System.Diagnostics.Process.Start(processInfo);
    }
}
