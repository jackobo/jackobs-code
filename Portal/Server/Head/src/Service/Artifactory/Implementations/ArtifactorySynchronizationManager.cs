using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spark.Infra.Configurations;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Entities;
using GamesPortal.Service.Synchronizers;
using Spark.Infra.Logging;


namespace GamesPortal.Service.Artifactory
{
    public class ArtifactorySynchronizationManager : IArtifactorySynchronizationManager
    {
        
        public ArtifactorySynchronizationManager(IGamesPortalInternalServices services)
        {
            Services = services;
            Logger = services.LoggerFactory.CreateLogger(typeof(ArtifactorySynchronizationManager));

            RestClientFactory = new ArtifactoryRestClientFactory(Services.ConfigurationReader);

            InitRepositoriesDescriptors();

            _gamesSynchronizer = new GamesBatchSynchronizer(services);
        }

        IEnumerable<GamesRepositoryDescriptor> _gamesRepositoryDescriptors;
        IEnumerable<ChillWrapperRepositoryDescriptor> _chillWrapperRespositories;
        private void InitRepositoriesDescriptors()
        {
            InitGamesRepositories();

            InitChillWrapperRepositories();
        }

      
       
        private void InitGamesRepositories()
        {
            _gamesRepositoryDescriptors = ConfigurationReader.ReadSection<ArtifactorySettings>().GamesRepositories.GetGamesRespositories(RestClientFactory);

            if (!_gamesRepositoryDescriptors.Any())
            {
                _gamesRepositoryDescriptors = CreateDefaultGamesRepositoryDescriptors();
            }
        }

        private void InitChillWrapperRepositories()
        {
            _chillWrapperRespositories = ConfigurationReader.ReadSection<ArtifactorySettings>()
                                                            .ChillWrapperRepositories
                                                            .GetChillWrapperRespositories(RestClientFactory);
            if (!_chillWrapperRespositories.Any())
            {
                _chillWrapperRespositories = CreateDefaultChillWrapperRepositoryDescriptors();
            }

        }


        private List<GamesRepositoryDescriptor> CreateDefaultGamesRepositoryDescriptors()
        {
            var defaultGamesRepositoryDescriptors = new List<GamesRepositoryDescriptor>();

            defaultGamesRepositoryDescriptors.Add(new GamesRepositoryDescriptor(GameTechnology.Flash, false, PlatformType.PC, new GamesRepository("modernGame-local", "Games", RestClientFactory)));
            defaultGamesRepositoryDescriptors.Add(new GamesRepositoryDescriptor(GameTechnology.Flash, true, PlatformType.PC, new GamesRepository("externalGame-local", "Games", RestClientFactory)));

            defaultGamesRepositoryDescriptors.Add(new GamesRepositoryDescriptor(GameTechnology.Html5, false, PlatformType.PcAndMobile, new GamesRepository("HTML5Game-local", "Games", RestClientFactory)));
            defaultGamesRepositoryDescriptors.Add(new GamesRepositoryDescriptor(GameTechnology.Html5, false, PlatformType.Mobile, new GamesRepository("HTML5Game-local", "Games_MOBILE", RestClientFactory)));
            defaultGamesRepositoryDescriptors.Add(new GamesRepositoryDescriptor(GameTechnology.Html5, false, PlatformType.PC, new GamesRepository("HTML5Game-local", "Games_PC", RestClientFactory)));
            
            return defaultGamesRepositoryDescriptors;
        }

        private IEnumerable<ChillWrapperRepositoryDescriptor> CreateDefaultChillWrapperRepositoryDescriptors()
        {
            var defaultChillWrapperRepositoryDescriptors = new List<ChillWrapperRepositoryDescriptor>();

            defaultChillWrapperRepositoryDescriptors.Add(new ChillWrapperRepositoryDescriptor(ChillWrapperType.Chill.Infrastructure, new ChillWrapperRepository("HTML5Game-local", "Wrapper/chill", RestClientFactory, GamingComponentCategory.Chill)));
            defaultChillWrapperRepositoryDescriptors.Add(new ChillWrapperRepositoryDescriptor(ChillWrapperType.Wrapper.Infrastructure, new ChillWrapperRepository("modernGame-local", "Wrapper", RestClientFactory, GamingComponentCategory.Wrapper)));

            return defaultChillWrapperRepositoryDescriptors;
        }


        GamesBatchSynchronizer _gamesSynchronizer;


        ILogger Logger { get; set; }

        IArtifactoryRestClientFactory RestClientFactory { get; set; }
        
        
        IGamesPortalInternalServices Services { get; set; }

        IConfigurationReader ConfigurationReader
        {
            get { return this.Services.ConfigurationReader; }
        }

        public bool IsGamesSynchronizationInProgress
        {
            get
            {
                return _gamesSynchronizer.IsWorkInProgress;
            }
        }

        public void StartSynchronization()
        {
           
            _gamesSynchronizer.Start();
            

            Services.GameLanguageToArtifactorySynchronizer.Start();
            
            
        }

