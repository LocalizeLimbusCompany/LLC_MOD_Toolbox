// Only the UI/logging boundary is replaced; network, retries, configuration and
// loading-text cache behavior are compiled directly from the production files.
namespace LLC_MOD_Toolbox
{
    internal static class Log
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("StartupChecks");
    }
}

namespace LLC_MOD_Toolbox.Services.UI
{
    public interface IDialogService
    {
        void ShowMessage(string message, string title = "提示");
    }
}
