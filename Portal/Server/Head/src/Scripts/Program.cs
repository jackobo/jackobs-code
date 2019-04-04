using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Configurations;
using Spark.Infra.Logging;

namespace GamesPortal.Service
{
    class Program
    {
        static IConfigurationReader ConfigurationReader { get; set; }
        static ILoggerFactory LoggerFactory { get; set; }
        static ILogger Logger
        {
            get { return LoggerFactory.CreateLogger(typeof(Program)); }
        }
        static void Main(string[] args)
        {
            LoggerFactory = Log4NetNotifierFactory.FromConfigurationFileInApplicationFolder();

            try
            {
                ConfigurationReader = new ConfigurationReader();
                
                var generator = new ApprovalStatusGenerator(ConfigurationReader, LoggerFactory);
                generator.Generate();
            }
            catch(Exception ex)
            {
                Logger.Exception(ex);
                Environment.Exit(1);
            }

        }
    }
}
