using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using Downloader;
using Microsoft.Win32;

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
        public static async Task Extract7zAsync(Stream archive, string destination)
        {
            await Task.Run(() =>
            {
                using var extractor = new SevenZip.SevenZipExtractor(archive);
                extractor.ExtractArchive(destination);
            });
        }

        /// <summary>
        /// 仅在 Windows 下有效，不过这个项目也只在 Windows 下有效
        /// </summary>
        /// <returns cref="string?">边狱公司路径</returns>
        public static string? LimbusCompanyPath
        {
            get
            {
                var path =
                    ConfigurationManager.AppSettings["GamePath"]
                    ?? Registry.GetValue(
                        @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 1973530",
                        "InstallLocation",
                        null
                    ) as string
                    ?? throw new ArgumentNullException("未找到边狱公司路径。可能是注册表被恶意修改了！");
                if (Directory.Exists(path))
                {
                    return path;
                }
                throw new DirectoryNotFoundException("未找到边狱公司路径。可能是注册表被恶意修改了！");
            }
        }

        // TODO: 解除封装直接暴露给 JsonHelper
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
        public static async Task InstallBepInExAsync(Uri uri)
        {
            if (string.IsNullOrEmpty(LimbusCompanyPath))
            {
                throw new Exception("未找到边狱公司路径。可能是注册表被恶意修改了！");
            }
            var stream = await HttpHelper.GetModAsync(uri);
            if (!await ValidateHelper.CheckHashAsync(stream, uri))
            {
                throw new Exception("Hash check failed.");
            }
            await Extract7zAsync(stream, LimbusCompanyPath);
        }

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
