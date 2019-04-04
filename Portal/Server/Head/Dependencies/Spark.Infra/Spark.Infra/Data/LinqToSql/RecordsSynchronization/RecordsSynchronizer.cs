using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Data.LinqToSql.RecordsSynchronization
{
    public class RecordsSynchronizer<TRecord> : IRecordsSynchronizer<TRecord>
        where TRecord : class
    {
        
        public RecordsSynchronizer(IRecordsComparer<TRecord> recordsComparer, 
                                   IPropertiesProvider<TRecord> propertiesProvider)
        {
            _recordsComparer = recordsComparer;
            _propertiesProvider = propertiesProvider;
        }

        IRecordsComparer<TRecord> _recordsComparer;
        IPropertiesProvider<TRecord> _propertiesProvider;
        
        protected virtual IEnumerable<PropertyInfo> Properties
        {
            get
            {
                return _propertiesProvider.GetProperties();
            }
        }


        public void Sync(IList<TRecord> currentRecords, IEnumerable<TRecord> newRecords)
        {
            var differences = _recordsComparer.Compare(currentRecords, newRecords);
            foreach (var deleted in differences.Deleted)
            {
                currentRecords.Remove(deleted);
            }

            
            foreach (var updated in differences.Updated)
            {
                UpdateRecord(updated.OldRecord, updated.NewRecord);
            }
            

            foreach (var inserted in differences.Inserted)
            {
                currentRecords.Add(inserted);
            }
            
        }

      
        
        protected virtual void UpdateRecord(TRecord oldRecord, TRecord newRecord)
        {
            UpdateProperties(oldRecord, newRecord);

            UpdateChildren(oldRecord, newRecord);
        }

        private void UpdateChildren(TRecord oldRecord, TRecord newRecord)
        {
            foreach (var child in _childSynchronizers)
            {
                UpdateChild(oldRecord, newRecord, child);
            }
        }

        protected virtual void UpdateProperties(TRecord oldRecord, TRecord newRecord)
        {
            foreach (var prop in this.Properties)
            {
                UpdateProperty(oldRecord, newRecord, prop);
            }
        }

        protected virtual void UpdateProperty(TRecord oldRecord, TRecord newRecord, PropertyInfo propertyInfo)
        {
            propertyInfo.SetValue(oldRecord, propertyInfo.GetValue(newRecord));
        }

        protected virtual void UpdateChild(TRecord oldRecord, TRecord newRecord, IChildRecordsSynchronizer<TRecord> child)
        {
            child.Sync(oldRecord, newRecord);
        }




        private List<IChildRecordsSynchronizer<TRecord>> _childSynchronizers = new List<IChildRecordsSynchronizer<TRecord>>();


        public void AddChild(IChildRecordsSynchronizer<TRecord> childRecordsSynchronizer)
        {
            _childSynchronizers.Add(childRecordsSynchronizer);
        }

        public IRecordsSynchronizer<TChildRecord> AddChild<TChildRecord, TChildKey>(
            Expression<Func<TRecord, IList<TChildRecord>>> childRecordsSelector,
            Func<TChildRecord, TChildKey> childRecordKeySelector)
            where TChildRecord : class
        {
     
            return AddChild(childRecordsSelector,
                            RecordsSynchronizerFactory<TChildRecord>.RecordsComparer(childRecordKeySelector),
                            RecordsSynchronizerFactory<TChildRecord>.PropertiesProvider());

   
        }

    
        public IRecordsSynchronizer<TChildRecord> AddChild<TChildRecord>(
           Expression<Func<TRecord, IList<TChildRecord>>> childRecordsSelector,
           IRecordsComparer<TChildRecord> recordsComparer,
           IPropertiesProvider<TChildRecord> propertiesProvider
           )
           where TChildRecord : class
        {
            var child = new ChildRecordsSynchronizer<TRecord, TChildRecord>(
                                        childRecordsSelector,
                                        recordsComparer,
                                        propertiesProvider);

            AddChild(child);

            return child;
        }
    }
}
