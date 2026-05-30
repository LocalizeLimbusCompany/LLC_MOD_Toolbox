using LLC_MOD_Toolbox.Services.Network;
using LLC_MOD_Toolbox.Services.UI;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace LLC_MOD_Toolbox.Services.Greytest
{
    public interface IGreytestService
    {
        bool IsActive { get; }
        string DownloadUrl { get; }
        Task<GreytestValidationResult> ValidateAndActivateAsync(string token);
    }

    public record GreytestValidationResult(bool IsValid, string? Note = null, string? ErrorMessage = null);

    public sealed class GreytestService : IGreytestService
    {
        private static readonly Regex MirrorChyanKeyPattern = new("^[0-9a-z]{24}$", RegexOptions.Compiled);
        private readonly AppState _appState;
        private readonly IHttpService _httpService;
        private readonly INodeService _nodeService;
        private readonly IDialogService _dialogService;

        public GreytestService(AppState appState, IHttpService httpService, INodeService nodeService, IDialogService dialogService)
        {
            _appState = appState;
            _httpService = httpService;
            _nodeService = nodeService;
            _dialogService = dialogService;
        }

        public bool IsActive => _appState.GreytestStatus;
        public string DownloadUrl => _appState.GreytestUrl;

        public async Task<GreytestValidationResult> ValidateAndActivateAsync(string token)
        {
            Log.logger.Info("Z-TECH 灰度测试客户端程序 v3.0 启动。");

            if (_appState.GreytestStatus)
            {
                return new GreytestValidationResult(true, "灰度测试模式已开启。");
            }

            if (string.IsNullOrEmpty(token) || token == "请输入秘钥")
            {
                Log.logger.Info("Token为空。");
                return new GreytestValidationResult(false, ErrorMessage: "请输入有效的Token。");
            }

            if (MirrorChyanKeyPattern.IsMatch(token))
            {
                Log.logger.Info("检测到疑似 Mirror 酱秘钥格式。");
                return new GreytestValidationResult(false, ErrorMessage: "不要输入你的Mirror酱秘钥。");
            }

            Log.logger.Info("Token为：" + token);
            string tokenUrl = _nodeService.ResolveApiUrl($"v2/grey_test/get_token?code={token}");

            using (HttpClient client = new())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(tokenUrl);
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        Log.logger.Info("秘钥无效。");
                        return new GreytestValidationResult(false, ErrorMessage: "请输入有效的Token。");
                    }
                    Log.logger.Info("秘钥有效。");
                }
                catch (Exception ex)
                {
                    Log.logger.Error("验证Token失败。", ex);
                    return new GreytestValidationResult(false, ErrorMessage: "验证失败：" + ex.Message);
                }
            }

            try
            {
                string tokenJson = await _httpService.GetTextAsync(tokenUrl);
                var tokenObject = JObject.Parse(tokenJson);
                string runStatus = tokenObject["status"]!.Value<string>()!;
                if (runStatus != "test")
                {
                    Log.logger.Info("Token已停止测试。");
                    return new GreytestValidationResult(false, ErrorMessage: "Token已停止测试。");
                }

                string note = tokenObject["note"]!.Value<string>()!;
                Log.logger.Info($"Token：{token}\n备注：{note}");

                _appState.GreytestStatus = true;
                _appState.GreytestUrl = _nodeService.ResolveApiUrl($"v2/grey_test/get_file?code={token}");

                return new GreytestValidationResult(true, note);
            }
            catch (Exception ex)
            {
                Log.logger.Error("验证Token失败。", ex);
                return new GreytestValidationResult(false, ErrorMessage: "验证失败：" + ex.Message);
            }
        }
    }
}
