using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LLC_MOD_Toolbox.Infrastructure
{
    internal static class StartupDiagnostics
    {
        private static readonly Stopwatch StartupClock = new();
        private static string _lastMilestone = "Application startup";

        public static void Start()
        {
            _lastMilestone = "Application startup";
            StartupClock.Restart();
        }

        public static void Mark(string milestone)
        {
            _lastMilestone = milestone;
            Log.logger.Info($"[Startup] {milestone}: {StartupClock.ElapsedMilliseconds}ms since application startup");
        }

        public static string? WriteFailureReport(Exception exception)
        {
            // Keep a diagnostic even if log4net configuration or the application directory is unavailable.
            try { LogFailure(exception); }
            catch { }

            string localData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return WriteFailureReport(exception,
                Path.Combine(AppContext.BaseDirectory, "Logs"),
                string.IsNullOrWhiteSpace(localData) ? Path.GetTempPath() : Path.Combine(localData, "LLC_MOD_Toolbox", "Logs"));
        }

        // Resolve log4net inside the guarded call, so a missing logging dependency cannot
        // prevent the independent report from being written during JIT compilation.
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void LogFailure(Exception exception) => Log.logger.Error("工具箱发生致命错误。", exception);

        internal static string? WriteFailureReport(Exception exception, params string[] directories)
        {
            string report = $"Time: {DateTimeOffset.Now:O}\n" +
                $"Last milestone: {_lastMilestone}\n" +
                $"Elapsed: {StartupClock.ElapsedMilliseconds}ms\n" +
                $"Runtime: {RuntimeInformation.FrameworkDescription}\n" +
                $"OS: {RuntimeInformation.OSDescription}\n" +
                $"Architecture: {RuntimeInformation.ProcessArchitecture}\n" +
                $"Application directory: {AppContext.BaseDirectory}\n" +
                $"Working directory: {Environment.CurrentDirectory}\n\n{exception}\n";
            foreach (string directory in directories)
            {
                try
                {
                    Directory.CreateDirectory(directory);
                    string path = Path.Combine(directory, "Startup-error.log");
                    File.WriteAllText(path, report);
                    return path;
                }
                catch (Exception writeError) when (writeError is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
                {
                    // Try the next writable location without hiding the original exception.
                }
            }
            return null;
        }

        public static IDisposable Measure(string stage) => new StageTiming(stage);

        public static async Task<T> MeasureAsync<T>(string stage, Func<Task<T>> action)
        {
            using var timing = Measure(stage);
            return await action();
        }

        private sealed class StageTiming(string stage) : IDisposable
        {
            private readonly Stopwatch _clock = Stopwatch.StartNew();

            public void Dispose() => Log.logger.Info($"[Startup] {stage}: {_clock.ElapsedMilliseconds}ms");
        }
    }
}
