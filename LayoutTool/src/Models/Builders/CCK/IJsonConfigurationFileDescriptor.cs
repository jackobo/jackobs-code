using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.Models.Builders.CCK
{
    public interface IJsonConfigurationFileDescriptor : IConfigurationFileDescriptor
    {
        void ApplyToParser(IJsonSkinDefinitionParser parser, IClientConfigurationFile file);
        void ApplyToConverter(IJsonSkinDefinitionConverter converter, IClientConfigurationFile file);
    }
}
