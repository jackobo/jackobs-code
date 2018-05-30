using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Configurations
{
    public abstract class ConfigurationCollectionBase<TItem> : ConfigurationElementCollection 
        where TItem : ConfigurationElement, new()
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TItem();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return GetItemKey(element as TItem);
        }

        protected abstract object GetItemKey(TItem item);
        
    }
}
