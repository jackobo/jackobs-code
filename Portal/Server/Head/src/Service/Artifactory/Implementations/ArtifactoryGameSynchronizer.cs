using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using GamesPortal.Service.DataAccessLayer;
using GGPGameServer.ApprovalSystem.Common;

namespace GamesPortal.Service.Artifactory
{
    public class ArtifactoryGameSynchronizer : ArtifactorySynchronizer<IArtifactorySynchronizerDataAccessLayer, Game, GameVersion, GameVersion_Property, GameVersion_Regulation, GameVersion_Property_History>
    {
        public ArtifactoryGameSynchronizer(IGamesPortalInternalServices services, IArtifactorySyncronizationLogger logger)
            : base(services, logger)
        {
            
        }


        
        
        SignalR.IGamesPortalHubContext HubContext
        {
            get { return Services.GamesPortalHubContext; }
        }
        

        protected override IArtifactorySynchronizerDataAccessLayer CreateDataAccessLayer()
        {
            return Services.ArtifactorySynchronizerDataAccessLayer;
        }

        protected override Game FindComponent(IArtifactorySynchronizerDataAccessLayer dal, int componentID, bool isExternal)
        {
            return dal.GetGame(componentID, isExternal);
        }


        protected override string GetComponentTypeForLogging(int componentId)
        {
            return "GameType = " + componentId;
        }

        protected override Game CreateNewComponent(IArtifactorySynchronizerDataAccessLayer dal, ArtifactoryRepositoryDescriptor repositoryDescriptor, int componentID)
        {
            var game = new Game()
            {
                Game_ID = Guid.NewGuid(),
                MainGameType = componentID,
                IsExternal = repositoryDescriptor.IsExternal
            };
            
            dal.InsertGame(game);

            return game;
        }

        protected override void SubmitChanges(IArtifactorySynchronizerDataAccessLayer dal, Game component, bool isNew)
        {
            bool hasChanges = dal.HasChanges;

            

            dal.SubmitChanges();

            if (hasChanges)
            {
                this.HubContext.GameSynchronizationFinished(new SignalR.GameSynchronizationFinishedData(component.Game_ID,
                                                                                                        isNew ? RecordChangeType.Added : RecordChangeType.Changed,
                                                                                                        component.IsExternal));
                
            }

            


        }

    

        protected override void UpdateComponentProperties(IArtifactorySynchronizerDataAccessLayer dal, Game component, int componentId)
        {
            component.GameName = dal.GetGameName(componentId);

            if (string.IsNullOrEmpty(component.GameName))
                component.GameName = componentId.ToString();
        }


        protected override void OnBeforeSynchronizeVersions(IArtifactorySynchronizerDataAccessLayer dal, int componentId, Game component)
        {
            SynchronizeGameTypes(dal.GetGameTypes(componentId), component);
            
            base.OnBeforeSynchronizeVersions(dal, componentId, component);
            
        }


        private void SynchronizeGameTypes(GameTypeDescriptor[] availableGameTypes, Game game)
        {
            //select only the MainGameType (which is 888) and all the other game types that don't belong to 888
            availableGameTypes = (availableGameTypes ?? new GameTypeDescriptor[0]).Where(gt => gt.GameType == game.MainGameType 
                                                                                               || gt.OperatorId != OperatorEnum.Operator888)
                                                                                  .ToArray();

            var mainGameType = game.GameTypes.FirstOrDefault(gt => gt.GameType_ID == game.MainGameType);
            if (mainGameType == null) //make sure that the main game type is always added
                game.GameTypes.Add(new GameType() { GameType_ID = game.MainGameType, Operator_ID = 0, Name = game.GameName, Game_ID = game.Game_ID, Row_ID = Guid.NewGuid() });
         

            

            var existingGameTypes = game.GameTypes;

            //sync new game types
            foreach (var newGameTypes in availableGameTypes.Where(gt => !existingGameTypes.Select(g => g.GameType_ID).Contains(gt.GameType)).ToArray())
            {
                game.GameTypes.Add(new GameType()
                {
                    Game_ID = game.Game_ID,
                    GameType_ID = newGameTypes.GameType,
                    Name = newGameTypes.GameName,
                    Operator_ID = (int)newGameTypes.OperatorId,
                    Row_ID = Guid.NewGuid()
                });
            }

            //sync deleted game types
            foreach (var deletedGameType in existingGameTypes.Where(gt => !availableGameTypes.Select(g => g.GameType).Contains(gt.GameType_ID)).ToArray())
            {
                if (deletedGameType.GameType_ID != game.MainGameType) //delete only if is not the main game type.
                {
                    game.GameTypes.Remove(deletedGameType);
                }
            }


            //sync updated game types
            foreach (var item in (from available in availableGameTypes
                                  join existing in game.GameTypes on available.GameType equals existing.GameType_ID
                                  select new { available, existing }))
            {
                item.existing.GameType_ID = item.available.GameType;
                item.existing.Name = item.available.GameName;
                item.existing.Operator_ID = (int)item.available.OperatorId;
            }


        }

        
    }


   
}
