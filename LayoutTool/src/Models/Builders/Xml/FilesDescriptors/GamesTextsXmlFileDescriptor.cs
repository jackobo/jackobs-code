using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.Models.Builders.Xml.FilesDescriptors
{
    public class GamesTextsXmlFileDescriptor : TextsXmlFileDescriptor
    {

        public override string DefaultFileName
        {
            get
            {
                return "games_texts.xmm";
            }
        }

        public override void ApplyToParser(IXmlSkinDefinitionParser parser, string xml)
        {
            parser.GamesTextsXml = xml;
        }

        public override void ApplyToConverter(IXmlSkinDefinitionConverter converter, IClientConfigurationFile file)
        {

            converter.GamesTexts = file;

        }

      
    }
}
