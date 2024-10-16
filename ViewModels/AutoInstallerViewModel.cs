using CommunityToolkit.Mvvm.ComponentModel;
using LLC_MOD_Toolbox.Models;

namespace LLC_MOD_Toolbox.ViewModels;

partial class AutoInstallerViewModel : ObservableObject
{
    [ObservableProperty]
    List<NodeInformation> downloadList = PrimaryNodeList.NodeInstance.DownloadNode;
    [ObservableProperty]
    List<NodeInformation> apiList = PrimaryNodeList.NodeInstance.ApiNode;
    [ObservableProperty]
    static NodeInformation defaultAPIEndPoint = PrimaryNodeList.NodeInstance.DownloadNode
        .First(x => x.IsDefault == true);
    [ObservableProperty]
    static NodeInformation defaultEndPoint = PrimaryNodeList.NodeInstance.ApiNode
        .First(x => x.IsDefault == true);

    [ObservableProperty]
    static NodeInformation selectedEndPoint = defaultEndPoint;
    [ObservableProperty]
    static NodeInformation selectedAPIEndPoint = defaultAPIEndPoint;

}
