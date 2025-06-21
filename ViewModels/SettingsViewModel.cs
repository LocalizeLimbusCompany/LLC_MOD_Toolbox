using System.Configuration;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LLC_MOD_Toolbox.Helpers;
using LLC_MOD_Toolbox.Models;
using LLC_MOD_Toolbox.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LLC_MOD_Toolbox.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ILogger<SettingsViewModel> _logger;

    private readonly IDialogDisplayService _dialogDisplayService;

    private string? limbusCompanyPath;
    public string? LimbusCompanyPath
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
            if (Directory.Exists(value))
            {
                _logger.LogInformation("设置边狱公司路径为：{value}", value);
                App.Current.Services.GetRequiredService<Config>().GamePath = value;

                SetProperty(ref limbusCompanyPath, value);
            }
        }
    }

    public List<NodeInformation> DownloadNodeList { get; }

    private NodeInformation? downloadNode;
    public NodeInformation DownloadNode
    {
        get
        {
            if (downloadNode == null)
            {
                _logger.LogWarning("下载节点未设置，使用默认节点。");
                downloadNode = DownloadNodeList.Last(n => n.IsDefault);
            }
            return downloadNode;
        }
        set
        {
            if (value == null)
            {
                _logger.LogWarning("尝试设置下载节点为 null，操作被忽略。");
                return;
            }
            _logger.LogInformation("设置下载节点为：{value}", value);
            App.Current.Services.GetRequiredService<Config>().DownloadNode = value;
            SetProperty(ref downloadNode, value);
        }
    }

    public List<NodeInformation> ApiNodeList { get; }

    private NodeInformation? apiNode;
    public NodeInformation ApiNode
    {
        get
        {
            if (apiNode == null)
            {
                _logger.LogWarning("API节点未设置，使用默认节点。");
                apiNode = ApiNodeList.Last(n => n.IsDefault);
            }
            return apiNode;
        }
        set
        {
            if (value == null)
            {
                _logger.LogWarning("尝试设置API节点为 null，操作被忽略。");
                return;
            }
            _logger.LogInformation("设置API节点为：{value}", value);
            App.Current.Services.GetRequiredService<Config>().ApiNode = value;
            SetProperty(ref apiNode, value);
        }
    }

    [ObservableProperty]
    private string? testToken;

    [RelayCommand]
    private Task ModUnistallation()
    {
        _logger.LogInformation("开始卸载 BepInEx。");
        if (!_dialogDisplayService.Confirm("删除后你需要重新安装汉化补丁。\n确定继续吗？"))
        {
            _logger.LogInformation("取消卸载 BepInEx。");
            return Task.FromCanceled(new CancellationToken(true));
        }
        try
        {
            FileHelper.DeleteBepInEx(
                LimbusCompanyPath ?? throw new ArgumentNullException(nameof(LimbusCompanyPath)),
                _logger
            );
        }
        catch (IOException ex)
        {
            _dialogDisplayService.ShowError("Limbus Company正在运行中，请先关闭游戏。");
            _logger.LogError(ex, "Limbus Company正在运行中，请先关闭游戏。");
            return Task.FromCanceled(new CancellationToken(true));
        }
        catch (ArgumentNullException ex)
        {
            _dialogDisplayService.ShowError("注册表内无数据，可能被恶意修改了！");
            _logger.LogError(ex, "注册表内无数据，可能被恶意修改了！");
            return Task.FromCanceled(new CancellationToken(true));
        }
        _logger.LogInformation("已卸载模组");
        _dialogDisplayService.Confirm("已卸载模组");

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
        _logger.LogInformation("开始测试节点连接。");
    }

    public SettingsViewModel(
        ILogger<SettingsViewModel> logger,
        PrimaryNodeList primaryNodeList,
        Config config,
        IDialogDisplayService dialogDisplayService
    )
    {
        _logger = logger;
        _dialogDisplayService = dialogDisplayService;
        DownloadNodeList = primaryNodeList.DownloadNode;
        ApiNodeList = primaryNodeList.ApiNode;
        DownloadNode = config.DownloadNode;
        ApiNode = config.ApiNode;
        LimbusCompanyPath = config.GamePath;
    }
}
