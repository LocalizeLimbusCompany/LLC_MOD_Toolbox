using Newtonsoft.Json;

namespace LLC_MOD_Toolbox.Models.Configuration;

public sealed class GachaSettings
{
    [JsonProperty("calciteEasterEggSeen")]
    public bool HasSeenCalciteEasterEgg { get; set; } = false;
}
