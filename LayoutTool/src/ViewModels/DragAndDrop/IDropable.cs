using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.ViewModels
{
    public interface IDropable
    {
        bool CanDrop(IDragableSource source, DropContext context);
        DropResult Drop(IDragableSource source, DropContext context);
    }

    public class DropResult
    {
        public DropResult()
        {
            DroppedItems = new object[0];
        }

        public DropResult(IEnumerable<object> droppedItems)
        {
            DroppedItems = droppedItems?.ToArray() ?? new object[0];
        }

        public object[] DroppedItems { get; private set; }
    }

    public class DropContext
    {
        public DropContext(object data)
        {
            this.Data = data;
        }

        public DropLocation DropLocation { get; set; }
        public object Data { get; private set; }
    }

    public enum DropLocation
    {
        Before,
        After
    }

}
