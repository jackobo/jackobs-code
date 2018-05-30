using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using Spark.Infra.Configurations;
using GamesPortal.Service.Services.WCF;
using Spark.Infra.Logging;
using Microsoft.Practices.Unity;

namespace GamesPortal.Service
{
    public partial class GamesPortalServiceHost : ServiceBase
    {
        private static ILogger Logger { get; set; }
        private static ILoggerFactory LoggerFactory { get; set; }

        public GamesPortalServiceHost(ILoggerFactory loggerFactory)
        {
            InitializeComponent();
        }


        private static void InitLogging()
        {
            LoggerFactory = Log4NetNotifierFactory.FromConfigurationFileInApplicationFolder();
            Logger = LoggerFactory.CreateLogger(typeof(GamesPortalServiceHost));
        }

        protected override void OnStart(string[] args)
        {
            var container = new UnityContainer();
            container.RegisterInstance(LoggerFactory);
            ServiceFramework.Start(container);
        }

        protected override void OnStop()
        {
            ServiceFramework.Stop();
        }

        internal static void ServiceMain(string[] args)
        {
            try
            {
                InitLogging();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed to initialize logging system! Exception details: {ex.ToString()}");
                Environment.Exit(ex.HResult);
                return;
            }

            try
            {
                using (var service = new GamesPortalServiceHost(LoggerFactory))
                {
                    if (Environment.UserInteractive)
                    {
                        service.OnStart(args);

                        Console.WriteLine("Press enter to stop the service...");
                        Console.Read();

                        service.OnStop();
                    }
                    else
                    {
                        ServiceBase.Run(service);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
    }
}
