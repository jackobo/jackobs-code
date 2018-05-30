using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Helpers;
using NUnit.Framework;
using NSubstitute;
using static GamesPortal.Service.Helpers.MockRecordsFactory;
using GamesPortal.Service.Artifactory;
using GamesPortal.Service.Entities;

namespace GamesPortal.Service
{
    [TestFixture]
    public class GameLanguageApproverTests
    {
        [SetUp]
        public void Setup()
        {
            _internalServices = GamesPortalInternalServicesHelper.Create();
            _dal = _internalServices.CreateGamesPortalDBDataContext();
            _gameLanguageApprover = new GameLanguageApprover(_dal);
        }

        IGamesPortalInternalServices _internalServices;
        GameLanguageApprover _gameLanguageApprover;
        IGamesPortalDataContext _dal;
        
        DateTime _qaApprovalDate = new DateTime(2016, 11, 4);
        string _qaApprovalUser = "gigel";


        private void MockTables(params GameVersion_Language[] gameVersionRecords)
        {
            _dal.MockTable(gameVersionRecords.GroupBy(lang => lang.GameVersion.Game.Game_ID).Select(g => g.First().GameVersion.Game).ToArray());
            _dal.MockTable(gameVersionRecords.GroupBy(lang => lang.GameVersion.GameVersion_ID).Select(g => g.First().GameVersion).ToArray());
            _dal.MockTable(gameVersionRecords);
        }

        private GameLanguageApprover_Request CreateRequest(Guid versionId, params string[] languages)
        {
            var langs = languages.Select(lng => Language.Find(lng)).ToArray();
            return new GameLanguageApprover_Request(_qaApprovalDate, _qaApprovalUser, versionId, langs);
        }

        [Test]
        public void Approve_ShouldApproveAllLanguagesWithTheSameHash()
        {
            string languageHash = "hash";

            var gameRecord = GameRecord(130017);
            var languageRecord1 = gameRecord
                                    .AddVersionRecord("1.2.3.4")
                                    .AddLanguageRecord("English", "en", languageHash, "Gibraltar");
            
            var languageRecord2 = gameRecord
                                    .AddVersionRecord("1.0.0.5")
                                    .AddLanguageRecord("English", "en", languageHash, "Delaware");


            MockTables(languageRecord1, languageRecord2);
            
            

            _gameLanguageApprover.Approve(CreateRequest(languageRecord1.GameVersion_ID, "English"));

            Assert.AreEqual(_qaApprovalDate, languageRecord1.QAApprovalDate);
            Assert.AreEqual(_qaApprovalDate, languageRecord2.QAApprovalDate);

            Assert.AreEqual(_qaApprovalUser, languageRecord1.QAApprovalUser);
            Assert.AreEqual(_qaApprovalUser, languageRecord2.QAApprovalUser);
        }

        [Test]
        public void Approve_SameIdenticalLanguageIsApprovedButForDifferentGameVersionsAndRegulations_InsertOneRecordForEachGameVersionInto_GameVersion_Property_History()
        {

            string languageHash = "hash";

            var gameRecord = GameRecord(130017);

            var languageRecord1 = gameRecord
                                    .AddVersionRecord("1.2.3.4")
                                    .AddLanguageRecord("English", "en", languageHash, "Gibraltar");

            var languageRecord2 = gameRecord
                                    .AddVersionRecord("1.0.0.5")
                                    .AddLanguageRecord("English", "en", languageHash, "Delaware");


            MockTables(languageRecord1, languageRecord2);



            _gameLanguageApprover.Approve(CreateRequest(languageRecord1.GameVersion_ID, "English"));

            Assert.AreEqual(1, languageRecord1.GameVersion.GameVersion_Property_Histories.Count);
            Assert.AreEqual(1, languageRecord2.GameVersion.GameVersion_Property_Histories.Count);

        }


