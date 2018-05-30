using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using GamesPortal.Service.Helpers;
using GamesPortal.Service.DataAccessLayer;
using static GamesPortal.Service.Helpers.MockRecordsFactory;
using static GamesPortal.Service.Helpers.Utils;
using GamesPortal.Service.Entities;
using Spark.Infra.Logging;
using GamesPortal.Service.Artifactory;

namespace GamesPortal.Service.Synchronizers
{
    [TestFixture]
    public class GameLanguageToArtifactorySynchronizerTests : SynchronizerTestHelper<GameLanguageToArtifactorySynchronizer>
    {

        

    

       
        protected override void SetupInternalServices()
        {
            base.SetupInternalServices();
            
            _internalServices.CreateGamesPortalDBDataContext().MockTable(new GameVersion_Language_ToArtifactorySyncQueue[0]);
        }

        protected override GameLanguageToArtifactorySynchronizer CreateSynchronizer()
        {
            return new GameLanguageToArtifactorySynchronizer(_internalServices);
        }
        
        
        [Test]
        public void DoWork_ShouldCallDisposeMethodOfTheDataContext()
        {
            var dal = _internalServices.CreateGamesPortalDBDataContext();

            _synchronizer.Start();
            
            dal.Received().Dispose();
        }

       
        [Test]
        public void DoWork_ShouldCall_LanguageQAApprovalStatusUpdater_UpdateForAllChangedLanguagesAndGetGamesVersionIds()
        {
            var languageQaApprovalStatusUpdater = _internalServices.GamesLanguageQAApprovalStatusNormalizer();

            _synchronizer.Start();

            languageQaApprovalStatusUpdater.Received(1).NormalizeApprovalStatusForAllLanguagesWithTheSameHash();
        }
      

        [Test]
        public void DoWork_ForEachGameVersionInThe_GameVersion_Language_ToArtifactorySyncQueue_Table_ShouldAskForTheCorrectArtifactoryRepository()
        {

            var repoDescriptor = new GamesRepositoryDescriptor(GameTechnology.Html5, false, PlatformType.PcAndMobile, Substitute.For<IGamesRepository>());

            _internalServices.ArtifactorySynchronizationManager.FindRepositoryDescriptor(Arg.Any<GameInfrastructureDTO>(),
                                                                              Arg.Any<bool>(),
                                                                              Arg.Any<GamingComponentCategory>())
                                                 .Returns(repoDescriptor);

            var game1 = GameRecord(130017, false);
            var game2 = GameRecord(230001, true);

            var versionId1 = Guid.NewGuid();
            var versionId2 = Guid.NewGuid();

            var dal = _internalServices.CreateGamesPortalDBDataContext();
            dal.MockTable(GameVersion_Language_ToArtifactorySyncQueueRecord(versionId1),
                          GameVersion_Language_ToArtifactorySyncQueueRecord(versionId2));

            DateTime qaApprovalDate = DateTime.Now;

            var gameVersionRecord1 = GameVersionRecord(versionId1, "1.0.0.1", game1, GameTechnology.Html5, PlatformType.Mobile);
            gameVersionRecord1.GameVersion_Languages.Add(GameVersion_LanguageRecord(versionId1, "English", "en", "hash1", qaApprovalDate, "Gibraltar"));
            gameVersionRecord1.GameVersion_Regulations.Add(GameVersion_RegulationRecord(versionId1, "Gibraltar"));

            var gameVersionRecord2 = GameVersionRecord(versionId2, "1.0.0.2", game2, GameTechnology.Flash, PlatformType.PC);
            gameVersionRecord2.GameVersion_Languages.Add(GameVersion_LanguageRecord(versionId2, "Italian", "it", "hash2", qaApprovalDate, "888Italy"));
            gameVersionRecord2.GameVersion_Regulations.Add(GameVersion_RegulationRecord(versionId2, "888Italy"));

            dal.MockTable(gameVersionRecord1, gameVersionRecord2);

            _synchronizer.Start();

            _internalServices.ArtifactorySynchronizationManager.Received(1).FindRepositoryDescriptor(new GameInfrastructureDTO(GameTechnology.Html5, PlatformType.Mobile), false, GamingComponentCategory.Game);
            _internalServices.ArtifactorySynchronizationManager.Received(1).FindRepositoryDescriptor(new GameInfrastructureDTO(GameTechnology.Flash, PlatformType.PC), true, GamingComponentCategory.Game);
        }


      

