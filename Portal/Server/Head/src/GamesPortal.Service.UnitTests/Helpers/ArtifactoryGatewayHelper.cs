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
    internal static class ArtifactoryGatewayHelper
    {
        public static ArtifactoryRepositoryDescriptor MockGameRepository(this IArtifactoryGateway artifactoryGateway, Entities.GameTechnology gameTechnology, bool isExternal = false)
        {
            var descriptor = CreateArtifactoryGamesRepositoryDescriptor(gameTechnology, isExternal);
            artifactoryGateway.GetRepositoryDescriptor(Arg.Any<GameTechnology>(),
                                                      Arg.Any<PlatformType>(),
                                                      Arg.Any<bool>())
                                                      .Returns(descriptor);

            return descriptor;
        }

        private static ArtifactoryRepositoryDescriptor CreateArtifactoryGamesRepositoryDescriptor(Entities.GameTechnology gameTechnology, bool isExternal = false)
        {
            var gamesRepository = Substitute.For<IArtifactoryGamesRepository>();
            gamesRepository.RepositoryName.Returns(string.Format("{0}-{1}-Repository", gamesRepository, isExternal ? "external" : "internal"));
            return new ArtifactoryRepositoryDescriptor(gameTechnology, isExternal, DEFAULT_PLATFORM_TYPE, gamesRepository);

        }


        public static ArtifactoryRepositoryDescriptor CreateRepositoryDescriptor(string repoName = "repoName", GameTechnology technolgoy = GameTechnology.Html5)
        {
            var repoStub = Substitute.For<IArtifactoryGamesRepository>();
            repoStub.RepositoryName.Returns(repoName);
            repoStub.GetRootFolderRelativeUrl().Returns(repoName);

            return new ArtifactoryRepositoryDescriptor(technolgoy, true, ArtifactoryGatewayHelper.DEFAULT_PLATFORM_TYPE, repoStub);
        }

        public static IArtifactorySyncronizationLogger CreateMockLogger()
        {
            return Substitute.For<IArtifactorySyncronizationLogger>();
        }

        public static readonly PlatformType DEFAULT_PLATFORM_TYPE = PlatformType.Both;
    }
}
