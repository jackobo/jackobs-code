using System;
using GamesPortal.Service.Artifactory;
using Spark.Infra.Configurations;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.SignalR;
using GamesPortal.Service.Synchronizers;
using Spark.Infra.Types;
using Spark.Infra.Logging;
using Microsoft.Practices.Unity;
using Spark.Infra.Windows;


namespace GamesPortal.Service
{
    internal class GamesPortalInternalServices : IGamesPortalInternalServices
    {
        IUnityContainer _container;
        public GamesPortalInternalServices(IUnityContainer container)
        {
            _container = container;
            _container.RegisterInstance<IGamesPortalInternalServices>(this);
            _container.RegisterInstance<ITimeServices>(new TimeServices());
            
            _container.RegisterInstance<IConfigurationReader>(new ConfigurationReader());
            _container.RegisterInstance<IFileSystemManager>(new FileSystemManager());
            
            _container.RegisterInstance<IApplicationLifetimeManager>(_container.Resolve<ApplicationLifetimeManager>());
            
            _container.RegisterType<IThreadingServices, ThreadingServices>();
            _container.RegisterType<ICallContextServices, WcfOperationCallContextServices>();

            _container.RegisterType<ISynchronizerStopperFactory, SynchronizerStopperFactory>();

            _container.RegisterInstance<ILanguageDictionary>(new InMemoryLanguageDictionary());

            _container.RegisterInstance<IArtifactorySynchronizationManager>(_container.Resolve<ArtifactorySynchronizationManager>());
            _container.RegisterType<IGamesPortalHubContext, GamesPortalHubContext>();
            _container.RegisterInstance(_container.Resolve<RegulationTypesSynchronizer>());
            _container.RegisterInstance<IGameLanguageToArtifactorySynchronizer>(_container.Resolve<GameLanguageToArtifactorySynchronizer>());
            
            _container.RegisterInstance<IRegulationsDictionary>(new RegulationsDictionary(this));
            _container.RegisterInstance<ITfsGateway>(_container.Resolve<TFSGateway>());
        }



        public ITfsGateway TfsGateway
        {
            get { return _container.Resolve<ITfsGateway>(); }
        }
        public ITimeServices TimeServices
        {
            get { return _container.Resolve<ITimeServices>(); }
        }
        

        public IApplicationLifetimeManager ApplicationLifetimeManager
        {
            get
            {
                return _container.Resolve<IApplicationLifetimeManager>();
            }
        }

        public IRegulationsDictionary RegulationsDictionary
        {
            get { return _container.Resolve<IRegulationsDictionary>(); }
        }

        public IArtifactorySynchronizationManager ArtifactorySynchronizationManager
        {
            get
            {
                return _container.Resolve<IArtifactorySynchronizationManager>();
            }
        }

        public ICallContextServices CallContextServices
        {
            get
            {
                return _container.Resolve<ICallContextServices>();
            }
        }

        public IConfigurationReader ConfigurationReader
        {
            get
            {
                return _container.Resolve<IConfigurationReader>();
            }
        }

        public IFileSystemManager FileSystemManager
        {
            get
            {
                return _container.Resolve<IFileSystemManager>();
            }
        }

        public IGamesPortalHubContext GamesPortalHubContext
        {
            get
            {
                return _container.Resolve<IGamesPortalHubContext>();
            }
        }

        public ILoggerFactory LoggerFactory
        {
            get
            {
                return _container.Resolve<ILoggerFactory>();
            }
        }

        public IArtifactorySynchronizerDataAccessLayer ArtifactorySynchronizerDataAccessLayer
        {
            get
            {
                return new ComponentSynchronizerDataAccessLayer(this);
            }
        }

      

        public IGamesLanguageQAApprovalStatusNormalizer GamesLanguageQAApprovalStatusNormalizer()
        {
            return new GamesLanguageQAApprovalStatusNormalizer(this);
        }

        
        public IGameLanguageToArtifactorySynchronizer GameLanguageToArtifactorySynchronizer
        {
            get { return _container.Resolve<IGameLanguageToArtifactorySynchronizer>(); }
        }

        
        public IThreadingServices ThreadingServices
        {
            get
            {
                return _container.Resolve<IThreadingServices>();
            }
        }


        public ISynchronizerStopperFactory SynchronizerStopperFactory
        {
            get { return _container.Resolve<ISynchronizerStopperFactory>(); }
        }

        public ILanguageDictionary LanguageDictionary
        {
            get
            {
                return _container.Resolve<ILanguageDictionary>();
            }
        }

      

        public GamesPortal.Service.GGPVersioning.IGGPVersioningDBDataContext CreateGGPVersioningDbContext()
        {
            return new GGPVersioning.GGPVersioningDBDataContext(ConfigurationReader.GetConnectionString("GGPVersioningDB"));
        }

        public SDM.ISdmDataContext CreateSdmDbContext()
        {
            return new SDM.SdmDataContext(ConfigurationReader.GetConnectionString("DictionariesDB"));
        }


        public IGamesPortalDataContext CreateGamesPortalDBDataContext()
        {
            return new GamesPortalDataContext(ConfigurationReader.GetConnectionString("GamesPortalDB"));
        }

        public IJackpotInfoDBDataContext CreateJackpotInfoDbContext()
        {
            return new Jackpot.JackpotInfoDBDataContext(ConfigurationReader.GetConnectionString("JackpotInfoDB"));
        }

        public IGameLanguageApprover CreateGameLanguageApprover(IGamesPortalDataContext dbContext)
        {
            return new GameLanguageApprover(dbContext);
        }
    }
}
