using System;
using System.IO;
namespace LLC_MOD_Toolbox
{   
    internal class logger
    {
        private string logFilePath;
        public logger(string logFilePath)
        {
            this.logFilePath = logFilePath;
        }

        public void Log(string message)
        {
            string logMessage = $"[{DateTime.Now.ToString()}] {message}{Environment.NewLine}";
            File.AppendAllText(logFilePath, logMessage);
            Console.WriteLine(logMessage);
        }
    }
}
