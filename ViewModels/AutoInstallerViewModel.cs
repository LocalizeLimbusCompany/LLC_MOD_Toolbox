using System.Configuration;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using LLC_MOD_Toolbox.Helpers;
using LLC_MOD_Toolbox.Models;
using LLC_MOD_Toolbox.Services;
using Microsoft.Extensions.Logging;

namespace LLC_MOD_Toolbox.ViewModels;

public partial class AutoInstallerViewModel : ObservableObject
{
    private readonly ILogger<AutoInstallerViewModel> _logger;
    private readonly IFileDownloadService fileDownloadService;

    private NodeInformation selectedEndPoint;
    private string limbusCompanyPath;

    [ObservableProperty]
    private Progress<double> installationProgress = new();

    public AutoInstallerViewModel(
        ILogger<AutoInstallerViewModel> logger,
        IFileDownloadService fileDownloadService,
        PrimaryNodeList primaryNodeList
    )
    {
        WeakReferenceMessenger.Default.Register<ValueChangedMessage<(NodeInformation, string)>>(
            this,
            SettingsChanged
        );
        _logger = logger;
        this.fileDownloadService = fileDownloadService;
        selectedEndPoint = primaryNodeList.DownloadNode[0];
        limbusCompanyPath =
            ConfigurationManager.AppSettings["GamePath"]
            ?? PathHelper.DetectedLimbusCompanyPath
            ?? throw new DirectoryNotFoundException("未找到边狱公司路径。可能是注册表被恶意修改了！");
    }

    private void SettingsChanged(
        object recipient,
        ValueChangedMessage<(NodeInformation, string)> message
    )
    {
        (selectedEndPoint, limbusCompanyPath) = message.Value;
    }

    [RelayCommand]
    private async Task ModInstallation()
    {
        _logger.LogInformation("开始安装 BepInEx。");
        _logger.LogInformation("选择的下载节点为：{selectedEndPoint}", selectedEndPoint);
        _logger.LogInformation("边狱公司路径为：{limbusCompanyPath}", limbusCompanyPath);
        MessageBoxResult result = MessageBox.Show(
            "安装前请确保游戏已经关闭。\n确定继续吗？",
            "警告",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning
        );
        if (result != MessageBoxResult.Yes)
        {
            _logger.LogInformation("用户取消了安装 BepInEx。");
            return;
        }
        if (ValidateHelper.CheckMelonloader(limbusCompanyPath))
        {
            MessageBox.Show(
                "Current environment has MelonLoader installed, please uninstall it first.",
                "Warning"
            );
            _logger.LogError(
                "Current environment has MelonLoader installed, please uninstall it first."
            );
            return;
        }
        try
        {
            using var stream = await fileDownloadService.DownloadFileAsync(
                selectedEndPoint.Endpoint,
                limbusCompanyPath,
                InstallationProgress
            );
            var onlineHash = await fileDownloadService.GetHashAsync(selectedEndPoint.Endpoint);
            if (!await ValidateHelper.CheckHashAsync(stream, onlineHash))
                throw new HashException();
            FileHelper.InstallPackage(stream, limbusCompanyPath);
            _logger.LogInformation("BepInEx 安装完成。");
            Stream tmpStream = await fileDownloadService.GetTmpAsync(selectedEndPoint.Endpoint);
            FileHelper.InstallPackage(tmpStream, limbusCompanyPath);
            _logger.LogInformation("Fonts 安装完成。");

            Stream locolizeStream = await fileDownloadService.GetModAsync(
                selectedEndPoint.Endpoint
            );
            FileHelper.InstallPackage(locolizeStream, limbusCompanyPath);
            _logger.LogInformation("Localize 安装完成。");
        }
        catch (IOException ex)
        {
            MessageBox.Show("Limbus Company正在运行中，请先关闭游戏。", "警告");
            _logger.LogWarning(ex, "Limbus Company正在运行中，请先关闭游戏。");
        }
        catch (ArgumentNullException ex)
        {
            MessageBox.Show("注册表内无数据，可能被恶意修改了！", "警告");
            _logger.LogWarning(ex, "注册表内无数据，可能被恶意修改了！");
        }
        catch (HashException ex)
        {
            MessageBox.Show("文件校验失败，请检查网络连接。", "警告");
            _logger.LogWarning(ex, "文件校验失败，请检查网络连接。");
        }
        catch (Exception ex)
        {
            MessageBox.Show("未知错误，请联系开发者。", "警告");
            _logger.LogError(ex, "未知错误，请联系开发者。");
        }
    }
}
