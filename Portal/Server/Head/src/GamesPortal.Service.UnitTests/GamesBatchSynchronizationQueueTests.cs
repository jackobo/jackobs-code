using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using GamesPortal.Service.Synchronizers;
using Spark.Infra.Logging;
using NSubstitute;
using NUnit.Framework;

namespace GamesPortal.Service
{
    [TestFixture]
    public class GamesBatchSynchronizationQueueTests : Helpers.SynchronizerTestHelper<GamesBatchSynchronizationQueue>
    {

        IComponentSynchronizer _artifactoryGameSynchronizer;
        protected override void SetupInternalServices()
        {
            base.SetupInternalServices();
            _artifactoryGameSynchronizer = Substitute.For<IComponentSynchronizer>();
            _internalServices.ArtifactorySynchronizationManager.CreateGameSynchronizer().Returns(_artifactoryGameSynchronizer);

        }
        protected override GamesBatchSynchronizationQueue CreateSynchronizer()
        {
            return new GamesBatchSynchronizationQueue(_internalServices);
        }


        [Test]
        public void DoWork_ShouldCallArtifactoryGatewayToConstructAn_ArtifactoryGameSynchronizer()
        {
            _synchronizer.Start();
            _internalServices.ArtifactorySynchronizationManager.Received(1).CreateGameSynchronizer();
        }


        [Test]
        public void DoWork_ShouldCallSynchronizer_FlushLogs()
        {
            _synchronizer.Start();
            _artifactoryGameSynchronizer.Received(1).FlushLogs(_logger);
        }


        [Test]
        public void DoWork_ShouldNotifySignalRClients()
        {
            _synchronizer.Start();
            _internalServices.GamesPortalHubContext.Received().FullGamesSynchronizationFinished(Arg.Any<SignalR.FullGamesSynchronizationFinishedData>());
        }

        [Test]
        public void ForceGame_IfGameAlreadyPending_LogsWithInfo()
        {
            var pendingGame = Substitute.For<IPendingGame>();
            _synchronizer.ForceGame(pendingGame);
            _synchronizer.ForceGame(pendingGame);

            _logger.Received().Info(Arg.Any<string>());
        }
        
        [Test]
        public void ForceGame_IfGameIsNotAlreadyPending_ShouldStartProcessing()
        {
            var pendingGame = Substitute.For<IPendingGame>();
            _synchronizer.ForceGame(pendingGame);
            pendingGame.ReceivedWithAnyArgs().SynchronizeGame(null);
        }

        [Test]
        public void ForceGame_IfGameIsAlreadyPending_ShouldNOTStartProcessingAgain()
        {
            var pendingGame = Substitute.For<IPendingGame>();
            _synchronizer.ForceGame(pendingGame);
            _synchronizer.ForceGame(pendingGame);
            pendingGame.ReceivedWithAnyArgs(1).SynchronizeGame(null);
        }

        [Test]
        public void DoWork_ForcedGamesShouldBeSynchronizedBeforeTheQueuedGames()
        {
            var pendingGame1 = Substitute.For<IPendingGame>();
            var pendingGame2 = Substitute.For<IPendingGame>();
            _synchronizer.EnqueueGame(pendingGame1);
            _synchronizer.ForceGame(pendingGame2);

            _synchronizer.Start();

            Received.InOrder(() =>
            {
                pendingGame2.SynchronizeGame(Arg.Any<IComponentSynchronizer>());
                pendingGame1.SynchronizeGame(Arg.Any<IComponentSynchronizer>());
            });
        }
        
        [Test]
        public void StartProcessing_WhenCalled_OnlyOneTaskShouldBeCreated()
        {
            _synchronizer.StartProcessing(Environment.UserName);
            _internalServices.ThreadingServices.Received(1).StartNewTask(Arg.Any<Action>());
        }
    }
}
