using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;


namespace LayoutTool.Models.Builders.Xml
{
    public interface IXmlSkinDefinitionConverter : ISkinDefinitionConverter
    {

        IClientConfigurationFile IconResources { set; }
        IClientConfigurationFile Skin { set; }
        IClientConfigurationFile NavigationPlan { set; }
        IClientConfigurationFile GamesProperties { set; }
        IClientConfigurationFile GamesTexts { set; }

        
    }


    

}
