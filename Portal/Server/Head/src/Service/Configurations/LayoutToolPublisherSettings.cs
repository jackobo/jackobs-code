using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Configurations;

namespace GamesPortal.Service.Configurations
{
    [ConfigurationSection("layoutToolPublisher")]
    public class LayoutToolPublisherSettings : ConfigurationSectionBase
    {
        [ConfigurationProperty("tfsUrl", IsRequired =true)]
        public string TfsUrl
        {
            get { return (string)this["tfsUrl"]; }
            set
            {
                this["tfsUrl"] = value;
            }
        }

        [ConfigurationProperty("localWorkspaceFolder")]
        public string LocalWorkspaceFolder
        {
            get { return (string)this["localWorkspaceFolder"]; }
            set
            {
                this["localWorkspaceFolder"] = value;
            }
        }

        [ConfigurationProperty("mailingList")]
        public string MailingList
        {
            get { return (string)this["mailingList"]; }
            set
            {
                this["mailingList"] = value;
            }
        }
    }
}
