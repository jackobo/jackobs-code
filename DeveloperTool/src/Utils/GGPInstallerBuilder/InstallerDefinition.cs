using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Build;

namespace GGPInstallerBuilder
{
    public class InstallerDefinition
    {
        public InstallerDefinition(string latestTxtFileContent)
        {
            this.LatestTxtContent = latestTxtFileContent;

            var latestTxtProperties = StringKeyValueCollection.Parse(latestTxtFileContent);
            this.InstallerID = new Guid(latestTxtProperties[AntPropertyNames.InstallerUniqueID].Value);
            this.Version = new VersionNumber(latestTxtProperties[AntPropertyNames.InstallerVersion].Value);
            this.PublisherEmailAddress = latestTxtProperties[AntPropertyNames.TriggeredBy].Value;

            if(latestTxtProperties.Contains(AntPropertyNames.CustomizedInstaller))
                this.IsCustomizedInstaller = 0 == string.Compare("true", latestTxtProperties[AntPropertyNames.CustomizedInstaller].Value);
        }

        public string LatestTxtContent { get; private set; }

        public Guid InstallerID { get; private set; }
        public VersionNumber Version { get; private set; }
        public string PublisherEmailAddress { get; private set; }
        public bool IsCustomizedInstaller { get; private set; } = false;


        public List<ComponentDefinition> Components { get; private set; } = new List<ComponentDefinition>();

        public class ComponentDefinition
        {
            public ComponentDefinition(string componentID, IServerPath distributionFolder)
            {
                this.ComponentID = componentID;
                this.DistributionFolder = distributionFolder;
            }

            public string ComponentID { get; private set; }
            public IServerPath DistributionFolder { get; private set; }
        }
    }
}