        [Test]
        public void Approve_IfMultipleLanguagesAreApprovedForTheSameGameVersionAndRegulation_ShouldInsertOnlyOneRecordInto_GameVersion_Property_History()
        {
                        
            var gameVersionRecord = GameRecord(130017)
                                    .AddVersionRecord("1.2.3.4");

            
            var languageRecord1 = gameVersionRecord.AddLanguageRecord("English", "en", "hash1", "Gibraltar");
            var languageRecord2 = gameVersionRecord.AddLanguageRecord("Italian", "it", "hash2", "Gibraltar");

            MockTables(languageRecord1, languageRecord2);

            
            _gameLanguageApprover.Approve(CreateRequest(gameVersionRecord.GameVersion_ID, "English", "Italian"));


            Assert.AreEqual(1, gameVersionRecord.GameVersion_Property_Histories.Count);
        }

        [Test]
        public void Approve_IfThereIsNoPreviousApprovedLanguage_RecordChangeTypeInTheHistory_ShouldBe_Added()
        {

            var gameVersionRecord = GameRecord(130017)
                                    .AddVersionRecord("1.2.3.4");


            var languageRecord1 = gameVersionRecord.AddLanguageRecord("English", "en", "hash1", "Gibraltar");
            var languageRecord2 = gameVersionRecord.AddLanguageRecord("Italian", "it", "hash2", "Gibraltar");

            MockTables(languageRecord1, languageRecord2);


            _gameLanguageApprover.Approve(CreateRequest(gameVersionRecord.GameVersion_ID, "English", "Italian"));

            var historyRecord = gameVersionRecord.GameVersion_Property_Histories[0];
            Assert.AreEqual(RecordChangeType.Added, (RecordChangeType)historyRecord.ChangeType);
        }

        [Test]
        public void Approve_IfThereArePreviousApprovedLanguagesThen_RecordChangeTypeInTheHistory_ShouldBe_Changed()
        {

            var gameVersionRecord = GameRecord(130017)
                                    .AddVersionRecord("1.2.3.4");


            var languageRecord1 = gameVersionRecord.AddLanguageRecord("English", "en", "hash1", "Gibraltar");
            languageRecord1.QAApprovalDate = new DateTime(2000, 1, 1);
            languageRecord1.QAApprovalUser = "me";

            var languageRecord2 = gameVersionRecord.AddLanguageRecord("Italian", "it", "hash2", "Gibraltar");

            MockTables(languageRecord1, languageRecord2);


            _gameLanguageApprover.Approve(CreateRequest(gameVersionRecord.GameVersion_ID, "English", "Italian"));

            var historyRecord = gameVersionRecord.GameVersion_Property_Histories[0];
            Assert.AreEqual(RecordChangeType.Changed, (RecordChangeType)historyRecord.ChangeType);
        }

        [Test]
        public void Approve_NoMatterWhatLanguagesAreApprovedInTheRequest_TheNewValueInTheHistory_ShouldContain_AllApprovedLanguages()
        {

            var gameVersionRecord = GameRecord(130017)
                                    .AddVersionRecord("1.2.3.4");


            var languageRecord1 = gameVersionRecord.AddLanguageRecord("English", "en", "hash1", "Gibraltar");
            languageRecord1.QAApprovalDate = new DateTime(2000, 1, 1);
            languageRecord1.QAApprovalUser = "me";

            var languageRecord2 = gameVersionRecord.AddLanguageRecord("Italian", "it", "hash2", "Gibraltar");

            MockTables(languageRecord1, languageRecord2);
            
            //here only Italian language is requested to be approved 
            _gameLanguageApprover.Approve(CreateRequest(gameVersionRecord.GameVersion_ID, "Italian"));

            var historyRecord = gameVersionRecord.GameVersion_Property_Histories[0];
            Assert.AreEqual("en,it", historyRecord.NewValue);
        }

        [Test]
        public void Approve_ShouldSetTheApprovalInfoInTheHistoryRecord_AccordingWithTheRecievedArguments()
        {

            var gameVersionRecord = GameRecord(130017)
                                    .AddVersionRecord("1.2.3.4");


            var languageRecord = gameVersionRecord.AddLanguageRecord("English", "en", "hash1", "Gibraltar");
            
            MockTables(languageRecord);

            _gameLanguageApprover.Approve(CreateRequest(gameVersionRecord.GameVersion_ID, "English"));

            var historyRecord = gameVersionRecord.GameVersion_Property_Histories[0];
            Assert.AreEqual(_qaApprovalDate, historyRecord.ChangeDate);
            Assert.AreEqual(_qaApprovalUser, historyRecord.ChangedBy);
        }