        [Test]
        public void DoWork_IfTheSameGameVersionIsInBoth_GameVersion_Language_ToArtifactorySyncQueue_Table_And_LanguageQAApprovalStatusUpdater_ShouldAskForTheArtifactoryRepositoryOnlyOnce()
        {
            var game = GameRecord(230001, true);

            var versionId = Guid.NewGuid();
            
            var dal = _internalServices.CreateGamesPortalDBDataContext();

            _internalServices.GamesLanguageQAApprovalStatusNormalizer().NormalizeApprovalStatusForAllLanguagesWithTheSameHash();

            dal.MockTable(GameVersion_Language_ToArtifactorySyncQueueRecord(versionId));
            DateTime qaApprovalDate = DateTime.Now;
            var gameVersionRecord = GameVersionRecord(versionId, "1.0.0.2", game, GameTechnology.Flash, PlatformType.PC);
            gameVersionRecord.GameVersion_Languages.Add(GameVersion_LanguageRecord(versionId, "English", "en", "hash1", qaApprovalDate, "Gibraltar"));
            gameVersionRecord.GameVersion_Regulations.Add(GameVersion_RegulationRecord(versionId, "Gibraltar"));
            
            dal.MockTable(gameVersionRecord);

            _synchronizer.Start();

            _internalServices.ArtifactorySynchronizationManager.Received(1).FindRepositoryDescriptor(new GameInfrastructureDTO(GameTechnology.Flash, PlatformType.PC), true, GamingComponentCategory.Game);
        }

        [Test]
        public void DoWork_IfTheGameVersionRecordDoenstExists_DontAskArtifactoryGatewayForARepositoryDescriptor()
        {
            
            var versionId = Guid.NewGuid();

            var dal = _internalServices.CreateGamesPortalDBDataContext();

            _internalServices.GamesLanguageQAApprovalStatusNormalizer()
                             .NormalizeApprovalStatusForAllLanguagesWithTheSameHash();
            
            dal.MockTable(new GameVersion[0]);

            _synchronizer.Start();

            _internalServices.ArtifactorySynchronizationManager.DidNotReceive().FindRepositoryDescriptor(Arg.Any<GameInfrastructureDTO>(), Arg.Any<bool>(), Arg.Any<GamingComponentCategory>());
        }

        [Test]
        public void DoWork_IfTheGameVersionRecordDoenstExists_MakeSureItDoesntThrowAnException()
        {
            
            var dal = _internalServices.CreateGamesPortalDBDataContext();
            _internalServices.GamesLanguageQAApprovalStatusNormalizer()
                             .NormalizeApprovalStatusForAllLanguagesWithTheSameHash();

            dal.MockTable(new GameVersion[0]);
            
            _synchronizer.Start();
            
            _logger.DidNotReceiveWithAnyArgs().Exception(null);
            
        }

        [Test]
        public void DoWork_IfNoLanguageRecors_Remove_LanguageApprovedPropertyForAllAvailableRegulationsForThatGameVersion()
        {
            var gameRecord = GameRecord(230001, true);

            var versionId = Guid.NewGuid();

            var dal = _internalServices.CreateGamesPortalDBDataContext();
            dal.MockTable(GameVersion_Language_ToArtifactorySyncQueueRecord(versionId));


            var gameVersionRecord = GameVersionRecord(versionId, "1.0.0.2", gameRecord, GameTechnology.Flash, PlatformType.PC);
            gameVersionRecord.GameVersion_Regulations.Add(GameVersion_RegulationRecord(versionId, "888Italy"));
            gameVersionRecord.GameVersion_Regulations.Add(GameVersion_RegulationRecord(versionId, "Gibraltar"));
            dal.MockTable(gameVersionRecord);

            
            
            var repositoryDescriptor = _internalServices.ArtifactorySynchronizationManager.MockGameRepository(GameTechnology.Flash, PlatformType.PC, true);
            var actualDeleteRequests = new List<DeleteArtifactPropertiesRequest>();
            repositoryDescriptor.Repository.DeleteArtifactProperties(Arg.Do<DeleteArtifactPropertiesRequest>(arg => actualDeleteRequests.Add(arg)));

            _synchronizer.Start();

            Assert.AreEqual(2, actualDeleteRequests.Count);
            Assert.IsTrue(actualDeleteRequests.Any(request => request.Regulation == "Gibraltar"));
            Assert.IsTrue(actualDeleteRequests.Any(request => request.Regulation == "888Italy"));
            Assert.IsTrue(actualDeleteRequests.All(request => request.Properties.First().Equals(LanguageProperty.Language_QAApproved)));
            Assert.IsTrue(actualDeleteRequests.All(request => request.ComponentId == gameRecord.MainGameType));
            Assert.IsTrue(actualDeleteRequests.All(request => request.Version == gameVersionRecord.VersionFolder));
        }


