using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLC_MOD_Toolbox.Helpers
{
    internal static class UrlHelper
    {
        private static readonly StringBuilder sb = new();

        public static string GetTmpUrl(string url) =>
            sb.Clear().AppendFormat(url, "tmpchinesefont_BIE.7z").ToString();

        public static string GetReleaseUrl(string url) =>
            sb.Clear().AppendFormat(url, "Toolbox_Release.json").ToString();

        public static string GetBepInExUrl(string url) =>
            sb.Clear().AppendFormat(url, "BepInEx-IL2CPP-x64.7z").ToString();

        public static string GetHashUrl(string url) =>
            sb.Clear().AppendFormat(url, "LimbusLocalizeHash.json").ToString();

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
    }
}
