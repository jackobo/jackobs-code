using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.Wpf.Common.ViewModels
{
    internal class LazyLoadingIndicatorExplorerBarItem : ViewModelBase, IExplorerBarItem
    {
        public LazyLoadingIndicatorExplorerBarItem(IExplorerBarItem parent)
        {
            this.Parent = parent;
        }


        public string Caption
        {
            get
            {
                return "Loading...";
            }
        }

        public bool AllowItemCheck
        {
            get { return false; }
            set
            {

            }
        }
        public override string ToString()
        {
            return this.Caption;
        }

        IExplorerBarItem Parent { get; set; }

        
        public IExplorerBar ExplorerBar
        {
            get { return Parent.ExplorerBar; }
        }

        public string FullPath
        {
            get
            {
                return string.Empty;
            }
        }

        public bool IsAnyDescendantSelected
        {
            get
            {
                return false;
            }
        }

        public bool? IsChecked
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        bool _isExpanded;
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                SetProperty(ref _isExpanded, value);
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                return _isLoading;                   
            }
            set
            {
                SetProperty(ref _isLoading, value);
            }
        }

        bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                SetProperty(ref _isSelected, value);
            }
        }

        public ObservableCollection<IExplorerBarItem> Items { get; } = new ObservableCollection<IExplorerBarItem>();

       

        public Optional<T> As<T>() where T : class
        {
            return Optional<T>.None();
        }

        public void CollapseAll()
        {
        }
        
        public void EnsureExpanded()
        {
            this.IsExpanded = true;
        }

        public void ExpandAll()
        {
            this.IsExpanded = true;
        }

        public IExplorerBarItem[] GetAccessorsList()
        {
            return new IExplorerBarItem[0];
        }

        public IEnumerable<IExplorerBarItem> Search(string[] words)
        {
            return new IExplorerBarItem[0];
        }
    }
}
