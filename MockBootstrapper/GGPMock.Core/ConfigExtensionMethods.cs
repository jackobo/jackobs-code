using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper
{
    public static class ConfigExtensionMethods
    {
        public static void WriteAppSetting(this  System.Configuration.Configuration config, string key, string value)
        {
            if (null == config.AppSettings.Settings[key])
            {
                config.AppSettings.Settings.Add(new System.Configuration.KeyValueConfigurationElement(key, value));
            }
            else
            {
                config.AppSettings.Settings[key].Value = value;
            }
        }
    }
}
