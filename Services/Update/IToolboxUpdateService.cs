namespace LLC_MOD_Toolbox.Services.Update
{
    public interface IToolboxUpdateService
    {
        Task<ToolboxUpdateCheckResult> CheckForUpdateAsync();
        Task<bool> PerformUpdateAsync(ToolboxUpdateCheckResult result, IProgress<float>? progress = null);
    }

    public sealed class ToolboxUpdateCheckResult
    {
        public bool HasUpdate { get; init; }
        public bool IsMirrorChyan { get; init; }
        public string LatestVersion { get; init; } = string.Empty;
        public Exception? Error { get; init; }
    }
}
