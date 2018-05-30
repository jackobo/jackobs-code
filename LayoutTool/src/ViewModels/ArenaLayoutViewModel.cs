using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{

    public interface IArenaLayoutViewModel : INotifyPropertyChanged
    {
        string Description { get; }
        bool IsNewGamesArena { get; }

        void NavigateToGame(ArenaGameViewModel game);
    }

    public class ArenaLayoutViewModel : ViewModelBase, IDropable, IArenaLayoutViewModel, ISupportDynamicLayout<ArenaLayoutViewModel>
    {
    

        public AttributeValueCollection Attributes { get; private set; }


        public ArenaLayoutViewModel(IArenaViewModel arena, PlayerStatusTypeViewModel playerStatus, AttributeValueCollection attributes, ArenaGridColumnCollection dataGridInfo)
        {
            _arena = arena;
            PlayerStatus = playerStatus;
            
            Attributes = attributes;
            DataGridInfo = dataGridInfo;
            _games = new ArenaGameCollectionViewModel(this);
            _filters = new ArenaFilterCollectionViewModel(this);
            _alsoPlayingGames = new AlsoPlayingGamesCollection();

            _arena.PropertyChanged += Arena_PropertyChanged;
            
        }


        public ArenaGridColumnCollection DataGridInfo { get; private set; }

        protected override List<string> GetPropertiesExcludedFromGlobalNotification()
        {
            var properties = base.GetPropertiesExcludedFromGlobalNotification();
            properties.Add(nameof(SelectedGame));
            return properties;
        }

        private ArenaGameViewModel _selectedGame;
        public ArenaGameViewModel SelectedGame
        {
            get
            {
                return _selectedGame;
            }

            set
            {
                _selectedGame = value;
                OnPropertyChanged(); //Always raise the SelectedGame property changed so the bound grids to scroll it into view.
            }
        }

        IArenaViewModel _arena;

        private void Arena_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(IArenaViewModel.IsNewGamesArena))
            {
                OnPropertyChanged(nameof(IsNewGamesArena));
            }
        }



        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if (_arena != null)
                {
                    this.Games.Dispose();
                    this.Filters.Dispose();
                    this.AlsoPlayingGames.Dispose();
                    _arena.PropertyChanged -= Arena_PropertyChanged;
                    _arena = null;
                }
            }
            base.Dispose(disposing);
        }

        private PlayerStatusTypeViewModel _playerStatus;

        public PlayerStatusTypeViewModel PlayerStatus
        {
            get
            {
                return _playerStatus;
            }

            set
            {
                if(SetProperty(ref _playerStatus, value))
                {
                    if(_playerStatus != null)
                    {
                        _playerStatus.PropertyChanged += PlayerStatus_PropertyChanged;
                    }
                }
            }
        }

        private void PlayerStatus_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PlayerStatus));
            OnPropertyChanged(nameof(Description));
        }

        public bool IsNewGamesArena
        {
            get
            {
                return _arena.IsNewGamesArena;
            }
           
        }

        
        public bool JackpotVisible
        {
            get
            {
                return Attributes.GetAttributeValue<bool>(ArenaLayout.JPVisibleAttributeName);
            }
            set
            {
                if(value != JackpotVisible)
                {
                    Attributes.SetAttributeValue(ArenaLayout.JPVisibleAttributeName, value.ToString().ToLowerInvariant());
                    OnPropertyChanged();
                }
                
            }
        }


        ArenaFilterCollectionViewModel _filters;
        public ArenaFilterCollectionViewModel Filters
        {
            get { return _filters; }
            private set
            {
                SetProperty(ref _filters, value);
            }
        }



        ArenaGameCollectionViewModel _games;
        public ArenaGameCollectionViewModel Games
        {
            get
            {
                return _games;
            }
            private set
            {
                SetProperty(ref _games, value);
            }
        }


        private AlsoPlayingGamesCollection _alsoPlayingGames;
        public AlsoPlayingGamesCollection AlsoPlayingGames
        {
            get { return _alsoPlayingGames; }
            private set
            {
                SetProperty(ref _alsoPlayingGames, value);
            }
        }

        private bool _isSelected;

        public event EventHandler IsActiveChanged;

        private void OnIsActiveChanged()
        {
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                if(SetProperty(ref _isSelected, value))
                {
                    OnIsActiveChanged();
                }
            }
        }

        public string Description
        {
            get
            {
                return _arena.Name + " - " + PlayerStatus.ToString();
            }
        }
        public override void Activate()
        {
            this.IsSelected = true;
            base.Activate();
        }

        public void Activate(ArenaGameViewModel gameToSelect)
        {
            Activate();

            if (gameToSelect != null)
            {
                SelectedGame = gameToSelect;
            }
        }

        bool IDropable.CanDrop(IDragableSource source, DropContext context)
        {
            return this.Games.CanDrop(source, context);
        }

        DropResult IDropable.Drop(IDragableSource source, DropContext context)
        {
            return this.Games.Drop(source, context);
        }

        public ArenaLayoutViewModel Clone(PlayerStatusTypeViewModel newPlayerStatus)
        {
            var clone = new ArenaLayoutViewModel(_arena, newPlayerStatus, this.Attributes.Clone(), this.DataGridInfo.Clone());

            foreach(var game in this.AlsoPlayingGames)
            {
                clone.AlsoPlayingGames.Add(game);
            }

            foreach(var filter in this.Filters)
            {
                clone.Filters.Add(filter);
            }

            foreach(var game in this.Games)
            {
                clone.Games.Add(new ArenaGameViewModel(game.ConvertToAvailableGame(), clone.Games));
            }

            clone.JackpotVisible = this.JackpotVisible;
            

            return clone;
        }

        void IArenaLayoutViewModel.NavigateToGame(ArenaGameViewModel game)
        {
            _arena.NavigateTo(this, game);
        }
    }
}
