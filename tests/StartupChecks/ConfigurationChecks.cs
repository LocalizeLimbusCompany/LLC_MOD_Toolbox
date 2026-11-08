using LLC_MOD_Toolbox.Services.Configuration;
using Newtonsoft.Json.Linq;

internal static class ConfigurationChecks
{
    public static IEnumerable<(string Name, Func<Task> Run)> All =>
    [
        ("Legacy configuration keys retain their values after renaming", () => WithConfigAsync("""
            {
              "general": { "skipLCBPathCheck": true, "LCBPath": "C:/Games/Limbus", "internationalMode": true },
              "install": { "installWhenLaunch": true, "afterInstallClose": true },
              "announcement": { "getAnno": false, "annoVersion": 42 },
              "mirrorChyan": { "enable": true, "notice": true },
              "nodeSelect": { "defaultNode": "download", "defaultApiNode": "api" },
              "skin": { "currentSkin": "kaltsit" },
              "gacha": { "calciteEasterEggSeen": true },
              "telemetry": { "acceptedAgreement": true, "clientGuid": "legacy-client", "telemeteringSubmitted": true, "lastSubmittedDate": "2026-11-08" }
            }
            """, (service, path) =>
        {
            var settings = service.Settings;
            Check(settings.General.SkipGamePathConfirmation && settings.General.GameDirectory == "C:/Games/Limbus"
                && settings.General.InternationalMode, "General settings changed.");
            Check(settings.Installation.InstallOnLaunch && settings.Installation.CloseAfterInstallation, "Installation settings changed.");
            Check(!settings.Announcements.IsEnabled && settings.Announcements.LastReadVersion == 42, "Announcement settings changed.");
            Check(settings.MirrorChyan.IsEnabled && settings.MirrorChyan.HasShownNotice, "MirrorChyan settings changed.");
            Check(settings.Nodes.DownloadNode == "download" && settings.Nodes.ApiNode == "api", "Node settings changed.");
            Check(settings.Skin.CurrentSkin == "kaltsit" && settings.Gacha.HasSeenCalciteEasterEgg, "Presentation settings changed.");
            Check(settings.Telemetry.HasAcceptedAgreement && settings.Telemetry.ClientId == "legacy-client"
                && settings.Telemetry.HasSubmittedTelemetry && settings.Telemetry.LastSubmittedDate == "2026-11-08", "Telemetry settings changed.");
            var original = JObject.Parse(File.ReadAllText(path));
            service.Save();
            Check(JToken.DeepEquals(original, JObject.Parse(File.ReadAllText(path))), "Saved JSON must keep the legacy schema and values.");
            return Task.CompletedTask;
        })),
        ("Missing and null configuration members retain initialized defaults", () => WithConfigAsync(
            """{"general":null,"announcement":{},"skin":{"currentSkin":null},"nodeSelect":{}}""", (service, _) =>
        {
            var settings = service.Settings;
            Check(settings.General is not null && !settings.General.SkipGamePathConfirmation, "Missing general defaults.");
            Check(settings.Announcements.IsEnabled, "Missing getAnno must retain its true default.");
            Check(settings.Skin.CurrentSkin == "default", "Missing skin must retain the default skin name.");
            Check(settings.Nodes.DownloadNode == "" && settings.Nodes.ApiNode == "", "Missing node defaults.");
            Check(settings.Installation is not null && settings.Telemetry is not null, "Missing sections were not initialized.");
            return Task.CompletedTask;
        })),
        ("Comments and unknown members do not become configuration properties", () => WithConfigAsync(
            """{"general":{"_LCBPath":"description","LCBPath":"D:/Game"},"futureOption":{"value":true}}""", (service, path) =>
        {
            Check(service.Settings.General.GameDirectory == "D:/Game", "Comment affected the configured path.");
            service.Save();
            var saved = JObject.Parse(File.ReadAllText(path));
            Check(saved["futureOption"] is null && saved["general"]!["_LCBPath"] is null, "Unknown fields were serialized.");
            return Task.CompletedTask;
        })),
        ("Concurrent saves leave valid JSON and no temporary files", () => WithConfigAsync("{}", async (service, path) =>
        {
            service.Settings.Skin.CurrentSkin = "season7";
            await Task.WhenAll(Enumerable.Range(0, 50).Select(_ => Task.Run(service.Save)));
            var reloaded = new ConfigurationService(path);
            Check(reloaded.Settings.Skin.CurrentSkin == "season7", "Saved configuration is incomplete.");
            Check(Directory.GetFiles(Path.GetDirectoryName(path)!, "*.tmp").Length == 0, "Temporary save files leaked.");
        })),
        ("Missing configuration creates defaults and malformed input is preserved", async () =>
        {
            string directory = Path.Combine(Path.GetTempPath(), "llc-config-checks-" + Guid.NewGuid().ToString("N"));
            string path = Path.Combine(directory, "nested", "config.json");
            try
            {
                var service = new ConfigurationService(path);
                Check(File.Exists(path) && service.Settings.Announcements.IsEnabled, "Defaults were not saved.");
                await File.WriteAllTextAsync(path, "broken JSON");
                try
                {
                    _ = new ConfigurationService(path);
                    throw new Exception("Malformed input should report an error.");
                }
                catch (InvalidOperationException)
                {
                    Check(await File.ReadAllTextAsync(path) == "broken JSON", "Malformed configuration was overwritten.");
                }
            }
            finally
            {
                if (Directory.Exists(directory)) Directory.Delete(directory, recursive: true);
            }
        })
    ];

    private static async Task WithConfigAsync(string json, Func<ConfigurationService, string, Task> check)
    {
        string directory = Path.Combine(Path.GetTempPath(), "llc-config-checks-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string path = Path.Combine(directory, "config.json");
        try
        {
            await File.WriteAllTextAsync(path, json);
            await check(new ConfigurationService(path), path);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
