using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spark.Infra.Configurations;
using GamesPortal.Service.DataAccessLayer;
using Spark.Infra.Data.LinqToSql;
using Newtonsoft.Json;
using GamesPortal.Service.Entities;
using Spark.Infra.Logging;

namespace GamesPortal.Service.Artifactory
{
    public class ComponentSynchronizerDataAccessLayer : IArtifactorySynchronizerDataAccessLayer
    {
        public ComponentSynchronizerDataAccessLayer(IGamesPortalInternalServices services)
        {
            Services = services;

            _gamesPortalDataContext = services.CreateGamesPortalDBDataContext();
            _sdmDataContext = services.CreateSdmDbContext();
            _ggpVersioningDataContext = services.CreateGGPVersioningDbContext();
            
        }

        IGamesPortalInternalServices Services { get; set; }

        IGamesPortalDataContext _gamesPortalDataContext;
        SDM.ISdmDataContext _sdmDataContext;
        GamesPortal.Service.GGPVersioning.IGGPVersioningDBDataContext _ggpVersioningDataContext;
        

        public void InsertGame(Game game)
        {
            _gamesPortalDataContext.GetTable<Game>().InsertOnSubmit(game);
        }

        public GamesPortal.Service.DataAccessLayer.Game GetGame(int gameType, bool isExternal)
        {
            return _gamesPortalDataContext.GetTable<Game>().FirstOrDefault(g => g.MainGameType == gameType && g.IsExternal == isExternal);
        }

        public void SubmitChanges(Game game)
        {
            InsertGameIfIsANewOne(game);

            var changes = _gamesPortalDataContext.GetChanges(); 

            var numberOfAffectedGameVersionsByLanguage = changes.Filter<GameVersion_Language>().GetAll().Count();
            
            _gamesPortalDataContext.SubmitChanges();
            
            if (numberOfAffectedGameVersionsByLanguage > 0)
            {
                Services.GameLanguageToArtifactorySynchronizer.Run();
            }
            
            if (!changes.IsEmpty())
            {
                NotifySignalRClients(game, changes);


            }
        }
        

        private IEnumerable<Guid> ExtractGameVersionIdsFromLanguageRecords(string changeType, IEnumerable<GameVersion_Language> records)
        {
            var result = new List<Guid>();

            foreach(var record in records)
            {
                var gameVersionId = record.GameVersion_ID;
                if (gameVersionId == Guid.Empty && record.GameVersion != null)
                {
                    gameVersionId = record.GameVersion.GameVersion_ID;   
                }
                
                if (gameVersionId == Guid.Empty)
                    Logger.Warning($"Empty GameVersionId detected in {changeType} records");
                else
                    result.Add(gameVersionId);

            }

            return result;
        }

        ILogger Logger
        {
            get { return Services.LoggerFactory.CreateLogger(this.GetType()); }
        }
        
        
        private void InsertGameIfIsANewOne(Game game)
        {
            var gameTable = _gamesPortalDataContext.GetTable<Game>();
            if (!gameTable.Any(g => g.Game_ID == game.Game_ID))
            {
                gameTable.InsertOnSubmit(game);
            }
        }

        private void NotifySignalRClients(Game game, ChangeSet<object> changes)
        {
            var changedGames = changes.Filter<Game>();

            if (changedGames.Inserted.Contains(game))
            {
                NotifySignalRClients(game, RecordChangeType.Added);
            }
            else if (changedGames.Deleted.Contains(game))
            {
                NotifySignalRClients(game, RecordChangeType.Deleted);
            }
            else
            {
                NotifySignalRClients(game, RecordChangeType.Changed);
            }
                   

        }


        private void NotifySignalRClients(Game game, RecordChangeType changeType)
        {
            
            Services.GamesPortalHubContext.GameSynchronizationFinished(new SignalR.GameSynchronizationFinishedData
                                    (game.Game_ID,
                                    changeType,
                                    game.IsExternal));
            
        }

        
        private bool IsChillOrWrapper(int gameType)
        {
            return ChillWrapperType.TryFindFromId(gameType).Any();
            
        }

        public string GetGameName(int gameType)
        {
            if (IsChillOrWrapper(gameType))
                return ChillWrapperType.FindFromId(gameType).Name;

            var gameName = _sdmDataContext.GetTable<SDM.GameType>().Where(g => g.GMT_ID == gameType)
                                                             .Select(row => row.GMT_Description)
                                                             .FirstOrDefault();

            if (!string.IsNullOrEmpty(gameName))
                return gameName;

            var componentGameType = _ggpVersioningDataContext.GetTable<GGPVersioning.Component_GameType>()
                                                             .FirstOrDefault(cgt => cgt.GameType == gameType);

            if (componentGameType != null)
                return componentGameType.Component.Name;

            return gameType.ToString();
            
        }

      

        public GameTypeDescriptor[] GetGameTypes(int mainGameType)
        {
            if (IsChillOrWrapper(mainGameType))
                return new GameTypeDescriptor[0];

            var componentsIDs = _ggpVersioningDataContext.GetTable<GGPVersioning.Component_GameType>().Where(row => row.GameType == mainGameType)
                                                                             .Select(row => row.Component_ID)
                                                                             .ToArray();
            if (componentsIDs.Length == 0)
                return new GameTypeDescriptor[0];


            var result = new List<GameTypeDescriptor>();

            var rows = _ggpVersioningDataContext.GetTable<GGPVersioning.Component_GameType>().ToArray();
            //select only the MainGameType (which is 888) and all the other game types that don't belong to 888 (like Bingo)
            foreach (var gameTypeRow in rows.Where(row => (row.GameType == mainGameType || row.Operator_ID != (int)OperatorEnum.Operator888)
                                                                            && componentsIDs.Contains(row.Component_ID))
                                            .Select(row => new { row.GameType, row.Operator_ID })
                                            .Distinct())
            {
                result.Add(new GameTypeDescriptor(gameTypeRow.GameType, (OperatorEnum)gameTypeRow.Operator_ID, GetGameName(gameTypeRow.GameType)));
            }

            return result.ToArray();

        }

        public void Dispose()
        {
            _gamesPortalDataContext.Dispose();
            _sdmDataContext.Dispose();
            _ggpVersioningDataContext.Dispose();
        }
        

        
    }
}
