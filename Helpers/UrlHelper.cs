using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLC_MOD_Toolbox.Helpers
{
    internal static class UrlHelper
    {
        private static StringBuilder sb = new();

        public static string GetTmpUrl(string url) =>
            sb.Clear().AppendFormat(url, "Tmp").ToString();

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
