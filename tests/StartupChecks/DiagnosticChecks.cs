using LLC_MOD_Toolbox.Infrastructure;

internal static class DiagnosticChecks
{
    public static IEnumerable<(string Name, Func<Task> Run)> All =>
    [
        ("Fatal diagnostics preserve the startup stage and nested exception", () => WithDirectory(directory =>
        {
            StartupDiagnostics.Start();
            StartupDiagnostics.Mark("Services configured");
            Exception error;
            try
            {
                throw new FileNotFoundException("Missing startup dependency", "missing.dll");
            }
            catch (Exception cause)
            {
                error = new InvalidOperationException("Window construction failed", cause);
            }

            string? path = StartupDiagnostics.WriteFailureReport(error, directory);
            Check(path is not null && File.Exists(path), "Fatal error report was not saved.");
            string report = File.ReadAllText(path!);
            Check(report.Contains("Services configured") && report.Contains("Window construction failed")
                && report.Contains("missing.dll") && report.Contains(nameof(FileNotFoundException))
                && report.Contains(nameof(DiagnosticChecks)), "Report lost the original exception, stack or stage.");
        })),
        ("Fatal diagnostics fall back when the application log directory is unavailable", () => WithDirectory(directory =>
        {
            string blocked = Path.Combine(directory, "blocked");
            File.WriteAllText(blocked, "This file prevents directory creation.");
            string fallback = Path.Combine(directory, "fallback");
            string? path = StartupDiagnostics.WriteFailureReport(new Exception("startup failed"), blocked, fallback);
            Check(path == Path.Combine(fallback, "Startup-error.log") && File.Exists(path), "Fallback report was not saved.");
            Check(File.ReadAllText(blocked) == "This file prevents directory creation.", "Existing file was changed.");
            Check(StartupDiagnostics.WriteFailureReport(new Exception("startup failed"), blocked) is null,
                "An unavailable log path must not cause another fatal error.");
        }))
    ];

    private static Task WithDirectory(Action<string> check)
    {
        string directory = Path.Combine(Path.GetTempPath(), "llc-diagnostic-checks-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try { check(directory); }
        finally { Directory.Delete(directory, recursive: true); }
        return Task.CompletedTask;
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
