namespace LLC_MOD_Toolbox.Services.Network
{
    public static class HttpServiceExtensions
    {
        // Small, optional metadata must not hold startup behind the download retry policy.
        public static Task<string> GetMetadataTextAsync(this IHttpService httpService, string url) =>
            httpService.GetTextAsync(url, reportError: false, maxRetries: 2, timeout: TimeSpan.FromSeconds(8));
    }
}
