using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamesPortal.Service.Configurations;
using GamesPortal.Service.DataAccessLayer;
using GGPGameServer.ApprovalSystem.Common.Databases;
using Newtonsoft.Json;


namespace GamesPortal.Service.Artifactory
{
    public class ArtifactorySynchronizerDataAccessLayer : IArtifactorySynchronizerDataAccessLayer
    {
        public ArtifactorySynchronizerDataAccessLayer(IGamesPortalInternalServices services)
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
        

        public void InsertGame(GamesPortal.Service.DataAccessLayer.Game game)
        {
            _gamesPortalDataContext.GetTable<Game>().InsertOnSubmit(game);
        }

        public GamesPortal.Service.DataAccessLayer.Game GetGame(int gameType, bool isExternal)
        {
            return _gamesPortalDataContext.GetTable<Game>().FirstOrDefault(g => g.MainGameType == gameType && g.IsExternal == isExternal);
        }

        public void SubmitChanges()
        {
            var numberOfAffectedVersion = UpdateLanguageSynchronizationQueue();

            _gamesPortalDataContext.SubmitChanges();

            if (numberOfAffectedVersion > 0)
            {
                Services.ArtifactoryToGameLanguageSynchronizer.Run();
            }
            
        }

        private int UpdateLanguageSynchronizationQueue()
        {
            int numberVersions = 0;
            var changes = _gamesPortalDataContext.GetChanges();

            if (changes != null)
            {
                foreach (var gameVersionId in changes.Filter<GameVersion_Property>().GetAll()
                                                      .Where(record => LanguageProperty.IsLanguage(record.PropertyKey))
                                                      .Select(record => record.GameVersion_ID)
                                                      .Distinct())
                {
                    _gamesPortalDataContext.GetTable<GameVersion_Language_FromArtifactorySyncQueue>().InsertOnSubmit(new GameVersion_Language_FromArtifactorySyncQueue()
                    {
                        GameVersion_ID = gameVersionId
                    });

                    numberVersions++;
                }
            }

            return numberVersions;
        }

        public string GetGameName(int gameType)
        {
            var gameName = _sdmDataContext.GetTable<SDM.GameType>().Where(g => g.GMT_ID == gameType)
                                                             .Select(row => row.GMT_Description)
                                                             .FirstOrDefault();

            if (!string.IsNullOrEmpty(gameName))
                return gameName;

            var componentGameType = _ggpVersioningDataContext.GetTable<GGPVersioning.Component_GameType>().FirstOrDefault(cgt => cgt.GameType == gameType);

            if (componentGameType == null)
                return gameType.ToString();

            return componentGameType.Component.Name;


        }

        public GameTypeDescriptor[] GetGameTypes(int mainGameType)
        {
            var componentsIDs = _ggpVersioningDataContext.GetTable<GGPVersioning.Component_GameType>().Where(row => row.GameType == mainGameType)
                                                                             .Select(row => row.Component_ID)
                                                                             .ToArray();
            if (componentsIDs.Length == 0)
                return new GameTypeDescriptor[0];


            var result = new List<GameTypeDescriptor>();

            var rows = _ggpVersioningDataContext.GetTable<GGPVersioning.Component_GameType>().ToArray();
            //select only the MainGameType (which is 888) and all the other game types that don't belong to 888 (like Bingo)
            foreach (var gameTypeRow in rows
                                                                 .Where(row => (row.GameType == mainGameType || row.Operator_ID != (int)OperatorEnum.Operator888)
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
        

        #region IArtifactorySynchronizerDataAccessLayer Members


        public bool HasChanges
        {
            get
            {
                return _gamesPortalDataContext.HasChanges;
            }
        }

        #endregion
    }
}
