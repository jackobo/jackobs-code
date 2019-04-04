using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GamesPortal.Client.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;

namespace GamesPortal.Client.ViewModels.ExplorerBar
{
    public class GameTreeViewItem : TreeViewItem
    {
        public GameTreeViewItem(Game game, TreeViewItem parent, IServiceLocator serviceLocator)
            : base(parent, serviceLocator)
        {
            this.Game = game;
            
            UpdateCaption();

            CreateSupportedTechnologies();

            SubscribeToGamePropertyChanged();
        }

        private void SubscribeToGamePropertyChanged()
        {
            this.Game.PropertyChanged += Game_PropertyChanged;
        }

        protected virtual void CreateSupportedTechnologies()
        {
            foreach (var t in this.Game.SupportedInfrastructures.OrderBy(t => t))
            {
                this.Items.Add(new GameTechnologyTreeViewItem(this.Game, t, this, this.ServiceLocator));
            }
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                UnsubscribeFromGamePropertyChanged();
            }
        }

        private void UnsubscribeFromGamePropertyChanged()
        {
            this.Game.PropertyChanged -= Game_PropertyChanged;
        }

        protected virtual void UpdateCaption()
        {
            this.Caption = string.Format("{0} [{1}]", this.Game.Name, this.Game.MainGameType);
        }

        void Game_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.Game.GetPropertyName(t => t.Name) || e.PropertyName == this.Game.GetPropertyName(t => t.MainGameType))
            {
                UpdateCaption();
            }
            else if (e.PropertyName == this.Game.GetPropertyName(g => g.SupportedInfrastructures))
            {
                RefreshTechnologies();
            }
        }

        protected virtual void RefreshTechnologies()
        {
            var currentTechnologies = this.Items.Cast<GameTechnologyTreeViewItem>().ToArray();
            foreach (var newTechnolgoy in this.Game.SupportedInfrastructures.Where(t => !currentTechnologies.Select(item => item.Infrastructure).Contains(t)))
            {
                this.Items.Add(new GameTechnologyTreeViewItem(this.Game, newTechnolgoy, this, this.ServiceLocator));
            }
            
            foreach (var removedTechnology in currentTechnologies.Where(item => !this.Game.SupportedInfrastructures.Contains(item.Infrastructure)))
            {
                RemoveItem(removedTechnology);                
            }

        }

        public Game Game { get; private set; }


        public override IWorkspaceItem CreateWorkspaceItem(IServiceLocator serviceLocator)
        {
            return new Workspace.GameWorkspaceItem(this.Game, serviceLocator);
        }

        public override IEnumerable<TreeViewItem.FoundItem> Search(string[] words)
        {
            var posibleGameTypes = new List<int>();
            var nonGameTypesWords = new List<string>();

            foreach (var word in words)
            {
                int gameType;
                if (int.TryParse(word, out gameType))
                {
                    posibleGameTypes.Add(gameType);
                }
                else
                {
                    nonGameTypesWords.Add(word);
                }
            }

            bool isMatch =IsMatch(this.Caption, nonGameTypesWords.ToArray())  
                            || (this.Game.GameTypes.Select(gt => gt.Id)).Intersect(posibleGameTypes).Any();

            var result = new List<TreeViewItem.FoundItem>();

            if (!isMatch)
                return result;

            foreach (GameTechnologyTreeViewItem gameTechnologyItem in this.Items.Where(item => item is GameTechnologyTreeViewItem))
            {
                result.Add(new TreeViewItem.FoundItem(this.Caption + " - " + gameTechnologyItem.Caption, gameTechnologyItem) { GameInfrastructure = gameTechnologyItem.Infrastructure });
            }

            return result;

        }


        
    }

    
    
}
