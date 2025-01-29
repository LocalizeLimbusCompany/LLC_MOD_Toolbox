using System.IO;

namespace LLC_MOD_Toolbox.Services;

public class RegularFileDownloadService : IFileDownloadService
{
    public Task<Stream> GetModAsync(Uri url)
    {
        throw new NotImplementedException();
    }
}
