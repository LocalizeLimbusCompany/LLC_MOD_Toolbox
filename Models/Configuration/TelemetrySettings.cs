using Newtonsoft.Json;

namespace LLC_MOD_Toolbox.Models.Configuration;

public sealed class TelemetrySettings
{
    [JsonProperty("acceptedAgreement")]
    public bool HasAcceptedAgreement { get; set; } = false;

    [JsonProperty("clientGuid")]
    public string? ClientId { get; set; } = null;

    [JsonProperty("telemeteringSubmitted")]
    public bool HasSubmittedTelemetry { get; set; } = false;

    [JsonProperty("lastSubmittedDate")]
    public string? LastSubmittedDate { get; set; } = null;
}
