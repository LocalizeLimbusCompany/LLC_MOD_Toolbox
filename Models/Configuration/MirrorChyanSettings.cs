using Newtonsoft.Json;

namespace LLC_MOD_Toolbox.Models.Configuration;

public sealed class MirrorChyanSettings
{
    [JsonProperty("enable")]
    public bool IsEnabled { get; set; } = false;

    [JsonProperty("notice")]
    public bool HasShownNotice { get; set; } = false;
}
