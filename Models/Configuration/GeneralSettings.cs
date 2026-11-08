using Newtonsoft.Json;

namespace LLC_MOD_Toolbox.Models.Configuration;

public sealed class GeneralSettings
{
    [JsonProperty("skipLCBPathCheck")]
    public bool SkipGamePathConfirmation { get; set; } = false;

    [JsonProperty("LCBPath")]
    public string? GameDirectory { get; set; } = null;

    [JsonProperty("internationalMode")]
    public bool InternationalMode { get; set; } = false;
}
