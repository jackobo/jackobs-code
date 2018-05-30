using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using GamesPortal.Service.Artifactory;
using Spark.Infra.Configurations;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Entities;
using Spark.Infra.Types;
using Spark.Infra.Logging;
using Microsoft.Practices.Unity;

namespace GamesPortal.Service
{
    
    public class GamesPortalService : WcfServiceBase, IGamesPortalService
    {
        //This constructor is for design time purpose only
        private GamesPortalService()
        {

        }

        public GamesPortalService(IGamesPortalInternalServices services)
            : base(services)
        {
        }


        public GetGameResponse GetGame(Guid gameId)
        {
            using (var dbContext = Services.CreateGamesPortalDBDataContext())
            {
                var gameRow = dbContext.GetTable<Game>().FirstOrDefault(row => row.Game_ID == gameId);

                if (gameRow == null)
                    throw new ArgumentException(string.Format("There is no game with ID {0} in the database", gameId));

                return new GetGameResponse(new GameDTO(gameRow.Game_ID,
                                              gameRow.GameName,
                                              gameRow.MainGameType,
                                              gameRow.IsExternal,
                                              (GamingComponentCategory)gameRow.ComponentCategory,
                                              gameRow.GameVersions.GroupBy(row => new { row.Technology, row.PlatformType })
                                                                  .Select(g => new GameInfrastructureDTO(g.Key.Technology, g.Key.PlatformType))
                                                                  .ToArray(),
                                              gameRow.GameTypes.Select(gt => new GameTypeDTO(gt.GameType_ID, gt.Name, gt.Operator_ID)).ToArray())
                                              );
            }
        }


        public GetAllGamesResponse GetAllGames()
        {
            try
            {
                using (var dbContext = Services.CreateGamesPortalDBDataContext())
                {
                    var supportedInfrastructuresPerGame = dbContext.GetTable<GameVersion>().Select(row => new { row.Game_ID, row.Technology, row.PlatformType })
                                          .Distinct()
                                          .ToArray()
                                          .GroupBy(row => row.Game_ID)
                                          .ToDictionary(row => row.Key, row => row.Select(item => new GameInfrastructureDTO(item.Technology, item.PlatformType)).ToArray());

                    var gameTypes = dbContext.GetTable<GameType>().ToArray()
                                    .GroupBy(gt => gt.Game_ID)
                                    .ToDictionary(g => g.Key, g => g.Select(row => new Entities.GameTypeDTO(row.GameType_ID, row.Name, row.Operator_ID)).ToArray());

                    return new GetAllGamesResponse( dbContext.GetTable<Game>().Select(row => new Entities.GameDTO(row.Game_ID,
                                                                                row.GameName,
                                                                                row.MainGameType,
                                                                                row.IsExternal,
                                                                                (GamingComponentCategory)row.ComponentCategory,
                                                                                supportedInfrastructuresPerGame.ContainsKey(row.Game_ID) ? supportedInfrastructuresPerGame[row.Game_ID] : new GameInfrastructureDTO[0],
                                                                                gameTypes.ContainsKey(row.Game_ID) ? gameTypes[row.Game_ID] : new GameTypeDTO[0]))
                                                                        .ToArray()
                                                    );
                }

                

            }
            catch (Exception ex)
            {
                LogException("GetAllGames", ex);
                throw;
            }
        }


        public GetGameVersionsResponse GetGameVersions(Guid gameId)
        {
            try
            {
                using (var dbContext = Services.CreateGamesPortalDBDataContext())
                {
                    

                    var result = new List<GameVersionDTO>();
                    foreach(var gameVersionRecord in dbContext.GetTable<GameVersion>().Where(record => record.Game_ID == gameId))
                    {
                        var gameVersionEntity = new GameVersionDTO(gameVersionRecord.GameVersion_ID,
                                                                      new VersionNumber(gameVersionRecord.VersionAsLong).ToString(),
                                                                      gameVersionRecord.VersionAsLong,
                                                                      gameVersionRecord.CreatedDate,
                                                                      gameVersionRecord.CreatedBy,
                                                                      gameVersionRecord.TriggeredBy,
                                                                      new GameInfrastructureDTO(gameVersionRecord.Technology, gameVersionRecord.PlatformType),
                                                                      (GamingComponentCategory)gameVersionRecord.Game.ComponentCategory,
                                                                      GetVersionRegulations(gameVersionRecord),
                                                                      GetGameVersionPropertyChangeHistory(gameVersionRecord));

                        result.Add(gameVersionEntity);
                                                                      
                    }

                    return new GetGameVersionsResponse(result.ToArray());
                }
            }
            catch (Exception ex)
            {

                LogException("GetGameVersions", ex);
                throw;
            }
        }

      

