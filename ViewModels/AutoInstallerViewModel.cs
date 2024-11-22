using System.IO;
using System.Net;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LLC_MOD_Toolbox.Helpers;
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
    private static Task ModUnistallation()
    {
        MessageBoxResult result = MessageBox.Show("删除后你需要重新安装汉化补丁。\n确定继续吗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes)
        {
            return Task.CompletedTask;
        }
        try
        {
            FileHelper.DeleteBepInEx();
        }
        catch(IOException)
        {
            MessageBox.Show("Limbus Company正在运行中，请先关闭游戏。", "警告");
        }
        catch (ArgumentNullException)
        {
            MessageBox.Show("注册表内无数据，可能被恶意修改了！", "警告");
        }
        catch (Exception ex)
        {
            MessageBox.Show("删除过程中出现了一些问题： " + ex.ToString(), "警告");
        }

        return Task.CompletedTask;
    }

    public AutoInstallerViewModel()
    {
        DownloadNodeList = PrimaryNodeList.DownloadNode;
        ApiNodeList = PrimaryNodeList.ApiNode;
        selectedEndPoint = DownloadNodeList.Last(p => p.IsDefault);
        selectedApiEndPoint = PrimaryNodeList.ApiNode.Last(p => p.IsDefault);
    }
}

