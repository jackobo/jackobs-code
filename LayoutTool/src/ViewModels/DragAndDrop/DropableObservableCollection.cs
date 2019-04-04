using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class DropableObservableCollection<TItem> : ObservableCollectionExtended<TItem>, IDropable
        where TItem : IDragableSource
    {
        public DropableObservableCollection()
        {

        }

        public DropableObservableCollection(IEnumerable<TItem> collection)
            : base(collection)
        {
        }

      
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        public bool CanDrop(IDragableSource source, DropContext context)
        {
            if (source == null)
                return false;


            return source.Items.All(droppedItem => CanDropItem(droppedItem, context));

        }

        protected virtual bool CanDropItem(object droppedItem, DropContext context)
        {
            var convertible = droppedItem as IConvertible<TItem>;

            if (typeof(TItem) != droppedItem.GetType() && convertible == null)
                return false;

            if (convertible != null)
                return !this.Contains(convertible.Convert());

            return !this.Contains((TItem)droppedItem);
        }

        public DropResult Drop(IDragableSource source, DropContext context)
        {
            if (!CanDrop(source, context))
                return new DropResult();


            var finalItems = new List<object>();
            foreach(var droppedItem in source.Items)
            {
                finalItems.Add(DropItem(droppedItem, context));
            }

            return new DropResult(finalItems);
        }

        protected virtual object DropItem(object droppedItem, DropContext context)
        {
            var convertible = droppedItem as IConvertible<TItem>;

            TItem item;
            if (convertible != null)
                item = convertible.Convert();
            else
                item = (TItem)droppedItem;

            if (!this.Contains(item))
            {
                this.Add(item);
            }

            return item;
        }
    }
}
