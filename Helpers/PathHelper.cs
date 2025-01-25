using System.Configuration;
using Microsoft.Win32;

namespace LLC_MOD_Toolbox.Helpers
{
    internal static class PathHelper
    {
        /// <summary>
        /// 获取边狱公司路径
        /// </summary>
        public static string DetectedLimbusCompanyPath { get; } =
            Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 1973530",
                "InstallLocation",
                null
            ) as string
            ?? throw new ArgumentNullException("未找到边狱公司路径。可能是注册表被删除了！");

        /// <summary>
        /// 打开指定的网址
        /// </summary>
        /// <param name="url">要打开的地址</param>
        public static void LaunchUrl(string url) =>
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true }
            );

        /// <summary>
        /// 打开指定的网址
        /// </summary>
        /// <param name="url">要打开的地址</param>
        public static void LaunchUrl(Uri url) => LaunchUrl(url.ToString());

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
                return openFolderDialog.FolderName;
            }
            throw new ConfigurationErrorsException("未选择文件！");
        }
    }
}
