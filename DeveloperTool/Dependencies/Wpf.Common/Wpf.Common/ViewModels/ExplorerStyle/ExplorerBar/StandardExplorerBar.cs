using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;

namespace Spark.Wpf.Common.ViewModels
{
    public class StandardExplorerBar : ServicedViewModelBase, IExplorerBar
    {
        public StandardExplorerBar(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            this.CheckAllCommand = new Command(CheckAll);
            this.UncheckAllCommand = new Command(UncheckAll);
            this.SearchBox = new ExplorerBarSearchViewModel(this, serviceLocator);
        }

        IExplorerBarItem _currentItem;
        public IExplorerBarItem CurrentItem
        {
            get { return _currentItem; }
            set
            {
                SetProperty(ref _currentItem, value);
            }
        }

        public bool AllowSearching
        {
            get { return SearchBox.IsVisible; }
            set
            {
                SearchBox.IsVisible = value;
                OnPropertyChanged(nameof(AllowSearching));
            }
        }

        public ExplorerBarSearchViewModel SearchBox { get; private set; }

        bool _allowItemsCheck = false;
        public virtual bool AllowItemsCheck
        {
            get
            {
                return _allowItemsCheck;
            }
            set
            {
                SetProperty(ref _allowItemsCheck, value);
            }
        }

        ObservableCollection<IExplorerBarItem> _items = new ObservableCollection<IExplorerBarItem>();
        public ObservableCollection<IExplorerBarItem> Items
        {
            get { return _items; }
        }

        public void SelectFirstItem()
        {
            if (this.Items.Count > 0)
                this.Items[0].IsSelected = true;
        }

        public ICommand CheckAllCommand { get; private set; }
        public ICommand UncheckAllCommand { get; private set; }

        public void ExpandAll()
        {
            foreach (var item in this.Items)
                item.ExpandAll();
        }


        public void CollapseAll()
        {
            foreach (var item in this.Items)
                item.CollapseAll();
        }

        public IExplorerBar GetCheckedItemsAsExplorerBar()
        {
            var explorerBar = new StandardExplorerBar(ServiceLocator);

            if (this.Items.Count == 0)
                return explorerBar;

            explorerBar.Items.AddRange(GetCheckedItems(this.Items, 
                                                       item => new ExplorerBarItemWrapper(item, 
                                                                                          explorerBar, 
                                                                                          ServiceLocator)));

            return explorerBar;
        }

        
        
        private IEnumerable<ExplorerBarItemWrapper> GetCheckedItems(IEnumerable<IExplorerBarItem> items, 
                                                                   Func<IExplorerBarItem, ExplorerBarItemWrapper> wrap)
        {
            var result = new List<ExplorerBarItemWrapper>();

            foreach(var checkedItem in items.Where(item => item.IsChecked != false))
            {
                var wrappedItem = wrap(checkedItem);
                wrappedItem.Items.AddRange(GetCheckedItems(checkedItem.Items, 
                                                           childItem => new ExplorerBarItemWrapper(childItem, 
                                                                                                   wrappedItem, 
                                                                                                   ServiceLocator)));
                result.Add(wrappedItem);
                
            }

            return result;
        }


      

        public void CheckAll()
        {
            CheckAllItems(true);
        }

        public void UncheckAll()
        {
            CheckAllItems(false);
        }

        private void CheckAllItems(bool value)
        {
            foreach (var item in this.Items)
            {
                item.IsChecked = value;
            }
        }


        private class ExplorerBarItemWrapper : ExplorerBarItem
        {
            public ExplorerBarItemWrapper(IExplorerBarItem innerItem, IExplorerBarItem parent, IServiceLocator serviceLocator)
                : base(parent, serviceLocator)
            {
                Init(innerItem);
            }

          

            public ExplorerBarItemWrapper(IExplorerBarItem innerItem, IExplorerBar explorerBar, IServiceLocator serviceLocator)
                : base(explorerBar, serviceLocator)
            {
                Init(innerItem);
            }


            private void Init(IExplorerBarItem innerItem)
            {
                _innerItem = innerItem;
                this.IsSelected = _innerItem.IsSelected;
                this.IsChecked = _innerItem.IsChecked;
                this.AllowItemCheck = _innerItem.AllowItemCheck;
            }

            public override string Caption
            {
                get
                {
                    return _innerItem.Caption;
                }
            }

            IExplorerBarItem _innerItem;

            public override Optional<T> As<T>()
            {
                var result = base.As<T>();
                if (result.Any())
                    return result;

                return _innerItem.As<T>();
            }
        }
    }
}
