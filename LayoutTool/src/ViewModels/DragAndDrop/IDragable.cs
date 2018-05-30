using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.ViewModels
{
    /*
    public interface IDragableX
    {
        
    }
    */

    public interface IDragableSource
    {
        object[] Items { get; }
    }
}
