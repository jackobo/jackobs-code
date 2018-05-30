using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.Models.Builders.Xml.FilesDescriptors
{
    public class SkinXmlFileDescriptor : XmlClientConfigurationFileDescriptor
    {

        public override string DefaultFileName
        {
            get
            {
                return "skin.xml";
            }
        }
        

        public override void ApplyToParser(IXmlSkinDefinitionParser parser, string xml)
        {
            parser.SkinXml = xml;
        }

        public override void ApplyToConverter(IXmlSkinDefinitionConverter converter, IClientConfigurationFile file)
        {
            converter.Skin = file;
        }

        public override PathDescriptor GetRelativePath(SkinCode skinCode)
        {
            return new PathDescriptor($"brand/brand_{skinCode.BrandId}/skin_{skinCode.SkinId}/{DefaultFileName}");
        }
    }
}
