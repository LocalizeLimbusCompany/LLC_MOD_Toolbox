using LLC_MOD_Toolbox.Infrastructure;
using System.Reflection;

namespace LLC_MOD_Toolbox.Services
{
    public sealed class AppState : ObservableObject
    {
        public string LimbusCompanyDir { get; set; } = string.Empty;
        public string LimbusCompanyGameDir { get; set; } = string.Empty;
        public string CurrentDir { get; } = AppDomain.CurrentDomain.BaseDirectory;
        public bool IsLauncherMode { get; } = Environment.GetCommandLineArgs().Contains("-launcher");
        public string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
        public bool GreytestStatus { get; set; }
        public string GreytestUrl { get; set; } = string.Empty;
        public bool IsMirrorChyanMode { get; set; }
        public string MirrorChyanToken { get; set; } = string.Empty;
    }
}
