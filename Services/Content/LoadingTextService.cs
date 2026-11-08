using LLC_MOD_Toolbox.Models;
using LLC_MOD_Toolbox.Services.Configuration;
using LLC_MOD_Toolbox.Services.Network;
using Newtonsoft.Json.Linq;
using System.IO;

namespace LLC_MOD_Toolbox.Services.Content
{
    public sealed class LoadingTextService : ILoadingTextService
    {
        private readonly ApplicationState _appState;
        private readonly IHttpService _httpService;
        private readonly IConfigurationService _config;
        private JArray? _cachedTexts;
        private JObject? _loadingObject;
        private bool _needsRefresh;
        private readonly Random _random = new();

        public LoadingTextService(ApplicationState appState, IHttpService httpService, IConfigurationService config)
        {
            _appState = appState;
            _httpService = httpService;
            _config = config;
        }

        public async Task InitializeAsync()
        {
            string filePath = Path.Combine(_appState.BaseDirectory, "loadingText.json");
            try
            {
                _loadingObject = JObject.Parse(await File.ReadAllTextAsync(filePath));
                _cachedTexts = ReadTexts(_loadingObject["loadingTexts"]);
                _needsRefresh = _cachedTexts.Count == 0 ||
                    !DateTime.TryParseExact(_loadingObject["loadingDate"]?.Value<string>(), "yyyy-MM-dd HH:mm", null,
                        System.Globalization.DateTimeStyles.None, out DateTime parsedDate) ||
                    Math.Abs((DateTime.Now - parsedDate).TotalDays) >= 14;
            }
            catch (Exception ex)
            {
                _needsRefresh = true;
                Log.logger.Warn("读取本地Loading文本失败，将使用默认文本。", ex);
            }
        }

        // Refresh only after startup; stale local text remains usable if the network fails.
        public async Task RefreshAsync()
        {
            if (!_needsRefresh)
                return;

            try
            {
                string url = _config.Settings.General.InternationalMode
                    ? "https://cdn-api.zeroasso.top/v2/loading/get_loading"
                    : "https://api.zeroasso.top/v2/loading/get_loading";
                string raw = await _httpService.GetMetadataTextAsync(url);
                if (string.IsNullOrWhiteSpace(raw))
                    return;

                JArray texts = ReadTexts(JArray.Parse(raw));
                if (texts.Count == 0)
                    return;

                var updated = (JObject?)_loadingObject?.DeepClone() ?? new JObject();
                updated["loadingDate"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                updated["loadingTexts"] = texts;
                await File.WriteAllTextAsync(Path.Combine(_appState.BaseDirectory, "loadingText.json"), updated.ToString());
                _loadingObject = updated;
                _cachedTexts = texts;
                _needsRefresh = false;
            }
            catch (Exception ex)
            {
                Log.logger.Warn("更新Loading文本失败，继续使用本地文本。", ex);
            }
        }

        public string GetRandomLoadingText()
        {
            if (_cachedTexts == null || _cachedTexts.Count == 0)
            {
                Log.logger.Error("Loading文本为空。");
                return "出现这个文本绝不是因为出了什么问题...";
            }

            int choice = _random.Next(0, 100);
            string text;
            if (choice < 25 && _cachedTexts.Count > 1)
                text = _cachedTexts[1]!.Value<string>()!;
            else if (choice < 35)
                text = _cachedTexts[0]!.Value<string>()!;
            else
                text = _cachedTexts[_random.Next(0, _cachedTexts.Count)]!.Value<string>()!;

            Log.logger.Info("Loading文本：" + text);
            return text;
        }

        private static JArray ReadTexts(JToken? token) => token is JArray texts
            ? new JArray(texts.Where(text => text.Type == JTokenType.String && !string.IsNullOrWhiteSpace(text.Value<string>())))
            : new JArray();
    }
}
