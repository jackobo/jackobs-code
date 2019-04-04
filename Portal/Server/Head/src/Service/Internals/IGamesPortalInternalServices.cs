using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using Spark.Infra.Configurations;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.SignalR;
using GamesPortal.Service.Synchronizers;
using Spark.Infra.Types;
using Spark.Infra.Logging;
using Spark.Infra.Windows;


namespace GamesPortal.Service
{
    public interface IGamesPortalInternalServices
    {
        ITimeServices TimeServices { get; }

        IApplicationLifetimeManager ApplicationLifetimeManager { get; }

        IConfigurationReader ConfigurationReader { get; }
        ILoggerFactory LoggerFactory { get; }

        IFileSystemManager FileSystemManager { get; }

        IArtifactorySynchronizationManager ArtifactorySynchronizationManager { get; }

        ICallContextServices CallContextServices { get; }

        IGamesPortalHubContext GamesPortalHubContext { get; }
        
        IGameLanguageToArtifactorySynchronizer GameLanguageToArtifactorySynchronizer { get; }
        
        ILanguageDictionary LanguageDictionary { get; }

        IArtifactorySynchronizerDataAccessLayer ArtifactorySynchronizerDataAccessLayer { get; }

        IThreadingServices ThreadingServices { get; }

        ISynchronizerStopperFactory SynchronizerStopperFactory { get; }

        IGamesLanguageQAApprovalStatusNormalizer GamesLanguageQAApprovalStatusNormalizer();
        
        IRegulationsDictionary RegulationsDictionary { get; }

        IGamesPortalDataContext CreateGamesPortalDBDataContext();
        GGPVersioning.IGGPVersioningDBDataContext CreateGGPVersioningDbContext();
        SDM.ISdmDataContext CreateSdmDbContext();
        IJackpotInfoDBDataContext CreateJackpotInfoDbContext();
        IGameLanguageApprover CreateGameLanguageApprover(IGamesPortalDataContext dbContext);

        ITfsGateway TfsGateway { get; }






    }
}
