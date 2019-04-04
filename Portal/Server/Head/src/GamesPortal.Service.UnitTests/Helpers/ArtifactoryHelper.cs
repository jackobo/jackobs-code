using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using GamesPortal.Service.Entities;
using NSubstitute;

namespace GamesPortal.Service.Helpers
{
    internal static class ArtifactoryHelper
    {
        public static GamesRepositoryDescriptor MockGameRepository(this IArtifactorySynchronizationManager artifactoryGateway, Entities.GameTechnology gameTechnology, bool isExternal = false)
        {
            var descriptor = CreateArtifactoryGamesRepositoryDescriptor(gameTechnology, isExternal);
            artifactoryGateway.FindRepositoryDescriptor(Arg.Any<GameInfrastructureDTO>(),
                                                             Arg.Any<bool>(), 
                                                             Arg.Any<GamingComponentCategory>())
                              .Returns(descriptor);

            return descriptor;
        }

        public static GamesRepositoryDescriptor MockGameRepository(this IArtifactorySynchronizationManager artifactoryGateway, Entities.GameTechnology gameTechnology, PlatformType platformType, bool isExternal = false)
        {
            var descriptor = CreateArtifactoryGamesRepositoryDescriptor(gameTechnology, platformType, isExternal);
            artifactoryGateway.FindRepositoryDescriptor(new GameInfrastructureDTO(gameTechnology, platformType),
                                                             isExternal,
                                                             GamingComponentCategory.Game)
                              .Returns(descriptor);

            return descriptor;
        }

        private static GamesRepositoryDescriptor CreateArtifactoryGamesRepositoryDescriptor(Entities.GameTechnology gameTechnology, bool isExternal = false)
        {
            return CreateArtifactoryGamesRepositoryDescriptor(gameTechnology, DEFAULT_PLATFORM_TYPE, isExternal);
        }


        private static GamesRepositoryDescriptor CreateArtifactoryGamesRepositoryDescriptor(Entities.GameTechnology gameTechnology, PlatformType platformType,  bool isExternal = false)
        {
            var gamesRepository = Substitute.For<IGamesRepository>();
            gamesRepository.RepositoryName.Returns(string.Format("{0}-{1}-Repository", gamesRepository, isExternal ? "external" : "internal"));
            return new GamesRepositoryDescriptor(gameTechnology, isExternal, platformType, gamesRepository);

        }


        public static GamesRepositoryDescriptor CreateRepositoryDescriptor(string repoName = "repoName", GameTechnology technolgoy = GameTechnology.Html5)
        {
            var repoStub = Substitute.For<IGamesRepository>();
            repoStub.RepositoryName.Returns(repoName);
            repoStub.GetRootFolderRelativeUrl().Returns(repoName);

            return new GamesRepositoryDescriptor(technolgoy, true, ArtifactoryHelper.DEFAULT_PLATFORM_TYPE, repoStub);
        }

        public static IArtifactorySyncronizationLogger CreateMockLogger()
        {
            return Substitute.For<IArtifactorySyncronizationLogger>();
        }

        public const PlatformType DEFAULT_PLATFORM_TYPE = PlatformType.PcAndMobile;
    }
}
