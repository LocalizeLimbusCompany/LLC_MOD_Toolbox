namespace LLC_MOD_Toolbox.Services.Network
{
    public interface IHttpService
    {
        // timeout covers the entire operation, including retries and retry delays.
        Task<string> GetTextAsync(string url, bool reportError = true, int maxRetries = 3, int delayMs = 300, bool parseErrorJson = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
        Task DownloadFileAsync(string url, string path, IProgress<float>? progress = null);
        Task DownloadFileWithoutProgressAsync(string url, string path);
    }
}
