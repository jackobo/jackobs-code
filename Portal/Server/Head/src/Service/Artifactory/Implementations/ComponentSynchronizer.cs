using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Entities;
using Spark.Infra.Data.LinqToSql.RecordsSynchronization;
using GamesPortal.Service.Synchronizers;
using Spark.Infra.Types;
using Spark.Infra.Logging;

namespace GamesPortal.Service.Artifactory
{

    public interface IComponentSynchronizer
    {
        void SynchronizeComponent(IArtifactoryRepositoryDescriptor repositoryDescriptor, int componentId);
        void FlushLogs(ILogger logger);
    }

    public class ComponentSynchronizer : IComponentSynchronizer
    {

        public ComponentSynchronizer(IGamesPortalInternalServices services, 
                                     IArtifactorySyncronizationLogger logger)
        {
            Services = services;
            Logger = logger;
        }

      

        protected IGamesPortalInternalServices Services { get; private set; }
        protected IArtifactorySyncronizationLogger Logger { get; private set; }

        public void FlushLogs(ILogger logger)
        {
            Logger.Flush(logger);
        }

    

        public void SynchronizeComponent(IArtifactoryRepositoryDescriptor repositoryDescriptor, int componentId)
        {

            using (var dataAccessLayer = CreateDataAccessLayer())
            {
                var component = FindComponent(dataAccessLayer, componentId, repositoryDescriptor);
                
                if (component == null)
                {
                    component = CreateNewComponent(dataAccessLayer, componentId, repositoryDescriptor);
                }
                                
                SynchronizeComponentVersions(component, repositoryDescriptor);

                SubmitChanges(dataAccessLayer, component);
            }
        }

        private void SynchronizeComponentVersions(Game component, IArtifactoryRepositoryDescriptor repositoryDescriptor)
        {

            var existingRecords = component.GameVersions;
            var newRecords = BuildNewVersionRecords(component, repositoryDescriptor).ToArray();

            VersionRecordsSynchronizer(repositoryDescriptor.Infrastructure)
                                      .Sync(existingRecords, newRecords);
        }
        
        IEnumerable<GameVersion> BuildNewVersionRecords(Game component, IArtifactoryRepositoryDescriptor repositoryDescriptor)
        {
            var versionRecords = new List<GameVersion>();

            foreach (var version in ReadVersionsToSynchronize(component.MainGameType, repositoryDescriptor.Repository))
            {
                var builder = VersionRecordBuilder(component, repositoryDescriptor, version);

                if (builder.CanBuild())
                {
                    versionRecords.Add(builder.Build());
                }
            }


            return versionRecords;
        }

        private IEnumerable<string> ReadVersionsToSynchronize(int componentId, IArtifactoryRepository repository)
        {
            var componentArtifacts = new List<string>();
            foreach(var regulation in  repository.GetComponentRegulations(componentId).ToEmptyIfNull())
            {
                foreach(var versionFolder in repository.GetVersionFolders(componentId, regulation).ToEmptyIfNull())
                {
                    if (VersionNumber.IsValid(versionFolder))
                    {
                        componentArtifacts.Add(versionFolder);
                    }
                }
            }

            return componentArtifacts.Distinct().ToList();
        }

        IComponentVersionBuilder VersionRecordBuilder(Game component, IArtifactoryRepositoryDescriptor repositoryDescriptor, string version)
        {
            return ComponentVersionRecordBuilder.Init(this.Logger, this.Services)
                                     .FromRepository(repositoryDescriptor)
                                     .WithComponentType(component.MainGameType)
                                     .WithComponentId(component.Game_ID)
                                     .WithVersionFolder(version);
        }




        IArtifactorySynchronizerDataAccessLayer CreateDataAccessLayer()
        {
            return Services.ArtifactorySynchronizerDataAccessLayer;
        }

        Game FindComponent(IArtifactorySynchronizerDataAccessLayer dal, int componentId, IArtifactoryRepositoryDescriptor repositoryDescriptor)
        {
            return dal.GetGame(componentId, repositoryDescriptor.IsExternal);
        }


        Game CreateNewComponent(IArtifactorySynchronizerDataAccessLayer dal,
                                                   int componentId,
                                                   IArtifactoryRepositoryDescriptor repositoryDescriptor)
        {

            var game = new Game()
            {
                Game_ID = Guid.NewGuid(),
                MainGameType = componentId,
                IsExternal = repositoryDescriptor.IsExternal,
                ComponentCategory = (int)GamingComponentCategory.Game
            };

            ChillWrapperType.TryFindFromId(componentId)
                            .Do(chillWrapper =>
                            {
                                game.ComponentCategory = (int)chillWrapper.Id;
                            });


            return game;
        }


