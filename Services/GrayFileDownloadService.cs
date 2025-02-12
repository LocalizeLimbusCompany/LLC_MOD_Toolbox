using System.IO;

namespace LLC_MOD_Toolbox.Services;

public class GrayFileDownloadService : IFileDownloadService
{
    public async Task<Stream> GetModAsync(string url)
    {
        Stream stream = await IFileDownloadService.ServiceDownloader.DownloadFileTaskAsync(
            string.Format(url, "tmpchinesefont_BIE.7z")
        );
        return stream;
    }
}