        public void StopSynchronization()
        {
            _gamesSynchronizer?.Stop();
        }
        
        
        public IArtifactoryRepositoryDescriptor FindRepositoryDescriptor(GameInfrastructureDTO gameInfrastructure, bool isExternal, GamingComponentCategory category)
        {
            IArtifactoryRepositoryDescriptor repo = null;
            if (category == GamingComponentCategory.Game)
                repo = _gamesRepositoryDescriptors.FirstOrDefault(r => r.Infrastructure== gameInfrastructure && r.IsExternal == isExternal);


            if (repo == null)
                repo = FindChillWrapperRepositoryDescriptor(category);

            if (repo == null)
                throw new ArgumentException($"Can't find a repository! {gameInfrastructure}; IsExternal = {isExternal}; ComponentCategory = {category}");

            return repo;
        }


        public IComponentSynchronizer CreateGameSynchronizer()
        {
            return new ComponentSynchronizer(Services, CreateLogger());
        }
        

        public IEnumerable<IArtifactoryRepositoryDescriptor> GetAllRepositories()
        {
            var result = new List<IArtifactoryRepositoryDescriptor>();
            result.AddRange(_gamesRepositoryDescriptors);
            result.AddRange(_chillWrapperRespositories);
            return result;       
        }

        public PropertySet[] GetAvailablePropertySets()
        {
            var result = new List<PropertySet>();

            try
            {
                var restClient = this.RestClientFactory.CreateAuthenticatedUiApi();

                foreach (var propertySet in ArtifactoryResponseExtensions.ParseArray<PropertySet>(restClient.Get("propertysets")))
                {
                    try
                    {
                        result.Add(ArtifactoryResponseExtensions.ParseObject<PropertySet>(restClient.Get("propertysets/" + propertySet.name)));
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(string.Format("Failed to read property set {0} from Artifactory. For this reason I will not be able to validated values for properies in this property set!", propertySet.name), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception("Failed to read property sets definitions from Artifactory. This means I can't validate the properties values!", ex);
            }
            
            return result.ToArray();
        }


        
        public void ForceGamesSynchronization(string initiatedBy)
        {
            _gamesSynchronizer.ForceFullSynchronization(initiatedBy);
        }

        public IArtifactorySyncronizationLogger CreateLogger()
        {
            return new ArtifactorySyncronizationLogger();
        }


        public void ForceGameSynchronization(int mainGameType, IArtifactoryRepositoryDescriptor repositoryDescriptor)
        {
            _gamesSynchronizer.ForceGameSynchronization(mainGameType, repositoryDescriptor);
        }

        public IArtifactoryRepositoryDescriptor FindRepositoryDescriptor(string repositoryName, string gamesFolderName)
        {
            var repositoryDescriptor = GetAllRepositories().FirstOrDefault(repo => repo.Repository.GetRootFolderRelativeUrl().Contains($"{repositoryName}/{gamesFolderName}"));

            if(repositoryDescriptor == null)
            {
                throw new ArgumentException($"Can't find a repository! name = {repositoryName} and folder = {gamesFolderName}");
            }

            return repositoryDescriptor;


        }

        private IArtifactoryRepositoryDescriptor FindChillWrapperRepositoryDescriptor(GamingComponentCategory componentType)
        {
            var repoDescriptor = _chillWrapperRespositories.FirstOrDefault(repo => repo.Repository.ComponentCategory == componentType);

            if (repoDescriptor == null)
                throw new ArgumentException("Can't find a repository for " + componentType);

            return repoDescriptor;
        }

        private class ArtifactorySyncronizationLogger : IArtifactorySyncronizationLogger
        {

            private StringBuilder _info = new StringBuilder();
            private StringBuilder _warn = new StringBuilder();
            private StringBuilder _errors = new StringBuilder();

            public ArtifactorySyncronizationLogger()
            {
            }
            
            public string Informations
            {
                get { return _info.ToString(); }
            }

            public string Warnings
            {
                get { return _warn.ToString(); }
            }

            public string Errors
            {
                get { return _errors.ToString(); }
            }

            #region IArtifactorySyncronizationLogger Members

            public void Info(string message)
            {
                _info.Append(message);
                _info.Append(Environment.NewLine);
                _info.Append(Environment.NewLine);
            }

            public void Warn(string message)
            {
                _warn.Append(message);
                _warn.Append(Environment.NewLine);
                _warn.Append(Environment.NewLine);

            }

            public void Error(string message)
            {
                _errors.Append(message);
                _errors.Append(Environment.NewLine);
                _errors.Append(Environment.NewLine);
            }

            public void Flush(ILogger logger)
            {
                if (!string.IsNullOrEmpty(Errors))
                {
                    logger.Error(Errors);
                }

                if (!string.IsNullOrEmpty(Warnings))
                {
                    logger.Warning(Warnings);
                }

                if (!string.IsNullOrEmpty(Informations))
                {
                    logger.Info(Informations);
                }
            }

            #endregion
        }
        
        
    }


}