        [Test]
        public void DoWork_IfAtLeastOneApprovedLanguage_DoNotRemoveTheProperty()
        {
            var gameRecord = GameRecord(230001, true);

            var versionId = Guid.NewGuid();

            var dal = _internalServices.CreateGamesPortalDBDataContext();
            _internalServices.GamesLanguageQAApprovalStatusNormalizer()
                             .NormalizeApprovalStatusForAllLanguagesWithTheSameHash();

            DateTime qaApprovalDate = DateTime.Now;

            var gameVersionRecord = GameVersionRecord(versionId, "1.0.0.2", gameRecord, GameTechnology.Flash, PlatformType.PC);
            gameVersionRecord.GameVersion_Regulations.Add(GameVersion_RegulationRecord(versionId, "888Italy"));
            gameVersionRecord.GameVersion_Regulations.Add(GameVersion_RegulationRecord(versionId, "Gibraltar"));
            
            gameVersionRecord.GameVersion_Languages.Add(GameVersion_LanguageRecord(versionId, "English", "en", "hash1", qaApprovalDate, "Gibraltar"));
            gameVersionRecord.GameVersion_Languages.Add(GameVersion_LanguageRecord(versionId, "English", "en", "hash1", qaApprovalDate, "888Italy"));

            dal.MockTable(gameVersionRecord);
            
            var repositoryDescriptor = _internalServices.ArtifactorySynchronizationManager.MockGameRepository(GameTechnology.Flash, PlatformType.PC, true);
            
            _synchronizer.Start();

            repositoryDescriptor.Repository.DidNotReceiveWithAnyArgs().DeleteArtifactProperties(null);
        }

        [Test]
        public void DoWork_IfNoApprovedLanguage_RemoveTheProperty()
        {
            var gameRecord = GameRecord(230001, true);

            var versionId = Guid.NewGuid();

            var dal = _internalServices.CreateGamesPortalDBDataContext();
            dal.MockTable(GameVersion_Language_ToArtifactorySyncQueueRecord(versionId));
            
            var gameVersionRecord = GameVersionRecord(versionId, "1.0.0.2", gameRecord, GameTechnology.Flash, PlatformType.PC);
            gameVersionRecord.GameVersion_Regulations.Add(GameVersion_RegulationRecord(versionId, "888Italy"));
            gameVersionRecord.GameVersion_Regulations.Add(GameVersion_RegulationRecord(versionId, "Gibraltar"));

            gameVersionRecord.GameVersion_Languages.Add(GameVersion_LanguageRecord(versionId, "English", "en", "hash1", "Gibraltar"));
            gameVersionRecord.GameVersion_Languages.Add(GameVersion_LanguageRecord(versionId, "English", "en", "hash1", "888Italy"));

            dal.MockTable(gameVersionRecord);

            var repositoryDescriptor = _internalServices.ArtifactorySynchronizationManager.MockGameRepository(GameTechnology.Flash, PlatformType.PC, true);

            _synchronizer.Start();

            repositoryDescriptor.Repository.ReceivedWithAnyArgs().DeleteArtifactProperties(null);
        }


