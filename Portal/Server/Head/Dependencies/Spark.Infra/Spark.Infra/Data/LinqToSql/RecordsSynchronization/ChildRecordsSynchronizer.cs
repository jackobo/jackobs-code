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
    public class ChildRecordsSynchronizer<TParentRecord, TChildRecord> 
                    : RecordsSynchronizer<TChildRecord>,
                      IChildRecordsSynchronizer<TParentRecord>
         where TChildRecord : class
    {
        public ChildRecordsSynchronizer(Expression<Func<TParentRecord, IList<TChildRecord>>> childrenSelector,
                                        IRecordsComparer<TChildRecord> recordsComparer,
                                        IPropertiesProvider<TChildRecord> propertiesProvider)
            : base(recordsComparer, propertiesProvider)
        {
            DetectForeignKeyProperty(childrenSelector);
            ChildrenSelector = childrenSelector.Compile();
        }

        private void DetectForeignKeyProperty(Expression<Func<TParentRecord, IList<TChildRecord>>> childRecordsSelector)
        {
            var propertyDescriptor = SparkReflector.GetPropertyDescriptor(childRecordsSelector);
            var associationAttribute = propertyDescriptor.Attributes[typeof(AssociationAttribute)] as AssociationAttribute;

            if(associationAttribute == null)
            {
                throw new ArgumentException($"There is no {typeof(AssociationAttribute).FullName} on property {propertyDescriptor.Name} of object {typeof(TParentRecord).FullName}");
            }

            this.ForeignKeyPropertyName = associationAttribute.OtherKey;
        }

        private string ForeignKeyPropertyName { get; set; }

     

        protected override IEnumerable<PropertyInfo> Properties
        {
            get
            {
                return base.Properties.Where(p => p.Name != this.ForeignKeyPropertyName);
            }
        }
        
        Func<TParentRecord, IList<TChildRecord>> ChildrenSelector { get; set; }

        private IList<TChildRecord> _existingRecords;
        private IEnumerable<TChildRecord> _newRecords;

        void IChildRecordsSynchronizer<TParentRecord>.Sync(TParentRecord existingParentRecord, 
                                                           TParentRecord newParentRecord)
        {
            _existingRecords = ChildrenSelector(existingParentRecord);
            _newRecords = ChildrenSelector(newParentRecord);
            this.Sync(_existingRecords, _newRecords);
        }
        


    }
}
