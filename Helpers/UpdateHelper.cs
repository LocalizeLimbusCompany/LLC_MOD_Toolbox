namespace LLC_MOD_Toolbox.Helpers;

public static class UpdateHelper
{
    public static async Task<Version> FetchLatestVersion(string url)
        => new Version((await HttpHelper.GetStringAsync(url))[1..]);
}
