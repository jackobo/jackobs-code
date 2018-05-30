using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;

namespace LayoutTool.Models.Builders.Xml.FilesDescriptors
{
    public class NavigationPlanXmlFileDescriptor : XmlClientConfigurationFileDescriptor
    {

        public override string DefaultFileName
        {
            get
            {
                return DEFAULT_FILE_NAME;
            }
        }


        public static readonly string DEFAULT_FILE_NAME = "navigation_plan_ndl.xmm";
      
        private void UpdateOrAddArenaLayout(Arena arena, ArenaLayout arenaLayout, XDocument xmlDocument)
        {
            var arenas = xmlDocument.Root.Element(XmlNames.lobby_data_ndl).Element(XmlNames.arenas).Elements(XmlNames.arena).ToArray();
        }

        public override void ApplyToParser(IXmlSkinDefinitionParser parser, string xml)
        {
            parser.NavigationPlanXml = xml;
        }


        public override void ApplyToConverter(IXmlSkinDefinitionConverter converter, IClientConfigurationFile file)
        {
            converter.NavigationPlan = file;
            
        }

        public override PathDescriptor GetRelativePath(SkinCode skinCode)
        {
            return new PathDescriptor($"navigation/plan/{skinCode.Code}/{DefaultFileName}");
        }
        
    }
}
