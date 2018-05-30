using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Configurations;

namespace GamesPortal.Service.Configurations
{
    [ConfigurationSection("signalR")]
    public class SignalRSettings : ConfigurationSectionBase
    {
        [ConfigurationProperty("url", DefaultValue="http://+:80/GamesPortal")]
        public string Url
        {
            get { return (string)this["url"]; }
            set
            {
                this["url"] = value;
            }
        }
    }
}
