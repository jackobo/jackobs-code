using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces.Entities;

namespace LayoutTool.Interfaces
{
    public interface ISkinDefinitionParser 
    {
        SkinDefinitionContext Parse();
    }
}
