using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spark.Infra.Configurations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationSectionAttribute : Attribute
    {
        public ConfigurationSectionAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }
     
    }
}
