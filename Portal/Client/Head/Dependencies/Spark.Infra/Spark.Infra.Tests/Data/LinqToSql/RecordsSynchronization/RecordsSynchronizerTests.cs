using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using System.Reflection;

namespace Spark.Infra.Data.LinqToSql.RecordsSynchronization
{
    [TestFixture]
    public class RecordsSynchronizerTests
    {

        IList<IMockRecord> _currentRecords;
        IEnumerable<IMockRecord> _newRecords = new IMockRecord[0];
        IRecordsComparer<IMockRecord> _recordsComparer;
        IPropertiesProvider<IMockRecord> _propertiesProvider;
        IRecordsSynchronizer<IMockRecord> _recordsSynchronizer;
        

        [SetUp]
        public void Setup()
        {
            _currentRecords = Substitute.For<IList<IMockRecord>>();
            _propertiesProvider = Substitute.For<IPropertiesProvider<IMockRecord>>();

            
            _recordsComparer = Substitute.For<IRecordsComparer<IMockRecord>>();
            _recordsSynchronizer = RecordsSynchronizerFactory<IMockRecord>.Create(_recordsComparer, _propertiesProvider);
        }

     
        public interface IMockRecord
        {
            string FirstName { get; set; }
            string LastName { get; set; }
        }


        RecordsComparisonResult<IMockRecord> Deleted(params IMockRecord[] deleted)
        {
            return new RecordsComparisonResult<IMockRecord>(new IMockRecord[0],
                                                                     new RecordsComparisonResult<IMockRecord>.UpdatedRecord[0],
                                                                     deleted);
        }


        RecordsComparisonResult<IMockRecord> Inserted(params IMockRecord[] inserted)
        {
            return new RecordsComparisonResult<IMockRecord>(inserted,
                                                                     new RecordsComparisonResult<IMockRecord>.UpdatedRecord[0],
                                                                     new IMockRecord[0]);
        }

        RecordsComparisonResult<IMockRecord> Updated(params RecordsComparisonResult<IMockRecord>.UpdatedRecord[] updated)
        {
            return new RecordsComparisonResult<IMockRecord>(new IMockRecord[0],
                                                                     updated,
                                                                     new IMockRecord[0]);
        }


        private IMockRecord CreateMockRecord()
        {
            return Substitute.For<IMockRecord>();
        }

        private IMockRecord CreateMockRecord(string firstName, string lastName)
        {
            var record =  Substitute.For<IMockRecord>();
            record.FirstName.Returns(firstName);
            record.LastName.Returns(lastName);
            return record;
        }


        [Test]
        public void Sync_WhenItemRemoved_ShouldCallRemoveInTheCurrentList()
        {
            var removedItem = CreateMockRecord();
            _recordsComparer.Compare(Arg.Any<IEnumerable<IMockRecord>>(), Arg.Any<IEnumerable<IMockRecord>>())
                                .Returns(Deleted(removedItem));

            _recordsSynchronizer.Sync(_currentRecords, _newRecords);

            _currentRecords.Received().Remove(removedItem);
        }
        
        [Test]
        public void Sync_WhenItemAdded_ShouldCallAddInTheCurrentList()
        {
            var addedItem = CreateMockRecord();
            _recordsComparer.Compare(Arg.Any<IEnumerable<IMockRecord>>(), Arg.Any<IEnumerable<IMockRecord>>())
                                .Returns(Inserted(addedItem));

            _recordsSynchronizer.Sync(_currentRecords, _newRecords);

            _currentRecords.Received().Add(addedItem);
        }


        [Test]
        public void Sync_WhenItemUpdated_OnlyThePropertiesProvidedByThePropertiesProviderShouldBeUpdated()
        {
            
            var oldRecord = CreateMockRecord();
            var newRecord = CreateMockRecord("Florin", "Iacob");
            
            _propertiesProvider.GetProperties().Returns(new PropertyInfo[]
                {
                    typeof(IMockRecord).GetProperty(nameof(IMockRecord.FirstName))
                });


            _recordsComparer.Compare(Arg.Any<IEnumerable<IMockRecord>>(), Arg.Any<IEnumerable<IMockRecord>>())
                                .Returns(Updated(new RecordsComparisonResult<IMockRecord>.UpdatedRecord(oldRecord, newRecord)));

            

            _recordsSynchronizer.Sync(_currentRecords, _newRecords);

            Assert.AreEqual("Florin", oldRecord.FirstName);
            Assert.AreNotEqual("Iacob", oldRecord.LastName);

        }

    }
}
