using System.IO;
using System.Net;
using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using LLC_MOD_Toolbox.Helpers;
using LLC_MOD_Toolbox.Models;

namespace LLC_MOD_Toolbox.ViewModels;

public partial class AutoInstallerViewModel : ObservableObject
{
    private readonly PrimaryNodeList primaryNodeList;
    [ObservableProperty]
    List<NodeInformation> downloadList;
    [ObservableProperty]
    List<NodeInformation> apiList;

    [ObservableProperty]
    NodeInformation selectedEndPoint;
    [ObservableProperty]
    NodeInformation selectedAPIEndPoint;

    [ObservableProperty]
    IWebProxy proxy = HttpClient.DefaultProxy;

    public AutoInstallerViewModel()
    {
        primaryNodeList = JsonHelper.DeserializePrimaryNodeList(
            File.ReadAllText("NodeList.json"))
            .Result;
        downloadList = primaryNodeList.DownloadNode;
        apiList = primaryNodeList.ApiNode;
        selectedEndPoint = downloadList.Last(x => x.IsDefault);
        selectedAPIEndPoint = apiList.Last(x => x.IsDefault);
    }

}
