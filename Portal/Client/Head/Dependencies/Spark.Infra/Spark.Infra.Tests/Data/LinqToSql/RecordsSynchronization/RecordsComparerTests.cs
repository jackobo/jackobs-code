using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;


namespace Spark.Infra.Data.LinqToSql.RecordsSynchronization
{
    [TestFixture]
    public class RecordsComparerTests
    {

        IRecordsComparer<MockRecord> _recordsComparer;

        [SetUp]
        public void Setup()
        {
            InitRecordsComparer();
        }


        private void InitRecordsComparer(Func<MockRecord, bool> filter = null)
        {
            _recordsComparer = RecordsSynchronizerFactory<MockRecord>.RecordsComparer(record => new { record.Key1, record.Key2 }, filter);
        }

        private class MockRecord
        {
            public MockRecord(int key1, string key2, string firstName, string lastName)
            {
                this.Key1 = key1;
                this.Key2 = key2;
                this.FirstName = firstName;
                this.LastName = lastName;
            }
            public string FirstName { get; }
            public string LastName { get; }
            public int Key1 { get; }
            public string Key2 { get; }

        }

        private IList<MockRecord> OldRecords(params MockRecord[] records)
        {
            return new List<MockRecord>(records);
        }

        private IEnumerable<MockRecord> NewRecords(params MockRecord[] records)
        {
            return records;
        }

        [Test]
        public void Compare_NoItemAdded_InsertedShouldBeEmpty()
        {
            var oldRecords = OldRecords(new MockRecord(1, "a", "florin", "iacob"));

            var differences = _recordsComparer.Compare(oldRecords, NewRecords());

            Assert.AreEqual(0, differences.Inserted.Count());
        }

        [Test]
        public void Compare_OneItemAdded_InsertedSouldContainNewItem()
        {
            var oldRecords = OldRecords();
            var newRecord = new MockRecord(1, "a", "florin", "iacob");
            
            var differences = _recordsComparer.Compare(oldRecords, NewRecords(newRecord));
            
            Assert.IsTrue(differences.Inserted.Contains(newRecord));
        }


        [Test]
        public void Compare_NoItemRemoved_DeletedShouldBeEmpty()
        {
            var differences = _recordsComparer.Compare(OldRecords(), NewRecords(new MockRecord(1, "a", "florin", "iacob")));

            Assert.AreEqual(0, differences.Deleted.Count());
        }

        [Test]
        public void Compare_OneItemRemoved_DeletedShouldContainTheRemovedItem()
        {
            var removedRecord = new MockRecord(1, "a", "florin", "iacob");
            var differences = _recordsComparer.Compare(OldRecords(removedRecord), NewRecords());

            Assert.IsTrue(differences.Deleted.Contains(removedRecord));
        }


        [Test]
        public void Compare_NoItemUpdated_UpdatedShouldBeEmpty()
        {
            var differences = _recordsComparer.Compare(OldRecords(new MockRecord(1, "a", "florin", "iacob")), 
                                                        NewRecords(new MockRecord(2, "x", "adrian", "parsan")));

            Assert.AreEqual(0, differences.Updated.Count());
        }

        [Test]
        public void Compare_OneItemUpdated_UpdatedShouldContainOneItemWithCorrectReferences()
        {
            var oldRecord = new MockRecord(1, "a", "florin", "iacob");
            var newRecord = new MockRecord(1, "a", "florinache", "iacobache");
            var differences = _recordsComparer.Compare(OldRecords(oldRecord),
                                                        NewRecords(newRecord));

            var updated = differences.Updated.FirstOrDefault();

            Assert.IsNotNull(updated);

            Assert.IsTrue(object.ReferenceEquals(oldRecord, updated.OldRecord));
            Assert.IsTrue(object.ReferenceEquals(newRecord, updated.NewRecord));
        }

        [Test]
        public void Compare_IfFilterApplied_DeletedRecordsShouldContainOnlyTheFilteredItems()
        {
            var record1 = new MockRecord(1, "a", "florin", "iacob");
            var record2 = new MockRecord(2, "x", "georgescu", "ion");

            InitRecordsComparer(record => record.Key2 == "a");

            var differences = _recordsComparer.Compare(OldRecords(record1, record2), NewRecords());

            Assert.IsTrue(differences.Deleted.Contains(record1));
            Assert.IsFalse(differences.Deleted.Contains(record2));
        }


        [Test]
        public void Compare_IfFilterApplied_AddedRecordsShouldContainOnlyTheFilteredItems()
        {
            var record1 = new MockRecord(1, "a", "florin", "iacob");
            var record2 = new MockRecord(2, "x", "georgescu", "ion");

            InitRecordsComparer(record => record.Key2 == "a");

            var differences = _recordsComparer.Compare(OldRecords(), NewRecords(record1, record2));

            Assert.IsTrue(differences.Inserted.Contains(record1));
            Assert.IsFalse(differences.Inserted.Contains(record2));
        }

        [Test]
        public void Compare_IfFilterApplied_UpdatedRecordsShouldContainOnlyTheFilteredItems()
        {

            var oldRecord1 = new MockRecord(1, "a", "florin", "iacob");
            var oldRecord2 = new MockRecord(2, "x", "georgescu", "ion");

            var newRecord1 = new MockRecord(1, "a", "florinache", "iacobache");
            var newRecord2 = new MockRecord(2, "x", "george", "ionescu");

            InitRecordsComparer(record => record.Key2 == "a");

            var differences = _recordsComparer.Compare(OldRecords(oldRecord1, oldRecord2), NewRecords(newRecord1, newRecord2));

            Assert.IsTrue(differences.Updated.Any(record => object.ReferenceEquals(record.OldRecord, oldRecord1) 
                                                            && object.ReferenceEquals(record.NewRecord, newRecord1)));

            Assert.IsFalse(differences.Updated.Any(record => object.ReferenceEquals(record.OldRecord, oldRecord2)
                                                && object.ReferenceEquals(record.NewRecord, newRecord2)));
        }

    }
}