        private GameVersionPropertyChangeHistoryDTO[] GetGameVersionPropertyChangeHistory(GameVersion gameVersionRecord)
        {
            var result = new List<GameVersionPropertyChangeHistoryDTO>();

            foreach(var gameVersionHistoryRecord in gameVersionRecord.GameVersion_Property_Histories)
            {
                result.Add(new GameVersionPropertyChangeHistoryDTO(gameVersionHistoryRecord.PropertyKey, 
                                                                        gameVersionHistoryRecord.OldValue, 
                                                                        gameVersionHistoryRecord.NewValue, 
                                                                        gameVersionHistoryRecord.Regulation, 
                                                                        gameVersionHistoryRecord.ChangeDate, 
                                                                        gameVersionHistoryRecord.ChangedBy, 
                                                                        gameVersionHistoryRecord.ChangeType));
            }

            return result.ToArray();
        }

        private GameVersionRegulationDTO[] GetVersionRegulations(GameVersion gameVersionRecord)
        {
            var result = new List<GameVersionRegulationDTO>();

            var languagesPerRegulation = gameVersionRecord.GameVersion_Languages
                                                          .GroupBy(record => record.Regulation)
                                                          .ToDictionary(group => group.Key, group => group.ToArray());
            foreach (var regulationRecord in gameVersionRecord.GameVersion_Regulations)
            {
                var languages = GetLanguagesForRegulation(regulationRecord, languagesPerRegulation);

                result.Add(new GameVersionRegulationDTO(regulationRecord.Regulation,
                                                        DownloadInfoDTO.CreateOrNull(regulationRecord.DownloadUri, regulationRecord.FileName, regulationRecord.FileSize, regulationRecord.MD5),
                                                        ApprovalInfoDTO.CreateInstanceOrNull(regulationRecord.QAApprovalDate, regulationRecord.QAApprovalUser),
                                                        ApprovalInfoDTO.CreateInstanceOrNull(regulationRecord.PMApprovalDate, regulationRecord.PMApprovalUser),
                                                        ProductionUploadInfoDTO.CreateInstanceOrNull(regulationRecord.ProductionUploadDate),
                                                        languages));
            }

            return result.ToArray();
        }

        private GameVersionRegulationLanguageDTO[] GetLanguagesForRegulation(GameVersion_Regulation regulationRecord, Dictionary<string, GameVersion_Language[]> languagesPerRegulation)
        {
            if (!languagesPerRegulation.ContainsKey(regulationRecord.Regulation))
                return new GameVersionRegulationLanguageDTO[0];

            var regulation = Services.RegulationsDictionary.GetRegulation(regulationRecord.Regulation);


            return languagesPerRegulation[regulationRecord.Regulation]
                    .Select(langRecord =>
                    {
                        var language = Services.LanguageDictionary.FindLanguage(langRecord.Language);
                        return new GameVersionRegulationLanguageDTO(
                                                language.ToLanguageDTO(),
                                                ApprovalInfoDTO.CreateInstanceOrNull(langRecord.QAApprovalDate, langRecord.QAApprovalUser),
                                                ProductionUploadInfoDTO.CreateInstanceOrNull(langRecord.ProductionUploadDate),
                                                regulation.IsLanguageMandatory(language));
                    })
                    .ToArray();
        }
        
        /*
        private bool IsClientTypeSupportedByRegulation(string regulation, string clientType)
        {
            return this.Services.RegulationsDictionary.GetRegulation(regulation).IsClientTypeSupported(clientType);
        }
        */
        
