using LLC_MOD_Toolbox.Infrastructure;
using LLC_MOD_Toolbox.Models;
using LLC_MOD_Toolbox.Services.Adaptation;
using System.IO;
using System.Net;
using System.Windows;
using static LLC_MOD_Toolbox.Services.Network.DnsResolverChecker;

namespace LLC_MOD_Toolbox.ViewModels
{
    public sealed partial class MainWindowViewModel
    {
        private Task? _initializationTask;

        public Task InitializeAsync() => _initializationTask ??= InitializeCoreAsync();

        private static bool StartupWindowClosed => Application.Current?.MainWindow?.IsVisible != true;

        private async Task InitializeCoreAsync()
        {
            IsGlobalOperationsEnabled = false;
            Log.logger.Info("—————新日志分割线—————");
            Log.logger.Info("工具箱已进入加载流程。");
            Log.logger.Info("We have a lift off.");
            Log.logger.Info($"WPF架构工具箱 版本：{_appState.ToolboxVersion} 。");

            using (StartupDiagnostics.Measure("Apply configured skin"))
            {
                LoadConfiguredSkin();
                SkinApplyResult startupSkinResult = ApplySkinRequested?.Invoke()
                    ?? SkinApplyResult.Failed("$", "皮肤应用入口尚未初始化");
                if (!startupSkinResult.Success)
                    _dialogService.ShowMessage(startupSkinResult.GetDisplayMessage(), "皮肤加载失败");
                else
                    NotifySkinDependentStateChanged();
                RefreshSkinMusicState();
            }
            SkinDescription = (_skinService.CurrentSkinInfo?.desc ?? "暂无描述。").Replace("\\n", "\n");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            _mirrorChyanService.Initialize();
            IsMirrorChyanLogoVisible = _appState.IsMirrorChyanMode;
            MirrorChyanButtonText = _appState.IsMirrorChyanMode ? "禁用" : "填写秘钥";
            _nodeService.Initialize();

            if (_appState.IsMirrorChyanMode)
            {
                SetNodeOptions(["已使用Mirror酱"]);
                UpdateSelectedNodeOption("已使用Mirror酱");
                SetApiOptions(["已使用Mirror酱"]);
                UpdateSelectedApiOption("已使用Mirror酱");
            }
            else
            {
                SetNodeOptions(_nodeService.GetNodeOptions());
                SetApiOptions(_nodeService.GetApiOptions());
                _nodeService.ReadConfigNode();
                string savedNode = _config.Settings.Nodes.DownloadNode;
                string savedApi = _config.Settings.Nodes.ApiNode;
                UpdateSelectedNodeOption(string.IsNullOrEmpty(savedNode) ? "恢复默认" : savedNode);
                UpdateSelectedApiOption(string.IsNullOrEmpty(savedApi) ? "恢复默认" : savedApi);
            }

            Task<Services.Update.ToolboxUpdateCheckResult> toolboxUpdateTask = StartupDiagnostics.MeasureAsync(
                "Toolbox update check", () => _updateService.CheckForUpdateAsync());
            Task<Services.Content.AnnouncementResult> announcementTask = StartupDiagnostics.MeasureAsync(
                "Announcement check", () => Announcement.CheckAsync());

            using (StartupDiagnostics.Measure("Locate game"))
            {
                if (!CheckLimbusCompanyPath())
                    return;
            }
            SevenZip.SevenZipBase.SetLibraryPath(Path.Combine(_appState.BaseDirectory, "7z.dll"));

            // The version request depends on the game path and nodes, not the toolbox update result.
            Task<Services.Version.ModVersionCheckResult> versionTask = StartupDiagnostics.MeasureAsync(
                "Mod version check", () => _versionService.CheckVersionsAsync());
            await InitializeLoadingTextAsync();

            var updateResult = await toolboxUpdateTask;
            if (StartupWindowClosed) return;
            if (updateResult.HasUpdate)
            {
                bool updated = await _updateService.PerformUpdateAsync(updateResult);
                if (updated)
                {
                    Application.Current.Shutdown();
                    return;
                }
            }

            var annoResult = await announcementTask;
            if (StartupWindowClosed) return;
            PresentAnnouncement(annoResult);

            var versionResult = await versionTask;
            if (StartupWindowClosed) return;
            CurrentVersionText = versionResult.CurrentText;
            LatestVersionText = versionResult.LatestText;
            bool needUpdate = versionResult.NeedsUpdate;

            LegacyStructureAdapter.CheckAdapt(_appState.GameDirectory);

            if (!_appState.IsLauncherMode)
            {
                StartLoadingTextRotation();
                LogModInstallationStatus();
            }

            if (_appState.IsLauncherMode && !Announcement.HasNewAnnouncement && !needUpdate)
            {
                try { OpenUrl("steam://rungameid/1973530"); } catch { }
                Application.Current.Shutdown();
                return;
            }

            if ((_config.Settings.Installation.InstallOnLaunch || _appState.IsLauncherMode) && !Announcement.HasNewAnnouncement && needUpdate)
                await ExecuteAutoInstallAsync();

            if (StartupWindowClosed) return;
            IsGlobalOperationsEnabled = true;
            StartupDiagnostics.Mark("Operations ready");
            Log.logger.Info("加载流程完成。");
            if (!_appState.IsLauncherMode && !StartupWindowClosed)
                _ = RefreshOptionalStartupContentAsync();
        }

        private async Task RefreshOptionalStartupContentAsync()
        {
            try
            {
                await Task.WhenAll(_loadingTextService.RefreshAsync(), CheckDnsInBackgroundAsync());
                StartupDiagnostics.Mark("Optional content refreshed");
            }
            catch (Exception ex)
            {
                Log.logger.Warn("刷新启动附加内容失败。", ex);
            }
        }

        private async Task CheckDnsInBackgroundAsync()
        {
            bool suspicious = await CheckForSuspiciousIpAsync("www.zeroasso.top");
            if (suspicious && !StartupWindowClosed)
                _dialogService.ShowMessage("警告！\n检测到您的DNS解析结果存在问题。\n您大概率无法使用工具箱。\n建议您更换DNS服务器后再使用工具箱。\n如果您不知道使用什么DNS服务器，请使用阿里云DNS。", "警告");
        }
    }
}
