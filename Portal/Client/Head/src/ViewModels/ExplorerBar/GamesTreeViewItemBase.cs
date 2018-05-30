using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.PubSubEvents;
using GamesPortal.Client.Interfaces.Services;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;

namespace GamesPortal.Client.ViewModels.ExplorerBar
{
    public class GamesTreeViewItemBase : TreeViewItem
    {
        public GamesTreeViewItemBase(IEnumerable<Game> games, IServiceLocator serviceLocator, bool containsExternalGames, params GamingComponentCategory[] supportedComponentCategories)
            : base(serviceLocator)
        {
            this.ContainsExternalGames = containsExternalGames;
            this.SupportedComponentCategories = supportedComponentCategories;
            foreach (var game in games.OrderBy(g => g.Name))
                this.Items.Add(CreateGameTreeViewItem(game));

            SubscribeToEvent<GameSynchronizationFinishedEventData>(GameSynchronizationFinishedHandler);
        }

        protected virtual GameTreeViewItem CreateGameTreeViewItem(Game game)
        {
            return new GameTreeViewItem(game, this, this.ServiceLocator);
        }

        bool ContainsExternalGames { get; set;}

        GamingComponentCategory[] SupportedComponentCategories { get; set; }

        protected IEnumerable<Game> Games
        {
            get
            {
                return this.Items.Cast<GameTreeViewItem>().Select(item => item.Game).ToList();
            }
        }

        IEnumerable<GameTreeViewItem> GameTreeViewItems
        {
            get { return this.Items.Cast<GameTreeViewItem>(); }
        }

             

        private void GameSynchronizationFinishedHandler(GameSynchronizationFinishedEventData eventData)
        {
            if (ContainsExternalGames != eventData.IsExternal)
                return;

            

            if (eventData.ChangeType == Interfaces.ChangeType.Added
                && this.SupportedComponentCategories.Contains(eventData.NewGame.Category))
            {
                AddNewGame(eventData.NewGame);
            }
            else if(eventData.ChangeType == Interfaces.ChangeType.Removed)
            {
                RemoveGameById(eventData.GameId);
            }
        }

        public void RemoveGameById(Guid gameId)
        {
            var foundItem = GameTreeViewItems.FirstOrDefault(item => item.Game.Id == gameId);

            if (foundItem == null)
                return;

            RemoveItem(foundItem);
        }

       

        public void AddNewGameById(Guid gameId)
        {
            AddNewGame(GamesRepository.GetGame(gameId));
        }

        private void AddNewGame(Game newGame)
        {
            var firstGameAfterTheNewOneInTheSortOrder = GameTreeViewItems.FirstOrDefault(item => string.Compare(item.Game.Name, newGame.Name) > 0);
            
            var newTreeViewItem = CreateGameTreeViewItem(newGame);

            if (firstGameAfterTheNewOneInTheSortOrder == null)
                this.Items.Add(newTreeViewItem);
            else
            {
                this.Items.Insert(this.Items.IndexOf(firstGameAfterTheNewOneInTheSortOrder), newTreeViewItem);
            }
        }

        IGamesRepository GamesRepository
        {
            get { return this.ServiceLocator.GetInstance<IGamesRepository>(); }

        }        

    }
}
