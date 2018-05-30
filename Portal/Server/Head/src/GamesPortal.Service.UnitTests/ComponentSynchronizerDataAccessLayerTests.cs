using System;
using NUnit.Framework;
using NSubstitute;
using GamesPortal.Service.Helpers;
using Spark.Infra.Data.LinqToSql;
using static GamesPortal.Service.Helpers.MockRecordsFactory;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.SignalR;

namespace GamesPortal.Service.Artifactory
{
    [TestFixture]
    public class ComponentSynchronizerDataAccessLayerTests
    {

        IGamesPortalInternalServices _internalServices;
        ComponentSynchronizerDataAccessLayer _artifactorySynchronizerDataAccessLayer;
        [SetUp]
        public void Setup()
        {
            _internalServices = GamesPortalInternalServicesHelper.Create();
            _internalServices.CreateGamesPortalDBDataContext().MockTable(new Game[0]);
            _artifactorySynchronizerDataAccessLayer = new ComponentSynchronizerDataAccessLayer(_internalServices);
        }


        [Test]
        public void SubmitChanges_GameLanguagesAreAffected_RunGameLanguageShynchronizerAfterSubmit()
        {
            var gamesPortalDb = _internalServices.CreateGamesPortalDBDataContext();
            var gameVersionId = Guid.NewGuid();
            var changes = ChangeSet<object>.CreateInserted(GameVersion_LanguageRecord(gameVersionId, "en", "English", "hash1"));
            gamesPortalDb.GetChanges().Returns(changes);

            var dal = _internalServices.CreateGamesPortalDBDataContext();

            _artifactorySynchronizerDataAccessLayer.SubmitChanges(GameRecord(13007));

            Received.InOrder(() =>
            {
                dal.SubmitChanges();
                _internalServices.GameLanguageToArtifactorySynchronizer.Received().Run();
            });
            
        }


        [Test]
        public void SubmitChanges_IfNoGameLanguagesIsAffected_DoNotRunGameLanguageShynchronizerAfterSubmit()
        {
            var gamesPortalDb = _internalServices.CreateGamesPortalDBDataContext();
            var gameVersionId = Guid.NewGuid();
            var changes = ChangeSet<object>.Empty();
            gamesPortalDb.GetChanges().Returns(changes);

            var dal = _internalServices.CreateGamesPortalDBDataContext();

            _artifactorySynchronizerDataAccessLayer.SubmitChanges(GameRecord(13007));

            _internalServices.GameLanguageToArtifactorySynchronizer.DidNotReceive().Run();

        }
        

        [Test]
        public void GetGameName_IfGameTypeExistsInSDM_ReturnItsName()
        {
            _internalServices.CreateSdmDbContext().MockTable(new SDM.GameType() { GMT_ID = 130017, GMT_Description = "Elm Street" });
            Assert.AreEqual("Elm Street", _artifactorySynchronizerDataAccessLayer.GetGameName(130017));
        }


        [Test]
        public void GetGameName_IfGameTypeDoesntExistsInSDM_ReturnItsNameFromTheApprovalSystem()
        {
            _internalServices.CreateSdmDbContext().MockTable(new SDM.GameType[0]);

            var componentRecord = new GGPVersioning.Component() { Name = "Elm Street" };
            _internalServices.CreateGGPVersioningDbContext().MockTable(new GGPVersioning.Component_GameType() { GameType = 130017, Component = componentRecord });

            Assert.AreEqual("Elm Street", _artifactorySynchronizerDataAccessLayer.GetGameName(130017));
        }


        [Test]
        public void GetGameName_IfGameTypeIsNotFoundAnyware_ReturnTheGameTypeAsTheName()
        {
            _internalServices.CreateSdmDbContext().MockTable(new SDM.GameType[0]);
            _internalServices.CreateGGPVersioningDbContext().MockTable(new GGPVersioning.Component_GameType[0]);

            Assert.AreEqual("130017", _artifactorySynchronizerDataAccessLayer.GetGameName(130017));
        }

        [Test]
        public void Dispose_WhenCalled_ShouldCallDisposeForAllInternalDBContexts()
        {
            _artifactorySynchronizerDataAccessLayer.Dispose();

            _internalServices.CreateGamesPortalDBDataContext().Received().Dispose();
            _internalServices.CreateGGPVersioningDbContext().Received().Dispose();
            _internalServices.CreateSdmDbContext().Received().Dispose();
        }

