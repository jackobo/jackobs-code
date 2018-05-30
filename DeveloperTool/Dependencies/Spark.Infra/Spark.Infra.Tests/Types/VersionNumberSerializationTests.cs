using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using NUnit.Framework;

namespace Spark.Infra.Types
{
    [TestFixture]
    public class VersionNumberSerializationTests
    {
      

        [Test]
        public void SerializeAsElement()
        {
            var sample = new SampleClassWithVersionAsElement("1.5.0.3");
            var serializedContent = Serialize(sample);

            var xmldDoc = XDocument.Parse(serializedContent);
            Assert.AreEqual("1.5.0.3", xmldDoc.Root.Element("version").Value);
        }


        private string Serialize(object obj)
        {
            var xmlSerializer = new XmlSerializer(obj.GetType());

            var stringBuilder = new StringBuilder();
            using (var writer = new StringWriter(stringBuilder))
            {
                xmlSerializer.Serialize(writer, obj);
            }
            return stringBuilder.ToString();
        }
        
        public class SampleClassWithVersionAsElement
        {
            public SampleClassWithVersionAsElement()
            {

            }

            public SampleClassWithVersionAsElement(string version)
            {
                this.Version = new VersionNumber(version);
            }
            [XmlElement("version")]
            public VersionNumber Version { get; set; }
        }
    }
}
