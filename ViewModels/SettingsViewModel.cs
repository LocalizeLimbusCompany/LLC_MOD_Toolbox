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
    private string limbusCompanyPath = string.Empty;

    private readonly ILogger<SettingsViewModel> _logger;

    /// <summary>
    /// 仅在 Windows 下有效，不过这个项目也只在 Windows 下有效
    /// </summary>
    public string LimbusCompanyPath
    {
        get
        {
            if (Directory.Exists(limbusCompanyPath))
            {
                return limbusCompanyPath;
            }
            _logger.LogWarning("当前路径不存在！");
            return PathHelper.SelectPath();
        }
        set
        {
            if (!Directory.Exists(value))
                return;
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

    [ObservableProperty]
    private string? testToken;

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
            FileHelper.DeleteBepInEx(LimbusCompanyPath, _logger);
        }
        catch (IOException ex)
        {
            MessageBox.Show("Limbus Company正在运行中，请先关闭游戏。", "警告");
            _logger.LogError(ex, "Limbus Company正在运行中，请先关闭游戏。");
        }
        catch (ArgumentNullException ex)
        {
            MessageBox.Show("注册表内无数据，可能被恶意修改了！", "警告");
            _logger.LogError(ex, "注册表内无数据，可能被恶意修改了！");
        }
        _logger.LogInformation("已卸载模组");
        MessageBox.Show("卸载完成。");

        return Task.CompletedTask;
    }

    [RelayCommand]
    private void SelectLimbusCompanyPath()
    {
        LimbusCompanyPath = PathHelper.SelectPath();
    }

    [RelayCommand]
    private void Test()
    {
        if (string.IsNullOrEmpty(TestToken))
        {
            MessageBox.Show("请输入秘钥！");
            return;
        }
    }

    public SettingsViewModel(ILogger<SettingsViewModel> logger, PrimaryNodeList primaryNodeList)
    {
        _logger = logger;
        DownloadNodeList = primaryNodeList.DownloadNode;
        ApiNodeList = primaryNodeList.ApiNode;
        downloadNode = DownloadNodeList.Last(n => n.IsDefault);
        apiNode = ApiNodeList.Last(n => n.IsDefault);
        LimbusCompanyPath =
            ConfigurationManager.AppSettings["GamePath"]
            ?? PathHelper.DetectedLimbusCompanyPath
            ?? PathHelper.SelectPath();
    }
}
