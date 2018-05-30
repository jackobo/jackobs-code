using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Artifactory
{
    public class StorageItemProperty
    {
        public StorageItemProperty()
        {

        }

        public StorageItemProperty(string key, params string[] values)
        {
            this.Key = key;
            this.Values = values;
        }
        public string Key { get; set; }
        public string[] Values { get; set; }

        public override string ToString()
        {
            return this.Key + " [" + string.Join("; ", Values) + "]";

        }

        public string ConcatValues()
        {
            return string.Join(";", this.Values);
        }
    }
}
