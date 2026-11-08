namespace LLC_MOD_Toolbox.Services.Update
{
    public interface IToolboxUpdateService
    {
        Task<ToolboxUpdateCheckResult> CheckForUpdateAsync();
        Task<bool> PerformUpdateAsync(ToolboxUpdateCheckResult result, IProgress<float>? progress = null);
    }
}
