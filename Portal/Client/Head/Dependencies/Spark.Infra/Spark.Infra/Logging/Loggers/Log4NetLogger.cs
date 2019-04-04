using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;

namespace Spark.Infra.Logging
{
    internal class Log4NetNotifier : ILogger
    {

        ILog Log { get; set; }
        public Log4NetNotifier(Type type)
        {
            this.Log = LogManager.GetLogger(type);
        }

      
        public void Exception(string message, Exception exeption)
        {
            Log.Error(FormatMessage(message), exeption);
        }

        private static string FormatMessage(string message)
        {
            //return string.Format("{1}{0}{0}Machine: {2}{0}OS Version: {3}", Environment.NewLine, message, Environment.MachineName, Environment.OSVersion.ToString());
            return message;
        }

        public void Exception(Exception ex)
        {
            Log.Error(FormatMessage(ex.ToString()));
        }

        public void Warning(string message)
        {
            Log.Warn(FormatMessage(message));
        }

        public void Info(string message)
        {
            Log.Info(FormatMessage(message));
        }

        public void Error(string message)
        {
            Log.Error(message);
        }

    }
}
