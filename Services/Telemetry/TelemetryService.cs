using LLC_MOD_Toolbox.Services.Configuration;
using LLC_MOD_Toolbox.Services.Network;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace LLC_MOD_Toolbox.Services.Telemetry
{
    public sealed class TelemetryService : ITelemetryService
    {
        private static readonly HttpClient HttpClient = CreateHttpClient();
        private static readonly string ClientGuidPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LLC_MOD_Toolbox",
            "TelemetryGuid.txt");
        private readonly ConfigurationManager _config;
        private readonly INodeService _nodeService;

        public TelemetryService(ConfigurationManager config, INodeService nodeService)
        {
            _config = config;
            _nodeService = nodeService;
        }

        public async Task SubmitDailyAsync()
        {
            if (_config.Settings.telemetry.lastSubmittedDate == DateTime.Today.ToString("yyyy-MM-dd"))
                return;

            EnsureClientGuid();
            _nodeService.Initialize();
            _nodeService.ReadConfigNode();

            var payload = new
            {
                guid = _config.Settings.telemetry.clientGuid,
                app_version = GetAppVersion(),
                os_version = Environment.OSVersion.VersionString
            };

            try
            {
                string json = JsonConvert.SerializeObject(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await HttpClient.PostAsync(_nodeService.ResolveApiUrl("v2/telemetering"), content);
                if (!response.IsSuccessStatusCode)
                {
                    Log.logger.Warn($"遥测提交失败：HTTP {(int)response.StatusCode}");
                    return;
                }

                _config.Settings.telemetry.telemeteringSubmitted = true;
                _config.Settings.telemetry.lastSubmittedDate = DateTime.Today.ToString("yyyy-MM-dd");
                _config.SaveConfig();
            }
            catch (Exception ex)
            {
                Log.logger.Warn("遥测提交失败。", ex);
            }
        }

        private void EnsureClientGuid()
        {
            if (!string.IsNullOrWhiteSpace(_config.Settings.telemetry.clientGuid))
            {
                SaveClientGuid(_config.Settings.telemetry.clientGuid);
                return;
            }

            string savedGuid = LoadClientGuid();
            if (!string.IsNullOrWhiteSpace(savedGuid))
            {
                _config.Settings.telemetry.clientGuid = savedGuid;
                _config.SaveConfig();
                return;
            }

            _config.Settings.telemetry.clientGuid = Guid.NewGuid().ToString();
            SaveClientGuid(_config.Settings.telemetry.clientGuid);
            _config.SaveConfig();
        }

        private static string LoadClientGuid()
        {
            try
            {
                if (!File.Exists(ClientGuidPath))
                    return string.Empty;

                string value = File.ReadAllText(ClientGuidPath, Encoding.UTF8).Trim();
                return Guid.TryParse(value, out _) ? value : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static void SaveClientGuid(string? guid)
        {
            if (!Guid.TryParse(guid, out _))
                return;

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ClientGuidPath)!);
                File.WriteAllText(ClientGuidPath, guid, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Log.logger.Warn("保存遥测 GUID 失败。", ex);
            }
        }

        private static string GetAppVersion()
        {
            System.Version? version = Assembly.GetEntryAssembly()?.GetName().Version;
            return version?.ToString() ?? "unknown";
        }

        private static HttpClient CreateHttpClient()
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("LLC_MOD_Toolbox");
            return client;
        }
    }
}