        private void SetGameName(Game game, IArtifactorySynchronizerDataAccessLayer dal)
        {
            game.GameName = dal.GetGameName(game.MainGameType);
            if (string.IsNullOrEmpty(game.GameName))
            {
                Logger.Warn($"Can't find a name for game type {game.MainGameType}");
                game.GameName = game.MainGameType.ToString();
            }


        }

        void SubmitChanges(IArtifactorySynchronizerDataAccessLayer dal, Game component)
        {

            SetGameName(component, dal);
            SynchronizeGameTypes(component, dal.GetGameTypes(component.MainGameType));

            dal.SubmitChanges(component);


        }

        void SynchronizeGameTypes(Game game, GameTypeDescriptor[] availableGameTypes)
        {

            var newGameTypes = availableGameTypes.Select(gt => new GameType(gt.GameType, gt.GameName, game.Game_ID, (int)gt.OperatorId))
                                                 .ToArray();


            //synchronize all except the MainGameType.
            RecordsSynchronizerFactory<GameType>.Create(row => new { row.Game_ID, row.GameType_ID },
                                                        row => row.GameType_ID != game.MainGameType)
                                                .Sync(game.GameTypes, newGameTypes);


            var mainGameType = game.GameTypes.FirstOrDefault(gt => gt.GameType_ID == game.MainGameType);
            if (mainGameType == null) //make sure that the main game type is always added
            {
                game.GameTypes.Add(new GameType(game.MainGameType, game.GameName, game.Game_ID, (int)OperatorEnum.Operator888));
            }
            else
            {
                mainGameType.Name = game.GameName;
            }

        }

        IRecordsSynchronizer<GameVersion> VersionRecordsSynchronizer(
                        GameInfrastructureDTO gameInfrastructure)
        {
            var gameVersionRecordsSynchronizer = GameVersionRecordsSynchronizer(gameInfrastructure);

            PropertyRecordsSynchronizer(gameVersionRecordsSynchronizer);
            RegulationRecordsSynchronizer(gameVersionRecordsSynchronizer);
            LanguageRecordsSynchronizer(gameVersionRecordsSynchronizer);

            return gameVersionRecordsSynchronizer;
        }

        private void LanguageRecordsSynchronizer(IRecordsSynchronizer<GameVersion> gameVersionRecordsSynchronizer)
        {
            gameVersionRecordsSynchronizer.AddChild(new GameVersionLanguageRecordsSynchronizer());
        }

        private IRecordsSynchronizer<GameVersion> GameVersionRecordsSynchronizer(GameInfrastructureDTO gameInfrastructure)
        {
            return RecordsSynchronizerFactory<GameVersion>.Create(new GameVersionRecordsComparer(gameInfrastructure));
        }


        private class GameVersionRecordsComparer : IRecordsComparer<GameVersion>
        {
            IRecordsComparer<GameVersion> _recordsComparer;
            GameInfrastructureDTO _gameInfrastructure;
            public GameVersionRecordsComparer(GameInfrastructureDTO gameInfrastructure)
            {
                _gameInfrastructure = gameInfrastructure;

                _recordsComparer = RecordsSynchronizerFactory<GameVersion>.RecordsComparer(
                     (versionRecord) => new
                     {
                         versionRecord.Game_ID,
                         versionRecord.Technology,
                         versionRecord.PlatformType,
                         versionRecord.VersionFolder
                     },
                     gameVersion => gameVersion.Technology == (int)_gameInfrastructure.GameTechnology
                                    && gameVersion.PlatformType == (int)_gameInfrastructure.PlatformType);
            }
            public RecordsComparisonResult<GameVersion> Compare(IEnumerable<GameVersion> oldRecords, IEnumerable<GameVersion> newRecords)
            {
                var differences = _recordsComparer.Compare(oldRecords, newRecords);

                return new RecordsComparisonResult<GameVersion>(differences.Inserted,
                                                                         differences.Updated,
                                                                         //delete only GameVersion records that don't have history records
                                                                         /*differences.Deleted.Where(record => record.GameVersion_Property_Histories.Count == 0)*/
                                                                         differences.Deleted);


            }
        }

        private void PropertyRecordsSynchronizer(IRecordsSynchronizer<GameVersion> gameVersionRecordsSynchronizer)
        {
            gameVersionRecordsSynchronizer.AddChild(versionRecord => versionRecord.GameVersion_Properties,
                                                propertyRecord => new { propertyRecord.PropertyKey, propertyRecord.Regulation });
        }

        private void RegulationRecordsSynchronizer(IRecordsSynchronizer<GameVersion> gameVersionRecordsSynchronizer)
        {
            gameVersionRecordsSynchronizer.AddChild(new GameVersionRegulationRecordsSynchronizer());   

        }
    }
}
