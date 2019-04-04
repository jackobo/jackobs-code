using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using GamesPortal.Service.Artifactory;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Entities;
using Microsoft.Practices.Unity;
using Spark.Infra.Types;

namespace GamesPortal.Service
{
    public class GamesPortalApprovalService : WcfServiceBase, IGamesPortalApprovalService
    {

        //This constructor is for design time purpose only
        private GamesPortalApprovalService()
        {
        }

        
        public GamesPortalApprovalService(IGamesPortalInternalServices services)
            : base(services)
        {
            
        }
        

        public void QAApprove(QAApproveRequest request)
        {
            try
            {
                Approve(request);
            }
            catch (Exception ex)
            {
                LogException("QAApprove", ex, request);
                throw;
            }
        }

        public void PMApprove(PMApproveRequest request)
        {
            try
            {
                Approve(request);
            }
            catch (Exception ex)
            {
                LogException("PMApprove", ex, request);
                throw;
            }
        }


        private void Approve(ApproveRequestBase request)
        {
            using (var dbContext = Services.CreateGamesPortalDBDataContext())
            {
                var gameVersionRow = dbContext.GetTable<GameVersion>().FirstOrDefault(row => row.GameVersion_ID == request.GameVersionID);
                if (gameVersionRow == null)
                    throw new ArgumentException(string.Format("Could not find a game version with ID {0}", request.GameVersionID));

                var approvalDate = Services.TimeServices.Now;
                var approvalUser = Services.CallContextServices.GetCallingUserName();


                foreach (var regulationRow in gameVersionRow.GameVersion_Regulations.Where(row => request.Regulations.Contains(row.Regulation)).ToArray())
                {
                    
                    var artifactoryRepository = FindArtifactoryRepository(new GameInfrastructureDTO(gameVersionRow.Technology, gameVersionRow.PlatformType),
                                                               gameVersionRow.Game.IsExternal,
                                                               (GamingComponentCategory)gameVersionRow.Game.ComponentCategory);

                    var artifactoryProperty = request.GetArtifactoryProperty();

                    artifactoryRepository.UpdateArtifactProperties(new UpdateArtifactPropertiesRequest(gameVersionRow.Game.MainGameType,
                                                                                                          regulationRow.Regulation,
                                                                                                          gameVersionRow.VersionFolder,
                                                                                                          artifactoryProperty
                                                                                                          ));
                    request.SetApprovalInfo(regulationRow, approvalDate, approvalUser);
                    UpdateGameVersionProperties(gameVersionRow, regulationRow.Regulation, artifactoryProperty, approvalDate, approvalUser);
                }
                
                dbContext.SubmitChanges();


                if (HubContext != null)
                {
                    HubContext.GameSynchronizationFinished(new SignalR.GameSynchronizationFinishedData(gameVersionRow.Game_ID, RecordChangeType.Changed, gameVersionRow.Game.IsExternal));
                }

            }
        }
        
        private SignalR.IGamesPortalHubContext HubContext
        {
            get
            {
                return Services.GamesPortalHubContext;
            }
        }

        private void UpdateGameVersionProperties(GameVersion gameVersionRow, string regulation, ArtifactoryProperty property, DateTime changeTime, string userName)
        {


            var propertyRow = gameVersionRow.GameVersion_Properties.FirstOrDefault(row => row.PropertyKey == property.Key && row.Regulation == regulation);
            string oldValue = null;
            RecordChangeType changeType;
            if (propertyRow == null)
            {
                propertyRow = new GameVersion_Property() { Row_ID = Guid.NewGuid() };
                changeType = RecordChangeType.Added;
                gameVersionRow.GameVersion_Properties.Add(propertyRow);
            }
            else
            {
                oldValue = propertyRow.PropertyValue;
                changeType = RecordChangeType.Changed;
            }

            propertyRow.ChangedBy = userName;
            propertyRow.GameVersion_ID = gameVersionRow.GameVersion_ID;
            propertyRow.LastChange = changeTime;
            propertyRow.PropertyKey = property.Key;
            propertyRow.PropertyName = property.Name;
            propertyRow.PropertySet = property.SetName;
            propertyRow.PropertyValue = property.ConcatValues();
            propertyRow.Regulation = regulation;

            gameVersionRow.GameVersion_Property_Histories.Add(new GameVersion_Property_History()
            {
                ChangeDate = changeTime,
                ChangedBy = userName,
                ChangeType = (int)changeType,
                GameVersion_ID = gameVersionRow.GameVersion_ID,
                NewValue = property.ConcatValues(),
                OldValue = oldValue,
                PropertyKey = property.Key,
                Regulation = regulation
            });


        }

        private IArtifactoryRepository FindArtifactoryRepository(GameInfrastructureDTO gameInfrastructure, bool isExternal, GamingComponentCategory category)
        {
            return Services.ArtifactorySynchronizationManager.FindRepositoryDescriptor(gameInfrastructure, isExternal, category).Repository;
            
        }

    

