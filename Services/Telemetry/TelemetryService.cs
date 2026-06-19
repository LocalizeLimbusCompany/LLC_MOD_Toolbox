using LLC_MOD_Toolbox.Services.Configuration;
using LLC_MOD_Toolbox.Services.Network;
using Newtonsoft.Json;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace LLC_MOD_Toolbox.Services.Telemetry
{
    public sealed class TelemetryService : ITelemetryService
    {
        private static readonly HttpClient HttpClient = CreateHttpClient();
        private readonly ConfigurationManager _config;
        private readonly INodeService _nodeService;

        public TelemetryService(ConfigurationManager config, INodeService nodeService)
        {
            _config = config;
            _nodeService = nodeService;
        }

        public async Task SubmitOnceAsync()
        {
            if (_config.Settings.telemetry.telemeteringSubmitted)
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
                return;

            _config.Settings.telemetry.clientGuid = Guid.NewGuid().ToString();
            _config.SaveConfig();
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
