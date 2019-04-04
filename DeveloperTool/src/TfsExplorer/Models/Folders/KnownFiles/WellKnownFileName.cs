using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.Models.Folders
{
    public class WellKnownFileName : SmartEnum<string, WellKnownFileName>
    {
        public WellKnownFileName(string fileName)
            : base(fileName, fileName)
        {

        }

        public static readonly WellKnownFileName BuildCustomizationXml = new WellKnownFileName("BuildCustomization.xml");
        public static readonly WellKnownFileName ComponentUniqueIdTxt = new WellKnownFileName("ComponentUniqueID.txt");
        public static readonly WellKnownFileName GGPGameServerSln = new WellKnownFileName("GGPGameServer.sln");
        public static readonly WellKnownFileName TriggerIni = new WellKnownFileName("trigger.ini");
        public static readonly WellKnownFileName LatestPublishXml = new WellKnownFileName("LatestPublish.xml");
        public static readonly WellKnownFileName PublishXml = new WellKnownFileName("Publish.xml");

    }
}
