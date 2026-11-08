using Newtonsoft.Json;

namespace LLC_MOD_Toolbox.Models.Configuration;

public sealed class SkinSettings
{
    [JsonProperty("currentSkin")]
    public string CurrentSkin { get; set; } = "default";
}
