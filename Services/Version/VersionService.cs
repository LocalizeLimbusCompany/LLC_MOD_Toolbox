using LLC_MOD_Toolbox.Services.Network;
using Newtonsoft.Json.Linq;
using System.IO;

namespace LLC_MOD_Toolbox.Services.Version
{
    public interface IVersionService
    {
        Task<VersionCheckResult> CheckVersionsAsync();
    }

    public record VersionCheckResult(int CurrentVersion, int LatestVersion, bool NeedsUpdate, string CurrentText, string LatestText);

    public sealed class VersionService : IVersionService
    {
        private readonly AppState _appState;
        private readonly IHttpService _httpService;
        private readonly INodeService _nodeService;
        private readonly IMirrorChyanService _mirrorChyanService;

        public VersionService(AppState appState, IHttpService httpService, INodeService nodeService, IMirrorChyanService mirrorChyanService)
        {
            _appState = appState;
            _httpService = httpService;
            _nodeService = nodeService;
            _mirrorChyanService = mirrorChyanService;
        }

        public async Task<VersionCheckResult> CheckVersionsAsync()
        {
            int latestVersion = _appState.IsMirrorChyanMode
                ? await GetLatestVersionMirrorChyan()
                : await GetLatestVersionNormal();

            string latestText = latestVersion == -100 ? "最新版本：获取失败" : $"最新版本：{latestVersion}";

            string langDir = Path.Combine(_appState.LimbusCompanyDir, "LimbusCompany_Data/Lang/LLC_zh-CN");
            string versionJsonPath = Path.Combine(langDir, "Info", "version.json");

            int nowVersion = 0;
            string nowText;
            bool needUpdate;

            if (!File.Exists(versionJsonPath))
            {
                needUpdate = true;
                nowText = "当前版本：未安装";
            }
            else
            {
                try
                {
                    JObject versionObj = JObject.Parse(File.ReadAllText(versionJsonPath));
                    nowVersion = versionObj["version"]!.Value<int>();
                    nowText = $"当前版本：{nowVersion}";
                }
                catch (Exception ex)
                {
                    nowText = "当前版本：解析失败";
                    Log.logger.Error("解析version.json出问题", ex);
                    nowVersion = 0;
                }
                needUpdate = nowVersion < latestVersion && nowVersion != 0;
            }

            if (needUpdate && nowVersion != 0)
                nowText += "（可更新）";

            if (!File.Exists(versionJsonPath))
                needUpdate = true;

            return new VersionCheckResult(nowVersion, latestVersion, needUpdate, nowText, latestText);
        }

        private async Task<int> GetLatestVersionMirrorChyan()
        {
            return await _mirrorChyanService.GetLatestModVersionAsync();
        }

        private async Task<int> GetLatestVersionNormal()
        {
            try
            {
                Log.logger.Info("获取模组标签。");
                string raw = await _httpService.GetTextAsync(_nodeService.ResolveApiUrl("v2/resource/get_version"), reportError: false);
                var json = JObject.Parse(raw);
                string version = json["version"]!.Value<string>()!;
                Log.logger.Info($"汉化模组最后标签为： {version}");
                return int.Parse(version);
            }
            catch (Exception ex)
            {
                Log.logger.Error("获取模组标签失败。", ex);
                return -100;
            }
        }
    }
}
