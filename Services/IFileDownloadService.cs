using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using Downloader;
using LLC_MOD_Toolbox.Helpers;

namespace LLC_MOD_Toolbox.Services;

public enum ServiceState
{
    Regular,
    GrayRelease,
}

public interface IFileDownloadService
{
    Task<string> GetJsonAsync(string url);
}
