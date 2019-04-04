using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Logging;
using Spark.Infra.Logging;

namespace Spark.Wpf.Common
{
    internal class LoggerFacade : ILoggerFacade
    {

        public LoggerFacade(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger(typeof(LoggerFacade));
        }

        private ILogger Logger { get; set; }

        
        public void Log(string message, Category category, Priority priority)
        {
            switch (category)
            {
                case Category.Exception:
                    Logger.Error(message);
                    break;
                case Category.Warn:
                    Logger.Warning(message);
                    break;
                case Category.Info:
                case Category.Debug:
                    Logger.Info(message);
                    break;
                default:
                    Logger.Error(string.Format("Unknown category {0}! Message to log: {1}", category.ToString(), message));
                    break;


            }
        }
    }

}
