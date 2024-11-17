using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using Downloader;
using Microsoft.Win32;

namespace LLC_MOD_Toolbox.Helpers
{
    public static class FileHelper
    {
        private static readonly DownloadConfiguration downloadConfig = new()
        {
            ChunkCount = 8,
            MaxTryAgainOnFailover = 3,
            ParallelDownload = true,
        };
        private static readonly DownloadService downloader = new(downloadConfig);

        public static async Task DownloadFileAsync(string url, string path, EventHandler<DownloadProgressChangedEventArgs> onDownloadProgressChanged, EventHandler<AsyncCompletedEventArgs> onDownloadFileCompleted)
        {
            downloader.DownloadProgressChanged += onDownloadProgressChanged;
            downloader.DownloadFileCompleted += onDownloadFileCompleted;
            await downloader.DownloadFileTaskAsync(url, path);
        }

        public static async Task<string> GetHashAsync(string path)
        {
            using var sha256 = SHA256.Create();
            await using var fileStream = File.OpenRead(path);
            var hash = await sha256.ComputeHashAsync(fileStream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// 解压文件 7z 文件
        /// </summary>
        public static async Task Extract7zAsync(string source, string destination)
        {
            await Task.Run(() =>
            {
                SevenZip.SevenZipBase.SetLibraryPath("7z.dll");
                using var extractor = new SevenZip.SevenZipExtractor(source);
                extractor.ExtractArchive(destination);
            });
        }

        /// <summary>
        /// 仅在 Windows 下有效，不过这个项目也只在 Windows 下有效
        /// </summary>
        /// <returns cref="string?">边狱公司路径</returns>
        public static Task<string?> GetLimbusCompanyPathAsync() =>
            Task.FromResult(Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 1973530",
                "InstallLocation",
                null)?.ToString());

        public static string GetJsonConfig() =>
            File.ReadAllText("NodeList.json");
    }
}
