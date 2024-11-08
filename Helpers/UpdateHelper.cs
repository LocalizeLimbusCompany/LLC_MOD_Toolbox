using System.Reflection;

namespace LLC_MOD_Toolbox.Helpers;

public static class UpdateHelper
{
    public static async Task<Version> GetLatestVersionAsync(string url)
        => new Version((await HttpHelper.GetStringAsync(url))[1..]);

    public static async Task<bool> IsUpdateRequiredAsync(string url)
        => await GetLatestVersionAsync(url) > Assembly.GetExecutingAssembly().GetName().Version;
}
