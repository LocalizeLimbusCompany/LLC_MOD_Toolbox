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
        private static readonly TimeSpan HeartbeatInterval = TimeSpan.FromMinutes(2);
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

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            EnsureClientGuid();

            while (!cancellationToken.IsCancellationRequested)
            {
                if (_config.Settings.telemetry.lastSubmittedDate == DateTime.Today.ToString("yyyy-MM-dd"))
                    await SubmitHeartbeatAsync(cancellationToken).ConfigureAwait(false);
                else
                    await SubmitDailyAsync(cancellationToken).ConfigureAwait(false);

                await Task.Delay(HeartbeatInterval, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task SubmitDailyAsync(CancellationToken cancellationToken)
        {
            if (_config.Settings.telemetry.lastSubmittedDate == DateTime.Today.ToString("yyyy-MM-dd"))
                return;

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
                using var response = await HttpClient.PostAsync(
                    _nodeService.ResolveApiUrl("v2/telemetering"),
                    content,
                    cancellationToken).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    Log.logger.Warn($"遥测提交失败：HTTP {(int)response.StatusCode}");
                    return;
                }

                _config.Settings.telemetry.telemeteringSubmitted = true;
                _config.Settings.telemetry.lastSubmittedDate = DateTime.Today.ToString("yyyy-MM-dd");
                _config.SaveConfig();
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                Log.logger.Warn("遥测提交失败。", ex);
            }
        }

        private async Task SubmitHeartbeatAsync(CancellationToken cancellationToken)
        {
            var payload = new { guid = _config.Settings.telemetry.clientGuid };

            try
            {
                string json = JsonConvert.SerializeObject(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await HttpClient.PostAsync(
                    _nodeService.ResolveApiUrl("v2/telemetering/heartbeat"),
                    content,
                    cancellationToken).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    Log.logger.Warn($"遥测心跳提交失败：HTTP {(int)response.StatusCode}");
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                Log.logger.Warn("遥测心跳提交失败。", ex);
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
