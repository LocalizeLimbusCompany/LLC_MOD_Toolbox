namespace LLC_MOD_Toolbox.Services.Telemetry
{
    public interface ITelemetryService
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
