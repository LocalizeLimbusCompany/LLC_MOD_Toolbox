using LLC_MOD_Toolbox.Models;
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
}
