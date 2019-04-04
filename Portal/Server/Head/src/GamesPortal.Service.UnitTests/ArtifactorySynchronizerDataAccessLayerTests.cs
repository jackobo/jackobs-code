using System;
using NUnit.Framework;
using NSubstitute;
using GamesPortal.Service.Helpers;
using GGPGameServer.ApprovalSystem.Common.Databases;
using static GamesPortal.Service.Helpers.MockRecordsFactory;
using GamesPortal.Service.DataAccessLayer;

namespace GamesPortal.Service.Artifactory
{
    [TestFixture]
    public class ArtifactorySynchronizerDataAccessLayerTests
    {

        IGamesPortalInternalServices _internalServices;
        ArtifactorySynchronizerDataAccessLayer _artifactorySynchronizerDataAccessLayer;
        [SetUp]
        public void Setup()
        {
            _internalServices = GamesPortalInternalServicesHelper.Create();
            _artifactorySynchronizerDataAccessLayer = new ArtifactorySynchronizerDataAccessLayer(_internalServices);
        }

        [Test]
        public void SubmitChanges_IfLanguagesAreAffected_ShouldCallRunInTheGameLanguageSynchronizerAfterSubmitChanges()
        {
            var prop = LanguageProperty.BuildLanguageHash("en", "hash1");
            var changes = ChangeSet<object>.CreateInserted(GameVersionPropertyRecord(prop));
            var gamesPortalDb = _internalServices.CreateGamesPortalDBDataContext();
            gamesPortalDb.GetChanges().Returns(changes);
            
            _artifactorySynchronizerDataAccessLayer.SubmitChanges();

            Received.InOrder(() =>
            {
                gamesPortalDb.SubmitChanges();
                _internalServices.ArtifactoryToGameLanguageSynchronizer.Received().Run();
            });
        }


        [Test]
        public void SubmitChanges_IfGamesPortalDbGetChangesReturnsNull_ShouldNotFail()
        {
            var gamesPortalDb = _internalServices.CreateGamesPortalDBDataContext();

            
            gamesPortalDb.GetChanges().Returns((ChangeSet<object>)null);

            var gameVersion_LanguageQueueTable = gamesPortalDb.MockTable(new GameVersion_Language_FromArtifactorySyncQueue[0]);

            _artifactorySynchronizerDataAccessLayer.SubmitChanges();

            gamesPortalDb.Received().SubmitChanges();

        }

        [Test]
        public void SubmitChanges_IfOneGameVersionLanguagesIsAffected_InsertOneRecordIntoTheQueue()
        {
            var gamesPortalDb = _internalServices.CreateGamesPortalDBDataContext();
            
            var changes = ChangeSet<object>.CreateInserted(GameVersionPropertyRecord(LanguageProperty.BuildLanguageHash("en", "hash1")));
            gamesPortalDb.GetChanges().Returns(changes);

            var gameVersion_LanguageQueueTable = gamesPortalDb.MockTable(new GameVersion_Language_FromArtifactorySyncQueue[0]);

            _artifactorySynchronizerDataAccessLayer.SubmitChanges();

            gameVersion_LanguageQueueTable.Received().InsertOnSubmit(Arg.Any<GameVersion_Language_FromArtifactorySyncQueue>());
            
        }

        [Test]
        public void SubmitChanges_IfMultipleGameVersionLanguagesAreAffected_InsertOneRecordForEachGameVersion()
        {
            
            var record1 = GameVersionPropertyRecord(LanguageProperty.BuildLanguageHash("en", "hash1"));
            record1.GameVersion_ID = Guid.NewGuid();
            var record2 = GameVersionPropertyRecord(LanguageProperty.BuildLanguageHash("en", "hash1"));
            record2.GameVersion_ID = Guid.NewGuid();
            var changes = ChangeSet<object>.CreateInserted(record1, record2);


            var gamesPortalDb = _internalServices.CreateGamesPortalDBDataContext();
            gamesPortalDb.GetChanges().Returns(changes);

            var gameVersion_LanguageQueueTable = gamesPortalDb.MockTable(new GameVersion_Language_FromArtifactorySyncQueue[0]);

            _artifactorySynchronizerDataAccessLayer.SubmitChanges();

            gameVersion_LanguageQueueTable.Received(2).InsertOnSubmit(Arg.Any<GameVersion_Language_FromArtifactorySyncQueue>());

        }


        [Test]
        public void SubmitChanges_IfMultipleLanguagePropertiesAreAffectedForTheSameGameVersion_InsertOnlyOneRecord()
        {

            var gameVersionId = Guid.NewGuid();
            var record1 = GameVersionPropertyRecord(LanguageProperty.BuildLanguageHash("en", "hash1"));
            record1.GameVersion_ID = gameVersionId;
            var record2 = GameVersionPropertyRecord(LanguageProperty.BuildLanguageHash("en", "hash1"));
            record2.GameVersion_ID = gameVersionId;
            var changes = ChangeSet<object>.CreateInserted(record1, record2);


            var gamesPortalDb = _internalServices.CreateGamesPortalDBDataContext();
            gamesPortalDb.GetChanges().Returns(changes);

            var gameVersion_LanguageQueueTable = gamesPortalDb.MockTable(new GameVersion_Language_FromArtifactorySyncQueue[0]);

            _artifactorySynchronizerDataAccessLayer.SubmitChanges();

            gameVersion_LanguageQueueTable.Received(1).InsertOnSubmit(Arg.Any<GameVersion_Language_FromArtifactorySyncQueue>());

        }


        [Test]
        public void SubmitChanges_INoLanguagePropertyIsAffected_DoNotInsertAnyRecordInTheLanguageQueue()
        {
            var changes = ChangeSet<object>.CreateInserted(GameVersionPropertyRecord("NDL.State", "InProgress"));
            
            var gamesPortalDb = _internalServices.CreateGamesPortalDBDataContext();
            gamesPortalDb.GetChanges().Returns(changes);

            var gameVersion_LanguageQueueTable = gamesPortalDb.MockTable(new GameVersion_Language_FromArtifactorySyncQueue[0]);

            _artifactorySynchronizerDataAccessLayer.SubmitChanges();

            gameVersion_LanguageQueueTable.DidNotReceive().InsertOnSubmit(Arg.Any<GameVersion_Language_FromArtifactorySyncQueue>());
        }

        [Test]
        public void SubmitChanges_INoLanguagePropertyIsAffected_DoNotCallRun()
        {
            var changes = ChangeSet<object>.CreateInserted(GameVersionPropertyRecord("NDL.State", "InProgress"));

            var gamesPortalDb = _internalServices.CreateGamesPortalDBDataContext();
            gamesPortalDb.GetChanges().Returns(changes);

            gamesPortalDb.MockTable(new GameVersion_Language_FromArtifactorySyncQueue[0]);

            _artifactorySynchronizerDataAccessLayer.SubmitChanges();

            _internalServices.ArtifactoryToGameLanguageSynchronizer.DidNotReceive().Run();
            
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

        

    }
}
