using LLC_MOD_Toolbox.Services.Security;
using LLC_MOD_Toolbox.Services.UI;
using System.IO;
using System.Windows;

namespace LLC_MOD_Toolbox.ViewModels
{
    public sealed partial class MainWindowViewModel
    {
        private void HandleNodeSelectionChanged(string? nodeName)
        {
            if (_appState.IsMirrorChyanMode) return;
            Log.logger.Info("选择节点。");
            if (string.IsNullOrEmpty(nodeName)) return;

            if (nodeName == "Github直连")
            {
                _dialogService.ShowMessage("如果您没有使用代理软件（包括Watt Toolkit）\n请不要使用此节点。", "警告");
            }
            _nodeService.SelectDownloadNode(nodeName);
            if (nodeName != "恢复默认" && nodeName != "Github直连")
                _dialogService.ShowMessage("切换成功。", "提示");
        }

        private void HandleApiSelectionChanged(string? apiName)
        {
            if (_appState.IsMirrorChyanMode) return;
            if (_nodeService.UseGithub)
            {
                UpdateSelectedApiOption("恢复默认");
                _dialogService.ShowMessage("切换失败。\n无法在节点为Github直连的情况下切换API。", "提示");
                return;
            }
            Log.logger.Info("选择API节点。");
            if (string.IsNullOrEmpty(apiName)) return;
            _nodeService.SelectApiNode(apiName);
            if (apiName != "恢复默认")
                _dialogService.ShowMessage("切换成功。", "提示");
        }

        private void HandleMirrorChyanConfig()
        {
            if (_appState.IsMirrorChyanMode)
            {
                if (!_dialogService.ShowConfirm("确定要禁用Mirror酱吗？\n关闭后，你可以在设置重新开启Mirror酱的服务。", "提示"))
                    return;
                SecureStringStorage.DeleteSecretFile();
                _mirrorChyanService.DisableMode();
                _dialogService.ShowMessage("已禁用Mirror酱并删除你的Mirror酱CDK。\n为了处理，软件将关闭，再次启动后效果生效。", "提示");
                Application.Current.Shutdown();
            }
            else
            {
                var result = _dialogService.ShowInput("请输入你的 Mirror 酱 CDK。\n你可以在 Mirror 酱官网购买。", "输入秘钥", "Mirror 酱 CDK", InputType.Password,
                    [new DialogButton("确定", true, false), new DialogButton("取消", false, true)]);
                if (result.IsCanceled) return;
                if (result.IsSuccess && !string.IsNullOrEmpty(result.Input))
                {
                    _mirrorChyanService.SetupMode(result.Input);
                    _dialogService.ShowMessage("Mirror酱秘钥设置成功。\n为了处理，软件将关闭，再次启动后效果生效。", "提示");
                    Application.Current.Shutdown();
                }
                else
                {
                    _dialogService.ShowMessage("设置失败。", "提示");
                }
            }
        }

        private void OnMirrorChyanModeDisabled()
        {
            IsMirrorChyanLogoVisible = false;
            MirrorChyanButtonText = "填写秘钥";
            _nodeService.Initialize();
            SetNodeOptions(_nodeService.GetNodeOptions());
            SetApiOptions(_nodeService.GetApiOptions());
            _nodeService.ReadConfigNode();
            string savedNode = _config.Settings.Nodes.DownloadNode;
            string savedApi = _config.Settings.Nodes.ApiNode;
            UpdateSelectedNodeOption(string.IsNullOrEmpty(savedNode) ? "恢复默认" : savedNode);
            UpdateSelectedApiOption(string.IsNullOrEmpty(savedApi) ? "恢复默认" : savedApi);
        }

        private void HandleLauncherShortcut()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string shortcutPath = Path.Combine(desktopPath, "LimbusCompany with LLC.lnk");
            IWshRuntimeLibrary.WshShell wsh = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)wsh.CreateShortcut(shortcutPath);
            shortcut.TargetPath = Path.Combine(_appState.BaseDirectory, "LLC_MOD_Toolbox.exe");
            shortcut.Arguments = "-launcher";
            shortcut.WorkingDirectory = _appState.BaseDirectory;
            shortcut.Description = "启动边狱公司并检查汉化更新";
            shortcut.IconLocation = Path.Combine(_appState.BaseDirectory, "PublicResource", "favicon.ico");
            shortcut.Save();
            _dialogService.ShowMessage("快捷方式已创建。\n可在桌面上找到\"LimbusCompany with LLC\"启动。", "提示");
        }

        private void HandleHotUpdateHelp()
        {
            _dialogService.ShowMessage("你想要知道怎么用热更新？那你可找对地方了兄弟！\n你现在有两种方式，随你便：\n1. 从快捷方式启动\n2. 从Steam启动\n详细请参阅官网文档。", "热更新教程");
        }

    }
}
