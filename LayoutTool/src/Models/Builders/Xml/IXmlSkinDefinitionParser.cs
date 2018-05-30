using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;

namespace LayoutTool.Models.Builders.Xml
{
    public interface IXmlSkinDefinitionParser : ISkinDefinitionParser
    {
        //string BrandXml { set; }
        string GamesPropertiesXml { set; }
        string GamesTextsXml { set; }
        string LobbyTextsXml { set; }
        string NavigationPlanXml { set; }
        string SkinXml { set; }
        
    }
}
