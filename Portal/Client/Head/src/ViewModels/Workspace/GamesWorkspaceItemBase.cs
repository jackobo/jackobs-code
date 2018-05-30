using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.PubSubEvents;
using GamesPortal.Client.Interfaces.Services;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;

namespace GamesPortal.Client.ViewModels.Workspace
{
    public abstract class GamesWorkspaceItemBase : WorkspaceItem
    {
        public GamesWorkspaceItemBase(IEnumerable<Game> games, IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            this.Games = new ObservableCollection<GameGridItem>();

            foreach (var g in games)
                this.Games.Add(new GameGridItem(g));

            SubscribeToEvent<GameSynchronizationFinishedEventData>(GameSynchronizationFinishedHandler, eventData => eventData.IsExternal == IsExternal);
            
        }

        protected abstract bool IsExternal { get; }
       
        public ObservableCollection<GameGridItem> Games
        {
            get;
            private set;
        }


        private void GameSynchronizationFinishedHandler(GameSynchronizationFinishedEventData eventData)
        {
            if (eventData.IsExternal != this.IsExternal)
            {
                return;
            }

            switch (eventData.ChangeType)
            {
                case Interfaces.ChangeType.Added:
                    AddGame(eventData.NewGame);
                    break;
                case Interfaces.ChangeType.Removed:
                    RemoveGame(eventData.GameId);
                    break;
            }

        }

        private void RemoveGame(Guid gameId)
        {
            var exitingGame = this.Games.FirstOrDefault(g => g.GetGameId() == gameId);
            if (exitingGame != null)
                this.Games.Remove(exitingGame);
        }

        private void AddGame(Game newGame)
        {
            var newGridItem = new GameGridItem(newGame);

            var beforeItem = this.Games.FirstOrDefault(item => string.Compare(item.Name, newGridItem.Name) > 0);

            if(beforeItem == null)
                this.Games.Add(newGridItem);
            else
                this.Games.Insert(this.Games.IndexOf(beforeItem), newGridItem);
        }

        private IGamesRepository GamesRepository
        {
            get { return this.ServiceLocator.GetInstance<IGamesRepository>(); }
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                foreach (var g in this.Games)
                    g.Dispose();
            }
        }

        
    }
}
