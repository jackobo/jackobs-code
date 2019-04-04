using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.Models.Builders.Xml
{
    public interface IXmlConfigurationFileDescriptor : IConfigurationFileDescriptor
    {
        void ApplyToParser(IXmlSkinDefinitionParser parser, string xml);
        void ApplyToConverter(IXmlSkinDefinitionConverter converter, IClientConfigurationFile file);
        
    }

}
