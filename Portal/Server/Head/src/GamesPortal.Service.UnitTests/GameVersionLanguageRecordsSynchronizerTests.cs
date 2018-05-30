using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.DataAccessLayer;
using Spark.Infra.Data.LinqToSql.RecordsSynchronization;
using NUnit.Framework;
using static GamesPortal.Service.Helpers.MockRecordsFactory;

namespace GamesPortal.Service.Artifactory
{
    [TestFixture]
    public class GameVersionLanguageRecordsSynchronizerTests
    {

        IChildRecordsSynchronizer<GameVersion> _synchronizer;

        [SetUp]
        public void Setup()
        {

            _synchronizer = new GameVersionLanguageRecordsSynchronizer();
        }

        [Test]
        public void Sync_IfQAApprovalDateHasValue_DontOverrideQAApprovalDateAndQAApprovalUser()
        {
            var gameId = Guid.NewGuid();
            //old record
            var oldGameVersionRecord = GameVersionRecord("1.0.1.0", gameId);
            var oldLanguageRecord = oldGameVersionRecord.AddLanguageRecord("English", "en", "hash");
            oldLanguageRecord.QAApprovalDate = new DateTime(2015, 1, 2);
            oldLanguageRecord.QAApprovalUser = "florin";


            //new record
            var newGameVersionRecord = GameVersionRecord("1.0.1.0", gameId);
            var newLanguageRecord = newGameVersionRecord.AddLanguageRecord("English", "en", "hash");
            newLanguageRecord.QAApprovalDate = new DateTime(2016, 2, 3);
            newLanguageRecord.QAApprovalUser = "smaster";

            _synchronizer.Sync(oldGameVersionRecord, newGameVersionRecord);

            Assert.AreEqual(new DateTime(2015, 1, 2), oldLanguageRecord.QAApprovalDate);
            Assert.AreEqual("florin", oldLanguageRecord.QAApprovalUser);
        }


        [Test]
        public void Sync_IfQAApprovalDateDoesntHaveValue_WriteTheNewValues()
        {
            var gameId = Guid.NewGuid();
            //old record
            var oldGameVersionRecord = GameVersionRecord("1.0.1.0", gameId);
            var oldLanguageRecord = oldGameVersionRecord.AddLanguageRecord("English", "en", "hash");
            oldLanguageRecord.QAApprovalDate = null;
            oldLanguageRecord.QAApprovalUser = null;


            //new record
            var newGameVersionRecord = GameVersionRecord("1.0.1.0", gameId);
            var newLanguageRecord = newGameVersionRecord.AddLanguageRecord("English", "en", "hash");
            newLanguageRecord.QAApprovalDate = new DateTime(2016, 2, 3);
            newLanguageRecord.QAApprovalUser = "smaster";

            _synchronizer.Sync(oldGameVersionRecord, newGameVersionRecord);

            Assert.AreEqual(new DateTime(2016, 2, 3), oldLanguageRecord.QAApprovalDate);
            Assert.AreEqual("smaster", oldLanguageRecord.QAApprovalUser);
        }

        [Test]
        public void Sync_IfProductionUploadDateHasValue_DontOverrideIt()
        {
            var gameId = Guid.NewGuid();
            //old record
            var oldGameVersionRecord = GameVersionRecord("1.0.1.0", gameId);
            var oldLanguageRecord = oldGameVersionRecord.AddLanguageRecord("English", "en", "hash");
            oldLanguageRecord.ProductionUploadDate = new DateTime(2015, 1, 2);
            


            //new record
            var newGameVersionRecord = GameVersionRecord("1.0.1.0", gameId);
            var newLanguageRecord = newGameVersionRecord.AddLanguageRecord("English", "en", "hash");
            newLanguageRecord.ProductionUploadDate = new DateTime(2016, 2, 3);
            

            _synchronizer.Sync(oldGameVersionRecord, newGameVersionRecord);

            Assert.AreEqual(new DateTime(2015, 1, 2), oldLanguageRecord.ProductionUploadDate);
            
        }

        [Test]
        public void Sync_IfProductionUploadDateDoesntHaveValue_WriteTheNewValue()
        {
            var gameId = Guid.NewGuid();
            //old record
            var oldGameVersionRecord = GameVersionRecord("1.0.1.0", gameId);
            var oldLanguageRecord = oldGameVersionRecord.AddLanguageRecord("English", "en", "hash");
            oldLanguageRecord.ProductionUploadDate = null;



            //new record
            var newGameVersionRecord = GameVersionRecord("1.0.1.0", gameId);
            var newLanguageRecord = newGameVersionRecord.AddLanguageRecord("English", "en", "hash");
            newLanguageRecord.ProductionUploadDate = new DateTime(2016, 2, 3);


            _synchronizer.Sync(oldGameVersionRecord, newGameVersionRecord);

            Assert.AreEqual(new DateTime(2016, 2, 3), oldLanguageRecord.ProductionUploadDate);

        }
    }
}