        [Test]
        public void Approve_ShouldSetTheCorrectRegulationInTheHistoryRecord()
        {

            var gameVersionRecord = GameRecord(130017)
                                    .AddVersionRecord("1.2.3.4");

            var languageRecord = gameVersionRecord.AddLanguageRecord("English", "en", "hash1", "Gibraltar");

            MockTables(languageRecord);

            _gameLanguageApprover.Approve(CreateRequest(gameVersionRecord.GameVersion_ID, "English"));

            var historyRecord = gameVersionRecord.GameVersion_Property_Histories[0];
            Assert.AreEqual("Gibraltar", historyRecord.Regulation);
        }

        [Test]
        public void Approve_ThePropertyKeyInThe_GameVersion_Property_History_SouldHaveTheCorrectValue()
        {

            var gameVersionRecord = GameRecord(130017)
                                    .AddVersionRecord("1.2.3.4");

            var languageRecord = gameVersionRecord.AddLanguageRecord("English", "en", "hash1", "Gibraltar");

            MockTables(languageRecord);

            _gameLanguageApprover.Approve(CreateRequest(gameVersionRecord.GameVersion_ID, "English"));

            var historyRecord = gameVersionRecord.GameVersion_Property_Histories[0];
            Assert.AreEqual(LanguageProperty.Language_QAApproved, historyRecord.PropertyKey);
        }

        [Test]
        public void Approve_IfThereArePreviousApprovedLanguagesThen_OldValueInThePropertiesHistory_ShouldContainTheseLanguages()
        {

            var gameVersionRecord = GameRecord(130017)
                                    .AddVersionRecord("1.2.3.4");


            var languageRecord1 = gameVersionRecord.AddLanguageRecord("English", "en", "hash1", "Gibraltar");
            languageRecord1.QAApprovalDate = new DateTime(2000, 1, 1);
            languageRecord1.QAApprovalUser = "me";

            var languageRecord2 = gameVersionRecord.AddLanguageRecord("Spanish", "es", "hash2", "Gibraltar");
            languageRecord2.QAApprovalDate = new DateTime(2002, 2, 2);
            languageRecord2.QAApprovalUser = "you";

            var languageRecord3 = gameVersionRecord.AddLanguageRecord("Italian", "it", "hash3", "Gibraltar");

            MockTables(languageRecord1, languageRecord2, languageRecord3);


            _gameLanguageApprover.Approve(CreateRequest(gameVersionRecord.GameVersion_ID, "Italian"));

            var historyRecord = gameVersionRecord.GameVersion_Property_Histories[0];
            Assert.AreEqual("en,es", historyRecord.OldValue);
        }


        [Test]
        public void Approve_IfThereIsNoPreviousApprovedLanguagesThen_OldValueInThePropertiesHistory_ShouldBeEmptyString()
        {

            var gameVersionRecord = GameRecord(130017)
                                    .AddVersionRecord("1.2.3.4");


            var languageRecord1 = gameVersionRecord.AddLanguageRecord("English", "en", "hash1", "Gibraltar");
            var languageRecord2 = gameVersionRecord.AddLanguageRecord("Spanish", "es", "hash2", "Gibraltar");
            var languageRecord3 = gameVersionRecord.AddLanguageRecord("Italian", "it", "hash3", "Gibraltar");

            MockTables(languageRecord1, languageRecord2, languageRecord3);


            _gameLanguageApprover.Approve(CreateRequest(gameVersionRecord.GameVersion_ID, "Italian", "Spanish", "English"));

            var historyRecord = gameVersionRecord.GameVersion_Property_Histories[0];
            Assert.AreEqual("", historyRecord.OldValue);
        }

        [Test]
        public void Approve_ShouldNotApprove_LanguagesThatAreNotInTheRequest()
        {

            var gameVersionRecord = GameRecord(130017)
                                    .AddVersionRecord("1.2.3.4");


            var languageRecord1 = gameVersionRecord.AddLanguageRecord("English", "en", "hash1", "Gibraltar");
            var languageRecord2 = gameVersionRecord.AddLanguageRecord("Spanish", "es", "hash2", "Gibraltar");
            var languageRecord3 = gameVersionRecord.AddLanguageRecord("Italian", "it", "hash3", "Gibraltar");

            MockTables(languageRecord1, languageRecord2, languageRecord3);


            _gameLanguageApprover.Approve(CreateRequest(gameVersionRecord.GameVersion_ID, "Italian", "Spanish"));

            
            Assert.IsNull(languageRecord1.QAApprovalDate);
            Assert.IsNull(languageRecord1.QAApprovalUser);
        }


