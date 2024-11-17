using System.IO;
using System.Net;
using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using LLC_MOD_Toolbox.Helpers;
using LLC_MOD_Toolbox.Models;

namespace LLC_MOD_Toolbox.ViewModels;

public partial class AutoInstallerViewModel : ObservableObject
{
    [ObservableProperty]
    List<NodeInformation> downloadList = PrimaryNodeList.Instance.DownloadNode;
    [ObservableProperty]
    List<NodeInformation> apiList = PrimaryNodeList.Instance.ApiNode;

    [ObservableProperty]
    NodeInformation selectedEndPoint = PrimaryNodeList.Instance.DownloadNode.Last(x => x.IsDefault);
    [ObservableProperty]
    NodeInformation selectedAPIEndPoint = PrimaryNodeList.Instance.ApiNode.Last(x => x.IsDefault);

    [ObservableProperty]
    IWebProxy proxy = HttpClient.DefaultProxy;

    public AutoInstallerViewModel()
    {
    }

}
