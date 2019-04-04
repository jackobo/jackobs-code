using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using Spark.Infra.Configurations;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Entities;
using GamesPortal.Service.GGPVersioning;
using GamesPortal.Service.SDM;
using GamesPortal.Service.SignalR;
using GamesPortal.Service.Synchronizers;
using Spark.Infra.Types;
using Spark.Infra.Data.LinqToSql;
using Spark.Infra.Logging;
using NSubstitute;
using Spark.Infra.Windows;


namespace GamesPortal.Service.Helpers
{
    public class GamesPortalInternalServicesHelper : IGamesPortalInternalServices
    {
        private GamesPortalInternalServicesHelper()
        {
            this.TimeServices = Substitute.For<ITimeServices>();
            this.TimeServices.Now.Returns(DateTime.Now);

            this.ArtifactorySynchronizationManager = Substitute.For<IArtifactorySynchronizationManager>();
            this.ArtifactorySynchronizerDataAccessLayer = Substitute.For<IArtifactorySynchronizerDataAccessLayer>();
            
            this.CallContextServices = Substitute.For<ICallContextServices>();
            this.ConfigurationReader = Substitute.For<IConfigurationReader>();
            this.FileSystemManager = Substitute.For<IFileSystemManager>();
            this.GamesPortalHubContext = Substitute.For<IGamesPortalHubContext>();
            this.LoggerFactory = Substitute.For<ILoggerFactory>();

            this.SynchronizerStopperFactory = Substitute.For<ISynchronizerStopperFactory>();

            this.RegulationsDictionary = Substitute.For<IRegulationsDictionary>();

            this.GameLanguageToArtifactorySynchronizer = Substitute.For<IGameLanguageToArtifactorySynchronizer>();
            

            this.ThreadingServices = Substitute.For<IThreadingServices>();
            this.ApplicationLifetimeManager = Substitute.For<IApplicationLifetimeManager>();
            this.LanguageDictionary = Substitute.For<ILanguageDictionary>();
            this.LanguageDictionary.FindLanguage(Arg.Any<string>())
                                   .Returns(args => Language.Find((string)args[0]));
            
            this.LanguageDictionary.TryFindLanguage(Arg.Any<string>())
                                   .Returns(args => Language.TryFind((string)args[0]));

            this.TfsGateway = Substitute.For<ITfsGateway>();

            _createSdmDbContext = CreateDataContextSubstitute<ISdmDataContext>();
            _createJackpotInfoDbContext = CreateDataContextSubstitute<IJackpotInfoDBDataContext>();
            _createGGPVersioningDbContext = CreateDataContextSubstitute<IGGPVersioningDBDataContext>();
            _createGamesPortalDBDataContext = CreateDataContextSubstitute<IGamesPortalDataContext>();

            _languageQAApprovalStatusNormalizer = Substitute.For<IGamesLanguageQAApprovalStatusNormalizer>();
            
            
            _gameLanguageApprover = Substitute.For<IGameLanguageApprover>();
        }



        private TDataContext CreateDataContextSubstitute<TDataContext>()
            where TDataContext : class, IDbDataContext
        {
            var dbContext = Substitute.For<TDataContext>();

            dbContext.GetChanges().Returns(ChangeSet<object>.Empty());

            return dbContext;
        }

        public static IGamesPortalInternalServices Create()
        {
            return new GamesPortalInternalServicesHelper();
        }


        public ITimeServices TimeServices { get; private set; }
        

        public ILanguageDictionary LanguageDictionary { get; private set; }


        IGamesPortalDataContext _createGamesPortalDBDataContext;
        public IGamesPortalDataContext CreateGamesPortalDBDataContext()
        {
            return _createGamesPortalDBDataContext;
        }

        IGGPVersioningDBDataContext _createGGPVersioningDbContext;
        public IGGPVersioningDBDataContext CreateGGPVersioningDbContext()
        {
            return _createGGPVersioningDbContext;
        }

        ISdmDataContext _createSdmDbContext;
        public ISdmDataContext CreateSdmDbContext()
        {
            return _createSdmDbContext;
        }

        public ISynchronizerStopperFactory SynchronizerStopperFactory { get; private set; }

        IJackpotInfoDBDataContext _createJackpotInfoDbContext;
        public IJackpotInfoDBDataContext CreateJackpotInfoDbContext()
        {
            return _createJackpotInfoDbContext;
        }

        IGamesLanguageQAApprovalStatusNormalizer _languageQAApprovalStatusNormalizer;
        public IGamesLanguageQAApprovalStatusNormalizer GamesLanguageQAApprovalStatusNormalizer()
        {
            return _languageQAApprovalStatusNormalizer;
        }

        
        IGameLanguageApprover _gameLanguageApprover;
        public IGameLanguageApprover CreateGameLanguageApprover(IGamesPortalDataContext dbContext)
        {
            return _gameLanguageApprover;
        }

        public IArtifactorySynchronizationManager ArtifactorySynchronizationManager { get; private set; }
        public IArtifactorySynchronizerDataAccessLayer ArtifactorySynchronizerDataAccessLayer { get; private set; }

        
        public IGameLanguageToArtifactorySynchronizer GameLanguageToArtifactorySynchronizer { get; private set; }
        public ICallContextServices CallContextServices { get; private set; }
        public IConfigurationReader ConfigurationReader { get; private set; }
        
        public IFileSystemManager FileSystemManager { get; private set; }
        public IGamesPortalHubContext GamesPortalHubContext { get; private set; }
        public ILoggerFactory LoggerFactory { get; private set; }

        public IThreadingServices ThreadingServices { get; private set; }

        public IApplicationLifetimeManager ApplicationLifetimeManager { get; private set; }

        public IRegulationsDictionary RegulationsDictionary { get; private set; }

        public ITfsGateway TfsGateway { get; private set; }
    }
}
