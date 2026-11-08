using Newtonsoft.Json;

namespace LLC_MOD_Toolbox.Models.Configuration;

public sealed class AppSettings
{
    [JsonProperty("general")]
    public GeneralSettings General { get; set; } = new();

    [JsonProperty("install")]
    public InstallationSettings Installation { get; set; } = new();

    [JsonProperty("announcement")]
    public AnnouncementSettings Announcements { get; set; } = new();

    [JsonProperty("mirrorChyan")]
    public MirrorChyanSettings MirrorChyan { get; set; } = new();

    [JsonProperty("nodeSelect")]
    public NodeSettings Nodes { get; set; } = new();

    [JsonProperty("skin")]
    public SkinSettings Skin { get; set; } = new();

    [JsonProperty("gacha")]
    public GachaSettings Gacha { get; set; } = new();

    [JsonProperty("telemetry")]
    public TelemetrySettings Telemetry { get; set; } = new();
}
