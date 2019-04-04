using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.Models.Builders.CCK
{
    public interface IJsonSkinDefinitionParser : Interfaces.ISkinDefinitionParser
    {
        IClientConfigurationFile NavigationPlanJson { set; }
        IClientConfigurationFile LanguageJson { set; }
    }
}
