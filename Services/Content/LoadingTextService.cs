using LLC_MOD_Toolbox.Services.Configuration;
using LLC_MOD_Toolbox.Services.Network;
using Newtonsoft.Json.Linq;
using System.IO;

namespace LLC_MOD_Toolbox.Services.Content
{
    public interface ILoadingTextService
    {
        Task InitializeAsync();
        string GetRandomLoadingText();
    }

    public sealed class LoadingTextService : ILoadingTextService
    {
        private readonly AppState _appState;
        private readonly IHttpService _httpService;
        private readonly ConfigurationManager _config;
        private JArray? _cachedTexts;
        private readonly Random _random = new();

        public LoadingTextService(AppState appState, IHttpService httpService, ConfigurationManager config)
        {
            _appState = appState;
            _httpService = httpService;
            _config = config;
        }

        public async Task InitializeAsync()
        {
            string filePath = Path.Combine(_appState.CurrentDir, "loadingText.json");
            JObject loadingObject = JObject.Parse(await File.ReadAllTextAsync(filePath));

            if (DateTime.TryParseExact(loadingObject["loadingDate"]!.Value<string>(), "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                TimeSpan difference = DateTime.Now - parsedDate;
                if (Math.Abs(difference.TotalDays) >= 14)
                {
                    Log.logger.Info("Loading文本需要更新。");
                    var newObj = await DownloadNewLoadingText(filePath, loadingObject);
                    if (newObj != null)
                        loadingObject = newObj;
                }
            }
            else
            {
                Log.logger.Error("读取Loading文本日期失败。");
            }

            _cachedTexts = loadingObject["loadingTexts"] as JArray;
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
            if (choice < 25)
                text = _cachedTexts[1]!.Value<string>()!;
            else if (choice < 35)
                text = _cachedTexts[0]!.Value<string>()!;
            else
                text = _cachedTexts[_random.Next(0, _cachedTexts.Count)]!.Value<string>()!;

            Log.logger.Info("Loading文本：" + text);
            return text;
        }

        private async Task<JObject?> DownloadNewLoadingText(string filePath, JObject loadingObject)
        {
            string url = _config.Settings.general.internationalMode
                ? "https://cdn-api.zeroasso.top/v2/loading/get_loading"
                : "https://api.zeroasso.top/v2/loading/get_loading";

            string loadingText = await _httpService.GetTextAsync(url, false);
            if (string.IsNullOrEmpty(loadingText))
                return null;

            JArray loadingArray = JArray.Parse(loadingText);
            loadingObject["loadingDate"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            loadingObject["loadingTexts"] = loadingArray;
            File.WriteAllText(filePath, loadingObject.ToString());
            return loadingObject;
        }
    }
}
