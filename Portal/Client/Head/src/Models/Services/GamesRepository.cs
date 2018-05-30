using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamesPortal.Client.Interfaces;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.PubSubEvents;
using GamesPortal.Client.Interfaces.Services;
using GamesPortal.Client.Models.GamesPortalService;
using Spark.Infra.Types;
using Prism.Events;
using Spark.Wpf.Common;
using Spark.Infra.Exceptions;

namespace GamesPortal.Client.Models.Services
{
    public class GamesRepository : IGamesRepository, IReportingService, IGamesRepositorySynchronizer
    {

        public GamesRepository()
        {            
            this.AllGames = ReadAllGames();
        }

        Dictionary<Guid, Game> AllGames { get; set; }


        Game IGamesRepositorySynchronizer.RefreshGame(Guid gameId)
        {
            var gameFromDb = ReadGameFromDatabase(gameId);
            if (AllGames.ContainsKey(gameId))
            {
                UpdateGame(gameFromDb, AllGames[gameId]);
            }
            else
            {
                AllGames.Add(gameFromDb.Id, gameFromDb);
            }

            return AllGames[gameId];
        }


        void IGamesRepositorySynchronizer.GameRemoved(Guid gameId)
        {
            if (AllGames.ContainsKey(gameId))
                AllGames.Remove(gameId);
        }

        private void UpdateGame(Game gameFromDb, Game exitingGame)
        {
            exitingGame.Name = gameFromDb.Name;
            exitingGame.IsExternal = gameFromDb.IsExternal;
            exitingGame.MainGameType = gameFromDb.MainGameType;
            exitingGame.SupportedInfrastructures = gameFromDb.SupportedInfrastructures;
            exitingGame.GameTypes = gameFromDb.GameTypes;
            exitingGame.ResetVersions();
        }

        public Game GetGame(Guid gameId)
        {
            if (this.AllGames.ContainsKey(gameId))
                return this.AllGames[gameId];
                        
            var game = ReadGameFromDatabase(gameId);

            AllGames.Add(game.Id, game);

            return game;
        }

        private Game ReadGameFromDatabase(Guid gameId)
        {
            using (var proxy = CreateProxy())
            {
                return CreateGameFromEntity(proxy.GetGame(gameId).Game);
            }
        }

        private  Dictionary<Guid, Interfaces.Entities.Game> ReadAllGames()
        {
            using (var proxy = CreateProxy())
            {
                return proxy.GetAllGames().Games.Select(entity => CreateGameFromEntity(entity)).ToDictionary(g => g.Id);
            }
        }

        public Interfaces.Entities.Game[] GetAllGames()
        {
            return this.AllGames.Values.ToArray();
        }

      
        public Game CreateGameFromEntity(GamesPortalService.GameDTO gameEntity)
        {
            return new Game(gameEntity.Id,
                            gameEntity.Name,
                            gameEntity.MainGameType,
                            gameEntity.IsExternal,
                            ConvertComponentCategory(gameEntity.Category),
                            gameEntity.SupportedInfrastructures.Select(infra => ConvertGameInfrastructure(infra)).ToArray(),
                            gameEntity.GameTypes.Select(gt => new Interfaces.Entities.GameType(gt.Id, gt.Name, gt.OperatorId)).ToArray(),
                            this);

        }

        private Interfaces.Entities.GamingComponentCategory ConvertComponentCategory(GamesPortalService.GamingComponentCategory category)
        {
            return (Interfaces.Entities.GamingComponentCategory)(int)category;
        }

        private GameInfrastructure ConvertGameInfrastructure(GameInfrastructureDTO infra)
        {
            return new GameInfrastructure((int)infra.GameTechnology, (int)infra.PlatformType);
        }