        [Test]
        public void GetGameTypes_IfNoGameTypesFoundForProvidedMainGameType_ReturnsEmtpyArray()
        {
            _internalServices.CreateGGPVersioningDbContext().MockTable(new GGPVersioning.Component_GameType[0]);
            Assert.AreEqual(0, _artifactorySynchronizerDataAccessLayer.GetGameTypes(130017).Length);
        }

        [Test]
        public void GetGameTypes_IfOnlyTheMainGameTypeIsAvailable_ReturnsOneRecord()
        {
            _internalServices.CreateSdmDbContext().MockTable(new SDM.GameType() { GMT_ID = 130017, GMT_Description = "Elm Street" });

            var record = Component_GameTypeRecord(130017, OperatorEnum.Operator888);
            _internalServices.CreateGGPVersioningDbContext().MockTable(record);
            Assert.AreEqual(1, _artifactorySynchronizerDataAccessLayer.GetGameTypes(130017).Length);
        }


        [Test]
        public void GetGameTypes_IfMultiple888GameTypesAreAvailableForTheSameGameType_ReturnsOnlyTheOneThatMatchTheRequestedGameType()
        {
            _internalServices.CreateSdmDbContext().MockTable(new SDM.GameType() { GMT_ID = 130017, GMT_Description = "Elm Street" });

            var record1 = Component_GameTypeRecord(130017, OperatorEnum.Operator888);
            var record2 = Component_GameTypeRecord(130018, OperatorEnum.Operator888);
            _internalServices.CreateGGPVersioningDbContext().MockTable(record1, record2);
            Assert.AreEqual(1, _artifactorySynchronizerDataAccessLayer.GetGameTypes(130017).Length);
        }

        [Test]
        public void GetGameTypes_IfOtherThan888GameTypesAreAvailable_ReturnsThoseToo()
        {
            
            _internalServices.CreateSdmDbContext().MockTable(SDM_GameTypeRecord(130017, "Elm Street"),
                                                                    SDM_GameTypeRecord(130018, "Elm Street2"),
                                                                    SDM_GameTypeRecord(9001, "Elm Street bingo1"),
                                                                    SDM_GameTypeRecord(9001, "Elm Street bingo2"));

            
            var record1 = Component_GameTypeRecord(130017, OperatorEnum.Operator888);
            var record2 = Component_GameTypeRecord(130018, OperatorEnum.Operator888);
            var record3 = Component_GameTypeRecord(9001, OperatorEnum.Bingo);
            var record4 = Component_GameTypeRecord(9002, OperatorEnum.Bingo);
            

            _internalServices.CreateGGPVersioningDbContext().MockTable(record1, record2, record3, record4);

            Assert.AreEqual(3, _artifactorySynchronizerDataAccessLayer.GetGameTypes(130017).Length);
        }

        [Test]
        public void SubmitChanges_IfNewGame_NotifySignalRClientsAccordingly()
        {
            var dal = _internalServices.CreateGamesPortalDBDataContext();

            var game = GameRecord(130017);
            game.IsExternal = true;
            var changes = ChangeSet<object>.CreateInserted(game);
            dal.GetChanges().Returns(changes);

            
            GameSynchronizationFinishedData gameSynchronizationFinishedData = null;
            _internalServices.GamesPortalHubContext.GameSynchronizationFinished(Arg.Do<GameSynchronizationFinishedData>(arg => gameSynchronizationFinishedData = arg));


            _artifactorySynchronizerDataAccessLayer.SubmitChanges(game);


            Assert.AreEqual(RecordChangeType.Added, gameSynchronizationFinishedData.ChangeType);
            Assert.AreEqual(game.Game_ID, gameSynchronizationFinishedData.GameId);
            Assert.AreEqual(game.IsExternal, gameSynchronizationFinishedData.IsExternal);

        }


