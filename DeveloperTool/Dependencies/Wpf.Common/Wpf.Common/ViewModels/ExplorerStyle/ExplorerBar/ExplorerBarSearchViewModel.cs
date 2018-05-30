using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.Wpf.Common.ViewModels
{
    public class ExplorerBarSearchViewModel : ServicedViewModelBase
    {
        public ExplorerBarSearchViewModel(IExplorerBar explorerBar, IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            _explorerBar = explorerBar;
        }

        IExplorerBar _explorerBar;


        private string _title = "Search";
        public string Title
        {
            get { return _title = "Search"; }
            set { SetProperty(ref _title, value); }
        }

        private bool _isSearching = false;

        public bool IsSearching
        {
            get { return _isSearching; }
            set
            {
                SetProperty(ref _isSearching, value);
            }
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                SetProperty(ref _isVisible, value);
            }
        }

        string _filter;

        public string Filter
        {
            get { return _filter; }
            set
            {
                if (SetProperty(ref _filter, value))
                {
                    var appServices = this.ServiceLocator.GetInstance<IApplicationServices>();
                    appServices.StartNewParallelTask(() =>  Search());
                }
                Show();
            }
        }

        private void Search()
        {
            var filter = _filter?.Trim();

            if (string.IsNullOrEmpty(filter))
            {
                this.SearchResult = new FoundItem[0];
                return;
            }

            var words = filter.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var searchResult = new List<FoundItem>();

            foreach(var item in _explorerBar.Items)
            {
                searchResult.AddRange(item.Search(words).Select(foundItem => new FoundItem(foundItem, this)));
            }

            this.SearchResult = searchResult.ToArray();

        }

        FoundItem[] _searchResult;
        public FoundItem[] SearchResult
        {
            get { return _searchResult; }
            private set
            {
                SetProperty(ref _searchResult, value);
            }

        }

        public void Close()
        {
            this.IsSearching = false;
        }

        public void Show()
        {
            this.IsSearching = true;
        }

        public class FoundItem : ViewModelBase
        {
            public FoundItem(IExplorerBarItem item, ExplorerBarSearchViewModel searcher)
            {
                _item = item;
                _searcher = searcher;
                this.GoToItemCommand = new Command(() => { _searcher.Close(); _item.IsSelected = true; });
            }

            ExplorerBarSearchViewModel _searcher;
            IExplorerBarItem _item;

            public string Caption
            {
                get { return _item.FullPath;  }
            }

            public ICommand GoToItemCommand { get; private set; }

            
        }

       
    }

   
}