        [Test]
        public void DoWork_IfApprovedLanguages_UpdateTheAccordingArtifactoryPropertyPerRegulation()
        {
            var gameRecord = GameRecord(230001, true);

            var versionId = Guid.NewGuid();

            var dal = _internalServices.CreateGamesPortalDBDataContext();
            dal.MockTable(GameVersion_Language_ToArtifactorySyncQueueRecord(versionId));

            DateTime qaApprovalDate = DateTime.Now;

            var gameVersionRecord = GameVersionRecord(versionId, "1.0.0.2", gameRecord, GameTechnology.Flash, PlatformType.PC);
            gameVersionRecord.GameVersion_Regulations.Add(GameVersion_RegulationRecord(versionId, "888Italy"));
            gameVersionRecord.GameVersion_Regulations.Add(GameVersion_RegulationRecord(versionId, "Gibraltar"));

            gameVersionRecord.GameVersion_Languages.Add(GameVersion_LanguageRecord(versionId, "English", "en", "hash1", qaApprovalDate, "Gibraltar"));

            gameVersionRecord.GameVersion_Languages.Add(GameVersion_LanguageRecord(versionId, "Italian", "it", "hash2", qaApprovalDate, "888Italy"));
            gameVersionRecord.GameVersion_Languages.Add(GameVersion_LanguageRecord(versionId, "English", "en", "hash1", qaApprovalDate, "888Italy"));
            


            dal.MockTable(gameVersionRecord);



            var repositoryDescriptor = _internalServices.ArtifactorySynchronizationManager.MockGameRepository(GameTechnology.Flash, PlatformType.PC, true);
            var actualDeleteRequests = new List<UpdateArtifactPropertiesRequest>();
            repositoryDescriptor.Repository.UpdateArtifactProperties(Arg.Do<UpdateArtifactPropertiesRequest>(arg => actualDeleteRequests.Add(arg)));

            _synchronizer.Start();

            Assert.AreEqual(2, actualDeleteRequests.Count);
            Assert.IsTrue(actualDeleteRequests.Any(request => request.Regulation == "Gibraltar"));
            Assert.IsTrue(actualDeleteRequests.Any(request => request.Regulation == "888Italy"));
            Assert.IsTrue(actualDeleteRequests.Any(request => request.Properties.First().Equals(LanguageProperty.BuildLanguageQaApprovedProperty("en", "it"))));
            Assert.IsTrue(actualDeleteRequests.Any(request => request.Properties.First().Equals(LanguageProperty.BuildLanguageQaApprovedProperty("en"))));

            Assert.IsTrue(actualDeleteRequests.All(request => request.ComponentId == gameRecord.MainGameType));
            Assert.IsTrue(actualDeleteRequests.All(request => request.Version == gameVersionRecord.VersionFolder));
        }


        [Test]
        public void DoWork_IfLanguagesForASpecificRegulationAreNotApproved_RemoveTheLanguageApprovedPropertyForThatRegulation()
        {
            var gameRecord = GameRecord(230001, true);

            var versionId = Guid.NewGuid();

            var dal = _internalServices.CreateGamesPortalDBDataContext();
            dal.MockTable(GameVersion_Language_ToArtifactorySyncQueueRecord(versionId));


            var gameVersionRecord = GameVersionRecord(versionId, "1.0.0.2", gameRecord, GameTechnology.Flash, PlatformType.PC);
            gameVersionRecord.GameVersion_Regulations.Add(GameVersion_RegulationRecord(versionId, "888Italy"));
            gameVersionRecord.GameVersion_Regulations.Add(GameVersion_RegulationRecord(versionId, "Gibraltar"));

            gameVersionRecord.GameVersion_Languages.Add(GameVersion_LanguageRecord(versionId, "English", "en", "hash1", DateTime.Now, "Gibraltar"));
            gameVersionRecord.GameVersion_Languages.Add(GameVersion_LanguageRecord(versionId, "Italian", "it", "hash2", "888Italy"));
            

            dal.MockTable(gameVersionRecord);



            var repositoryDescriptor = _internalServices.ArtifactorySynchronizationManager.MockGameRepository(GameTechnology.Flash, PlatformType.PC, true);
            DeleteArtifactPropertiesRequest actualDeleteRequest = null;
            repositoryDescriptor.Repository.DeleteArtifactProperties(Arg.Do<DeleteArtifactPropertiesRequest>(arg => actualDeleteRequest = arg));

            _synchronizer.Start();

            repositoryDescriptor.Repository.ReceivedWithAnyArgs(1).DeleteArtifactProperties(null);
            Assert.AreEqual("888Italy", actualDeleteRequest.Regulation);
            Assert.AreEqual(LanguageProperty.Language_QAApproved, actualDeleteRequest.Properties.First());
            Assert.AreEqual(gameVersionRecord.VersionFolder, actualDeleteRequest.Version);
            Assert.AreEqual(gameRecord.MainGameType, actualDeleteRequest.ComponentId);

        }
    }
}
