using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LayoutTool.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{

  

    
    public class AvailableGameViewModel : DragableDropableViewModel, ILobbyItemSource
    {
        public AvailableGameViewModel(int gameType, string name, string gameGroup, bool newGame, int userMode, bool? isApproved, string vendorName, int[] jackpotIds, IServiceLocator serviceLocator)
        {
            this.GameType = gameType;
            this.Name = name;
            this.GameGroup = gameGroup;
            this.IsNewGame = newGame;
            this.IsVipGame = userMode == UserModes.VIP;
            IsApproved = isApproved;
            this.VendorName = vendorName;
            this.Owners.CollectionChanged += Owners_CollectionChanged;
            _jackpotIds = jackpotIds;
            _serviceLocator = serviceLocator;
            
            this.GoToOwnerCommnad = new Command<IArenaGameCollectionViewModel>(GoToOwner);
        }

        int[] _jackpotIds = new int[0];

       

        IServiceLocator _serviceLocator;
        
        public bool? IsApproved { get; private set; }
        
        
        public string ApprovedText
        {
            get
            {
                if (IsApproved == true)
                    return "Yes";
                else if (IsApproved == false)
                    return "No";

                return "?";
            }
        }

        private void Owners_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsNewGame));
            

            if (e.OldItems != null)
                UnsubscribeFromOwnerPropertyChanged(e.OldItems.Cast<IArenaGameCollectionViewModel>());

            if (e.NewItems != null)
                SubscribeToOwnerPropertyChanged(e.NewItems.Cast<IArenaGameCollectionViewModel>());

        }

        private void SubscribeToOwnerPropertyChanged(IEnumerable<IArenaGameCollectionViewModel> owners)
        {
            foreach (var o in owners)
            {
                o.PropertyChanged += Owner_PropertyChanged;
            }
        }

        private void UnsubscribeFromOwnerPropertyChanged(IEnumerable<IArenaGameCollectionViewModel> owners)
        {
            foreach(var o in owners)
            {
                o.PropertyChanged -= Owner_PropertyChanged;
            }
        }

        private void Owner_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(IArenaGameCollectionViewModel.IsNewGamesArena))
            {
                OnPropertyChanged(nameof(IsNewGame));
            }
            
        }

        private bool _isVipGame;

        public bool IsVipGame
        {
            get { return _isVipGame; }
            set
            {
                SetProperty(ref _isVipGame, value);
            }
        }
        
        public int GameType { get; private set; }

        public string Name { get; private set; }

        bool _isNewGame;
        public bool IsNewGame
        {
            get
            {
                return _isNewGame || IsInTheNewGamesArena;
            }
            set
            {
                if(IsInTheNewGamesArena)
                {
                    MessageBox.ShowMessage("You can't change the IsNewGame status if the game is in the New Games arena!" 
                                            + Environment.NewLine
                                            + "You must first remove the game from the New Games arena.");
                    return;
                }

                SetProperty(ref _isNewGame, value);
            }
        }

        IMessageBox MessageBox
        {
            get { return _serviceLocator.GetInstance<IMessageBox>(); }
        }

        private bool IsInTheNewGamesArena
        {
            get
            {
                return this.Owners.Any(o => o.IsNewGamesArena == true);
            }
        }

        public string VendorName { get; private set; }

        private ObservableCollection<IArenaGameCollectionViewModel> _owners = new ObservableCollection<IArenaGameCollectionViewModel>();

        public ObservableCollection<IArenaGameCollectionViewModel> Owners
        {
            get { return _owners; }
        }

    
        
      
        public ICommand GoToOwnerCommnad { get; private set; }

        private void GoToOwner(IArenaGameCollectionViewModel owner)
        {
            owner.NavigateTo(this);
        }

        int ILobbyItemSource.Id
        {
            get
            {
                return this.GameType;
            }
        }

        string ILobbyItemSource.Name
        {
            get
            {
                return this.Name;
            }
        }

        bool ILobbyItemSource.ShouldShowTheJackpot(PlayerStatusTypeViewModel playerStatus)
        {
            return _jackpotIds.Any();
        }

        public string NameWithGameType
        {
            get
            {
                return $"{Name} [{GameType}]";
            }
        }

        public string GameGroup { get; private set; }

        protected override bool CanDropItem(object droppedItem, DropContext context)
        {
            if (base.CanDropItem(droppedItem, context))
                return true;

            return droppedItem is IConvertible<AvailableGameViewModel>;
        }

        protected override object DropItem(object droppedItem, DropContext context)
        {
            var convertible = droppedItem as IConvertible<AvailableGameViewModel>;
            if (convertible != null)
            {
                droppedItem = convertible.Convert();
            }

            return base.DropItem(droppedItem, context);
        }

       
        public override bool Equals(object obj)
        {
            var theOther = obj as AvailableGameViewModel;
            if (theOther == null)
                return false;

            return this.GameType == theOther.GameType;
        }

        public override int GetHashCode()
        {
            return this.GameType.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name} [{GameType}]";
        }

        public ArenaGame BuildArenaGame()
        {
            return new ArenaGame(GameType, Name, _isNewGame, IsVipGame ? UserModes.VIP : UserModes.Both);
        }
    }
}
