using System.Reflection;

namespace LLC_MOD_Toolbox.Models
{
    public sealed class ApplicationState
    {
        public string GameDirectory { get; set; } = string.Empty;
        public string GameExecutablePath { get; set; } = string.Empty;
        public string BaseDirectory { get; } = AppDomain.CurrentDomain.BaseDirectory;
        public bool IsLauncherMode { get; } = Environment.GetCommandLineArgs().Contains("-launcher");
        public string ToolboxVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
        public bool IsGreytestMode { get; set; }
        public string GreytestDownloadUrl { get; set; } = string.Empty;
        public bool IsMirrorChyanMode { get; set; }
        public string MirrorChyanToken { get; set; } = string.Empty;
    }
}
