using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Data.LinqToSql.RecordsSynchronization
{
    public interface IRecordsComparer<TRecord>
        where TRecord : class
    {
        RecordsComparisonResult<TRecord> Compare(IEnumerable<TRecord> oldRecords, IEnumerable<TRecord> newRecords);
    }


    public class RecordsComparer<TRecord, TKey> : IRecordsComparer<TRecord>
        where TRecord: class
    {
        public RecordsComparer(Func<TRecord, TKey> recordKeySelector, Func<TRecord, bool> filter = null)
        {
            this.RecordKeySelector = recordKeySelector;
            if (filter != null)
                _filter = new RecordsFilter(filter);
            else
                _filter = new NoFilter();
        }

        private Func<TRecord, TKey> RecordKeySelector { get; set; }


        private interface IRecordsFilter
        {
            IEnumerable<TRecord> ApplyFilter(IEnumerable<TRecord> records);
        }

        private class RecordsFilter : IRecordsFilter
        {
            public RecordsFilter(Func<TRecord, bool> filter)
            {
                _filter = filter;
            }


            Func<TRecord, bool> _filter;

            public IEnumerable<TRecord> ApplyFilter(IEnumerable<TRecord> records)
            {
                return records.Where(r => _filter(r));
            }
        }


        private class NoFilter : IRecordsFilter
        {
            public IEnumerable<TRecord> ApplyFilter(IEnumerable<TRecord> records)
            {
                return records;
            }
        }

        IRecordsFilter _filter = new NoFilter();

        public RecordsComparisonResult<TRecord> Compare(IEnumerable<TRecord> oldRecords, IEnumerable<TRecord> newRecords)
        {
            oldRecords = _filter.ApplyFilter(oldRecords);
            newRecords = _filter.ApplyFilter(newRecords);
            return new RecordsComparisonResult<TRecord>(Inserted(oldRecords, newRecords),
                                                                 Updated(oldRecords, newRecords),
                                                                 Delected(oldRecords, newRecords));
        }

        private IEnumerable<TRecord> Delected(IEnumerable<TRecord> oldRecords, IEnumerable<TRecord> newRecords)
        {
            foreach (var record in FindMissingRecord(oldRecords, newRecords))
            {
                yield return record;
            }
        }

        private IEnumerable<RecordsComparisonResult<TRecord>.UpdatedRecord> Updated(IEnumerable<TRecord> oldRecords, IEnumerable<TRecord> newRecords)
        {
            return (from currentRecord in oldRecords
                                  join newRecord in newRecords on this.RecordKeySelector(currentRecord) equals this.RecordKeySelector(newRecord)
                                  select new RecordsComparisonResult<TRecord>.UpdatedRecord(currentRecord, newRecord));

        }

        private IEnumerable<TRecord> Inserted(IEnumerable<TRecord> oldRecords, IEnumerable<TRecord> newRecords)
        {
            foreach (var record in FindMissingRecord(newRecords, oldRecords))
            {
                yield return record;
            }
        }

        private IEnumerable<TRecord> FindMissingRecord(IEnumerable<TRecord> oldRecords,
                                                      IEnumerable<TRecord> newRecords)
        {
            return (from oldRecord in oldRecords
                    join newRecord in newRecords on RecordKeySelector(oldRecord) equals RecordKeySelector(newRecord) into temp
                    from subCurrent in temp.DefaultIfEmpty()
                    where subCurrent == null
                    select oldRecord)
                   .ToArray();
        }
    }

    public class RecordsComparisonResult<TRecord>
    {
        public RecordsComparisonResult(IEnumerable<TRecord> inserted, IEnumerable<UpdatedRecord> updated, IEnumerable<TRecord> deleted)
        {
            this.Inserted = inserted;
            this.Updated = updated;
            this.Deleted = deleted;
        }


        public IEnumerable<TRecord> Inserted { get; set; }
        public IEnumerable<UpdatedRecord> Updated { get; private set; }
        public IEnumerable<TRecord> Deleted { get; private set; }

        public class UpdatedRecord
        {
            public UpdatedRecord(TRecord oldRecord, TRecord newRecord)
            {
                OldRecord = oldRecord;
                NewRecord = newRecord;
            }

            public TRecord OldRecord { get; private set; }
            public TRecord NewRecord { get; private set; }
        }
    }
    
}
