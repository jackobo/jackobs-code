using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using GGPGameServer.ApprovalSystem.Common;

namespace GGPMockBootstrapper.ViewModels
{
    public abstract class TreeViewItem : ViewModelBase
    {

        public TreeViewItem(IExplorerBar explorerBar)
            : this(null, explorerBar)
        {
            
        }

        public TreeViewItem(TreeViewItem parent)
            : this(parent, null)
        {
                        
        }

        private TreeViewItem(TreeViewItem parent, IExplorerBar explorerBar)
        {
            this.Items = new ObservableCollection<TreeViewItem>();
            this.Parent = parent;
            this.ExplorerBar = explorerBar;
        }

        public abstract string Caption { get; }

        IWorkAreaItemViewModel _workAreaItem;
        public virtual IWorkAreaItemViewModel WorkAreaItem
        {
            get
            {
                
                if (_workAreaItem == null)
                    _workAreaItem = CreateWorkAreaItem(this.ExplorerBar.GetWorkArea());

                return _workAreaItem;
                
            }
        }

        protected abstract IWorkAreaItemViewModel CreateWorkAreaItem(IWorkArea workArea);
        

        public virtual ImageSource ImageSource
        {
            get { return null; }
        }


        public TreeViewItem Parent { get; private set; }

        public virtual ObservableCollection<TreeViewItem> Items { get; private set; }

        IExplorerBar _explorerBar;
        protected IExplorerBar ExplorerBar
        {
            get
            {
                if (this.Parent == null)
                {
                    return _explorerBar;
                }
                else
                {
                    return this.Parent.ExplorerBar;
                }
            }
            set
            {
                _explorerBar = value;
            }
        }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged(this.GetPropertyName(t => t.IsSelected));
                if (_isSelected)
                {
                    if (this.Parent != null)
                    {
                        this.Parent.EnsureExpanded();
                    }

                    this.ExplorerBar.SelectedItem = this;
                }
            }
        }


        private bool _isExpanded = false;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                OnPropertyChanged(this.GetPropertyName(t => t.IsExpanded));
            }
        }


        public void EnsureExpanded()
        {
            this.IsExpanded = true;

            if (this.Parent != null)
            {
                this.Parent.EnsureExpanded();
            }
        }

    }
}
