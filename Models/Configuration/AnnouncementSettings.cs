using Newtonsoft.Json;

namespace LLC_MOD_Toolbox.Models.Configuration;

public sealed class AnnouncementSettings
{
    [JsonProperty("getAnno")]
    public bool IsEnabled { get; set; } = true;

    [JsonProperty("annoVersion")]
    public int LastReadVersion { get; set; } = 0;
}
