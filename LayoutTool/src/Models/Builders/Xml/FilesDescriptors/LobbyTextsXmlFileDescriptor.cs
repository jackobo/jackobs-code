using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.Models.Builders.Xml.FilesDescriptors
{
    public class LobbyTextsXmlFileDescriptor : TextsXmlFileDescriptor
    {

        public override string DefaultFileName
        {
            get
            {
                return "lobby_text.xmm";
            }
        }

        public override void ApplyToParser(IXmlSkinDefinitionParser parser, string xml)
        {
            parser.LobbyTextsXml = xml;
        }

        public override void ApplyToConverter(IXmlSkinDefinitionConverter converter, IClientConfigurationFile file)
        {
            
        }
    }
}