        public GetAvailableQAApprovalStatesResponse GetAvailableQAApprovalStates()
        {
            return new GetAvailableQAApprovalStatesResponse()
            {
                States = new string[] { "Approved"/*, "Rejected", "InProgress", "NotRelevant"*/ }
            };
        }


        public GetAvailablePMApprovalStatesResponse GetAvailablePMApprovalStates()
        {
            return new GetAvailablePMApprovalStatesResponse()
            {
                States = new string[] { "True", /*"False" */}
            };
        }

       

        public ForceSynchronizationResponse ForceSynchronization()
        {
            var artifactoryGateway = Services.ArtifactorySynchronizationManager;

            if (artifactoryGateway.IsGamesSynchronizationInProgress)
                return new ForceSynchronizationResponse() { SynchronizationAlreadyInProgress = true };

            artifactoryGateway.ForceGamesSynchronization(Services.CallContextServices.GetCallingUserName());

            return new ForceSynchronizationResponse();
        }

        public void LanguageApprove(LanguageApproveRequest request)
        {
            using (var dbContext = Services.CreateGamesPortalDBDataContext())
            {
                

                var approver = CreateGameLanguageApprover(dbContext);
                var languages = request.Languages.Select(lng => Services.LanguageDictionary.FindLanguage(lng)).ToArray();
                var approvalResult = approver.Approve(new GameLanguageApprover_Request(
                                                                Services.TimeServices.Now,
                                                                Services.CallContextServices.GetCallingUserName(),
                                                                request.GameVersionId,
                                                                languages));

              

                dbContext.SubmitChanges();

                if (approvalResult.AffectedGameVersions.Any())
                {
                    Services.GameLanguageToArtifactorySynchronizer.Run();
                }


                foreach (Game game in approvalResult.AffectedGames)
                {
                    Services.GamesPortalHubContext.GameSynchronizationFinished(new SignalR.GameSynchronizationFinishedData(game.Game_ID, RecordChangeType.Changed, game.IsExternal));
                }


            }
        }

        private IGameLanguageApprover CreateGameLanguageApprover(IGamesPortalDataContext dbContext)
        {
            return Services.CreateGameLanguageApprover(dbContext);
        }
        
        public void ForceGameSynchronization(ImplicitForceGameSynchronizationRequest request)
        {
            try
            {
                
                using (var dbContext = Services.CreateGamesPortalDBDataContext())
                {
                    var gameRecord = dbContext.GetTable<Game>().FirstOrDefault(row => row.MainGameType == request.MainGameType && row.IsExternal == request.isExternal);
                    if (gameRecord == null)
                        throw new ArgumentException($"Can't find a game with game type {request.MainGameType} and IsExternal = {request.isExternal}");

                    foreach(var gameInfrastructure in gameRecord.GameVersions.Select(gv => new GameInfrastructureDTO(gv.Technology, gv.PlatformType)).Distinct())
                    {
                        
                        var repositoryDescriptor = Services.ArtifactorySynchronizationManager.FindRepositoryDescriptor(gameInfrastructure, gameRecord.IsExternal, (GamingComponentCategory)gameRecord.ComponentCategory);

                        Services.ArtifactorySynchronizationManager.ForceGameSynchronization(gameRecord.MainGameType, repositoryDescriptor);
                    }

                }

                

                
            }
            catch (Exception ex)
            {
                LogException(nameof(ForceGameSynchronization), ex, request);
                throw;
            }
        }

        public void ForceLanguageSynchronization(ForceLanguageSynchronizationRequest request)
        {
            using (var dbContext = this.Services.CreateGamesPortalDBDataContext())
            {
                var version = new VersionNumber(request.Version).ToLong();
                var gameVersionIds = (from game in dbContext.GetTable<Game>()
                                    join gameVersion in dbContext.GetTable<GameVersion>() on game.Game_ID equals gameVersion.Game_ID
                                    where game.MainGameType == request.GameType
                                           && gameVersion.Technology == (int)request.GameTechnology
                                           && gameVersion.PlatformType == (int)request.PlatformType
                                           && gameVersion.VersionAsLong == version
                                    select gameVersion.GameVersion_ID)
                                    .ToArray();

                foreach (var versionId in gameVersionIds)
                {
                    dbContext.GetTable<GameVersion_Language_ToArtifactorySyncQueue>().InsertOnSubmit(
                                new GameVersion_Language_ToArtifactorySyncQueue()
                                {
                                    GameVersion_ID = versionId,
                                    InsertedTime = DateTime.Now
                                }
                        );
                }

                dbContext.SubmitChanges();

                this.Services.GameLanguageToArtifactorySynchronizer.Run();
            }

            
        }
    }
}
