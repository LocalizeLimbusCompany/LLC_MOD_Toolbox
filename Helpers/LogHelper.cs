using System.IO;
using log4net;
using log4net.Config;

namespace LLC_MOD_Toolbox.Helpers
{
    /// <summary>
    /// log4net日志专用
    /// </summary>
    public class LogHelper
    {
        private static readonly ILog Instance = LogManager.GetLogger("LogHelper");

        public static void SetConfig()
        {
            XmlConfigurator.Configure();
        }

        public static void SetConfig(string filePath)
        {
            FileInfo configFile = new(filePath);
            XmlConfigurator.Configure(configFile);
        }

        public static void SetConfig(FileInfo configFile)
        {
            XmlConfigurator.Configure(configFile);
        }

        /// <summary>
        /// 记录普通文件记录
        /// </summary>
        /// <param name="info"></param>
        public static void Info(string info)
        {
            if (Instance.IsInfoEnabled)
            {
                Instance.Info(info);
            }
        }

        /// <summary>
        ///记录调试信息
        /// </summary>
        /// <param name="info"></param>
        public static void Debug(string info)
        {
            if (Instance.IsErrorEnabled)
            {
                Instance.Debug(info);
            }
        }

        /// <summary>
        ///记录警告信息
        /// </summary>
        /// <param name="info"></param>
        public static void Warn(string info)
        {
            if (Instance.IsWarnEnabled)
            {
                Instance.Warn(info);
            }
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="info"></param>
        /// <param name="se"></param>
        public static void Error(string info, Exception se)
        {
            if (Instance.IsErrorEnabled)
            {
                Instance.Error(info, se);
            }
        }

        /// <summary>
        /// 记录严重错误
        /// </summary>
        /// <param name="info"></param>
        /// <param name="se"></param>
        public static void Fatal(string info, Exception se)
        {
            if (Instance.IsFatalEnabled)
            {
                Instance.Fatal(info, se);
            }
        }
    }

}
