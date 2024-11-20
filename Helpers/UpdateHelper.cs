using System.Reflection;

namespace LLC_MOD_Toolbox.Helpers;

public static class UpdateHelper
{
    /// <summary>
    /// 通过反射获取的本地版本号
    /// </summary>
    public static Version LocalVersion => Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0, 0);

    /// <summary>
    /// 联网获取最新版本号
    /// </summary>
    /// <param name="url">api 节点</param>
    /// <returns>Version</returns>
    public static async Task<Version> GetLatestVersionAsync(string url)
        => new Version((await HttpHelper.GetJsonAsync(url))["tag_name"].ToString()[1..]);

    public static async Task<bool> IsUpdateRequiredAsync(string url)
        => await GetLatestVersionAsync(url) > LocalVersion;
}