        [Test]
        public void Approve_IfTheLanguageIsAlreadyApproved_DontOverrideApprovalInfo()
        {

            var gameVersionRecord = GameRecord(130017)
                                    .AddVersionRecord("1.2.3.4");


            var languageRecord1 = gameVersionRecord.AddLanguageRecord("English", "en", "hash1", "Gibraltar");
            languageRecord1.QAApprovalUser = "meme";
            languageRecord1.QAApprovalDate = new DateTime(1977, 4, 14);

            var languageRecord2 = gameVersionRecord.AddLanguageRecord("Spanish", "es", "hash2", "Gibraltar");
            

            MockTables(languageRecord1, languageRecord2);


            _gameLanguageApprover.Approve(CreateRequest(gameVersionRecord.GameVersion_ID, "English", "Spanish"));


            Assert.AreEqual(new DateTime(1977, 4, 14), languageRecord1.QAApprovalDate);
            Assert.AreEqual("meme", languageRecord1.QAApprovalUser);
        }


        [Test]
        public void Approve_IfTheLanguageIsAlreadyApproved_DontWriteAnythingTo_GameVersion_Property_History()
        {

            var gameVersionRecord = GameRecord(130017)
                                    .AddVersionRecord("1.2.3.4");


            var languageRecord = gameVersionRecord.AddLanguageRecord("English", "en", "hash1", "Gibraltar");
            languageRecord.QAApprovalUser = "meme";
            languageRecord.QAApprovalDate = new DateTime(1977, 4, 14);
            
            MockTables(languageRecord);


            _gameLanguageApprover.Approve(CreateRequest(gameVersionRecord.GameVersion_ID, "English"));


            Assert.AreEqual(0, gameVersionRecord.GameVersion_Property_Histories.Count);
        }


        [Test]
        public void Approve_OneGameVersionAffected_ShouldReturnOnlyOnceAffectedGameVersionRecord()
        {

            var gameVersionRecord = GameRecord(130017)
                                     .AddVersionRecord("1.2.3.4");


            var languageRecord1 = gameVersionRecord.AddLanguageRecord("English", "en", "hash1", "Gibraltar");
            var languageRecord2 = gameVersionRecord.AddLanguageRecord("Spanish", "es", "hash2", "Gibraltar");
            

            MockTables(languageRecord1, languageRecord2);


            var response = _gameLanguageApprover.Approve(CreateRequest(gameVersionRecord.GameVersion_ID, "English", "Spanish"));


            Assert.AreEqual(1, response.AffectedGameVersions.Count());
            Assert.IsTrue(response.AffectedGameVersions.Contains(gameVersionRecord));
        }

        [Test]
        public void Approve_TwoGameVersionsAffected_ShouldReturnTwoAffectedGameVersionRecord()
        {

            var languageHash = "hash";
            var gameRecord = GameRecord(130017);

            var languageRecord1 = gameRecord
                                    .AddVersionRecord("1.2.3.4")
                                    .AddLanguageRecord("English", "en", languageHash, "Gibraltar");

            var languageRecord2 = gameRecord
                                    .AddVersionRecord("1.0.0.5")
                                    .AddLanguageRecord("English", "en", languageHash, "Delaware");


            MockTables(languageRecord1, languageRecord2);

            var response = _gameLanguageApprover.Approve(CreateRequest(languageRecord1.GameVersion_ID, "English"));


            Assert.AreEqual(2, response.AffectedGameVersions.Count());
            Assert.IsTrue(response.AffectedGameVersions.Contains(languageRecord1.GameVersion));
            Assert.IsTrue(response.AffectedGameVersions.Contains(languageRecord2.GameVersion));
        }

