using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public abstract class TreeViewItem : ServicedViewModelBase, IExplorerBarItem
    {
        public TreeViewItem(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {

            this.Items = new ObservableCollection<TreeViewItem>();

        }


        
        public TreeViewItem(TreeViewItem parent, IServiceLocator serviceLocator)
            : this(serviceLocator)
        {
            this.Parent = parent;
        }

        

        public abstract string Caption { get; }
        

        public override string ToString()
        {
            return this.Caption;
        }
        public bool IsAnyDescendantSelected
        {
            get
            {
                if (this.Items.Any(item => item.IsSelected))
                    return true;

                return this.Items.Any(item => item.IsAnyDescendantSelected);
            }
        }


        ObservableCollection<TreeViewItem> _items;
        public ObservableCollection<TreeViewItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged(() => Items);
            }
        }

        public TreeViewItem Parent
        {
            get;
            private set;
        }


        protected virtual void RemoveItem(TreeViewItem item)
        {
            if (item.IsSelected || item.IsAnyDescendantSelected)
            {
                if (this.Items.Count == 1)
                {
                    this.IsSelected = true;
                }
                else
                {
                    int foundItemIndex = this.Items.IndexOf(item);
                    if (foundItemIndex == this.Items.Count - 1)
                        this.Items[foundItemIndex - 1].IsSelected = true;
                    else
                        this.Items[foundItemIndex + 1].IsSelected = true;
                }
            }

            this.Items.Remove(item);

            item.Dispose();
        }

        protected override List<string> GetPropertiesExcludedFromGlobalNotification()
        {
            var properties =  base.GetPropertiesExcludedFromGlobalNotification();

            properties.Add(nameof(IsSelected));
            properties.Add(nameof(IsExpanded));

            return properties;
        }

        private bool _isSelected = false;
        public virtual bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (SetProperty(ref _isSelected, value))
                {
                    if (_isSelected)
                    {
                        if (this.Parent != null)
                        {
                            this.Parent.EnsureExpanded();
                        }
                        
                        OnSelected();
                    }
                }
            }
        }


        protected virtual void OnSelected()
        {

        }
        
        private bool _isExpanded = false;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                OnPropertyChanged(() => IsExpanded);
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

    public abstract class TreeViewItem<TViewModel> : TreeViewItem
        where TViewModel : IViewModel, IActivationAware
    {
        public TreeViewItem(TViewModel viewModel, IServiceLocator serviceLocator)
          : base(serviceLocator)
        {
            InitViewModel(viewModel);
        }
        
        public TreeViewItem(TViewModel viewModel, TreeViewItem parent, IServiceLocator serviceLocator)
            : base(parent, serviceLocator)
        {
            InitViewModel(viewModel);
        }

        private void InitViewModel(TViewModel viewModel)
        {
            this.ViewModel = viewModel;

            var activeAware = viewModel as IActivationAware;
            if (activeAware != null)
            {
                activeAware.Activated += ActiveAware_Activated;
            }

        }

        private void ActiveAware_Activated(object sender, EventArgs e)
        {
            this.IsSelected = true;
        }
        
        protected TViewModel ViewModel { get; private set; }

        protected override void OnSelected()
        {
            ServiceLocator.GetInstance<ISkinDesigner>().NavigateToWorkspace(ViewModel);
        }
    }
}
