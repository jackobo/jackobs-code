using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;

namespace LayoutTool.ViewModels
{
    public class ExplorerBarViewModel : ServicedViewModelBase
    {
        public ExplorerBarViewModel(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            this.Items = new TreeViewItemCollection();

            
        }

        private ObservableCollection<TreeViewItem> _items;
        public ObservableCollection<TreeViewItem> Items
        {
            get { return _items; }
            set
            {
                SetProperty(ref _items, value);
            }
        }

        public TreeViewItem GetSelectedItem()
        {
            return this.FindSelectedItem(Items);
        }

        private TreeViewItem FindSelectedItem(ObservableCollection<TreeViewItem> items)
        {
            foreach(var item in items)
            {
                if (item.IsSelected)
                    return item;

                var selectedChild = FindSelectedItem(item.Items);

                if (selectedChild != null)
                    return selectedChild;
            }

            return null;
        }
    }
}
