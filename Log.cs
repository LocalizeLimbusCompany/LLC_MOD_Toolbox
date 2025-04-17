using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LLC_MOD_Toolbox
{
    public static class Log
    {
        internal static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(MainWindow));
    }
}
