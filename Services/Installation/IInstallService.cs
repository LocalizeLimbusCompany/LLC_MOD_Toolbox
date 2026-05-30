using LLC_MOD_Toolbox.Models;

namespace LLC_MOD_Toolbox.Services.Installation
{
    public interface IInstallService
    {
        Task InstallAsync(IProgress<InstallProgress> progress, CancellationToken ct = default);
        Task StopInstallAsync();
    }

    public record InstallProgress(int Phase, float Percentage, string? StatusMessage = null);
}
