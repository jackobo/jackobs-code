using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.ViewModels
{
    public class DraggableSet : List<object>, IDragableSource
    {
        public DraggableSet()
        {

        }

        public DraggableSet(IEnumerable<object> items)
            : base(items)
        {

        }

        object[] IDragableSource.Items
        {
            get
            {
                return this.ToArray();
            }
        }
    }
}
