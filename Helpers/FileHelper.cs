using System.ComponentModel;
using System.IO;
using Downloader;

namespace LLC_MOD_Toolbox.Helpers
{
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
            "BepInEx",
            "dotnet",
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

        public static async Task DownloadFileAsync(
            string url,
            string path,
            EventHandler<DownloadProgressChangedEventArgs> onDownloadProgressChanged,
            EventHandler<AsyncCompletedEventArgs> onDownloadFileCompleted
        )
        {
            downloader.DownloadProgressChanged += onDownloadProgressChanged;
            downloader.DownloadFileCompleted += onDownloadFileCompleted;
            await downloader.DownloadFileTaskAsync(url, path);
        }

        /// <summary>
        /// 解压文件 7z 文件
        /// </summary>
        public static Task Extract7zAsync(Stream archive, string destination)
        {
            using var extractor = new SevenZip.SevenZipExtractor(archive);
            extractor.ExtractArchive(destination);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 读取节点列表配置文件
        /// </summary>
        /// <returns></returns>
        public static Task<string> LoadNodeListConfigAsync =>
            File.ReadAllTextAsync("NodeList.json");

        /// <summary>
        /// 下载边狱公司的 BepInEx 框架
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task InstallBepInExAsync(Stream stream, string limbusCompanyPath)
        {
            if (string.IsNullOrEmpty(limbusCompanyPath))
            {
                throw new Exception("未找到边狱公司路径。可能是注册表被恶意修改了！");
            }
            await Extract7zAsync(stream, limbusCompanyPath);
        }

        /// <summary>
        /// 删除Mod文件夹，删除内容为 <seealso cref="BepInExFiles"/>
        /// </summary>
        /// <returns></returns>
        public static void DeleteBepInEx(string limbusCompanyPath)
        {
            if (string.IsNullOrEmpty(limbusCompanyPath))
            {
                throw new Exception("未找到边狱公司路径。可能是注册表被恶意修改了！");
            }
            foreach (string file in BepInExFiles)
            {
                File.Delete(Path.Combine(limbusCompanyPath, file));
            }
        }
    }
}
