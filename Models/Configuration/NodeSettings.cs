using Newtonsoft.Json;

namespace LLC_MOD_Toolbox.Models.Configuration;

public sealed class NodeSettings
{
    [JsonProperty("defaultNode")]
    public string DownloadNode { get; set; } = string.Empty;

    [JsonProperty("defaultApiNode")]
    public string ApiNode { get; set; } = string.Empty;
}
