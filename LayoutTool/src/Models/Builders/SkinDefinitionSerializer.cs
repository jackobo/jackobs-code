using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;

namespace LayoutTool.Models.Builders
{
    public class SkinDefinitionSerializer : ISkinDefinitionSerializer
    {
        
        public string Serialize(SkinDefinitionContext skinDefintionContext)
        {
            var xmlSerializer = CreateXmlSerializer();
            var stringBuilder = new StringBuilder();
            using (var stringWriter = new Utf8StringWriter(stringBuilder))
            using (var xmlWriter = CreateXmlWriter(stringWriter))
            {
                xmlSerializer.Serialize(xmlWriter, skinDefintionContext);
            }

            return stringBuilder.ToString();
        }

        XmlWriter CreateXmlWriter(StringWriter stringWriter)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;
            return XmlWriter.Create(stringWriter, settings);
        }

        private class Utf8StringWriter : StringWriter
        {
            public Utf8StringWriter(StringBuilder sb)
                : base(sb)
            {

            }
            public override Encoding Encoding { get { return Encoding.UTF8; } }
        }

        public SkinDefinitionContext Deserialize(string content)
        {
            var xmlSerializer = CreateXmlSerializer();
            using (var reader = new StringReader(content))
            {
                return (SkinDefinitionContext)xmlSerializer.Deserialize(reader);
            }
        }


        private static XmlSerializer CreateXmlSerializer()
        {
            return new XmlSerializer(typeof(SkinDefinitionContext));
        }
    }
}
