using System.IO;

namespace LLC_MOD_Toolbox.Services;

public class GrayFileDownloadService : IFileDownloadService
{
    public async Task<Stream> GetModAsync(Uri url)
    {
        Stream stream = await IFileDownloadService.ServiceDownloader.DownloadFileTaskAsync(
            new Uri(url, "tmpchinesefont_BIE.7z").AbsolutePath
        );
        return stream;
    }
}