        [Test]
        public void SubmitChanges_IfUpdatedGame_NotifySignalRClientsAccordingly()
        {
            var dal = _internalServices.CreateGamesPortalDBDataContext();

            var game = GameRecord(130017);
            game.IsExternal = true;
            var changes = ChangeSet<object>.CreateUpdated(game);
            dal.GetChanges().Returns(changes);


            GameSynchronizationFinishedData gameSynchronizationFinishedData = null;
            _internalServices.GamesPortalHubContext.GameSynchronizationFinished(Arg.Do<GameSynchronizationFinishedData>(arg => gameSynchronizationFinishedData = arg));


            _artifactorySynchronizerDataAccessLayer.SubmitChanges(game);


            Assert.AreEqual(RecordChangeType.Changed, gameSynchronizationFinishedData.ChangeType);
            Assert.AreEqual(game.Game_ID, gameSynchronizationFinishedData.GameId);
            Assert.AreEqual(game.IsExternal, gameSynchronizationFinishedData.IsExternal);

        }


        [Test]
        public void SubmitChanges_IfDeletedGame_NotifySignalRClientsAccordingly()
        {
            var dal = _internalServices.CreateGamesPortalDBDataContext();

            var game = GameRecord(130017);
            game.IsExternal = true;
            var changes = ChangeSet<object>.CreateDeleted(game);
            dal.GetChanges().Returns(changes);


            GameSynchronizationFinishedData gameSynchronizationFinishedData = null;
            _internalServices.GamesPortalHubContext.GameSynchronizationFinished(Arg.Do<GameSynchronizationFinishedData>(arg => gameSynchronizationFinishedData = arg));


            _artifactorySynchronizerDataAccessLayer.SubmitChanges(game);


            Assert.AreEqual(RecordChangeType.Deleted, gameSynchronizationFinishedData.ChangeType);
            Assert.AreEqual(game.Game_ID, gameSynchronizationFinishedData.GameId);
            Assert.AreEqual(game.IsExternal, gameSynchronizationFinishedData.IsExternal);

        }

        [Test]
        public void SubmitChanges_IfNoChanges_DO_NOT_NotifySignalRClients()
        {
            var dal = _internalServices.CreateGamesPortalDBDataContext();

            var game = GameRecord(130017);
            
            dal.GetChanges().Returns(ChangeSet<object>.Empty());

            _artifactorySynchronizerDataAccessLayer.SubmitChanges(game);

            _internalServices.GamesPortalHubContext.DidNotReceiveWithAnyArgs().GameSynchronizationFinished(null);

        }


        [Test]
        public void SubmitChanges_IfTheSubmitedGameIsANewOne_ExecuteInserOnSubmitOnTheGameTable()
        {
            var dal = _internalServices.CreateGamesPortalDBDataContext();
            var mockTable = dal.MockTable(new Game[0]);

            var newGame = GameRecord(130017);
            _artifactorySynchronizerDataAccessLayer.SubmitChanges(newGame);

            mockTable.Received().InsertOnSubmit(newGame);
        }
        
        [Test]
        public void SubmitChanges_IfTheSubmitedGameIsAnExistingOne_DO_NOT_ExecuteInserOnSubmitOnTheGameTable()
        {
            var dal = _internalServices.CreateGamesPortalDBDataContext();
            var existingGame = GameRecord(130017);
            var mockTable = dal.MockTable(existingGame);
            
            _artifactorySynchronizerDataAccessLayer.SubmitChanges(existingGame);

            mockTable.DidNotReceive().InsertOnSubmit(existingGame);
        }


        [Test]
        public void SubmitChanges_IAnyOtherChangeIsDetected_NotifySignalRClientsAccordingly()
        {
            var dal = _internalServices.CreateGamesPortalDBDataContext();

            var game = GameRecord(130017);
            game.IsExternal = true;
            
            var changes = ChangeSet<object>.CreateUpdated(new GameVersion());
            dal.GetChanges().Returns(changes);


            GameSynchronizationFinishedData gameSynchronizationFinishedData = null;
            _internalServices.GamesPortalHubContext.GameSynchronizationFinished(Arg.Do<GameSynchronizationFinishedData>(arg => gameSynchronizationFinishedData = arg));


            _artifactorySynchronizerDataAccessLayer.SubmitChanges(game);
            
            Assert.AreEqual(RecordChangeType.Changed, gameSynchronizationFinishedData.ChangeType);
            Assert.AreEqual(game.Game_ID, gameSynchronizationFinishedData.GameId);
            Assert.AreEqual(game.IsExternal, gameSynchronizationFinishedData.IsExternal);

        }
    }
}
