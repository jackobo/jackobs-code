using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using GamesPortal.Service.Entities;
using Spark.Infra.Configurations;
using Spark.Infra.Logging;
using Spark.Infra.Types;

namespace GamesPortal.Service
{
    class ApprovalStatusGenerator
    {
        public ApprovalStatusGenerator(IConfigurationReader configurationReader, ILoggerFactory loggerFactory)
        {
            this.ConfigurationReader = configurationReader;
            this.Logger = loggerFactory.CreateLogger(this.GetType());
            var restClientFactory = new Artifactory.ArtifactoryRestClientFactory(configurationReader);
            this.RepositoryDescriptors = GetRepositoryDescriptors(restClientFactory);
        }

        private IEnumerable<IArtifactoryRepositoryDescriptor> GetRepositoryDescriptors(ArtifactoryRestClientFactory restClientFactory)
        {
            var repositories = ConfigurationReader.ReadSection<ArtifactorySettings>()
                                                .GetAllComponentsRepositoryDescriptors(restClientFactory)
                                                .ToList();
            if(repositories.Count == 0)
            {
                repositories.AddRange(CreateDefaultChillWrapperRepositoryDescriptors(restClientFactory));
                repositories.AddRange(CreateDefaultGamesRepositoryDescriptors(restClientFactory));
            }

            return repositories;
        }


        private List<GamesRepositoryDescriptor> CreateDefaultGamesRepositoryDescriptors(ArtifactoryRestClientFactory restClientFactory)
        {
            var defaultGamesRepositoryDescriptors = new List<GamesRepositoryDescriptor>();

            defaultGamesRepositoryDescriptors.Add(new GamesRepositoryDescriptor(GameTechnology.Flash, false, PlatformType.PC, new GamesRepository("modernGame-local", "Games", restClientFactory)));
            defaultGamesRepositoryDescriptors.Add(new GamesRepositoryDescriptor(GameTechnology.Flash, true, PlatformType.PC, new GamesRepository("externalGame-local", "Games", restClientFactory)));

            defaultGamesRepositoryDescriptors.Add(new GamesRepositoryDescriptor(GameTechnology.Html5, false, PlatformType.PcAndMobile, new GamesRepository("HTML5Game-local", "Games", restClientFactory)));
            defaultGamesRepositoryDescriptors.Add(new GamesRepositoryDescriptor(GameTechnology.Html5, false, PlatformType.Mobile, new GamesRepository("HTML5Game-local", "Games_MOBILE", restClientFactory)));
            defaultGamesRepositoryDescriptors.Add(new GamesRepositoryDescriptor(GameTechnology.Html5, false, PlatformType.PC, new GamesRepository("HTML5Game-local", "Games_PC", restClientFactory)));

            return defaultGamesRepositoryDescriptors;
        }

        private IEnumerable<ChillWrapperRepositoryDescriptor> CreateDefaultChillWrapperRepositoryDescriptors(ArtifactoryRestClientFactory restClientFactory)
        {
            var defaultChillWrapperRepositoryDescriptors = new List<ChillWrapperRepositoryDescriptor>();

            defaultChillWrapperRepositoryDescriptors.Add(new ChillWrapperRepositoryDescriptor(ChillWrapperType.Chill.Infrastructure, new ChillWrapperRepository("HTML5Game-local", "Wrapper/chill", restClientFactory, GamingComponentCategory.Chill)));
            defaultChillWrapperRepositoryDescriptors.Add(new ChillWrapperRepositoryDescriptor(ChillWrapperType.Wrapper.Infrastructure, new ChillWrapperRepository("modernGame-local", "Wrapper", restClientFactory, GamingComponentCategory.Wrapper)));

            return defaultChillWrapperRepositoryDescriptors;
        }


