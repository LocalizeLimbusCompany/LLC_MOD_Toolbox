using LLC_MOD_Toolbox.Models;
using LLC_MOD_Toolbox.Services.Configuration;
using LLC_MOD_Toolbox.Services.Security;
using LLC_MOD_Toolbox.Services.UI;
using Newtonsoft.Json.Linq;

namespace LLC_MOD_Toolbox.Services.Network
{
    public sealed class MirrorChyanService : IMirrorChyanService
    {
        private readonly AppState _appState;
        private readonly ConfigurationManager _config;
        private readonly IHttpService _httpService;
        private readonly IDialogService _dialogService;

        public MirrorChyanService(AppState appState, ConfigurationManager config, IHttpService httpService, IDialogService dialogService)
        {
            _appState = appState;
            _config = config;
            _httpService = httpService;
            _dialogService = dialogService;
        }

        public bool IsEnabled => _appState.IsMirrorChyanMode;
        public string Token => _appState.MirrorChyanToken;
        public event Action? ModeDisabledByError;

        public void Initialize()
        {
            if (_config.Settings.mirrorChyan.enable && SecureStringStorage.HasSavedData())
            {
                string token = SecureStringStorage.LoadToken();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    Log.logger.Info("设置Mirror酱模式。");
                    _appState.IsMirrorChyanMode = true;
                    _appState.MirrorChyanToken = token;
                }
                else
                {
                    bool result = _dialogService.ShowConfirm("读取Mirror酱秘钥失败，你想要再输入一次秘钥吗？", "提示");
                    if (result)
                    {
                        var inputResult = _dialogService.ShowInput(
                            "请输入你的 Mirror 酱 CDK。\n你可以在 Mirror 酱官网购买。",
                            "输入秘钥",
                            "Mirror 酱 CDK",
                            InputType.Password,
                            [new DialogButton("确定", true, false), new DialogButton("取消", false, true)]);
                        if (inputResult.IsSuccess && !string.IsNullOrEmpty(inputResult.Input))
                        {
                            SetupMode(inputResult.Input);
                        }
                    }
                    else
                    {
                        DisableMode();
                    }
                }
            }
        }

        public void SetupMode(string token)
        {
            Log.logger.Info("设置Mirror酱模式。");
            _appState.IsMirrorChyanMode = true;
            _appState.MirrorChyanToken = token.Trim();
            SecureStringStorage.SaveToken(_appState.MirrorChyanToken);
            _config.Settings.mirrorChyan.enable = true;
            _config.SaveConfig();
        }

        public void DisableMode()
        {
            _appState.IsMirrorChyanMode = false;
            _appState.MirrorChyanToken = string.Empty;
            _config.Settings.mirrorChyan.enable = false;
            _config.SaveConfig();
            Log.logger.Info("MirrorChyan Mode 已关闭。");
        }

        public JObject ParseResponse(string json)
        {
            JObject parsed = JObject.Parse(json);
            int code = parsed["code"]!.Value<int>();
            if (code != 0)
                throw new MirrorChyanException(code);
            return parsed;
        }

        private bool HandleCdkError(MirrorChyanException ex)
        {
            Log.logger.Error($"Mirror酱 CDK 错误，错误码: {ex.ErrorCode}", ex);

            if (!ex.IsFatal)
            {
                _dialogService.ShowMessage($"访问 Mirror 酱服务出现了问题。\n原因：{ex.Message}", "提示");
                return false;
            }

            bool reEnter = _dialogService.ShowConfirm(
                $"访问 Mirror 酱服务失败。\n原因：{ex.Message}\n\n点击\"是\"重新输入秘钥，点击\"否\"关闭 Mirror 酱模式。",
                "Mirror 酱错误");

            if (reEnter)
            {
                var inputResult = _dialogService.ShowInput(
                    "请输入你的 Mirror 酱 CDK。\n你可以在 Mirror 酱官网购买。",
                    "输入秘钥",
                    "Mirror 酱 CDK",
                    InputType.Password,
                    [new DialogButton("确定", true, false), new DialogButton("取消", false, true)]);
                if (inputResult.IsSuccess && !string.IsNullOrEmpty(inputResult.Input))
                {
                    SetupMode(inputResult.Input);
                    return true;
                }
            }

            DisableMode();
            ModeDisabledByError?.Invoke();
            return false;
        }

        public async Task<(string url, string sha256)> GetFontInfoAsync()
        {
            return await GetFontInfoCoreAsync(allowRetry: true);
        }