        public Interfaces.Entities.GameVersion[] GetGameVersions(Guid gameId)
        {
            var result = new List<GameVersion>();
            using (var proxy = new GamesPortalServiceClient())
            {
                var response = proxy.GetGameVersions(gameId);


                foreach (var version in response.GameVersions)
                {
                    var gameVersion = new GameVersion();

                    gameVersion.Id = version.VersionId;
                    gameVersion.CreatedBy = version.CreatedBy;
                    gameVersion.TriggeredBy = version.TriggeredBy;
                    gameVersion.CreatedDate = version.CreatedDate;
                    gameVersion.Infrastructure = ConvertGameInfrastructure(version.GameInfrastructure);
                    gameVersion.Category = ConvertComponentCategory(version.ComponentCategory);
                    gameVersion.Version = new VersionNumber(version.Version);
                    gameVersion.PropertiesChangeHistory = version.PropertiesChangeHistory
                                                                .Select(item => new GameVersionPropertyChangedHistory(
                                                                                                item.PropertyKey,
                                                                                                item.OldValue,
                                                                                                item.NewValue,
                                                                                                item.Regulation,
                                                                                                item.ChangeDate,
                                                                                                item.ChangedBy,
                                                                                                (ChangeType)item.ChangeType))
                                                                .ToArray();



                    gameVersion.Regulations = version.Regulations.Select(x => BuildGameVersionRegulation(x)).ToArray();



                    result.Add(gameVersion);
                }


                return result.ToArray();
            }
        }

        private GameVersionRegulation BuildGameVersionRegulation(GameVersionRegulationDTO x)
        {
            return new GameVersionRegulation(RegulationType.GetRegulation(x.Regulation),
                                            CreateDownloadInfo(x.DownloadInfo),
                                            new GameVersionApprovalInfo(BuildApproval(x.QAApprovalInfo),
                                                                        BuildApproval(x.PMApprovalInfo),
                                                                        BuildProductionUploadInfo(x.ProductionUploadInfo)),
                                            x.Languages.Select(lng => BuildGameVersionRegulationLanguage(lng)).ToArray());
        }

        private GameVersionRegulationLanguage BuildGameVersionRegulationLanguage(GameVersionRegulationLanguageDTO languageDTO)
        {
            return new GameVersionRegulationLanguage(CreateLanguage(languageDTO.Language),
                                                     new LanguageApprovalInfo(BuildApproval(languageDTO.QaApprovalInfo), 
                                                                              languageDTO.IsMandatory, 
                                                                              BuildProductionUploadInfo(languageDTO.ProductionUploadInfo)));
        }

        
        private Language CreateLanguage(LanguageDTO language)
        {
            return new Language(language.Name, language.Iso2, language.Iso3);
        }

        private IApproval BuildApproval(ApprovalInfoDTO approvalInfo)
        {
            if (approvalInfo == null)
                return NotApproved.Instance;

            return new Approved(approvalInfo.ApprovalDate, approvalInfo.ApprovedBy);
        }

        private IProductionUploadInfo BuildProductionUploadInfo(ProductionUploadInfoDTO productionUploadInfo)
        {
            if (productionUploadInfo == null)
                return new NotInProduction();
            else
                return new InProduction(productionUploadInfo.UploadDate);
        }

        private class NotInProduction : IProductionUploadInfo
        {
            public string Description
            {
                get
                {
                    return string.Empty;
                }
            }

            public bool IsInProduction
            {
                get
                {
                    return false;
                }
            }
        }

        private class InProduction : IProductionUploadInfo
        {
            public InProduction(DateTime uploadDate)
            {
                _uploadDate = uploadDate;
            }

            DateTime _uploadDate;
            public string Description
            {
                get
                {
                    return _uploadDate.ToString();
                }
            }

            public bool IsInProduction
            {
                get
                {
                    return true;
                }
            }
        }