        [Test]
        public void Approve_OneAffectedGame_ShouldReturnOneAffectedGame()
        {

            var languageHash = "hash";
            var gameRecord = GameRecord(130017);

            var languageRecord1 = gameRecord
                                    .AddVersionRecord("1.2.3.4")
                                    .AddLanguageRecord("English", "en", languageHash, "Gibraltar");

            var languageRecord2 = gameRecord
                                    .AddVersionRecord("1.0.0.5")
                                    .AddLanguageRecord("English", "en", languageHash, "Delaware");


            MockTables(languageRecord1, languageRecord2);

            var response = _gameLanguageApprover.Approve(CreateRequest(languageRecord1.GameVersion_ID, "English"));


            Assert.AreEqual(1, response.AffectedGames.Count());
            Assert.IsTrue(response.AffectedGames.Contains(gameRecord));
        }

        [Test]
        public void Approve_IfTheLanguageIsAlreadyApproved_TheRelatedGameVersionShouldNotBeReturnedAsAffected()
        {

            var languageRecord = GameRecord(130017)
                                    .AddVersionRecord("1.2.3.4")
                                    .AddLanguageRecord("English", "en", "hash1", "Gibraltar");

            
            languageRecord.QAApprovalUser = "meme";
            languageRecord.QAApprovalDate = new DateTime(1977, 4, 14);
            

            MockTables(languageRecord);


            var response = _gameLanguageApprover.Approve(CreateRequest(languageRecord.GameVersion_ID, "English"));


            Assert.AreEqual(0, response.AffectedGameVersions.Count());
            Assert.AreEqual(0, response.AffectedGames.Count());
        }

        [Test]
        public void Approve_ShouldNeverReturnANullResponse()
        {
            
            var languageRecord = GameRecord(130017)
                                    .AddVersionRecord("1.2.3.4")
                                    .AddLanguageRecord("English", "en", "hash1", "Gibraltar");
            
            languageRecord.QAApprovalUser = "meme";
            languageRecord.QAApprovalDate = new DateTime(1977, 4, 14);


            MockTables(languageRecord);


            var response = _gameLanguageApprover.Approve(CreateRequest(languageRecord.GameVersion_ID, "English"));


            Assert.IsNotNull(response);
            Assert.IsNotNull(response.AffectedGames);
            Assert.IsNotNull(response.AffectedGameVersions);
        }


        [Test]
        public void Approve_ShouldNotApprove_LanguagesWithTheSameHashesButDifferentPlatform()
        {

            var gameRecord = GameRecord(130017);

            var gameVersionRecord1 = gameRecord.AddVersionRecord("1.2.3.4");
            gameVersionRecord1.PlatformType = (int)PlatformType.PC;
            var languageRecord1 = gameVersionRecord1.AddLanguageRecord("English", "en", "hash1", "Gibraltar");

            var gameVersionRecord2 = gameRecord.AddVersionRecord("1.0.0.0");
            gameVersionRecord2.PlatformType = (int)PlatformType.Mobile;
            var languageRecord2 = gameVersionRecord2.AddLanguageRecord("English", "en", "hash1", "Gibraltar");
            
            MockTables(languageRecord1, languageRecord2);
            
            _gameLanguageApprover.Approve(CreateRequest(gameVersionRecord1.GameVersion_ID, "English"));

            Assert.IsNull(languageRecord2.QAApprovalDate);
            Assert.IsNull(languageRecord2.QAApprovalUser);
        }

        [Test]
        public void Approve_ShouldNotApprove_LanguagesWithTheSameHashesButDifferentTechnology()
        {

            var gameRecord = GameRecord(130017);

            var gameVersionRecord1 = gameRecord.AddVersionRecord("1.2.3.4");
            gameVersionRecord1.Technology = (int)GameTechnology.Html5;
            var languageRecord1 = gameVersionRecord1.AddLanguageRecord("English", "en", "hash1", "Gibraltar");

            var gameVersionRecord2 = gameRecord.AddVersionRecord("1.0.0.0");
            gameVersionRecord2.Technology = (int)GameTechnology.Flash;
            var languageRecord2 = gameVersionRecord2.AddLanguageRecord("English", "en", "hash1", "Gibraltar");

            MockTables(languageRecord1, languageRecord2);

            _gameLanguageApprover.Approve(CreateRequest(gameVersionRecord1.GameVersion_ID, "English"));

            Assert.IsNull(languageRecord2.QAApprovalDate);
            Assert.IsNull(languageRecord2.QAApprovalUser);
        }
    }
}
