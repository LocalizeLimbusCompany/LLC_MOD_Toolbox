using System.Text;
using System.Threading.Tasks;

namespace LLC_MOD_Toolbox.Helpers
{
    internal static class UrlHelper
    {
        private static readonly StringBuilder sb = new();

        private static readonly List<string> paths =
        [
            "BepInEx-IL2CPP-x64.7z",
            "tmpchinesefont_BIE.7z",
            "LimbusLocalize_BIE.7z",
            "Resource/LimbusLocalize_Resource_latest.7z",
        ];
        private static readonly string thisRepo =
            "repos/LocalizeLimbusCompany/LLC_Mod_Toolbox/releases/latest";
        private static readonly List<string> repos =
        [
            "repos/LocalizeLimbusCompany/BepInEx_For_LLC/releases/latest",
            "repos/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/latest",
            "repos/LocalizeLimbusCompany/LLC_ChineseFontAsset/releases/latest",
            "repos/LocalizeLimbusCompany/LLC_Release/releases/latest",
        ];

        public static List<string> GetCustumApiUrls(string url, string? testToken)
        {
            List<string> results = [];
            foreach (var path in paths)
            {
                results.Add(sb.Clear().AppendFormat(url, path).ToString());
            }
            if (testToken is not null)
            {
                paths[2] = GetTestUrl(testToken);
            }
            return paths;
        }

        internal static async Task<List<string>> GetGitHubApiUrl(
            string endpoint,
            Services.IFileDownloadService fileDownloadService
        )
        {
            List<string> results = [];
            foreach (var repo in repos)
            {
                var jsonPayload = await fileDownloadService.GetJsonAsync(
                    sb.Clear().AppendFormat(endpoint, repo).ToString()
                );
                results.Add(JsonHelper.DeserializeValue("browser_download_url", jsonPayload));
            }
            return results;
        }

        public static string GetReleaseUrl(string url) =>
            sb.Clear().AppendFormat(url, thisRepo).ToString();

        public static string GetHashUrl(string url) =>
            sb.Clear().AppendFormat(url, "LimbusLocalizeHash.json").ToString();

        public static string GetTestUrl(string testCode) =>
            sb.Clear()
                .Append("https://api.zeroasso.top/v2/grey_test/get_file?code=")
                .Append(testCode)
                .ToString();

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
