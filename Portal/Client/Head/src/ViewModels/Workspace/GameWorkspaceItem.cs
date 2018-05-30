using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GamesPortal.Client.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using System.Windows.Input;
using GamesPortal.Client.Interfaces.Services;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.Workspace
{
    public class GameWorkspaceItem : GameRelatedWorkspaceItem
    {
        public GameWorkspaceItem(Game game, IServiceLocator serviceLocator)
            : base(game, serviceLocator)
        {
            ReloadGameTypes();
            this.ForceGameSynchronizationCommand = new Command(ForceGameSynchronization);
                
        }

        public ICommand ForceGameSynchronizationCommand { get; private set; }

        private void ForceGameSynchronization()
        {
            this.ServiceLocator.GetInstance<IGamesRepositorySynchronizer>().ForceGameSynchronization(this.Game);
            this.ServiceLocator.GetInstance<IMessageBox>().ShowMessage("Game synchronization started on server!");
        }



        private void ReloadGameTypes()
        {
            var gameTypes = new ObservableCollection<GameTypeListItem>();

            foreach (var gt in this.Game.GameTypes.OrderBy(g => g.OperatorName).ThenBy(g => g.Id))
            {
                gameTypes.Add(new GameTypeListItem(gt, gt.Id == this.Game.MainGameType));
            }

            this.GameTypes = gameTypes;

        }


        protected override void OnGamePropertyChanged(string propertyName)
        {
            base.OnGamePropertyChanged(propertyName);

            if (propertyName == this.Game.GetPropertyName(g => g.GameTypes))
            {
                ReloadGameTypes();
            }
        }


        public override string Title
        {
            get { return this.Game.Name; }
        }

        ObservableCollection<GameTypeListItem> _gameTypes;

        public ObservableCollection<GameTypeListItem> GameTypes
        {
            get { return _gameTypes; }
            set
            {
                SetProperty(ref _gameTypes, value);
            }
        }
        
        
    }


    public class GameTypeListItem
    {
        public GameTypeListItem(GameType gameType, bool isMainGameType)
        {
            _gameType = gameType;
            _isMainGameType = isMainGameType;
        }

        GameType _gameType;
        bool _isMainGameType;

        
        public int GameTypeId
        {
            get { return _gameType.Id; }
        }
        
        public string Name
        {
            get { return this._gameType.Name; }
        }

        public string Operator
        {
            get
            {
                return this._gameType.OperatorName;
            }
        }

        public string IsMainGameType
        {
            get
            {
                if (_isMainGameType)
                    return "Yes";
                else
                    return "No";
            }
        }

    }

}
