using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using Spark.Infra.Logging;

namespace GamesPortal.Service.Synchronizers
{
    
    public interface IGamesBatchSynchronizationQueue
    {
        
        void EnqueueGame(IPendingGame game);
        void ForceGame(IPendingGame game);
        

        void StartProcessing(string initiatedBy);
        
        void StopProcessing();
    }

    public class GamesBatchSynchronizationQueue : Synchronizer, IGamesBatchSynchronizationQueue
    {
        public GamesBatchSynchronizationQueue(IGamesPortalInternalServices services)
            : base(services)
        {

        }

        ConcurrentQueue<IPendingGame> _regularPendingGamesQueue = new ConcurrentQueue<IPendingGame>();
        ConcurrentQueue<IPendingGame> _forcedGamesQueue = new ConcurrentQueue<IPendingGame>();

        public void EnqueueGame(IPendingGame game)
        {
            _regularPendingGamesQueue.Enqueue(game);
        }

        public void ForceGame(IPendingGame game)
        {
            if (_forcedGamesQueue.Contains(game))
            {
                Logger.Info($"Game already pending! Pending game details: {game}");
            }
            else
            {
                _forcedGamesQueue.Enqueue(game);
                StartProcessing(Environment.UserName);
            }
            
        }

        protected override void DoWork()
        {
            var userName = SynchronizationInitiatedBy;
            var gameSynchronizer = Services.ArtifactorySynchronizationManager.CreateGameSynchronizer();
            
            try
            {
                while (HasMorePendingGames)
                {
                    var pendingGame = GetNextPendingGame();

                    Logger.Info($"Start synchronizing: {pendingGame.GetDescription()}");

                    try
                    {
                        pendingGame.SynchronizeGame(gameSynchronizer);

                        Logger.Info($"Finish synchronizing: {pendingGame.GetDescription()}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception($"Synchronization failed: {pendingGame.GetDescription()}", ex);
                    }
                }
            }
            finally
            {
                gameSynchronizer.FlushLogs(Logger);
                SendNotifications(userName);
            }
        }


        private bool HasMorePendingGames
        {
            get
            {
                return _regularPendingGamesQueue.Count > 0 || _forcedGamesQueue.Count > 0;
            }
        }

        private void SendNotifications(string userWhoInitiatedTheSynchronization)
        {
            Services.GamesPortalHubContext.FullGamesSynchronizationFinished(new SignalR.FullGamesSynchronizationFinishedData(DateTime.Now, userWhoInitiatedTheSynchronization));
        }


        
        public void StartProcessing(string initiatedBy)
        {
            SynchronizationInitiatedBy = initiatedBy;
            if (IsStarted)
            {
                Run();
            }
            else
            {
                Start();
            }
        }

       
        public void StopProcessing()
        {
            Stop();
        }

        private string SynchronizationInitiatedBy { get; set; }

        private IPendingGame GetNextPendingGame()
        {
            IPendingGame pendingGame = null;
            
            if (_forcedGamesQueue.TryDequeue(out pendingGame))
                return pendingGame;

            if (_regularPendingGamesQueue.TryDequeue(out pendingGame))
                return pendingGame;


            return VoidPendingGame.Instance;
        }

        private class VoidPendingGame : IPendingGame
        {
            private VoidPendingGame()
            {

            }
            public string GetDescription()
            {
                return string.Empty;
            }

            public void SynchronizeGame(IComponentSynchronizer gameSynchronizer)
            {
            }

            public static readonly VoidPendingGame Instance = new VoidPendingGame();
        } 

        private void SynchronizeGame(IComponentSynchronizer gameSynchronizer, GamesRepositoryDescriptor repoDescriptor, int gameType)
        {
            var description = $"Game Type = {gameType}; Repository = {repoDescriptor.Repository.GetRootFolderRelativeUrl()}";

            Logger.Info($"Start synchronizing: {description}");

            try
            {
                gameSynchronizer.SynchronizeComponent(repoDescriptor, gameType);

                Logger.Info($"Finish synchronizing: {description}");
            }
            catch (Exception ex)
            {
                Logger.Exception($"Synchronization failed: {description}", ex);
            }
        }
        
        protected override void OnBeginWork()
        {
            base.OnBeginWork();
            Logger.Info(string.Format("Artifactory games synchronization started at {0}", DateTime.Now));
        }

        protected override void OnFinishWork(TimeSpan elapsedTime)
        {
            base.OnFinishWork(elapsedTime);
            Logger.Info(string.Format("Artifactory games synchronization finished in {0}", elapsedTime.ToString()));
        }
        
    }


    
}
