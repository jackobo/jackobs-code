using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;

namespace GamesPortal.Client.ViewModels.Workspace
{
    public abstract class GameRelatedWorkspaceItem : WorkspaceItem
    {
        public GameRelatedWorkspaceItem(Game game, IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            this.Game = game;
            this.Game.PropertyChanged += Game_PropertyChanged;
        }


        void Game_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnGamePropertyChanged(e.PropertyName);
        }


        protected virtual void OnGamePropertyChanged(string propertyName)
        {
            OnPropertyChanged(() => Title);
        }

        protected Game Game { get; private set; }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.Game.PropertyChanged -= Game_PropertyChanged;
            }
        }
    }
}
