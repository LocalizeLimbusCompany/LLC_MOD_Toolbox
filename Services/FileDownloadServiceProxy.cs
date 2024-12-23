using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;
using Downloader;

namespace LLC_MOD_Toolbox.Services;

public class FileDownloadServiceProxy(
    RegularFileDownloadService regular,
    GrayFileDownloadService gray
) : IFileDownloadService
{
    private IFileDownloadService fileDownloadService = regular;
    private readonly RegularFileDownloadService regular = regular;
    private readonly GrayFileDownloadService gray = gray;

    public void SetState(ServiceState state) =>
        fileDownloadService = state switch
        {
            ServiceState.Regular => regular,
            ServiceState.GrayRelease => gray,
            _ => throw new NotImplementedException(),
        };

    public Task<Stream> GetAppAsync(Uri url) => fileDownloadService.GetAppAsync(url);

    public Task<Stream> DownloadFileAsync(
        Uri url,
        string path,
        EventHandler<DownloadProgressChangedEventArgs> onDownloadProgressChanged,
        EventHandler<AsyncCompletedEventArgs> onDownloadFileCompleted
    ) =>
        fileDownloadService.DownloadFileAsync(
            url,
            path,
            onDownloadProgressChanged,
            onDownloadFileCompleted
        );

    public Task<string> GetJsonAsync(Uri url) => fileDownloadService.GetJsonAsync(url);

    public Task<string> GetHashAsync(Uri url) => fileDownloadService.GetHashAsync(url);

    public Task<BitmapImage> GetImageAsync(Uri url) => fileDownloadService.GetImageAsync(url);
}
