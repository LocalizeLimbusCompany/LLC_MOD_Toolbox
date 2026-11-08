namespace LLC_MOD_Toolbox.Services.Version
{
    public interface IModVersionService
    {
        Task<ModVersionCheckResult> CheckVersionsAsync();
    }
}
