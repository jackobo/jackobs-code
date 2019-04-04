using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using Spark.TfsExplorer.Models.Publish;

namespace Spark.TfsExplorer.Models
{
    [TestFixture]
    public class PublishPayloadXmlSerializerTests
    {
        [Test]
        [Ignore("d'aia")]
        public void Serialize()
        {
            var payload = new PublishPayload();
            payload.CoreComponents.Add(new PublishPayload.CoreComponent("GGPCore", new Infra.Types.VersionNumber("3.10.5.0")));
            payload.GameEngines.Add(new PublishPayload.GameEngine(new Interfaces.GameEngineName("AllPays"), new Infra.Types.VersionNumber("3.10.5.0")));

            var serializer = new PublishPayloadXmlSerializer();
            var fileSystemManager = new Spark.Infra.Windows.FileSystemManager();
            fileSystemManager.WriteFileContent(@"c:\temp\a\publish.xml", serializer.Serialize(payload));
        }

    }
}
