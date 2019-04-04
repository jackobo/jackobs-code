using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.Models.Builders.Xml.FilesDescriptors
{
    public class BrandXmlFileDescriptor : XmlClientConfigurationFileDescriptor
    {

        public override string DefaultFileName
        {
            get
            {
                return "brand.xml";
            }
        }
        
        public override void ApplyToParser(IXmlSkinDefinitionParser parser, string xml)
        {
            parser.BrandXml = xml;
        }


        public override void ApplyToConverter(IXmlSkinDefinitionConverter converter, IClientConfigurationFile file)
        {
            
        }

        public override PathDescriptor GetRelativePath(SkinCode skinCode)
        {
            return new PathDescriptor($"brand/brand_{skinCode.BrandId}/{DefaultFileName}");
        }
    }
}
