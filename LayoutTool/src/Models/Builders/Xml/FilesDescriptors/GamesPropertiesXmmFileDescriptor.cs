using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.Models.Builders.Xml.FilesDescriptors
{
    public class GamesPropertiesXmmFileDescriptor : XmlClientConfigurationFileDescriptor
    {
        public override string DefaultFileName
        {
            get
            {
                return "games_properties.xmm";
            }
        }

     
        public override void ApplyToParser(IXmlSkinDefinitionParser parser, string xml)
        {
            parser.GamesPropertiesXml = xml;
        }

    

        public override void ApplyToConverter(IXmlSkinDefinitionConverter converter, IClientConfigurationFile file)
        {
            converter.GamesProperties = file;
        }

        public override PathDescriptor GetRelativePath(SkinCode skinCode)
        {
            return new PathDescriptor($"navigation/plan/{DefaultFileName}");
        }
        
    }
}
