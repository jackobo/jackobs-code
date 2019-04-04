using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Spark.Infra.Configurations
{
    public class ConfigurationReader : IConfigurationReader
    {
        

        public TSection ReadSection<TSection>() where TSection : ConfigurationSectionBase, new()
        {
            var configSectionAttribute = GetConfigurationSectionAttribute(typeof(TSection));

            var section = ConfigurationManager.GetSection(configSectionAttribute.Name) as TSection;

            if (section == null || !section.ElementInformation.IsPresent)
            {
                section = new TSection();
            }

            return section;
        }


    
        public  ConfigurationSectionAttribute GetConfigurationSectionAttribute(Type type)
        {
            var configAttributeType = typeof(ConfigurationSectionAttribute);
            var configSectionAttribute = type.GetCustomAttributes(configAttributeType, false).FirstOrDefault() as ConfigurationSectionAttribute;

            if (configSectionAttribute != null)
                return configSectionAttribute;

            var result = type.GetCustomAttributes(configAttributeType, true).FirstOrDefault() as ConfigurationSectionAttribute;

            if (result == null)
                throw new ArgumentException(string.Format("The type '{0}' is not marked with '{1}' attribute", type.FullName, configAttributeType.FullName));

            return result;
        }


        public string GetConnectionString(string key)
        {
            var config = ConfigurationManager.ConnectionStrings[key];
            if (config == null)
                throw new ApplicationException(string.Format("{0} connection string is not configured", key));

            return config.ConnectionString;
        }
        

    }
}
