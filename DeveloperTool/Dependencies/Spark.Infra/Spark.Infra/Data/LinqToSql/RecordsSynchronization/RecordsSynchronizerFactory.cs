using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Spark.Infra.Types;

namespace Spark.Infra.Data.LinqToSql.RecordsSynchronization
{
    
    public static class RecordsSynchronizerFactory<TRecord>
        where TRecord : class
    {
        public static IRecordsSynchronizer<TRecord> Create<TRecordKey>(Func<TRecord, TRecordKey> recordKeySelector,
                                                                       Func<TRecord, bool> recordsFilter = null)
        {

            return Create(RecordsComparer(recordKeySelector, recordsFilter),
                          PropertiesProvider());
        }
        
        public static IRecordsSynchronizer<TRecord> Create(IRecordsComparer<TRecord> recordsComparer, 
                                                           IPropertiesProvider<TRecord> propertiesProvider)
        {
            return new RecordsSynchronizer<TRecord>(recordsComparer, propertiesProvider);
        }


        public static IRecordsSynchronizer<TRecord> Create(IRecordsComparer<TRecord> recordsComparer)
        {
            return new RecordsSynchronizer<TRecord>(recordsComparer, PropertiesProvider());
        }

        public static IRecordsComparer<TRecord> RecordsComparer<TRecordKeySelector>
                                                (Func<TRecord, TRecordKeySelector> recordKeySelector,
                                                Func<TRecord, bool> filter = null)
        {
            return new RecordsComparer<TRecord, TRecordKeySelector>(recordKeySelector, filter);
        }

        public static IPropertiesProvider<TRecord> PropertiesProvider()
        {
            return new ReflectionBasedPropertiesProvider<TRecord>();
        }
    }



}
