using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Interfaces
{
    public interface IConfigurationFileDescriptor
    {
        string DefaultFileName { get; }

        PathDescriptor GetRelativePath(SkinCode skinCode);
        
    }

}
