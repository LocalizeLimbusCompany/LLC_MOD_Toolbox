namespace LLC_MOD_Toolbox.Services.Network
{
    public interface IHttpService
    {
        Task<string> GetTextAsync(string url, bool reportError = true, int maxRetries = 3, int delayMs = 300, bool parseErrorJson = false);
        Task DownloadFileAsync(string url, string path, IProgress<float>? progress = null);
        Task DownloadFileWithoutProgressAsync(string url, string path);
    }
}
