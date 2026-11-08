using System.Diagnostics;

namespace LLC_MOD_Toolbox.Infrastructure
{
    internal static class StartupDiagnostics
    {
        private static readonly Stopwatch StartupClock = new();

        public static void Start() => StartupClock.Restart();

        public static void Mark(string milestone) =>
            Log.logger.Info($"[Startup] {milestone}: {StartupClock.ElapsedMilliseconds}ms since application startup");

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
