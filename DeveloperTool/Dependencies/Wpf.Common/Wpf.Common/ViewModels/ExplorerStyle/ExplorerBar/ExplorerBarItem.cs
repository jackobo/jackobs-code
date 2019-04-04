using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;
using Spark.Infra.Types;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.Wpf.Common.ViewModels
{
    public abstract class ExplorerBarItem : ServicedViewModelBase, IExplorerBarItem, ISupportCheckedChangeNotification
    {
        public ExplorerBarItem(IExplorerBar explorerBar, IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            _explorerBar = () => explorerBar;
            InitLazyLoading();
           
        }


        public ExplorerBarItem(IExplorerBarItem parent, IServiceLocator serviceLocator)
           : base(serviceLocator)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            this.Parent = Optional<IExplorerBarItem>.Some(parent);
            _explorerBar = () => parent.ExplorerBar;
            InitLazyLoading();
        }

        protected virtual bool UseLazyLoading
        {
            get { return false; }
        }

        Optional<LazyLoadingIndicatorExplorerBarItem> _lazyLoadIndicator = Optional<LazyLoadingIndicatorExplorerBarItem>.None();

        private void InitLazyLoading()
        {
            if (UseLazyLoading)
            {
                _lazyLoadIndicator = Optional<LazyLoadingIndicatorExplorerBarItem>.Some(new LazyLoadingIndicatorExplorerBarItem(this));               
                this.Items.Add(_lazyLoadIndicator.First());
            }
        }

        Func<IExplorerBar> _explorerBar;
        public IExplorerBar ExplorerBar
        {
            get { return _explorerBar(); }
        }

        bool _allowItemCheck = true;
        public bool AllowItemCheck
        {
            get { return _allowItemCheck; }
            set
            {
                SetProperty(ref _allowItemCheck, value);
            }
        }

        public string FullPath
        {
            get
            {
                if (!this.Parent.Any())
                    return this.Caption;

                return this.Parent.First().FullPath + "/" + this.Caption;
            }
        }
        public virtual IEnumerable<IExplorerBarItem> Search(string[] words)
        {
            var result = new List<IExplorerBarItem>();

            if (IsMatchingAllWords(this.Caption, words))
                result.Add(this);

            foreach(var child in this.Items)
            {
                result.AddRange(child.Search(words));
            }

            return result;
        }

        private bool IsMatchingAllWords(string text, string[] words)
        {
            if (words.Length == 0)
                return false;

            return words.All(w => Regex.IsMatch(this.Caption, w, RegexOptions.IgnoreCase));
        }



       


        public void ExpandAll()
        {
            foreach(var item in this.Items)
            {
                item.ExpandAll();
            }

            this.IsExpanded = true;
        }


        public void CollapseAll()
        {
            this.IsExpanded = false;
            foreach (var item in this.Items)
            {
                item.CollapseAll();
            }
        }

        public virtual IContextCommand[] Actions
        {
            get { return new IContextCommand[0]; }
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

        ObservableCollection<IExplorerBarItem> _items = new ObservableCollection<IExplorerBarItem>();
        public ObservableCollection<IExplorerBarItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged(() => Items);
            }
        }

        public Optional<IExplorerBarItem> Parent { get; private set; } = Optional<IExplorerBarItem>.None();


      

        public void Delete()
        {
            ObservableCollection<IExplorerBarItem> items = null;

            this.Parent.Do(p => items = p.Items);
            
            if (items == null)
                items = this.ExplorerBar.Items;

            var removedItemIndex = items.IndexOf(this);

            if (removedItemIndex < 0)
                throw new InvalidOperationException($"The item {this.FullPath} is already deleted");

            bool removedItemIsSelected = this.IsSelected;

            items.Remove(this);

            if(removedItemIsSelected)
            {
                if(items.Count == 0)
                {
                    this.Parent.Do(p => p.IsSelected = true);
                }
                else
                {
                    items[Math.Min(removedItemIndex, items.Count - 1)].IsSelected = true;
                }
            }

            this.Dispose();
        }

        protected override List<string> GetPropertiesExcludedFromGlobalNotification()
        {
            var properties = base.GetPropertiesExcludedFromGlobalNotification();

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
                        this.Parent.Do(p => p.EnsureExpanded());
                        this.ExplorerBar.CurrentItem = this;
                        OnSelected();
                        Activate();
                    }
                    else
                    {
                        Deactivate();
                    }

                }
            }
        }


        bool? _isChecked = false;
        public virtual bool? IsChecked
        {
            get
            {
                if(this.Items.Count == 0)
                    return _isChecked;

                if (this.Items.All(item => !item.AllowItemCheck))
                    return _isChecked;

                if (this.Items.All(item => item.IsChecked == true))
                    return true;

                if (this.Items.All(item => item.IsChecked == false))
                    return false;

                return null;
            }
            set
            {
                if (value == null)
                    value = !_isChecked.Value;

                SetProperty(ref _isChecked, value);

                foreach (var child in this.Items)
                {
                    child.IsChecked = value;
                }

                NotifyParentIsCheckedChanged();


            }
        }


        private void NotifyParentIsCheckedChanged()
        {
            this.Parent.Do(parent => parent.As<ISupportCheckedChangeNotification>()
                                           .Do(checkable => checkable.RaiseIsCheckedChanged()));
        }

        void ISupportCheckedChangeNotification.RaiseIsCheckedChanged()
        {
            OnPropertyChanged(nameof(IsChecked));
            NotifyParentIsCheckedChanged();
        }

        public virtual Optional<T> As<T>() where T : class
        {
            if (this is T)
                return Optional<T>.Some(this as T);

            return Optional<T>.None();
        }

        protected virtual void OnSelected()
        {

        }

        private bool _isExpanded = false;
        public virtual bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if(SetProperty(ref _isExpanded, value))
                {
                    if(_isExpanded)
                    {
                        DoLazyLoad();
                    }
                }
            }
        }

        private void DoLazyLoad()
        {
            _lazyLoadIndicator.Do(indicator =>
            {
                indicator.IsLoading = true;
                _lazyLoadIndicator = Optional<LazyLoadingIndicatorExplorerBarItem>.None();

                StartBusyAction(() =>
                {
                    var appServices = this.ServiceLocator.GetInstance<IApplicationServices>();

                    IEnumerable<IExplorerBarItem> items = new IExplorerBarItem[0];
                    try
                    {
                        items = GetLazyLoadedItems();
                    }
                    finally
                    {
                        appServices.ExecuteOnUIThread(() =>
                        {
                            indicator.IsLoading = false;
                            this.Items.Remove(indicator);
                        });
                    }

                    appServices.ExecuteOnUIThread(() =>
                        {
                            foreach (var item in items)
                            {
                                this.Items.Add(item);
                            }
                        });
                   
                });
            });
        }

        protected virtual IEnumerable<IExplorerBarItem> GetLazyLoadedItems()
        {
            throw new InvalidOperationException($"{nameof(GetLazyLoadedItems)} must be implemented in the the derived classes");
        }

        public virtual void EnsureExpanded()
        {
            this.IsExpanded = true;
            this.Parent.Do(p => p.EnsureExpanded());
        }

        public IExplorerBarItem[] GetAccessorsList()
        {
            var accessors = new List<IExplorerBarItem>();

            this.Parent.Do(p => accessors.AddRange(p.GetAccessorsList()));
            accessors.Add(this);

            return accessors.ToArray();
        }
    }

    

}
