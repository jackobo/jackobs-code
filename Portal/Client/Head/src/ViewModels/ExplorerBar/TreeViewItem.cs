using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using GamesPortal.Client.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.ExplorerBar
{
    public class TreeViewItem : ServicedViewModelBase, IExplorerBarItem
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

        private string _caption;

        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                OnPropertyChanged(() => Caption);
            }
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
            this.ServiceLocator.GetInstance<IApplicationServices>().ExecuteOnUIThread(() =>
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
            });
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
                
                if (_isSelected)
                {
                    if (this.Parent != null)
                    {
                        this.Parent.EnsureExpanded();
                    }

                    PublishEvent<IExplorerBarItem>(this);
                }

                OnPropertyChanged();
            }
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

        #region IExplorerBarItem Members


        public virtual IWorkspaceItem CreateWorkspaceItem(Microsoft.Practices.ServiceLocation.IServiceLocator serviceLocator)
        {
            return null;
        }

        #endregion

        public virtual IEnumerable<FoundItem> Search(string[] words)
        {
            var foundItems = new List<FoundItem>();

            if (IsMatch(this.Caption, words))
                foundItems.Add(new FoundItem(this));

            foundItems.AddRange(this.Items.SelectMany(item => item.Search(words)));

            return foundItems;

        }


        protected bool IsMatch(string text, string[] words)
        {
            if (words.Length == 0)
                return false;

            return words.All(w => Regex.IsMatch(this.Caption, w, RegexOptions.IgnoreCase));
        }


        public class FoundItem
        {
            public FoundItem(TreeViewItem item)
                : this(item.Caption, item)
            {
            }

            public FoundItem(string caption, TreeViewItem item)
            {
                this.Caption = caption;
                this.Item = item;
            }

            public string Caption { get; private set; }
            public TreeViewItem Item { get; private set; }

            public GameInfrastructure GameInfrastructure { get; set; }
            

        }
    }
}
