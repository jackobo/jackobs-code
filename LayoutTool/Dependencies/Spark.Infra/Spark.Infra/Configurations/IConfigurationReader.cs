using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spark.Infra.Configurations
{
    public interface IConfigurationReader
    {
        TSection ReadSection<TSection>() where TSection : ConfigurationSectionBase, new();
        string GetConnectionString(string key);
       
    }

    
}