        public LatestApprovedGameVersionForEachRegulation[] GetLatestApprovedGameVersionForEachRegulationAndGameType()
        {
            using (var proxy = CreateProxy())
            {
                var response = proxy.GetLatestApprovedGameVersionForEachRegulation();

                return response.LatestApprovedGamesVersions.Select(item => 
                    new LatestApprovedGameVersionForEachRegulation(
                        item.GameId, 
                        item.GameName, 
                        item.MainGameType, 
                        VersionNumber.Parse(item.LastVersion),
                        RegulationType.GetRegulation(item.Regulation), 
                        item.IsExternal,
                        ConvertGameInfrastructure(item.GameInfrastructure),
                        CreateLatestVersionInfo(item.QAVersionInfo),
                        CreateLatestVersionInfo(item.PMVersionInfo),
                        CreateLatestVersionInfo(item.ProductionVersionInfo), 
                        item.LatestQAApprovedVersion
                        ))
                        .ToArray();
            }
        }

        private LatestApprovedGameVersionForEachRegulation.LatestVersionInfo CreateLatestVersionInfo(LatestVersionInfoDTO versionInfo)
        {
            if (versionInfo == null)
                return null;

            return new LatestApprovedGameVersionForEachRegulation.LatestVersionInfo(versionInfo.VersionId, new VersionNumber(versionInfo.Version), CreateDownloadInfo(versionInfo.DownloadInfo));
        }

        private DownloadInfo CreateDownloadInfo(DownloadInfoDTO downloadInfo)
        {
            if (downloadInfo == null)
                return null;

            return new DownloadInfo(downloadInfo.Uri, downloadInfo.FileName, downloadInfo.FileSize, downloadInfo.MD5);
        }

        public void QAApprove(Guid gameVersionID, RegulationType[] regulations)
        {
            using (var proxy = CreateAuthenticatedProxy())
            {
                proxy.QAApprove(new GamesPortalApprovalService.QAApproveRequest()
                {
                    GameVersionID = gameVersionID,
                    Regulations = regulations.Select(r => r.Name).ToArray()
                });
            }
        }

        
        public void PMApprove(Guid gameVersionID, RegulationType[] regulations)
        {
            using (var proxy = CreateAuthenticatedProxy())
            {
                proxy.PMApprove(new GamesPortalApprovalService.PMApproveRequest()
                {
                    GameVersionID = gameVersionID,
                    Regulations = regulations.Select(r => r.Name).ToArray()
                });
            }
        }

        private static GamesPortalApprovalService.GamesPortalApprovalServiceClient CreateAuthenticatedProxy()
        {
            return new GamesPortalApprovalService.GamesPortalApprovalServiceClient();
        }

      

        public string[] GetAvailableQAApprovalStates()
        {
            using (var proxy = new GamesPortalApprovalService.GamesPortalApprovalServiceClient())
            {
                return proxy.GetAvailableQAApprovalStates().States;
            }
        }

     

        public string[] GetAvailablePMApprovalStates()
        {
            using (var proxy = new GamesPortalApprovalService.GamesPortalApprovalServiceClient())
            {
                return proxy.GetAvailablePMApprovalStates().States;
            }
        }

        
     
        public void ForceSynchronization()
        {
            using (var proxy = new GamesPortalApprovalService.GamesPortalApprovalServiceClient())
            {
                var response = proxy.ForceSynchronization();

                if (response.SynchronizationAlreadyInProgress)
                    throw new ValidationException("Synchronnization already in progress!");
            }
        }


        #region IReportingService Members


        public LatestGameVersionForRegulation[] GetLatestGameVersionForEachRegulation()
        {
            using (var proxy = CreateProxy())
            {
                var response = proxy.GetLatestGameVersionForEachRegulation();

                return response.Versions.Select(item => new LatestGameVersionForRegulation(item.GameId, 
                    item.GameName, 
                    item.MainGameType, 
                    item.IsExternal, 
                    item.GameVersionId, 
                    ConvertGameInfrastructure(item.GameInfrastructure), 
                    item.Regulation, 
                    item.Version,
                    CreateDownloadInfo(item.DownloadInfo))).ToArray();
            }
        }

