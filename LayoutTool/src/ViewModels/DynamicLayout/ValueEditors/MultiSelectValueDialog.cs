using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels.DynamicLayout.ValueEditors
{
    public class MultiSelectValueDialog : ViewModelBase, IOkCancelDialogBoxViewModel
    {

        public MultiSelectValueDialog(MultiSelectValueEditor multiSelectValueEditor)
        {
            _multiSelectValueEditor = multiSelectValueEditor;

            var items = new ObservableCollectionExtended<MultiSelectValueEditor.Item>();
            foreach(var item in _multiSelectValueEditor.Items)
            {
                items.Add(new MultiSelectValueEditor.Item(item.Id, item.Name, item.IsSelected, item.OriginalItem));
            }

            this.Items = items;
        }

        MultiSelectValueEditor _multiSelectValueEditor;

        

        public ObservableCollectionExtended<MultiSelectValueEditor.Item> Items
        {
            get; private set;
        }

        public void ExecuteCancel()
        {
            
        }

        public void ExecuteOk()
        {
            _multiSelectValueEditor.Items = Items;
        }
    }
}
