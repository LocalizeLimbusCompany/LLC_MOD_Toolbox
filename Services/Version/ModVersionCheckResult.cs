namespace LLC_MOD_Toolbox.Services.Version
{
    public record ModVersionCheckResult(int CurrentVersion, int LatestVersion, bool NeedsUpdate, string CurrentText, string LatestText);
}
