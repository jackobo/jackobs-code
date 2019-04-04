using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Data.LinqToSql.RecordsSynchronization
{
    public interface IPropertiesProvider<TRecord>
    {
        IEnumerable<PropertyInfo> GetProperties();
    }
    public class ReflectionBasedPropertiesProvider<TRecord> : IPropertiesProvider<TRecord>
    {
        private string[] _excludedProperties;

        
        public ReflectionBasedPropertiesProvider(params string[] excludedProperties)
        {
            _excludedProperties = excludedProperties;
        }

        public IEnumerable<PropertyInfo> GetProperties()
        {
            return typeof(TRecord).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                        .Where(prop => IsRegularKeyColumn(prop) 
                                                       && !_excludedProperties.Contains(prop.Name))
                                        .ToArray();
        }

        private bool IsRegularKeyColumn(PropertyInfo prop)
        {
            var attr = prop.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;

            if (attr == null)
                return false;


            return !attr.IsPrimaryKey;

        }
    }
}
