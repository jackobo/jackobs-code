using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class LobbyViewModel : ViewModelBase, IDropable, ISupportDynamicLayout<LobbyViewModel>
    {
        public LobbyViewModel(PlayerStatusTypeViewModel playerStatus)
        {
            this.PlayerStatus = playerStatus;
            
        }
        public LobbyViewModel(PlayerStatusTypeViewModel playerStatus, int favoritesSize)
        {
            this.PlayerStatus = playerStatus;
            this.FavoritesSize = favoritesSize;
        }
        
       
        PlayerStatusTypeViewModel _playerStatus;
        public PlayerStatusTypeViewModel PlayerStatus
        {
            get { return _playerStatus; }
            set
            {
                if(SetProperty(ref _playerStatus, value))
                {
                    if(_playerStatus != null)
                        _playerStatus.PropertyChanged += PlayerStatus_PropertyChanged;
                }
            }
        }

        private void PlayerStatus_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PlayerStatus));
        }

        private int _favoritesSize = 4;
        public int FavoritesSize
        {
            get { return _favoritesSize; }
            set
            {
                SetProperty(ref _favoritesSize, value);
            }
        }

        private LobbyItemViewModelCollection _items = new LobbyItemViewModelCollection();

        public LobbyItemViewModelCollection Items
        {
            get
            {
                return _items;
            }
            private set
            {
                SetProperty(ref _items, value);
            }
        }

        bool IDropable.CanDrop(IDragableSource source, DropContext context)
        {
            return this.Items.CanDrop(source, context);
        }

        DropResult IDropable.Drop(IDragableSource source, DropContext context)
        {
            return Items.Drop(source, context);
        }

        public LobbyViewModel Clone(PlayerStatusTypeViewModel newPlayerStatus)
        {
            var clone = new LobbyViewModel(newPlayerStatus, this.FavoritesSize);

            foreach(var item in this.Items)
            {
                clone.Items.Add(item);
            }

            return clone;
        }
    }

    public class LobbyCollectionViewModel : ObservableCollectionExtended<LobbyViewModel>
    {

        public string Title
        {
            get { return "Arenas"; }
        }

        public LobbyCollectionViewModel(int alowedNumberOfItems, IPlayerStatusFriendlyNameProvider playerStatusFriendlyNameProvider)
        {
            this.AlowedNumberOfItems = alowedNumberOfItems;
            this.AddDynamicLayoutHandler = new AddDynamicLayoutHandler<LobbyViewModel>(this,
                                                                                       ps => new LobbyViewModel(ps, AlowedNumberOfItems),
                                                                                       playerStatusFriendlyNameProvider);
        }

        public int AlowedNumberOfItems { get; private set; }

        public AddDynamicLayoutHandler<LobbyViewModel> AddDynamicLayoutHandler { get; private set; }
    }

    public class LobbyItemViewModelCollection :  DropableObservableCollection<LobbyItemViewModel>
    {
        public LobbyItemViewModelCollection()
        {
        }

        protected override bool CanDropItem(object droppedItem, DropContext context)
        {
            if (base.CanDropItem(droppedItem, context))
                return true;

            var lobbyItemSource = droppedItem as ILobbyItemSource;
            if (lobbyItemSource == null)
                return false;

            return !this.Any(item => item.Id == lobbyItemSource.Id);
        }


        protected override object DropItem(object droppedItem, DropContext context)
        {
            if (droppedItem is ILobbyItemSource)
            {
                return base.DropItem(new LobbyItemViewModel(droppedItem as ILobbyItemSource), context);
            }
            else
            {
                return base.DropItem(droppedItem, context);
            }
        }

    }
    
    public interface ILobbyItemSource
    {

        int Id { get; }
        string Name { get; }
        bool ShouldShowTheJackpot(PlayerStatusTypeViewModel playerStatus);
    }

    public class LobbyItemViewModel : DragableDropableViewModel
    {
        public LobbyItemViewModel(ILobbyItemSource source)
        {
            this.Source = source;
        }

        ILobbyItemSource Source { get; set; }

        public int Id
        {
            get { return this.Source.Id; }
        }

        public string Name
        {
            get { return this.Source.Name; }
        }

        public bool ShouldShowJackpot(PlayerStatusTypeViewModel playerStatus)
        {
            return this.Source.ShouldShowTheJackpot(playerStatus);
            
        }

        protected override bool CanDropItem(object droppedItem, DropContext context)
        {
            if (base.CanDropItem(droppedItem, context))
                return true;

            if (droppedItem is ILobbyItemSource)
                return true;

            return false;
        }


        protected override object DropItem(object droppedItem, DropContext context)
        {

            if (droppedItem is ILobbyItemSource)
            {
                return base.DropItem(new LobbyItemViewModel(droppedItem as ILobbyItemSource), context);
            }
            else
            {
                return base.DropItem(droppedItem, context);
            }

            
        }

    
        public override bool Equals(object obj)
        {
            var theOther = obj as LobbyItemViewModel;

            if (theOther == null)
                return false;

            return this.Id == theOther.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }


}
