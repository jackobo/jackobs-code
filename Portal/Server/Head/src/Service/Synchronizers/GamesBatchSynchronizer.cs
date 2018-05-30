using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using GamesPortal.Service.DataAccessLayer;
using Spark.Infra.Windows;

namespace GamesPortal.Service.Synchronizers
{
    public class GamesBatchSynchronizer : Synchronizer
    {

        public GamesBatchSynchronizer(IGamesPortalInternalServices services)
            : this(services, new GamesBatchSynchronizationQueue(services))
        {
        }
        public GamesBatchSynchronizer(IGamesPortalInternalServices services, IGamesBatchSynchronizationQueue queue) 
            : base(services)
        {
            _queue = queue;
            
        }

        IGamesBatchSynchronizationQueue _queue;

        public override void Start()
        {
            if (Services.ConfigurationReader.ReadSection<ArtifactorySettings>().SynchronizationEnabled)
            {
                base.Start();
            }
            else
            {
                _queue.StartProcessing(Environment.UserName);
            }
            
        }
        
        public override void Stop()
        {
            base.Stop();

            _queue.StopProcessing();

        }

        public override void Run()
        {
            Run(Environment.UserName);
        }

        private void Run(string initiatedBy)
        {
            SynchronizationInitiatedBy = initiatedBy ?? Environment.UserName;
            base.Run();
        }

        private string SynchronizationInitiatedBy { get; set; }

        protected override void DoWork()
        {
            var userName = SynchronizationInitiatedBy;
            foreach (var repoDescriptor in Services.ArtifactorySynchronizationManager.GetAllRepositories())
            {
                foreach (var gameType in GetGameTypesToSynchronize(repoDescriptor))
                {
                    _queue.EnqueueGame(new PendingGame(gameType, repoDescriptor));
                }
            }

            _queue.StartProcessing(userName);
        }

        private int[] GetGameTypesToSynchronize(IArtifactoryRepositoryDescriptor repoDescriptor)
        {
            var gameTypesInDB = new int[0];

            using (var dbContext = this.Services.CreateGamesPortalDBDataContext())
            {
                gameTypesInDB = (from game in dbContext.GetTable<Game>()
                                    join gameVersion in dbContext.GetTable<GameVersion>() on game.Game_ID equals gameVersion.Game_ID
                                    where game.IsExternal == repoDescriptor.IsExternal
                                          && game.ComponentCategory == (int)repoDescriptor.ComponentCategory
                                          && gameVersion.PlatformType == (int)repoDescriptor.Infrastructure.PlatformType
                                          && gameVersion.Technology == (int)repoDescriptor.Infrastructure.GameTechnology
                                    select game.MainGameType)
                                .ToArray();

            }

            var gameTypesInArtifactory = repoDescriptor.Repository.GetGames();

            return gameTypesInArtifactory
                   .Union(gameTypesInDB)
                   .Distinct()
                   .OrderBy(gt => gt)
                   .ToArray();
        }

        public void ForceGameSynchronization(int gameType, IArtifactoryRepositoryDescriptor repositoryDescriptor)
        {
            _queue.ForceGame(new PendingGame(gameType, repositoryDescriptor));
        }
        
        protected override void WaitOne(IAutoresetEvent autoresetEvent)
        {
            autoresetEvent.WaitOne(Services.ConfigurationReader.ReadSection<ArtifactorySettings>().SynchronizationInterval);
        }
        
        public void ForceFullSynchronization(string initiatedBy)
        {
            Run(initiatedBy);
        }
        
    }
    
}
