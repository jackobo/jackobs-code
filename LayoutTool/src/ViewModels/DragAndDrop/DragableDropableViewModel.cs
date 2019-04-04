using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class DragableDropableViewModel : ViewModelBase, IDragableSource, IDropable
    {
        object[] IDragableSource.Items
        {
            get
            {
                return new object[] { this };
            }
        }

        public bool CanDrop(IDragableSource source, DropContext context)
        {
            if (source == null)
                return false;

            if(!(context.Data is System.Collections.IList))
            {
                return false;
            }

            return source.Items.All(item => CanDropItem(item, context));
        }

        protected virtual bool CanDropItem(object droppedItem, DropContext context)
        {
            return this.GetType().Equals(droppedItem.GetType());
        }

        public DropResult Drop(IDragableSource source, DropContext context)
        {
            if (!CanDrop(source, context))
                return new DropResult();

            var resultItems = new List<object>();
            foreach(object item in source.Items)
            {
                var finalItem = DropItem(item, context);
                if(finalItem != null)
                    resultItems.Add(finalItem);
            }

            return new DropResult(resultItems);
        }


        protected virtual object DropItem(object droppedItem, DropContext context)
        {
            if (this.Equals(droppedItem))
                return droppedItem;


            var theList = context.Data as System.Collections.IList;

            if (theList == null)
                return droppedItem;

            if (theList.Contains(droppedItem))
            {
                theList.Remove(droppedItem);
            }

            var thisIndex = theList.IndexOf(this);

            if (context.DropLocation == DropLocation.Before)
            {
                theList.Insert(thisIndex, droppedItem);
            }
            else
            {
                theList.Insert(thisIndex + 1, droppedItem);
            }

            return droppedItem;
        }
    
    }
}
