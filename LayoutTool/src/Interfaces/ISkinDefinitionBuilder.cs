using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces.Entities;

namespace LayoutTool.Interfaces
{
    public interface ISkinDefinitionBuilder
    {
        string ClientVersion { get; }
        
        SkinDefinitionContext Build();
        SkinConversionResult Convert(SkinDefinition skinDefinition);
        
    }

    public interface IClientConfigurationFile
    {
        string FileName { get; }
        PathDescriptor Location { get; }
        string Content { get; }
    }

    
}
