using Newtonsoft.Json.Linq;

namespace LLC_MOD_Toolbox.Services.Network
{
    public interface IMirrorChyanService
    {
        bool IsEnabled { get; }
        string Token { get; }
        event Action? ModeDisabledByError;
        void Initialize();
        void SetupMode(string token);
        void DisableMode();
        JObject ParseResponse(string json);
        Task<(string url, string sha256)> GetFontInfoAsync();
        Task<int> GetLatestModVersionAsync();
        Task<(int version, string url, string sha256)> GetLatestModInfoAsync();
        Task<(bool hasUpdate, string latestVersion)> CheckToolboxUpdateAsync();
        Task<string> GetToolboxDownloadUrlAsync();
    }
}
