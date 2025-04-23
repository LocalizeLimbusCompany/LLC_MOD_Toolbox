using System.Configuration;
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
    private readonly ILogger<AutoInstallerViewModel> _logger;
    private readonly IFileDownloadService fileDownloadService;
    private readonly IDialogDisplayService dialogDisplayService;

    private readonly NodeInformation selectedEndPoint;
    private readonly string limbusCompanyPath;
    private readonly string? testToken;

    [ObservableProperty]
    private double percent;
    private readonly Progress<double> installationProgress;

    public AutoInstallerViewModel(
        ILogger<AutoInstallerViewModel> logger,
        IFileDownloadService fileDownloadService,
        IDialogDisplayService dialogDisplayService,
        Config config
    )
    {
        _logger = logger;
        this.fileDownloadService = fileDownloadService;
        this.dialogDisplayService = dialogDisplayService;
        selectedEndPoint = config.DownloadNode;
        limbusCompanyPath =
            ConfigurationManager.AppSettings["GamePath"]
            ?? PathHelper.DetectedLimbusCompanyPath
            ?? PathHelper.SelectPath();

        installationProgress = new Progress<double>(value => Percent = value);
    }

    [RelayCommand]
    private async Task ModInstallation()
    {
        _logger.LogInformation("开始安装 BepInEx。");
        _logger.LogInformation("选择的下载节点为：{selectedEndPoint}", selectedEndPoint);
        _logger.LogInformation("边狱公司路径为：{limbusCompanyPath}", limbusCompanyPath);

        if (ValidateHelper.CheckMelonloader(limbusCompanyPath))
        {
            MessageBox.Show("当前环境检测到 MelonLoader，请先卸载", "Warning");
            _logger.LogError("当前环境检测到 MelonLoader，提醒用户卸载。");
            return;
        }

        if (!dialogDisplayService.Confirm("安装前请确保游戏已经关闭。\n确定继续吗？"))
        {
            _logger.LogInformation("用户取消了安装 BepInEx。");
            return;
        }
        try
        {
            List<string> webFiles = [];
            switch (selectedEndPoint.ApiType)
            {
                case ApiType.Custom:
                {
                    webFiles = UrlHelper.GetCustumApiUrls(selectedEndPoint.Endpoint, testToken);
                    break;
                }

                case ApiType.GitHub:
                    webFiles = await UrlHelper.GetGitHubApiUrl(
                        selectedEndPoint.Endpoint,
                        fileDownloadService
                    );
                    break;
                default:
                    throw new NotImplementedException("暂不支持的 API 类型。");
            }
            List<Task> tasks = [];
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
