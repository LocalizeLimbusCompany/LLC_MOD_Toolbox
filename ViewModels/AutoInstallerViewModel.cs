using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LLC_MOD_Toolbox.Models;

namespace LLC_MOD_Toolbox.ViewModels;

public partial class AutoInstallerViewModel : ObservableObject
{
    public static PrimaryNodeList PrimaryNodeList { get; set; } = new();

    [ObservableProperty]
    private List<NodeInformation> downloadNodeList;

    [ObservableProperty]
    private List<NodeInformation> apiNodeList;

    [ObservableProperty]
    private NodeInformation selectedEndPoint;

    [ObservableProperty]
    private NodeInformation selectedApiEndPoint;

    [ObservableProperty]
    private static WebProxy? proxy = null;

    [RelayCommand]
    private static void ModUnistallation()
    {

    }

    public AutoInstallerViewModel()
    {
        DownloadNodeList = PrimaryNodeList.DownloadNode;
        ApiNodeList = PrimaryNodeList.ApiNode;
        selectedEndPoint = DownloadNodeList.Last(p => p.IsDefault);
        selectedApiEndPoint = PrimaryNodeList.ApiNode.Last(p => p.IsDefault);
    }
}