        public GetLatestApprovedGameVersionForEachRegulation GetLatestApprovedGameVersionForEachRegulation()
        {
           
            try
            {
                using (var dbContext = Services.CreateGamesPortalDBDataContext())
                {


                    var rows = dbContext.GetTable<LatestApprovedGameVersionForEachRegulation>()
                                                                                     .ToArray()
                                                                                     .Select(row => new LatestApprovedGameVersionForRegulationDTO(row.Game_ID,
                                                                                     row.GameName,
                                                                                     row.LatestVersion,
                                                                                     row.MainGameType,
                                                                                     row.Regulation,
                                                                                     row.IsExternal,
                                                                                     new GameInfrastructureDTO(row.Technology, row.PlatformType),
                                                                                     LatestVersionInfoDTO.CreateOrNull(row.QA_VersionID, row.QA_Version, DownloadInfoDTO.CreateOrNull(row.QA_DownloadUri, row.QA_FileName, row.QA_FileSize, row.QA_MD5)),
                                                                                     LatestVersionInfoDTO.CreateOrNull(row.PM_VersionID, row.PM_Version, DownloadInfoDTO.CreateOrNull(row.PM_DownloadUri, row.PM_FileName, row.PM_FileSize, row.PM_MD5)),
                                                                                     LatestVersionInfoDTO.CreateOrNull(row.PROD_VersionID, row.PROD_Version, DownloadInfoDTO.CreateOrNull(row.PROD_DownloadUri, row.PROD_FileName, row.PROD_FileSize, row.PROD_MD5)),
                                                                                     row.LatestQAApprovedVersion
                                                                                     ))
                                                                                     .ToArray();

                    return new GetLatestApprovedGameVersionForEachRegulation(rows);
                }

            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
            }
          
        }

        

        #region IGamesPortalService Members


        public GetLatestGameVersionForEachGameResponse GetLatestGameVersionForEachRegulation()
        {
            using (var dbContext = Services.CreateGamesPortalDBDataContext())
            {
                return new GetLatestGameVersionForEachGameResponse(
                    dbContext.GetTable<LatestVersionForEachGameAndRegulation>()
                             .Select(row => new LatestGameVersionForRegulationDTO(row.Game_ID, 
                                                                         row.GameName, 
                                                                         row.MainGameType, 
                                                                         row.IsExternal, 
                                                                         row.GameVersion_ID, 
                                                                         new GameInfrastructureDTO(row.Technology, row.PlatformType), 
                                                                         row.Regulation, 
                                                                         row.VersionAsLong.Value,
                                                                         DownloadInfoDTO.CreateOrNull(row.DownloadUri, row.FileName, row.FileSize, row.MD5)))
                             .ToArray());
            }
        }

        #endregion

        #region IGamesPortalService Members


        public void GetOptions()
        {
            //JUST A METHOD REQUIRED FOR ENABLING CORS IN THE ENDPOINT
        }

        public GetGamesReleasesResponse GetGameReleases(GetGamesReleasesRequest request)
        {
            try
            {
                using (var dbContext = Services.CreateGamesPortalDBDataContext())
                {
                    var query = (from game in dbContext.GetTable<Game>()
                                 join gameVersion in dbContext.GetTable<GameVersion>() on game.Game_ID equals gameVersion.Game_ID
                                 join gameVersionRegulation in dbContext.GetTable<GameVersion_Regulation>() on gameVersion.GameVersion_ID equals gameVersionRegulation.GameVersion_ID
                                 where request.FromDate <= gameVersion.CreatedDate && gameVersion.CreatedDate <= request.ToDate
                                 select new { game.Game_ID, game.MainGameType, game.GameName, game.IsExternal, gameVersion.GameVersion_ID, gameVersion.Technology, gameVersion.PlatformType, gameVersion.VersionAsLong, gameVersion.CreatedDate, gameVersion.CreatedBy, gameVersion.TriggeredBy, gameVersionRegulation.Regulation })
                             .GroupBy(item => new { item.Game_ID, item.MainGameType, item.GameName, item.IsExternal, item.GameVersion_ID, item.Technology, item.PlatformType, item.VersionAsLong, item.CreatedDate, item.CreatedBy, item.TriggeredBy, item.Regulation })
                             .Select(game => new Entities.GameVersionReleaseDTO(
                                game.Key.Game_ID,
                                game.Key.MainGameType,
                                game.Key.GameName,
                                game.Key.IsExternal,
                                new GameInfrastructureDTO(game.Key.Technology, game.Key.PlatformType),
                                game.Key.GameVersion_ID,
                                game.Key.VersionAsLong,
                                game.Key.Regulation,
                                null,
                                game.Key.CreatedDate,
                                game.Key.CreatedBy,
                                game.Key.TriggeredBy));



                    var items = query.ToArray();
                    return new GetGamesReleasesResponse(items);
                }
            }
            catch(Exception ex)
            {
                LogException(ex);
                throw;
            }
            
        }

