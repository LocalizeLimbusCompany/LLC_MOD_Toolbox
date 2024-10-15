using CommunityToolkit.Mvvm.ComponentModel;
using LLC_MOD_Toolbox.Models;

namespace LLC_MOD_Toolbox.ViewModels;

partial class AutoInstallerViewModel : ObservableObject
{
    [ObservableProperty]
    List<ApiNodeInfo> nodeList = PrimaryNodeList.NodeList.DownloadNode;
    [ObservableProperty]
    List<ApiNodeInfo> apiList = PrimaryNodeList.NodeList.ApiNode;
    [ObservableProperty]
    static ApiNodeInfo defaultAPIEndPoint = PrimaryNodeList.NodeList.DownloadNode
        .First(x => x.IsDefault == true);
    [ObservableProperty]
    static ApiNodeInfo defaultEndPoint = PrimaryNodeList.NodeList.ApiNode
        .First(x => x.IsDefault == true);

    [ObservableProperty]
    static ApiNodeInfo selectedEndPoint = defaultEndPoint;
    [ObservableProperty]
    static ApiNodeInfo selectedAPIEndPoint = defaultAPIEndPoint;

}
