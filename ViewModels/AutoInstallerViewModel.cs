using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LLC_MOD_Toolbox.Helpers;
using LLC_MOD_Toolbox.Models;
using LLC_MOD_Toolbox.Services;
using Microsoft.Extensions.Logging;

namespace LLC_MOD_Toolbox.ViewModels;

public partial class AutoInstallerViewModel : ObservableObject
{
    public static PrimaryNodeList PrimaryNodeList { get; set; } = new();
    private readonly ILogger<AutoInstallerViewModel> logger;
    private readonly FileDownloadServiceProxy fileDownloadServiceProxy;

    [ObservableProperty]
    private List<NodeInformation> downloadNodeList;

    [ObservableProperty]
    private List<NodeInformation> apiNodeList;

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

    [RelayCommand]
    private async Task ModInstallation(NodeInformation selectedEndPoint)
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
            var stream = await fileDownloadServiceProxy.GetAppAsync(selectedEndPoint.Endpoint);

            await FileHelper.InstallBepInExAsync("待补充", stream);
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

    public AutoInstallerViewModel(
        ILoggerFactory loggerFactory,
        FileDownloadServiceProxy grayFileDownloadServiceProxy
    )
    {
        logger = loggerFactory.CreateLogger<AutoInstallerViewModel>();
        fileDownloadServiceProxy = grayFileDownloadServiceProxy;
        DownloadNodeList = PrimaryNodeList.DownloadNode;
        ApiNodeList = PrimaryNodeList.ApiNode;
    }
}
