using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.ViewModels
{
    public interface IConvertible<T>
    {
        T Convert();
    }
}
