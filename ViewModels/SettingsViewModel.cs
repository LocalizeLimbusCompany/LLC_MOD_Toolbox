using System.Configuration;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using LLC_MOD_Toolbox.Helpers;
using LLC_MOD_Toolbox.Models;
using Microsoft.Extensions.Logging;

namespace LLC_MOD_Toolbox.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private string limbusCompanyPath = PathHelper.DetectedLimbusCompanyPath;

    private readonly ILogger<SettingsViewModel> _logger;

    /// <summary>
    /// 仅在 Windows 下有效，不过这个项目也只在 Windows 下有效
    /// </summary>
    public string LimbusCompanyPath
    {
        get
        {
            var path =
                ConfigurationManager.AppSettings["GamePath"]
                ?? PathHelper.DetectedLimbusCompanyPath
                ?? throw new ArgumentNullException("未找到边狱公司路径。可能是注册表被删除了！");
            if (Directory.Exists(path))
            {
                limbusCompanyPath = path;
                return path;
            }
            _logger.LogWarning("未找到边狱公司路径。");
            return PathHelper.DetectedLimbusCompanyPath;
        }
        set
        {
            _logger.LogInformation("设置边狱公司路径为：{value}", value);
            ConfigurationManager.AppSettings["GamePath"] = value;
            WeakReferenceMessenger.Default.Send(
                new ValueChangedMessage<(NodeInformation, string)>((DownloadNode, value))
            );
            SetProperty(ref limbusCompanyPath, value);
        }
    }

    [ObservableProperty]
    private List<NodeInformation> downloadNodeList;

    [ObservableProperty]
    private NodeInformation downloadNode;

    [ObservableProperty]
    private List<NodeInformation> apiNodeList;

    [ObservableProperty]
    private NodeInformation apiNode;

    [RelayCommand]
    private Task ModUnistallation()
    {
        _logger.LogInformation("开始卸载 BepInEx。");
        MessageBoxResult result = MessageBox.Show(
            "删除后你需要重新安装汉化补丁。\n确定继续吗？",
            "警告",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning
        );
        if (result != MessageBoxResult.Yes)
        {
            _logger.LogInformation("取消卸载 BepInEx。");
            return Task.FromCanceled(new CancellationToken(true));
        }
        try
        {
            FileHelper.DeleteBepInEx(LimbusCompanyPath);
        }
        catch (IOException ex)
        {
            MessageBox.Show("Limbus Company正在运行中，请先关闭游戏。", "警告");
            _logger.LogError(ex, "Limbus Company正在运行中，请先关闭游戏。");
        }
        catch (ArgumentNullException)
        {
            MessageBox.Show("注册表内无数据，可能被恶意修改了！", "警告");
            _logger.LogError("注册表内无数据，可能被恶意修改了！");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"删除过程中出现了一些问题：\n{ex}", "警告");
            _logger.LogError(ex, "未处理异常");
        }

        return Task.CompletedTask;
    }

    [RelayCommand]
    private void SelectLimbusCompanyPath()
    {
        LimbusCompanyPath = PathHelper.SelectPath();
    }

    public SettingsViewModel(ILogger<SettingsViewModel> logger, PrimaryNodeList primaryNodeList)
    {
        _logger = logger;
        DownloadNodeList = primaryNodeList.DownloadNode;
        ApiNodeList = primaryNodeList.ApiNode;
        downloadNode = DownloadNodeList.Last(n => n.IsDefault);
        apiNode = ApiNodeList.Last(n => n.IsDefault);
    }
}
