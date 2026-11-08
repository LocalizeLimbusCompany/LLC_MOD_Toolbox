using LLC_MOD_Toolbox.Models;
using System.IO;
using System.Windows;

namespace LLC_MOD_Toolbox.ViewModels
{
    public sealed partial class MainWindowViewModel
    {
        private bool _skinCatalogLoaded;

        private void LoadConfiguredSkin()
        {
            string skinName = _config.Settings.Skin.CurrentSkin;
            if (string.IsNullOrWhiteSpace(skinName))
                skinName = "default";

            SkinApplyResult result = _skinService.LoadSkin(skinName);
            if (result.Success)
                return;

            Log.logger.Warn($"启动时加载皮肤失败: {skinName}, {result.ErrorPath}, {result.ErrorMessage}");
            _dialogService.ShowMessage(result.GetDisplayMessage(), "皮肤加载失败");
            _skinService.LoadSkin("default");
        }

        private async void HandleSkinSelectionChanged(SkinCatalogItem? selectedSkin)
        {
            try
            {
                if (selectedSkin == null || string.IsNullOrWhiteSpace(selectedSkin.name)) return;
                Log.logger.Info($"选择皮肤: {selectedSkin.DisplayText}");

                bool installedFromServer = false;
                if (!selectedSkin.isInstalled)
                {
                    bool installed = await _skinService.InstallSkinFromServerAsync(selectedSkin.name);
                    if (!installed)
                    {
                        _dialogService.ShowMessage("皮肤安装失败。");
                        return;
                    }
                    installedFromServer = true;
                }

                SkinApplyResult loadResult = _skinService.LoadSkin(selectedSkin.name);
                if (!loadResult.Success)
                {
                    _dialogService.ShowMessage(loadResult.GetDisplayMessage(), "皮肤加载失败");
                    return;
                }

                SkinApplyResult applyResult = ApplySkinRequested?.Invoke()
                    ?? SkinApplyResult.Failed("$", "皮肤应用入口尚未初始化");
                if (!applyResult.Success)
                {
                    _dialogService.ShowMessage(applyResult.GetDisplayMessage(), "皮肤加载失败");
                    return;
                }

                var skinInfo = _skinService.CurrentSkinInfo ?? _skinService.GetSkinInfo(selectedSkin.name);
                if (skinInfo != null)
                    SkinDescription = (skinInfo.desc ?? "暂无描述。").Replace("\\n", "\n");
                NotifySkinDependentStateChanged();
                RefreshSkinMusicState();
                RefreshSkinHotReloadStatus();
                _config.Settings.Skin.CurrentSkin = selectedSkin.name;
                _config.Save();

                if (installedFromServer)
                {
                    await RefreshSkinCatalogAsync(selectedSkin.name);
                    _dialogService.ShowMessage("皮肤安装并应用完成。");
                }
            }
            catch (Exception ex)
            {
                Log.logger.Error($"切换皮肤时出错: {ex.Message}");
            }
        }

        private async Task ShowSkinPageAsync()
        {
            SelectInstallSubPage(InstallSubPage.Skin);
            if (!_skinCatalogLoaded)
                await RefreshSkinCatalogAsync();
        }

        private async Task RefreshSkinCatalogAsync(string? preferredSkinName = null)
        {
            try
            {
                UpdateSkinCatalog([], preferredSkinName);
                List<SkinDefinition> remoteSkins = [];
                try { remoteSkins = await _skinService.GetRemoteSkinDefinitionsAsync(); }
                catch (Exception ex) { Log.logger.Warn($"获取远端皮肤列表失败: {ex.Message}"); }

                if (remoteSkins.Count > 0 && !StartupWindowClosed)
                {
                    // Preserve any selection made while the remote catalog was loading.
                    UpdateSkinCatalog(remoteSkins, SelectedSkinOption?.name ?? preferredSkinName);
                    _skinCatalogLoaded = true;
                }
            }
            catch (Exception ex)
            {
                Log.logger.Error($"初始化皮肤选择框失败: {ex.Message}");
            }
        }

        private void UpdateSkinCatalog(IEnumerable<SkinDefinition> remoteSkins, string? preferredSkinName)
        {
            var localSkinNames = _skinService.GetAvailableSkins();
            var skinOptions = _skinService.BuildSkinCatalog(remoteSkins, localSkinNames);
            // Clearing the collection can make ComboBox write null back to SelectedSkinOption.
            _suppressSkinSelectionNotification = true;
            try
            {
                SetSkinOptions(skinOptions);
                SkinCatalogItem? selectedOption = SkinOptions.FirstOrDefault(s => s.name == preferredSkinName)
                    ?? SkinOptions.FirstOrDefault(s => s.name == _skinService.CurrentSkinInfo?.name)
                    ?? SkinOptions.FirstOrDefault(s => s.name == _config.Settings.Skin.CurrentSkin)
                    ?? SkinOptions.FirstOrDefault(s => s.name == "default");
                SelectedSkinOption = selectedOption;
                if (selectedOption != null)
                    SkinDescription = (string.IsNullOrEmpty(selectedOption.desc) ? "暂无描述。" : selectedOption.desc).Replace("\\n", "\n");
            }
            finally
            {
                _suppressSkinSelectionNotification = false;
            }
        }

