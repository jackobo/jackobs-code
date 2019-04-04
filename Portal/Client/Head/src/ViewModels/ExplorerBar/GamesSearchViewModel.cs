using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GamesPortal.Client.Interfaces.Entities;
using Prism.Commands;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.ExplorerBar
{
    public class GamesSearchViewModel : ViewModelBase
    {

        public GamesSearchViewModel(IEnumerable<TreeViewItem> originalItems)
        {
            _originalItems = originalItems ?? new TreeViewItem[0];
            this.Items = new ObservableCollection<FoundItemWrapper>();
            this.Filter = string.Empty;
            this.CloseCommand = new Command(() => this.IsOpen = false);
        }


        public ICommand CloseCommand { get; private set; }

        private IEnumerable<TreeViewItem> _originalItems;

        private ObservableCollection<FoundItemWrapper> _items;

        public ObservableCollection<FoundItemWrapper> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged(() => Items);
            }
        }

        private string _filter;

        public string Filter
        {
            get { return _filter; }
            set
            {
                if (SetProperty(ref _filter, value))
                {
                    Search(_filter);
                }
                
            }
        }

        private bool _isOpen = false;

        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                SetProperty(ref _isOpen, value);
            }
        }

        private void Search(string filter)
        {
            filter = (filter ?? string.Empty).Trim();

            this.IsOpen = !string.IsNullOrEmpty(filter);

            if (string.IsNullOrEmpty(filter))
            {
                this.Items.Clear();
                return;
            }

            var result = new ObservableCollection<FoundItemWrapper>();

            foreach (var originalItem in _originalItems)
            {
                result.AddRange(originalItem.Search(this.Filter.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                                    .Select(foundItem => new FoundItemWrapper(this, foundItem)));
            }

            this.Items = result;
        }


        public class FoundItemWrapper
        {
            public FoundItemWrapper(GamesSearchViewModel gamesSearcher, TreeViewItem.FoundItem foundItem)
            {
                this.GamesSearcher = gamesSearcher;
                this.FoundItem = foundItem;

                this.GoToCommand = new Command(() => 
                    {
                        this.FoundItem.Item.IsSelected = true;
                        this.GamesSearcher.CloseCommand.Execute(null);
                    });
            }


            GamesSearchViewModel GamesSearcher { get; set; }

            TreeViewItem.FoundItem FoundItem { get; set; }

            public string Caption
            {
                get { return this.FoundItem.Caption; }
            }


            public ICommand GoToCommand { get; private set; }


            public GameInfrastructure Infrastructure
            {
                get
                {
                    return this.FoundItem.GameInfrastructure;
                }
            }
            

        }


       
    }
}
