using LLC_MOD_Toolbox.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace LLC_MOD_Toolbox.Helpers
{
    internal static class PathHelper
    {
        /// <summary>
        /// 获取边狱公司路径
        /// </summary>
        public static string? DetectedLimbusCompanyPath { get; } =
            Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 1973530",
                "InstallLocation",
                null
            ) as string;

        /// <summary>
        /// 选择边狱公司游戏文件路径
        /// </summary>
        /// <returns>选择的文件路径</returns>
        public static string SelectPath()
        {
            OpenFolderDialog openFolderDialog =
                new() { DefaultDirectory = DetectedLimbusCompanyPath, Title = "选择边狱公司文件夹" };
            if (openFolderDialog.ShowDialog() == true)
            {
                App.Current.Services.GetRequiredService<Config>().GamePath =
                    openFolderDialog.FolderName;
                return openFolderDialog.FolderName;
            }
            return SelectPath();
        }
    }
}
