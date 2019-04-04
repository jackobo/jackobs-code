using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Synchronizers;
using NUnit.Framework;
using NSubstitute;
using GamesPortal.Service.Artifactory;
using GamesPortal.Service.Helpers;
using GamesPortal.Service.DataAccessLayer;
using static GamesPortal.Service.Helpers.MockRecordsFactory;

namespace GamesPortal.Service
{
    [TestFixture]
    public class GamesBatchSynchronizerTests : Helpers.SynchronizerTestHelper<GamesBatchSynchronizer>
    {
        
        IGamesBatchSynchronizationQueue _synchronizationQueue;

        protected override void SetupInternalServices()
        {
            base.SetupInternalServices();
                        
            _internalServices.ConfigurationReader.ReadSection<ArtifactorySettings>().Returns(new ArtifactorySettings() { SynchronizationEnabled = true });
            _internalServices.CreateGamesPortalDBDataContext().MockTable(new Game[0]);
            _internalServices.CreateGamesPortalDBDataContext().MockTable(new GameVersion[0]);
            _synchronizationQueue = Substitute.For<IGamesBatchSynchronizationQueue>();
        }
        


        protected override GamesBatchSynchronizer CreateSynchronizer()
        {
            return new GamesBatchSynchronizer(_internalServices, _synchronizationQueue);
        }

        [Test]
        public void DoWork_BeforeStarting_ShouldQueryTheRepositoryDescriptorFromTheArtifactoryGateway()
        {
            _synchronizer.Start();
            _internalServices.ArtifactorySynchronizationManager.Received(1).GetAllRepositories();
        }

        [Test]
        public void DoWork_ForEachGameTypeInEachGameRepository_InsertItInTheQueue()
        {
            var repo1 = Substitute.For<IGamesRepository>();
            repo1.GetRootFolderRelativeUrl().Returns("HTML5Game-local/Games");
            repo1.GetGames().Returns(new int[] { 130014, 130015 });

            var repo2 = Substitute.For<IGamesRepository>();
            repo2.GetRootFolderRelativeUrl().Returns("modernGame-local");
            repo2.GetGames().Returns(new int[] { 130043});

            var repoDescriptor1 = new GamesRepositoryDescriptor(Entities.GameTechnology.Html5, false, Entities.PlatformType.Mobile, repo1);
            var repoDescriptor2 = new GamesRepositoryDescriptor(Entities.GameTechnology.Flash, false, Entities.PlatformType.PC, repo2);

            _internalServices.ArtifactorySynchronizationManager.GetAllRepositories().Returns(new GamesRepositoryDescriptor[]
                {repoDescriptor1, repoDescriptor2 });

            _synchronizer.Start();

            _synchronizationQueue.Received(3).EnqueueGame(Arg.Any<IPendingGame>());
            
        }


        [Test]
        public void DoWork_ForEachGameTypeDataBase_InsertItInTheQueue()
        {
            var repo = Substitute.For<IGamesRepository>();
            repo.GetRootFolderRelativeUrl().Returns("HTML5Game-local/Games");
            repo.GetGames().Returns(new int[0]);

            var repoDescriptor = new GamesRepositoryDescriptor(Entities.GameTechnology.Html5, false, Entities.PlatformType.Mobile, repo);
            
            _internalServices.ArtifactorySynchronizationManager.GetAllRepositories().Returns(new GamesRepositoryDescriptor[]
                {repoDescriptor});

            var dal = _internalServices.CreateGamesPortalDBDataContext();
            var gameRecord = GameRecord(130043, false);
            
            var gameVersionRecord = GameVersionRecord(Guid.NewGuid(), "1.2.0.5", gameRecord, Entities.GameTechnology.Html5, Entities.PlatformType.Mobile);
            
            dal.MockTable(gameRecord);
            dal.MockTable(gameVersionRecord);

            _synchronizer.Start();

            _synchronizationQueue.Received(1).EnqueueGame(Arg.Any<IPendingGame>());

        }

        [Test]
        public void DoWork_IfTheGameTypeExistsInBothArtifactoryAndDatabase_InsertItInTheQueueOnlyOnce()
        {
            var repo = Substitute.For<IGamesRepository>();
            repo.GetRootFolderRelativeUrl().Returns("HTML5Game-local/Games");
            repo.GetGames().Returns(new int[] { 130043 });

            var repoDescriptor = new GamesRepositoryDescriptor(Entities.GameTechnology.Html5, false, Entities.PlatformType.Mobile, repo);

            _internalServices.ArtifactorySynchronizationManager.GetAllRepositories().Returns(new GamesRepositoryDescriptor[]
                {repoDescriptor});

            var dal = _internalServices.CreateGamesPortalDBDataContext();
            var gameRecord = GameRecord(130043, false);

            var gameVersionRecord = GameVersionRecord(Guid.NewGuid(), "1.2.0.5", gameRecord, Entities.GameTechnology.Html5, Entities.PlatformType.Mobile);

            dal.MockTable(gameRecord);
            dal.MockTable(gameVersionRecord);

            _synchronizer.Start();

            _synchronizationQueue.Received(1).EnqueueGame(Arg.Any<IPendingGame>());

        }

        [TestCase(false)]
        [TestCase(true)]
        public void Start_TheQueueProcessingShouldBeStartedOnlyOnce(bool artifactorySynchronizationEnabled)
        {
            _internalServices.ConfigurationReader.ReadSection<ArtifactorySettings>().SynchronizationEnabled = artifactorySynchronizationEnabled;
            _synchronizer.Start();
            _synchronizationQueue.Received(1).StartProcessing(Arg.Any<string>());
        }

        [Test]
        public void ForceGameSynchronization_ShouldCallForceGameInTheQueue()
        {
            var repoDescriptor = Helpers.ArtifactoryHelper.CreateRepositoryDescriptor("repo");
            _synchronizer.ForceGameSynchronization(130017, repoDescriptor);
            _synchronizationQueue.ReceivedWithAnyArgs().ForceGame(null);

        }


        [Test]
        public void ForceGameSynchronization_ShouldNOTCallStartProcessingInTheQueue()
        {
            var repoDescriptor = Helpers.ArtifactoryHelper.CreateRepositoryDescriptor("repo");
            _synchronizer.ForceGameSynchronization(130017, repoDescriptor);
            _synchronizationQueue.DidNotReceiveWithAnyArgs().StartProcessing(null);

        }



    }
}
