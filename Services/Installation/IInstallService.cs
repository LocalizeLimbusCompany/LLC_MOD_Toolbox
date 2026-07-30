namespace LLC_MOD_Toolbox.Services.Installation
{
    public interface IInstallService
    {
        Task<InstallResult> InstallAsync(IProgress<InstallProgress> progress, CancellationToken ct = default);
        Task StopInstallAsync();
    }

    public enum InstallResult
    {
        Succeeded,
        Aborted
    }

    public record InstallProgress(float Percentage, string? StatusMessage = null);
}
