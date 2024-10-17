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
            //downloader.AddLogger(); //回头记得把ILogger传进来
            downloader.DownloadProgressChanged += onDownloadProgressChanged;
            downloader.DownloadFileCompleted += onDownloadFileCompleted;
            await downloader.DownloadFileTaskAsync(url, path);
        }

        public static async Task<string> GetHashAsync(string path)
        {
            using var sha256 = SHA256.Create();
            using var fileStream = File.OpenRead(path);
            var hash = await sha256.ComputeHashAsync(fileStream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        public static Task<string> GetLimbusCompanyPathAsync()
        {
            //仅在 Windows 下有效，不过这个项目也只在 Windows 下有效
            var path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 1973530", "InstallLocation", null) as string;
            return Task.FromResult(path??string.Empty);
        }
    }
}
