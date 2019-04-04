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
    public class GameGroupLayoutViewModel : ViewModelBase, IDropable, ISupportDynamicLayout<GameGroupLayoutViewModel>
    {
        public GameGroupLayoutViewModel(PlayerStatusTypeViewModel playerStatus)
        {
            this.PlayerStatus = playerStatus;
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
        }

        private GamesGroupCollection _games = new GamesGroupCollection();
        public GamesGroupCollection Games
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

        bool IDropable.CanDrop(IDragableSource source, DropContext context)
        {
            return Games.CanDrop(source, context);
        }

        DropResult IDropable.Drop(IDragableSource source, DropContext context)
        {
            return Games.Drop(source, context);
        }

        public GameGroupLayoutViewModel Clone(PlayerStatusTypeViewModel newPlayerStatus)
        {
            var clone = new GameGroupLayoutViewModel(newPlayerStatus);

            foreach(var game in this.Games)
            {
                clone.Games.Add(game);
            }

            return clone;
        }
    }
}
