using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using Spark.Infra.Configurations;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Services.WCF;
using Spark.Infra.Logging;
using Microsoft.Owin.Hosting;
using Microsoft.Practices.Unity;
using GamesPortal.Service.Configurations;

namespace GamesPortal.Service
{
    public class ServiceFramework
    {
        

        private ServiceFramework(IUnityContainer container)
        {
            this.Container = container;
            
            this.Services = new GamesPortalInternalServices(container);
        }

        private ILogger Logger
        {
            get { return Container.Resolve<ILoggerFactory>().CreateLogger(this.GetType()); }
        }


        IGamesPortalInternalServices Services { get; set; }

        private void CreateWcfServiceHosts()
        {
            _wcfServices.Add(new DependencyInjectionServiceHost<GamesPortalService>(this.Container));
            _wcfServices.Add(new DependencyInjectionServiceHost<GamesPortalApprovalService>(this.Container));
            _wcfServices.Add(new DependencyInjectionServiceHost<LayoutToolService>(this.Container));
            _wcfServices.Add(new DependencyInjectionServiceHost<LayoutToolPublisher>(this.Container));
            _wcfServices.Add(new DependencyInjectionServiceHost<GamesPortalToBuildMachineAdapter>(this.Container));

            _wcfServices.Add(new ServiceHost(typeof(SignalR.GamesPortalHubContext)));
            
        }




        IUnityContainer Container { get; set; }
     

        public static ServiceFramework Instance
        {
            get;
            private set;
        }


        List<ServiceHost> _wcfServices = new List<ServiceHost>();



        public static void Start(IUnityContainer container)
        {

            Instance = new ServiceFramework(container);

            Task.Factory.StartNew(Instance.AsyncStart);
        }


        private void UpgradeDatabase()
        {
            var dbUpgrader = new GamesPortalDatabaseUpgrader();
            var retryInterval = TimeSpan.FromSeconds(5);
            dbUpgrader.UpgradeWithRetry(Services.CreateGamesPortalDBDataContext(), 
                                        retryInterval, 
                                        () => AppLifetimeManager.IsShuttingDown, 
                                        (exeptionToLog) => Logger.Exception("Failed to run database upgrade scripts! I will retry every " + retryInterval.ToString(), exeptionToLog));
        }

        
        public static void Stop()
        {
            if (Instance != null)
            {
                Instance.StopService();
            }   
           
        }
        
        private ApplicationLifetimeManager AppLifetimeManager
        {
            get { return Services.ApplicationLifetimeManager as ApplicationLifetimeManager; }
        }

        private void StopService()
        {
            AppLifetimeManager.ShuttingDown();
            try
            {
                StopWcfSerices();
                Services.ArtifactorySynchronizationManager?.StopSynchronization();
                StopSignalRServer();
            }
            catch (Exception ex)
            {
                Logger.Exception("GamesPortalService.OnStop failed!", ex);
            }
        }

        
        private void StopWcfSerices()
        {
            foreach (var svcHost in _wcfServices)
                svcHost.Close();
        }

        private void StartWcfServices()
        {
            foreach (var svcHost in _wcfServices)
                svcHost.Open();
        }

        private void AsyncStart()
        {
            try
            {
                UpgradeDatabase();

                CreateWcfServiceHosts();

                StartWcfServices();

                Services.ArtifactorySynchronizationManager.StartSynchronization();

                StartSignalRServer();

                StartRegulationTypesSynchronizer();

            }
            catch (Exception ex)
            {
                Logger.Exception("GamesPortalService.OnStart failed!", ex);
                throw;
            }
        }

        private void StartRegulationTypesSynchronizer()
        {
            Container.Resolve<Synchronizers.RegulationTypesSynchronizer>().Start();
        }

        private IDisposable SignalrRApp { get; set; }
        private void StartSignalRServer()
        {
            SignalrRApp = WebApp.Start<SignalR.SignalRStartup>(Services.ConfigurationReader.ReadSection<SignalRSettings>().Url);
        }

        private void StopSignalRServer()
        {
            if (SignalrRApp != null)
                SignalrRApp.Dispose();
        }


    }
}
