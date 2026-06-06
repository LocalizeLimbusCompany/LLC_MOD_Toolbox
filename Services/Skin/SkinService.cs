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
        bool LoadSkin(string skinName);
        void ApplySkinToWindow(Window window);
        Task<List<SkinDefinition>> GetRemoteSkinDefinitionsAsync();
        Task<bool> InstallSkinFromServerAsync(string skinName);
        List<SkinCatalogItem> BuildSkinCatalog(IEnumerable<SkinDefinition> remoteSkins, IEnumerable<string> localSkinNames);
        string? GetCurrentSkinMusicPath();
        bool SaveCurrentSkinMusicEnabled(bool enabled);
        string? CurrentSkinName { get; }
        SkinDefinition? CurrentSkinInfo { get; }
    }

    public sealed class SkinService : ISkinService
    {
        private readonly AppState _appState;
        private readonly IHttpService _httpService;
        private readonly IFileService _fileService;

        public SkinService(AppState appState, IHttpService httpService, IFileService fileService)
        {
            _appState = appState;
            _httpService = httpService;
            _fileService = fileService;
        }

        public List<string> GetAvailableSkins() => SkinManager.Instance.GetAvailableSkins();
        public SkinDefinition? GetSkinInfo(string skinName) => SkinManager.Instance.GetSkinInfo(skinName);
        public bool LoadSkin(string skinName) => SkinManager.Instance.LoadSkin(skinName);
        public void ApplySkinToWindow(Window window) => SkinManager.Instance.ApplySkinToWindow(window);
        public string? GetCurrentSkinMusicPath() => SkinManager.Instance.GetCurrentSkinMusicPath();
        public bool SaveCurrentSkinMusicEnabled(bool enabled) => SkinManager.Instance.SaveCurrentSkinMusicEnabled(enabled);
        public string? CurrentSkinName => SkinManager.Instance.CurrentSkinName;
        public SkinDefinition? CurrentSkinInfo => SkinManager.Instance.CurrentSkinInfo;

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