        private async Task<(string url, string sha256)> GetFontInfoCoreAsync(bool allowRetry)
        {
            try
            {
                Log.logger.Info("获取字体MirrorChyan链接。");
                string raw = await _httpService.GetTextAsync(
                    $"https://mirrorchyan.com/api/resources/LLCCN-Font/latest?user_agent=LLC_MOD_Toolbox&current_version=&cdk={_appState.MirrorChyanToken}",
                    parseErrorJson: true);
                if (string.IsNullOrEmpty(raw))
                {
                    Log.logger.Error("获取字体MirrorChyan链接失败。");
                    return (string.Empty, string.Empty);
                }
                var json = ParseResponse(raw);
                string url = json["data"]!["url"]!.Value<string>()!;
                string sha256 = json["data"]!["sha256"]!.Value<string>()!;
                return (url, sha256);
            }
            catch (MirrorChyanException ex)
            {
                if (allowRetry && HandleCdkError(ex))
                    return await GetFontInfoCoreAsync(allowRetry: false);
                return (string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                Log.logger.Error("获取字体信息失败。", ex);
                return (string.Empty, string.Empty);
            }
        }

        public async Task<int> GetLatestModVersionAsync()
        {
            return await GetLatestModVersionCoreAsync(allowRetry: true);
        }

        private async Task<int> GetLatestModVersionCoreAsync(bool allowRetry)
        {
            try
            {
                Log.logger.Info("获取模组标签。");
                string raw = await _httpService.GetTextAsync(
                    "https://mirrorchyan.com/api/resources/LLC/latest?user_agent=LLC_MOD_Toolbox&current_version=&cdk=",
                    reportError: false);
                var json = ParseResponse(raw);
                string version = json["data"]!["version_name"]!.Value<string>()!;
                Log.logger.Info($"汉化模组最后标签为： {version}");
                return int.Parse(version);
            }
            catch (MirrorChyanException ex)
            {
                if (allowRetry && HandleCdkError(ex))
                    return await GetLatestModVersionCoreAsync(allowRetry: false);
                return -100;
            }
            catch (Exception ex)
            {
                Log.logger.Error("获取模组标签失败。", ex);
                return -100;
            }
        }

        public async Task<(int version, string url, string sha256)> GetLatestModInfoAsync()
        {
            return await GetLatestModInfoCoreAsync(allowRetry: true);
        }

        private async Task<(int version, string url, string sha256)> GetLatestModInfoCoreAsync(bool allowRetry)
        {
            try
            {
                Log.logger.Info("获取模组标签。");
                string raw = await _httpService.GetTextAsync(
                    $"https://mirrorchyan.com/api/resources/LLC/latest?user_agent=LLC_MOD_Toolbox&current_version=&cdk={_appState.MirrorChyanToken}",
                    parseErrorJson: true);
                var json = ParseResponse(raw);
                string version = json["data"]!["version_name"]!.Value<string>()!;
                Log.logger.Info($"汉化模组最后标签为： {version}");
                int parseVersion = int.Parse(version);
                string url = json["data"]!["url"]!.Value<string>()!;
                string sha256 = json["data"]!["sha256"]!.Value<string>()!;
                return (parseVersion, url, sha256);
            }
            catch (MirrorChyanException ex)
            {
                if (allowRetry && HandleCdkError(ex))
                    return await GetLatestModInfoCoreAsync(allowRetry: false);
                return (-100, string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                Log.logger.Error("获取模组信息失败。", ex);
                return (-100, string.Empty, string.Empty);
            }
        }

        public async Task<(bool hasUpdate, string latestVersion)> CheckToolboxUpdateAsync()
        {
            return await CheckToolboxUpdateCoreAsync(allowRetry: true);
        }

        private async Task<(bool hasUpdate, string latestVersion)> CheckToolboxUpdateCoreAsync(bool allowRetry)
        {
            try
            {
                string raw = await _httpService.GetTextAsync(
                    "https://mirrorchyan.com/api/resources/LLC_MOD_Toolbox/latest?user_agent=LLC_MOD_Toolbox&current_version=&cdk=",
                    reportError: false);
                var json = ParseResponse(raw);
                string latestReleaseTag = json["data"]!["version_name"]!.Value<string>()!.TrimStart('v', 'V');
                Log.logger.Info("最新安装器tag：" + latestReleaseTag);
                bool hasUpdate = new System.Version(latestReleaseTag) > System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return (hasUpdate, latestReleaseTag);
            }
            catch (MirrorChyanException ex)
            {
                if (allowRetry && HandleCdkError(ex))
                    return await CheckToolboxUpdateCoreAsync(allowRetry: false);
                return (false, string.Empty);
            }
            catch (Exception ex)
            {
                Log.logger.Error("检查安装器更新出现问题。", ex);
                return (false, string.Empty);
            }
        }

        public async Task<string> GetToolboxDownloadUrlAsync()
        {
            return await GetToolboxDownloadUrlCoreAsync(allowRetry: true);
        }

        private async Task<string> GetToolboxDownloadUrlCoreAsync(bool allowRetry)
        {
            try
            {
                string raw = await _httpService.GetTextAsync(
                    $"https://mirrorchyan.com/api/resources/LLC_MOD_Toolbox/latest?user_agent=LLC_MOD_Toolbox&current_version=&cdk={_appState.MirrorChyanToken}",
                    parseErrorJson: true);
                var json = ParseResponse(raw);
                return json["data"]!["url"]!.Value<string>()!;
            }
            catch (MirrorChyanException ex)
            {
                if (allowRetry && HandleCdkError(ex))
                    return await GetToolboxDownloadUrlCoreAsync(allowRetry: false);
                return string.Empty;
            }
            catch (Exception ex)
            {
                Log.logger.Error("获取下载链接失败。", ex);
                return string.Empty;
            }
        }
    }
}