        private void RefreshSkinMusicState()
        {
            var musicPath = _skinService.GetCurrentSkinMusicPath();
            var music = _skinService.CurrentSkinInfo?.music;
            bool isAvailable = music != null && !string.IsNullOrWhiteSpace(musicPath);

            IsSkinMusicAvailable = isAvailable;
            IsSkinMusicEnabled = isAvailable ? music!.enableMusic : false;

            if (!isAvailable)
            {
                _skinMusicService.FadeOutAndStop();
                return;
            }

            if (IsSkinMusicEnabled)
                _skinMusicService.PlayWithFadeIn(musicPath!);
            else
                _skinMusicService.FadeOutAndStop();
        }

        private void ToggleSkinMusic()
        {
            if (!IsSkinMusicAvailable)
                return;

            var musicPath = _skinService.GetCurrentSkinMusicPath();
            if (string.IsNullOrWhiteSpace(musicPath))
            {
                IsSkinMusicAvailable = false;
                IsSkinMusicEnabled = false;
                _skinMusicService.FadeOutAndStop();
                return;
            }

            bool enabled = !IsSkinMusicEnabled;
            IsSkinMusicEnabled = enabled;
            bool saved = _skinService.SaveCurrentSkinMusicEnabled(enabled);

            if (enabled)
                _skinMusicService.PlayWithFadeIn(musicPath);
            else
                _skinMusicService.FadeOutAndStop();

            string message = enabled ? "已开启皮肤音乐。" : "已关闭皮肤音乐。";
            if (!saved)
                message += "\n但音乐开关状态保存失败。";
            _dialogService.ShowMessage(message, "提示");
        }

        private void OnSkinReloaded(object? sender, SkinReloadedEventArgs e)
        {
            SkinHotReloadStatus = e.Result.Success
                ? $"监听中 · 最近重载 {DateTime.Now:HH:mm:ss}"
                : "监听中 · 最近重载失败";

            if (e.Result.Success)
            {
                _lastSkinReloadFailureSignature = null;
                NotifySkinDependentStateChanged();
                RefreshSkinMusicState();
            }
            else
            {
                RecordSkinReloadFailure(e.Result);
            }
        }

        private void RecordSkinReloadFailure(SkinApplyResult result)
        {
            string signature = $"{result.ErrorPath}|{result.ErrorMessage}";
            if (string.Equals(signature, _lastSkinReloadFailureSignature, StringComparison.Ordinal))
                return;

            _lastSkinReloadFailureSignature = signature;
            string path = string.IsNullOrWhiteSpace(result.ErrorPath) ? "$" : result.ErrorPath;
            _skinReloadFailureLogs.Enqueue(
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n位置：{path}\n原因：{result.ErrorMessage}");

            while (_skinReloadFailureLogs.Count > 10)
                _skinReloadFailureLogs.Dequeue();
            HasSkinReloadFailureLogs = true;
        }

        private void ShowSkinFailureLogs()
        {
            if (_skinReloadFailureLogs.Count == 0)
            {
                _dialogService.ShowMessage("暂无皮肤重载失败记录。", "皮肤失败日志");
                return;
            }

            string content = string.Join(
                "\n\n————————————\n\n",
                _skinReloadFailureLogs.Reverse());
            _dialogService.ShowMessage(content, "最近皮肤失败日志");
        }

        private void ToggleNavigationDebugBounds()
        {
#if DEBUG
            AreNavigationDebugBoundsVisible = !AreNavigationDebugBoundsVisible;
            NavigationDebugBoundsRequested?.Invoke(AreNavigationDebugBoundsVisible);
#endif
        }

        private void OnHotReloadStatusChanged(object? sender, EventArgs e)
        {
            RefreshSkinHotReloadStatus();
        }

        public void RefreshSkinHotReloadStatus()
        {
#if DEBUG
            SkinHotReloadStatus = _skinService.IsHotReloadWatching
                ? "监听中 · Ctrl+Shift+R 手动重载"
                : "皮肤热重载未启动";
#else
            SkinHotReloadStatus = string.Empty;
#endif
        }

        public Task FadeOutSkinMusicAsync()
        {
            return _skinMusicService.FadeOutAndStopAsync();
        }

        public void SetSkinMusicVolumeMultiplier(double multiplier)
        {
            _skinMusicService.SetVolumeMultiplier(multiplier);
        }
    }
}
