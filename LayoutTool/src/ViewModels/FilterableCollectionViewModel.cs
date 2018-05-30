using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class FilterableCollectionViewModel<TItem> : ViewModelBase, ICollectionView
        where TItem : class
    {
        private static PropertyDescriptorCollection AllPropertyDescriptors;

        static FilterableCollectionViewModel()
        {
            AllPropertyDescriptors = TypeDescriptor.GetProperties(typeof(TItem));
        }

        public FilterableCollectionViewModel(params string[] filtarableProperties)
        {
            Init(new ObservableCollection<TItem>(), filtarableProperties);
        }
        

        public FilterableCollectionViewModel(IEnumerable<TItem> collection, params string[] filtarableProperties)
        {
            Init(new ObservableCollection<TItem>(collection), filtarableProperties);
        }


        private void Init(ObservableCollection<TItem> collection, string[] filtarableProperties)
        {
            _observableCollection = collection;
            _collectionView = CollectionViewSource.GetDefaultView(_observableCollection);
            _collectionView.CurrentChanged += CollectionView_CurrentChanged;
            _collectionView.CurrentChanging += CollectionView_CurrentChanging;
            _collectionView.CollectionChanged += CollectionView_CollectionChanged;
            
            LoadFiltarableProperies(filtarableProperties);
            
        }

        PropertyDescriptor[] _filtarableProperies;


        private void LoadFiltarableProperies(string[] filtarableProperties)
        {

            if (0 == (filtarableProperties?.Length ?? 0))
            {
                _filtarableProperies = AllPropertyDescriptors.Cast<PropertyDescriptor>().ToArray();
            }
            else
            {
                _filtarableProperies = AllPropertyDescriptors.Cast<PropertyDescriptor>().Where(pd => filtarableProperties.Contains(pd.Name)).ToArray();
            }

            
        }

        private ObservableCollection<TItem> _observableCollection;

        private string _filter;
        private string[] _words;

        ICollectionView _collectionView;

        public TItem[] GetOriginalItems()
        {
            return _observableCollection.ToArray();
        }

        public string Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                if (SetProperty(ref _filter, value))
                {
                    _words = _filter?.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
                    if (_words.Length == 0)
                        _collectionView.Filter = null;
                    else
                        _collectionView.Filter = FilterPredicate;
                }
            }
        }

        private bool FilterPredicate(object item)
        {
         
            foreach(var w in _words)
            {

                if(!_filtarableProperies.Any(pd =>
                {
                    var propertyValue = pd.GetValue(item)?.ToString() ?? string.Empty;

                    return propertyValue.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0;
                }))
                {
                    return false;
                }
            }


            return true;
        }


        public int Count
        {
            get
            {
                return _collectionView.Cast<TItem>().Count();
            }
        }

        #region ICollectionView implementation

        public event CurrentChangingEventHandler CurrentChanging;
        public event EventHandler CurrentChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void CollectionView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(e);
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        private void CollectionView_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            CurrentChanging?.Invoke(this, e);
        }

        private void CollectionView_CurrentChanged(object sender, EventArgs e)
        {
            CurrentChanged?.Invoke(this, e);
        }

        public CultureInfo Culture
        {
            get
            {
                return _collectionView.Culture;
            }

            set
            {
                _collectionView.Culture = value;
            }
        }

        IEnumerable ICollectionView.SourceCollection
        {
            get
            {
                return _collectionView.SourceCollection;
            }
        }

        Predicate<object> ICollectionView.Filter
        {
            get
            {
                return _collectionView.Filter;
            }

            set
            {
                _collectionView.Filter = value;
            }
        }

        public bool CanFilter
        {
            get
            {
                return _collectionView.CanFilter;
            }
        }

        public SortDescriptionCollection SortDescriptions
        {
            get
            {
                return _collectionView.SortDescriptions;
            }
        }

        public bool CanSort
        {
            get
            {
                return _collectionView.CanSort;
            }
        }

        public bool CanGroup
        {
            get
            {
                return _collectionView.CanGroup;
            }
        }

        public ObservableCollection<GroupDescription> GroupDescriptions
        {
            get
            {
                return _collectionView.GroupDescriptions;
            }
        }

        public ReadOnlyObservableCollection<object> Groups
        {
            get
            {
                return _collectionView.Groups;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return _collectionView.IsEmpty;
            }
        }

        object ICollectionView.CurrentItem
        {
            get
            {
                return _collectionView.CurrentItem;
            }
        }


        public TItem CurrentItem
        {
            get { return _collectionView.CurrentItem as TItem; }
        }

        public int CurrentPosition
        {
            get
            {
                return _collectionView.CurrentPosition;
            }
        }

        public bool IsCurrentAfterLast
        {
            get
            {
                return _collectionView.IsCurrentAfterLast;
            }
        }

        public bool IsCurrentBeforeFirst
        {
            get
            {
                return _collectionView.IsCurrentBeforeFirst;
            }
        }

      

        bool ICollectionView.Contains(object item)
        {
            return _collectionView.Contains(item);
        }


        public bool Contains(TItem item)
        {
            return _collectionView.Contains(item);
        }


        public void Refresh()
        {
            _collectionView.Refresh();
        }

        public IDisposable DeferRefresh()
        {
            return _collectionView.DeferRefresh();
        }

        public bool MoveCurrentToFirst()
        {
            return _collectionView.MoveCurrentToFirst();
        }

        public bool MoveCurrentToLast()
        {
            return _collectionView.MoveCurrentToLast();
        }

        public bool MoveCurrentToNext()
        {
            return _collectionView.MoveCurrentToNext();
        }

        public bool MoveCurrentToPrevious()
        {
            return _collectionView.MoveCurrentToPrevious();
        }

        bool ICollectionView.MoveCurrentTo(object item)
        {
            return _collectionView.MoveCurrentTo(item);
        }

        public bool MoveCurrentTo(TItem item)
        {
            return _collectionView.MoveCurrentTo(item);
        }

        public bool MoveCurrentToPosition(int position)
        {
            return _collectionView.MoveCurrentToPosition(position);
        }

        public IEnumerator GetEnumerator()
        {
            return _collectionView.GetEnumerator();
        }

        #endregion
    }
}
