using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using LLC_MOD_Toolbox.Helpers;
using LLC_MOD_Toolbox.Models;

namespace LLC_MOD_Toolbox.ViewModels;

partial class AutoInstallerViewModel : ObservableObject
{
    [ObservableProperty]
    List<NodeInformation> downloadList = PrimaryNodeList.NodeInstance.DownloadNode;
    [ObservableProperty]
    List<NodeInformation> apiList = PrimaryNodeList.NodeInstance.ApiNode;

    [ObservableProperty]
    static NodeInformation selectedEndPoint = PrimaryNodeList.NodeInstance.DownloadNode
        .Last(x => x.IsDefault == true);
    [ObservableProperty]
    static NodeInformation selectedAPIEndPoint = PrimaryNodeList.NodeInstance.ApiNode
        .Last(x => x.IsDefault == true);

    public AutoInstallerViewModel()
    {
        _ = JsonHelper.DeserializePrimaryNodeList(File.ReadAllText("NodeList.json"));
    }
}
