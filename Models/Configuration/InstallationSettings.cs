using Newtonsoft.Json;

namespace LLC_MOD_Toolbox.Models.Configuration;

public sealed class InstallationSettings
{
    [JsonProperty("installWhenLaunch")]
    public bool InstallOnLaunch { get; set; } = false;

    [JsonProperty("afterInstallClose")]
    public bool CloseAfterInstallation { get; set; } = false;
}
