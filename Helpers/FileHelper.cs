using System.ComponentModel;
using System.Configuration;
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

        public static async Task DownloadFileAsync(string url, string path, EventHandler<DownloadProgressChangedEventArgs> onDownloadProgressChanged, EventHandler<AsyncCompletedEventArgs> onDownloadFileCompleted)
        {
            downloader.DownloadProgressChanged += onDownloadProgressChanged;
            downloader.DownloadFileCompleted += onDownloadFileCompleted;
            await downloader.DownloadFileTaskAsync(url, path);
        }

        public static async Task<bool> CheckHashAsync(Stream archive, Uri endpoint)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] hash = await sha256.ComputeHashAsync(archive);
            return Convert.ToHexString(hash)
                .Equals(
                await HttpHelper.GetHashAsync(new(endpoint, "/LimbusLocalizeHash.json")),
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 解压文件 7z 文件
        /// </summary>
        public static async Task Extract7zAsync(Stream archive, string destination)
        {
            await Task.Run(() =>
            {
                SevenZip.SevenZipBase.SetLibraryPath("7z.dll");
                using var extractor = new SevenZip.SevenZipExtractor(archive);
                extractor.ExtractArchive(destination);
            });
        }

        /// <summary>
        /// 仅在 Windows 下有效，不过这个项目也只在 Windows 下有效
        /// </summary>
        /// <returns cref="string?">边狱公司路径</returns>
        public static string? LimbusCompanyPath => ConfigurationManager.AppSettings["GamePath"] ??
            Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 1973530",
                "InstallLocation",
                null)
                ?.ToString();

        /// <summary>
        /// 读取节点列表配置文件
        /// </summary>
        /// <returns></returns>
        public static Task<string> LoadNodeListConfigAsync =>
            File.ReadAllTextAsync("NodeList.json");

        /// <summary>
        /// 删除Mod文件夹，删除内容为 <seealso cref="BepInExFiles"/>
        /// </summary>
        /// <returns></returns>
        public static void DeleteBepInEx()
        {
            if (string.IsNullOrEmpty(LimbusCompanyPath))
            {
                throw new ArgumentNullException("未找到边狱公司路径。可能是注册表被恶意修改了！");
            }
            foreach (string file in BepInExFiles)
            {
                File.Delete(Path.Combine(LimbusCompanyPath, file));
            }

        }
    }
}
