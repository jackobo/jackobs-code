using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.Services;
using GGPGameServer.ApprovalSystem.Common;

namespace GamesPortal.Client.Services
{
    public class GamesRepository : IGamesRepository, IReportingService
    {
        

        public Interfaces.Entities.Game[] ReadAllGames()
        {
            var games = new List<Interfaces.Entities.Game>();

            using (var proxy = new GamesPortalService.GamesPortalServiceClient())
            {
                var response = proxy.GetAllGames();
                foreach (var gameEntity in response.Games)
                {
                    games.Add(new Interfaces.Entities.Game(this)
                    {
                        Id = gameEntity.Id,
                        IsExternal = gameEntity.IsExternal,
                        MainGameType = gameEntity.MainGameType,
                        Name = gameEntity.Name,
                        SupportedTechnologies = gameEntity.SupportedTechnologies.Select(tech => (Interfaces.Entities.GameTechnology) ((int)tech)).ToArray(),
                        GameTypes = gameEntity.GameTypes.Select(gt => new Interfaces.Entities.GameType(gt.Id, gt.Name, gt.OperatorId)).ToArray()
                    });

                }
            }

            return games.ToArray();
        }

        public Interfaces.Entities.GameVersion[] GetGameVersions(Guid gameId)
        {
            var result = new List<Interfaces.Entities.GameVersion>();
            using (var proxy = new GamesPortalService.GamesPortalServiceClient())
            {
                var response = proxy.GetGameVersions(new GamesPortalService.GetGameVersionsRequest() { GameID = gameId });


                foreach (var version in response.GameVersions)
                {
                    var gameVersion = new Interfaces.Entities.GameVersion();

                    gameVersion.CreatedBy = version.CreatedBy;
                    gameVersion.TriggeredBy = version.TriggeredBy;
                    gameVersion.CreatedDate = version.CreatedDate;
                    gameVersion.Technology = (Interfaces.Entities.GameTechnology)((int)version.Technology);
                    gameVersion.Version = new VersionNumber(version.Version);
                    gameVersion.ProperiesChangeHistory = version.PropertiesChangeHistory.Select(item => new GameVersionPropertyChangedHistory(item.PropertyKey, item.OldValue, item.NewValue, item.Regulation, item.ChangeDate, item.ChangedBy, (PropertyChangeType)item.ChangeType)).ToArray();

                    gameVersion.PropertySets = version.Properties.GroupBy(p => new { p.PropertySetName, p.Regulation })
                                                                  .Select(g =>{
                                                                                    var regulation = RegulationType.GetRegulation(g.Key.Regulation);
                                                                                    return new GameVersionPropertySet(g.Key.PropertySetName,
                                                                                                                    regulation,
                                                                                                                    g.Select(item => new GameVersionProperty(item.Key, regulation, item.Value)).ToArray());
                                                                               }
                                                                         )
                                                                  .ToArray();

                    result.Add(gameVersion);
                }


                return result.ToArray();
            }
        }

     
        public LatestApprovedGameVersion[] GetLatestApprovedGamesVersions()
        {
            using (var proxy = new GamesPortalService.GamesPortalServiceClient())
            {
                var response = proxy.GetLatestApprovedVersionForEachGame();

                return response.LatestApprovedGamesVersions.Select(item => 
                    new LatestApprovedGameVersion(
                        item.Game_ID, 
                        item.GameName, 
                        item.MainGameType, 
                        VersionNumber.Parse(item.LastVersion),
                        RegulationType.GetRegulation(item.Regulation), 
                        item.ClientType,
                        item.IsExternal,
                        (GameTechnology)item.Technology,
                        VersionNumber.Parse(item.QAApprovedVersion),
                        VersionNumber.Parse(item.PMApprovedVersion)))
                        .ToArray();
            }
        }

    }
}
