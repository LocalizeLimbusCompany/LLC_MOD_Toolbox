using LLC_MOD_Toolbox.Models;
using LLC_MOD_Toolbox.Services.IO;
using LLC_MOD_Toolbox.Services.Network;
using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace LLC_MOD_Toolbox.Services.Skin
{
    public interface ISkinService
    {
        List<string> GetAvailableSkins();
        SkinDefinition? GetSkinInfo(string skinName);
        SkinApplyResult LoadSkin(string skinName);
        SkinApplyResult ApplySkinToWindow(Window window);
        SkinApplyResult ReloadCurrentSkin();
        void StartHotReload();
        void StopHotReload();
        Task<List<SkinDefinition>> GetRemoteSkinDefinitionsAsync();
        Task<bool> InstallSkinFromServerAsync(string skinName);
        List<SkinCatalogItem> BuildSkinCatalog(IEnumerable<SkinDefinition> remoteSkins, IEnumerable<string> localSkinNames);
        string? GetCurrentSkinMusicPath();
        bool SaveCurrentSkinMusicEnabled(bool enabled);
        bool IsHotReloadWatching { get; }
        event EventHandler<SkinReloadedEventArgs>? SkinReloaded;
        event EventHandler? HotReloadStatusChanged;
        string? CurrentSkinName { get; }
        SkinDefinition? CurrentSkinInfo { get; }
    }

    public sealed class SkinService : ISkinService
    {
        private readonly AppState _appState;
        private readonly IHttpService _httpService;
        private readonly IFileService _fileService;
#if DEBUG
        private readonly object _watcherLock = new();
        private readonly List<FileSystemWatcher> _watchers = [];
        private HashSet<string> _watchedFiles = new(StringComparer.OrdinalIgnoreCase);
        private System.Threading.Timer? _reloadTimer;
        private Window? _window;
        private DateTime _suppressWatcherUntilUtc;
#endif
        private string? _lastReportedError;

        public SkinService(AppState appState, IHttpService httpService, IFileService fileService)
        {
            _appState = appState;
            _httpService = httpService;
            _fileService = fileService;
        }

        public List<string> GetAvailableSkins() => SkinManager.Instance.GetAvailableSkins();
        public SkinDefinition? GetSkinInfo(string skinName) => SkinManager.Instance.GetSkinInfo(skinName);
        public SkinApplyResult LoadSkin(string skinName) => SkinManager.Instance.LoadSkin(skinName);

        public SkinApplyResult ApplySkinToWindow(Window window)
        {
#if DEBUG
            _window = window;
#endif
            SkinApplyResult result = SkinManager.Instance.ApplySkinToWindow(window);
#if DEBUG
            if (result.Success)
                StartHotReload();
#endif
            return result;
        }

        public SkinApplyResult ReloadCurrentSkin()
        {
            SkinApplyResult result = SkinManager.Instance.ReloadCurrentSkin();
            if (result.Success)
                RefreshWatchers();
            PublishReloadResult(result, userInitiated: true);
            return result;
        }

        public event EventHandler<SkinReloadedEventArgs>? SkinReloaded;
#if DEBUG
        public event EventHandler? HotReloadStatusChanged;
#else
        public event EventHandler? HotReloadStatusChanged
        {
            add { }
            remove { }
        }
#endif

        public bool IsHotReloadWatching
        {
            get
            {
#if DEBUG
                lock (_watcherLock)
                    return _watchers.Count > 0;
#else
                return false;
#endif
            }
        }

        public void StartHotReload()
        {
#if DEBUG
            RefreshWatchers();
#endif
        }

        public void StopHotReload()
        {
#if DEBUG
            bool changed;
            lock (_watcherLock)
            {
                changed = _watchers.Count > 0;
                foreach (FileSystemWatcher watcher in _watchers)
                    watcher.Dispose();
                _watchers.Clear();
                _watchedFiles.Clear();
                _reloadTimer?.Dispose();
                _reloadTimer = null;
                _window = null;
            }
            if (changed)
                HotReloadStatusChanged?.Invoke(this, EventArgs.Empty);
#endif
        }
        public string? GetCurrentSkinMusicPath() => SkinManager.Instance.GetCurrentSkinMusicPath();
        public bool SaveCurrentSkinMusicEnabled(bool enabled)
        {
#if DEBUG
            _suppressWatcherUntilUtc = DateTime.UtcNow.AddSeconds(1);
#endif
            return SkinManager.Instance.SaveCurrentSkinMusicEnabled(enabled);
        }
        public string? CurrentSkinName => SkinManager.Instance.CurrentSkinName;
        public SkinDefinition? CurrentSkinInfo => SkinManager.Instance.CurrentSkinInfo;

#if DEBUG
        private void RefreshWatchers()
        {
            if (_window == null)
                return;

            var files = new HashSet<string>(SkinManager.Instance.CurrentAssetPaths, StringComparer.OrdinalIgnoreCase)
            {
                SkinManager.Instance.CurrentConfigPath
            };
            var directories = files
                .Select(Path.GetDirectoryName)
                .Where(directory => !string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory))
                .Select(directory => directory!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            lock (_watcherLock)
            {
                foreach (FileSystemWatcher watcher in _watchers)
                    watcher.Dispose();
                _watchers.Clear();
                _watchedFiles = files;

                foreach (string directory in directories)
                {
                    var watcher = new FileSystemWatcher(directory)
                    {
                        Filter = "*.*",
                        IncludeSubdirectories = false,
                        NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.CreationTime
                    };
                    watcher.Changed += OnWatchedFileChanged;
                    watcher.Created += OnWatchedFileChanged;
                    watcher.Deleted += OnWatchedFileChanged;
                    watcher.Renamed += OnWatchedFileRenamed;
                    watcher.EnableRaisingEvents = true;
                    _watchers.Add(watcher);
                }
            }

            HotReloadStatusChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnWatchedFileChanged(object sender, FileSystemEventArgs e)
        {
            ScheduleReloadIfWatched(e.FullPath);
        }

        private void OnWatchedFileRenamed(object sender, RenamedEventArgs e)
        {
            if (IsWatchedPath(e.FullPath) || IsWatchedPath(e.OldFullPath))
                ScheduleReload();
        }

        private void ScheduleReloadIfWatched(string path)
        {
            if (IsWatchedPath(path))
                ScheduleReload();
        }

        private bool IsWatchedPath(string path)
        {
            string fullPath;
            try { fullPath = Path.GetFullPath(path); }
            catch { return false; }

            lock (_watcherLock)
                return _watchedFiles.Contains(fullPath);
        }

        private void ScheduleReload()
        {
            if (DateTime.UtcNow <= _suppressWatcherUntilUtc)
                return;

            lock (_watcherLock)
            {
                _reloadTimer ??= new System.Threading.Timer(_ => ReloadFromWatcher(), null, Timeout.Infinite, Timeout.Infinite);
                _reloadTimer.Change(400, Timeout.Infinite);
            }
        }

        private void ReloadFromWatcher()
        {
            Window? window = _window;
            if (window == null || window.Dispatcher.HasShutdownStarted)
                return;

            _ = window.Dispatcher.BeginInvoke(() =>
            {
                SkinApplyResult result = SkinManager.Instance.ReloadCurrentSkin();
                if (result.Success)
                    RefreshWatchers();
                PublishReloadResult(result, userInitiated: false);
            });
        }
#else
        private void RefreshWatchers()
        {
        }
#endif

        private void PublishReloadResult(SkinApplyResult result, bool userInitiated)
        {
            bool shouldNotify = false;
            if (result.Success)
            {
                _lastReportedError = null;
            }
            else
            {
                string signature = $"{result.ErrorPath}|{result.ErrorMessage}";
                shouldNotify = userInitiated || !string.Equals(signature, _lastReportedError, StringComparison.Ordinal);
                _lastReportedError = signature;
            }

            SkinReloaded?.Invoke(this, new SkinReloadedEventArgs(result, shouldNotify));
        }

        public async Task<List<SkinDefinition>> GetRemoteSkinDefinitionsAsync()
        {
            string raw = await _httpService.GetTextAsync("https://api.zeroasso.top/v2/skin/get_skin_info", reportError: false);
            if (string.IsNullOrWhiteSpace(raw))
                return [];
            return JsonConvert.DeserializeObject<List<SkinDefinition>>(raw) ?? [];
        }

        public async Task<bool> InstallSkinFromServerAsync(string skinName)
        {
            if (string.IsNullOrWhiteSpace(skinName))
                return false;

            string archivePath = Path.Combine(_appState.CurrentDir, $"{skinName}.7z");
            string downloadUrl = $"https://api.zeroasso.top/v2/skin/get_skin/{Uri.EscapeDataString(skinName)}";

            try
            {
                Log.logger.Info($"开始下载皮肤: {skinName}");
                await _httpService.DownloadFileWithoutProgressAsync(downloadUrl, archivePath);
                Log.logger.Info($"开始解压皮肤: {skinName}");
                _fileService.ExtractArchive(archivePath, _appState.CurrentDir);

                string installedSkinPath = Path.Combine(_appState.CurrentDir, "Skins", skinName, "skin.json");
                bool installed = File.Exists(installedSkinPath);
                Log.logger.Info(installed ? $"皮肤安装完成: {skinName}" : $"皮肤已解压但未找到预期配置: {installedSkinPath}");
                return installed;
            }
            catch (Exception ex)
            {
                Log.logger.Error($"安装皮肤失败: {skinName}", ex);
                return false;
            }
            finally
            {
                try
                {
                    if (File.Exists(archivePath))
                        File.Delete(archivePath);
                }
                catch (Exception ex)
                {
                    Log.logger.Warn($"清理皮肤安装包失败: {archivePath}, {ex.Message}");
                }
            }
        }

        public List<SkinCatalogItem> BuildSkinCatalog(IEnumerable<SkinDefinition> remoteSkins, IEnumerable<string> localSkinNames)
        {
            var items = new List<SkinCatalogItem>();
            var installedSkinNames = new HashSet<string>(localSkinNames);
            var addedNames = new HashSet<string>();

            foreach (var skinName in localSkinNames)
            {
                var skinInfo = SkinManager.Instance.GetSkinInfo(skinName);
                if (skinInfo != null)
                {
                    items.Add(CreateSkinCatalogItem(skinInfo, true));
                    addedNames.Add(skinInfo.name);
                }
                else
                {
                    items.Add(new SkinCatalogItem { name = skinName, displayName = skinName, isInstalled = true });
                    addedNames.Add(skinName);
                }
            }

            foreach (var skinInfo in remoteSkins.Where(s => s != null && !string.IsNullOrWhiteSpace(s.name)))
            {
                if (addedNames.Contains(skinInfo.name))
                    continue;
                items.Add(CreateSkinCatalogItem(skinInfo, installedSkinNames.Contains(skinInfo.name)));
                addedNames.Add(skinInfo.name);
            }

            return items;
        }

        private static SkinCatalogItem CreateSkinCatalogItem(SkinDefinition skinInfo, bool isInstalled)
        {
            return new SkinCatalogItem
            {
                name = skinInfo.name ?? string.Empty,
                displayName = skinInfo.displayName ?? skinInfo.name ?? string.Empty,
                desc = skinInfo.desc ?? string.Empty,
                author = skinInfo.author ?? string.Empty,
                version = skinInfo.version ?? "1.0.0",
                isInstalled = isInstalled
            };
        }
    }
}