        ILogger Logger { get; set; }
        IEnumerable<IArtifactoryRepositoryDescriptor> RepositoryDescriptors { get; set; }
        IConfigurationReader ConfigurationReader { get; set; }
        public void Generate()
        {

            using (var dbContext = new DataAccessLayer.GamesPortalDataContext(System.Configuration.ConfigurationManager.ConnectionStrings["GamesPortal"].ConnectionString))
            {
                var query = (from game in dbContext.Games
                            join gameVersion in dbContext.GameVersions on game.Game_ID equals gameVersion.Game_ID
                            join gameVersionRegulation in dbContext.GameVersion_Regulations on gameVersion.GameVersion_ID equals gameVersionRegulation.GameVersion_ID
                            where gameVersionRegulation.QAApprovalDate != null
                                  || gameVersionRegulation.PMApprovalDate != null
                                  || gameVersionRegulation.ProductionUploadDate != null
                            select new
                            {
                                game.IsExternal,
                                game.ComponentCategory,
                                game.GameName,
                                game.MainGameType,
                                gameVersion.Technology,
                                gameVersion.PlatformType,
                                gameVersion.VersionFolder,
                                gameVersionRegulation.Regulation,
                                QAApproved = gameVersionRegulation.QAApprovalDate != null,
                                PMApproved = gameVersionRegulation.PMApprovalDate != null,
                                Production = gameVersionRegulation.ProductionUploadDate != null
                            })
                            .ToList();



                foreach (var group in query.GroupBy(r => new { r.Technology, r.PlatformType, r.ComponentCategory, r.IsExternal })
                                            .OrderBy(r => r.Key.IsExternal)
                                            .ThenBy(r => r.Key.Technology)
                                            .ThenBy(r => r.Key.PlatformType))
                {
                    var repoDescriptor = FindRepositoryDescriptor((GameTechnology)group.Key.Technology,
                                                                          (PlatformType)group.Key.PlatformType,
                                                                          (GamingComponentCategory)group.Key.ComponentCategory,
                                                                          group.Key.IsExternal);

                    foreach (var record in group.OrderBy(r => r.MainGameType).ThenBy(r => r.Regulation).ThenBy(r => r.VersionFolder))
                    {
                        Logger.Info($"Begin PlatformType={(PlatformType)record.PlatformType}; Technology={(GameTechnology)record.Technology}; Game={record.GameName}; GameType={record.MainGameType}; Regulation={record.Regulation}; Version={record.VersionFolder}");
                        try
                        {
                            repoDescriptor.Do(repo =>
                            {
                                repo.Repository.UpdateArtifactProperties(
                                    new UpdateArtifactPropertiesRequest(record.MainGameType,
                                                                        record.Regulation,
                                                                        record.VersionFolder,
                                                                        GetArtifactoryPropertiesToUpdate(record.QAApproved, record.PMApproved, record.Production)));
                            });

                        }
                        catch (Exception ex)
                        {
                            Logger.Exception(ex);
                        }
                    }
                }
            }

        }

        private static ArtifactoryProperty[] GetArtifactoryPropertiesToUpdate(bool qaApproved, bool pmApproved, bool production)
        {
            var artifactoryProperties = new List<ArtifactoryProperty>();

            if (qaApproved)
            {
                artifactoryProperties.Add(new ArtifactoryProperty(WellKnownNamesAndValues.QAApproved, WellKnownNamesAndValues.True));
            }

            if (pmApproved)
            {
                artifactoryProperties.Add(new ArtifactoryProperty(WellKnownNamesAndValues.PMApproved, WellKnownNamesAndValues.True));
            }
            
            if (production)
            {
                artifactoryProperties.Add(new ArtifactoryProperty(WellKnownNamesAndValues.Production, WellKnownNamesAndValues.True));
            }
           

           

            return artifactoryProperties.ToArray();
        }

        private Optional<IArtifactoryRepositoryDescriptor> FindRepositoryDescriptor(GameTechnology gameTechnology, PlatformType platformType, GamingComponentCategory componentCategory, bool isExternal)
        {

            var repositoryDescriptor = RepositoryDescriptors.FirstOrDefault(repo => repo.ComponentCategory == componentCategory
                                                                                    && repo.Infrastructure == new GameInfrastructureDTO(gameTechnology, platformType)
                                                                                    && repo.IsExternal == isExternal);

            if (repositoryDescriptor == null)
            {
                Logger.Error($"Can't find a Repository for GameTechnology = {gameTechnology}; PlatformType = {platformType}; ComponentCategory = {componentCategory}");
                return Optional<IArtifactoryRepositoryDescriptor>.None();
            }

            return Optional<IArtifactoryRepositoryDescriptor>.Some(repositoryDescriptor);
        }
    }
}
