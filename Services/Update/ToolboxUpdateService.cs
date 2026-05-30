using LLC_MOD_Toolbox.Services.Network;
using LLC_MOD_Toolbox.Services.UI;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace LLC_MOD_Toolbox.Services.Update
{
    public sealed class ToolboxUpdateService : IToolboxUpdateService
    {
        private readonly AppState _appState;
        private readonly IHttpService _httpService;
        private readonly INodeService _nodeService;
        private readonly IMirrorChyanService _mirrorChyanService;
        private readonly IDialogService _dialogService;

        public ToolboxUpdateService(
            AppState appState,
            IHttpService httpService,
            INodeService nodeService,
            IMirrorChyanService mirrorChyanService,
            IDialogService dialogService)
        {
            _appState = appState;
            _httpService = httpService;
            _nodeService = nodeService;
            _mirrorChyanService = mirrorChyanService;
            _dialogService = dialogService;
        }

        public async Task<ToolboxUpdateCheckResult> CheckForUpdateAsync()
        {
            try
            {
                Log.logger.Info("正在检查工具箱更新。");
                if (_appState.IsMirrorChyanMode)
                {
                    var (hasUpdate, latestVersion) = await _mirrorChyanService.CheckToolboxUpdateAsync();
                    return new ToolboxUpdateCheckResult { HasUpdate = hasUpdate, IsMirrorChyan = true, LatestVersion = latestVersion };
                }
                else
                {
                    string raw = await _httpService.GetTextAsync(
                        _nodeService.ResolveApiUrl("v2/get_api/get/repos/LocalizeLimbusCompany/LLC_Mod_Toolbox/releases/latest"),
                        reportError: false);
                    var jsonObject = JObject.Parse(raw);
                    string latestReleaseTag = jsonObject["tag_name"]!.Value<string>()!.TrimStart('v', 'V');
                    Log.logger.Info("最新安装器tag：" + latestReleaseTag);
                    bool hasUpdate = new System.Version(latestReleaseTag) > Assembly.GetExecutingAssembly().GetName().Version;
                    return new ToolboxUpdateCheckResult { HasUpdate = hasUpdate, IsMirrorChyan = false, LatestVersion = latestReleaseTag };
                }
            }
            catch (Exception ex)
            {
                Log.logger.Error("检查安装器更新出现问题。", ex);
                return new ToolboxUpdateCheckResult { Error = ex };
            }
        }

        public async Task<bool> PerformUpdateAsync(ToolboxUpdateCheckResult result)
        {
            if (result.Error != null || !result.HasUpdate)
                return false;

            Log.logger.Info("安装器存在更新。");
            bool confirmed = _dialogService.ShowConfirm("安装器存在更新。\n点击是下载最新版工具箱安装包并安装。\n你也可以在官网直接下载最新版。", "安装器更新");
            if (!confirmed)
                return true;

            Log.logger.Info("用户选择下载更新。");
            string installerEXE = Path.Combine(Path.GetTempPath(), "LLC_Mod_Toolbox_Installer.exe");
            string downloadUrl = result.IsMirrorChyan
                ? await _mirrorChyanService.GetToolboxDownloadUrlAsync()
                : "https://download.zeroasso.top/files/LLC_MOD_Toolbox_Installer.exe";

            if (string.IsNullOrWhiteSpace(downloadUrl))
            {
                Log.logger.Error("安装器下载地址为空，取消自动更新流程。");
                return false;
            }

            await _httpService.DownloadFileWithoutProgressAsync(downloadUrl, installerEXE);
            Log.logger.Info("下载完成。");
            _dialogService.ShowMessage("下载完成，即将启动安装器。", "提示");

            string batPath = CreateBatchFile(installerEXE);
            StartBatchProcess(batPath);
            return true;
        }

        private static string CreateBatchFile(string targetExePath)
        {
            string batPath = Path.Combine(Path.GetTempPath(), "Cleanup_" + Guid.NewGuid() + ".bat");
            string batContent = $"""
                @echo off
                timeout /t 1 /nobreak >nul
                start /wait "" "{targetExePath}"
                del /f /q "{targetExePath}"
                del /f /q "{batPath}"
                """;
            File.WriteAllText(batPath, batContent);
            return batPath;
        }

        private static void StartBatchProcess(string batPath)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{batPath}\"",
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            Process.Start(processInfo);
        }
    }
}