        public GetNeverApprovedGameResponse GetNeverApprovedGames()
        {
            try
            {
                using (var dbContext = Services.CreateGamesPortalDBDataContext())
                {
                    return new GetNeverApprovedGameResponse(
                                    dbContext.GetTable<NeverApprovedGame>().Select(row => new NeverApprovedGameDto(row.GameName, row.MainGameType, new GameInfrastructureDTO(row.Technology, row.PlatformType), row.LatestVersion.Value))
                                                           .ToArray());
                }
            }
            catch(Exception ex)
            {
                LogException(ex);
                throw;
            }
        }

        public GetRegulationsInfoResponse GetRegulationsInfo()
        {
            try
            {
                var regulations = new List<RegulationDTO>();
                using (var dbContext = Services.CreateGamesPortalDBDataContext())
                {
                    foreach (var record in dbContext.GetTable<RegulationType>())
                    {
                        var mandatoryLanguages = record.RegulationType_MandatoryLanguages
                                                       .Select(mandatoryLangRecord =>
                                                       {
                                                           var lang = Services.LanguageDictionary.FindLanguage(mandatoryLangRecord.LanguageIso3);
                                                           return new LanguageDTO(lang.Name, lang.Iso2, lang.Iso3_1);
                                                       })
                                                       .ToArray();
                        regulations.Add(new RegulationDTO(record.RegulationName, mandatoryLanguages));
                    }
                }

                return new GetRegulationsInfoResponse(regulations.ToArray());
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
            }
        }

        public GetApprovedGamesInPeriodResponse GetQAApprovedGamesInPeriod(GetApprovedGamesInPeriodRequest request)
        {
            try
            {
                using (var dbContext = Services.CreateGamesPortalDBDataContext())
                {
                    var query = from game in dbContext.GetTable<Game>()
                                join gameVersion in dbContext.GetTable<GameVersion>() on game.Game_ID equals gameVersion.Game_ID
                                join gameVersionRegulation in dbContext.GetTable<GameVersion_Regulation>() on gameVersion.GameVersion_ID equals gameVersionRegulation.GameVersion_ID
                                where (gameVersionRegulation.QAApprovalDate != null 
                                       && request.StartDate <= gameVersionRegulation.QAApprovalDate && gameVersionRegulation.QAApprovalDate <= request.EndDate)

                                select new ApprovedGameVersionDTO(game.GameName,
                                                                    game.MainGameType,
                                                                    game.IsExternal,
                                                                    (GamingComponentCategory)game.ComponentCategory,
                                                                    VersionNumber.FromLong(gameVersion.VersionAsLong).ToString(),
                                                                    new GameInfrastructureDTO(gameVersion.Technology, gameVersion.PlatformType),
                                                                    gameVersionRegulation.Regulation,
                                                                    ApprovalInfoDTO.CreateInstanceOrNull(gameVersionRegulation.QAApprovalDate, gameVersionRegulation.QAApprovalUser));

                    return new GetApprovedGamesInPeriodResponse(query.ToArray());
                }
            }
            catch (Exception ex)
            {
                LogException(ex, request);
                throw;
            }
        }

        #endregion
    }
}
