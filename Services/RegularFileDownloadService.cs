using System.IO;

namespace LLC_MOD_Toolbox.Services;

public class RegularFileDownloadService : IFileDownloadService
{
    public Task<Stream> GetBepInExAsync(Uri url)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> GetModAsync(Uri url)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> GetTmpAsync(Uri url)
    {
        throw new NotImplementedException();
    }
}
