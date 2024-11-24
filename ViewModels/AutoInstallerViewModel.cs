using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LLC_MOD_Toolbox.Helpers;
using LLC_MOD_Toolbox.Models;
using Microsoft.Extensions.Logging;

namespace LLC_MOD_Toolbox.ViewModels;

public partial class AutoInstallerViewModel : ObservableObject
{
    public static PrimaryNodeList PrimaryNodeList { get; set; } = new();
    private readonly ILogger<AutoInstallerViewModel> logger;

    [ObservableProperty]
    private List<NodeInformation> downloadNodeList;

    [ObservableProperty]
    private List<NodeInformation> apiNodeList;

    [Obsolete("直接使用参数传递")]
    [ObservableProperty]
    private NodeInformation selectedEndPoint;

    [Obsolete("直接使用参数传递")]
    [ObservableProperty]
    private NodeInformation selectedApiEndPoint;

    [RelayCommand]
    private static Task ModUnistallation()
    {
        MessageBoxResult result = MessageBox.Show(
            "删除后你需要重新安装汉化补丁。\n确定继续吗？",
            "警告",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning
        );
        if (result != MessageBoxResult.Yes)
        {
            return Task.CompletedTask;
        }
        try
        {
            FileHelper.DeleteBepInEx();
        }
        catch (IOException)
        {
            MessageBox.Show("Limbus Company正在运行中，请先关闭游戏。", "警告");
        }
        catch (ArgumentNullException)
        {
            MessageBox.Show("注册表内无数据，可能被恶意修改了！", "警告");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"删除过程中出现了一些问题：\n{ex}", "警告");
        }

        return Task.CompletedTask;
    }

    // TODO: 将参数改为 NodeInformation 类型
    [RelayCommand]
    private async Task ModInstallation()
    {
        MessageBoxResult result = MessageBox.Show(
            "安装前请确保游戏已经关闭。\n确定继续吗？",
            "警告",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning
        );
        if (result != MessageBoxResult.Yes)
        {
            return;
        }
        try
        {
            await FileHelper.InstallBepInExAsync(SelectedEndPoint.Endpoint);
            logger.LogInformation("BepInEx 安装完成。");
        }
        catch (IOException)
        {
            MessageBox.Show("Limbus Company正在运行中，请先关闭游戏。", "警告");
        }
        catch (ArgumentNullException)
        {
            MessageBox.Show("注册表内无数据，可能被恶意修改了！", "警告");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"安装过程中出现了一些问题：\n{ex}", "警告");
        }
    }

    [Obsolete]
    public AutoInstallerViewModel(ILoggerFactory loggerFactory)
    {
        DownloadNodeList = PrimaryNodeList.DownloadNode;
        ApiNodeList = PrimaryNodeList.ApiNode;
        selectedEndPoint = DownloadNodeList.Last(p => p.IsDefault);
        selectedApiEndPoint = PrimaryNodeList.ApiNode.Last(p => p.IsDefault);
        logger = loggerFactory.CreateLogger<AutoInstallerViewModel>();
    }
}
