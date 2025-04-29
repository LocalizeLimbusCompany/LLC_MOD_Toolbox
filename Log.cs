using log4net;
using System.Reflection;

namespace LLC_MOD_Toolbox
{
    public static class Log
    {
        internal static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(MainWindow));
    }
}