        public GameVersionRelease[] GetGameReleases(DateTimeInterval inPeriod)
        {
            using (var proxy = CreateProxy())
            {
                var gameVersions = proxy.GetGameReleases(new GetGamesReleasesRequest() { FromDate = inPeriod.StartDate, ToDate = inPeriod.EndDate })
                                        .GameVersions;

                return
                     gameVersions.GroupBy(item => new { item.CreatedBy, item.GameId, item.GameInfrastructure, item.GameVersionId, item.IsExternal, item.MainGameType, item.Name, item.TriggeredBy, item.Version })
                     .Select(group =>
                     {
                         var dto = group.Key;
                         return new GameVersionRelease(dto.GameId,
                                                        dto.MainGameType,
                                                        dto.Name,
                                                        dto.IsExternal,
                                                        ConvertGameInfrastructure(dto.GameInfrastructure),
                                                        dto.GameVersionId,
                                                        new VersionNumber(dto.Version),
                                                        string.Join(", ", group.Select(x => x.Regulation)),
                                                        group.Max(x => x.CreatedDate),
                                                        dto.CreatedBy,
                                                        dto.TriggeredBy);
                     }).ToArray();

            }
        }

        private static GamesPortalServiceClient CreateProxy()
        {
            return new GamesPortalServiceClient();
        }

        public NeverApprovedGame[] GetNeverApprovedGames()
        {
            using (var proxy = CreateProxy())
            {
                return proxy.GetNeverApprovedGames().Games.Select(g =>
                        new NeverApprovedGame(
                                    g.GameName,
                                    g.MainGameType,
                                    ConvertGameInfrastructure(g.GameInfrastructure),
                                    new VersionNumber(g.LatestVersion))).ToArray();

            }
        }

        public void LanguageApprove(Guid gameVersionId, string[] languages)
        {
            using (var proxy = CreateAuthenticatedProxy())
            {
                proxy.LanguageApprove(new GamesPortalApprovalService.LanguageApproveRequest()
                {
                    GameVersionId = gameVersionId,
                    Languages = languages
                });
            }
        }

        public void ForceGameSynchronization(Game game)
        {
            using (var proxy = CreateAuthenticatedProxy())
            {
                proxy.ForceGameSynchronization(new GamesPortalApprovalService.ImplicitForceGameSynchronizationRequest()
                {
                    MainGameType = game.MainGameType,
                    isExternal = game.IsExternal
                });
            }
        }

        IMandatoryLanguagesProvider _mandatoryLanguagesProvider;

        public IMandatoryLanguagesProvider GetMandatoryLanguagesPerRegulationProvider()
        {
            if (_mandatoryLanguagesProvider == null)
            {
                var mandatoryLanguagesProvider = new MandatoryLanguagesPerRegulationProvider();
                using (var proxy = CreateProxy())
                {
                    foreach (var regulationInfo in proxy.GetRegulationsInfo().Regulations)
                    {
                        mandatoryLanguagesProvider.AddMandatoryLanguages(RegulationType.GetRegulation(regulationInfo.RegulationName),
                                                                         regulationInfo.MandatoryLanguages.Select(ml => CreateLanguage(ml)));
                    }
                }

                _mandatoryLanguagesProvider = mandatoryLanguagesProvider;
            }

            return _mandatoryLanguagesProvider;
        }

        public ApprovedGameVersion[] GetQAApprovedGamesInPeriod(DateTime startDate, DateTime endDate)
        {
            using (var proxy = CreateProxy())
            {
                return proxy.GetQAApprovedGamesInPeriod(new GetApprovedGamesInPeriodRequest()
                {
                    StartDate = startDate,
                    EndDate = endDate
                })
                .ApprovedGames
                .Select(g => new ApprovedGameVersion(g.GameName,
                                                    g.MainGameType,
                                                    g.IsExternal,
                                                    ConvertComponentCategory(g.Category),
                                                    VersionNumber.Parse(g.Version),
                                                    ConvertGameInfrastructure(g.GameInfra),
                                                    RegulationType.GetRegulation(g.Regulation),
                                                    BuildApproval(g.ApprovalInfo)))
                .ToArray();
            }
        }

        #endregion
    }

  
   
}
