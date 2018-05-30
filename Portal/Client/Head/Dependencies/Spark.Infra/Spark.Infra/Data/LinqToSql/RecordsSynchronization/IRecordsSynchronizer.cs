using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Data.LinqToSql.RecordsSynchronization
{
    public interface IRecordsSynchronizer<TRecord>
       where TRecord : class
    {
        
        void Sync(IList<TRecord> currentRecords, IEnumerable<TRecord> newRecords);

        void AddChild(IChildRecordsSynchronizer<TRecord> childRecordsSynchronizer);

        IRecordsSynchronizer<TChildRecord> AddChild<TChildRecord, TChildKey>(
                                                Expression<Func<TRecord, IList<TChildRecord>>> childRecordsSelector,
                                                Func<TChildRecord, TChildKey> childRecordKeySelector)
                                                where TChildRecord : class;

        IRecordsSynchronizer<TChildRecord> AddChild<TChildRecord>(
                                              Expression<Func<TRecord, IList<TChildRecord>>> childRecordsSelector,
                                              IRecordsComparer<TChildRecord> recordsComparer,
                                              IPropertiesProvider<TChildRecord> propertiesProvider
                                              )
                                              where TChildRecord : class;

    }
}
