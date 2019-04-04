using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamesPortal.Client.Interfaces.Entities;
using Spark.Wpf.Common.ViewModels;
using Spark.Infra.Types;

namespace GamesPortal.Client.ViewModels
{
    public class GameGridItem : ViewModelBase
    {
        public GameGridItem(Game game)
        {
            this.Game = game;
            this.Game.PropertyChanged += Game_PropertyChanged;
        }

        void Game_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == this.Game.GetPropertyName(g => g.Name))
                OnPropertyChanged(() => Name);
            else if(e.PropertyName == this.Game.GetPropertyName(g => g.GameTypes))
                OnPropertyChanged(() => GameTypes);
            else if (e.PropertyName == this.Game.GetPropertyName(g => g.SupportedInfrastructures))
            {
                OnPropertyChanged(() => Flash);
                OnPropertyChanged(() => Html5);
            }
        }

        

        private Game Game { get; set; }

        public Guid GetGameId()
        {
            return this.Game.Id; 
        }

        public string Name
        {
            get { return this.Game.Name; }
        }

        public string GameTypes
        {
            get
            {
                return string.Join(", ", this.Game.GameTypes.OrderByDescending(gt => gt.Id).Select(gt => gt.Id.ToString()));
            }
        }


        public string Flash
        {
            get
            {
                return GetSupportedInfraDescription(GameTechnology.Flash);
            }
        }

        public string Html5
        {
            get
            {
                return GetSupportedInfraDescription(GameTechnology.Html5);
            }
        }

        private string GetSupportedInfraDescription(GameTechnology technology)
        {
            var platformTypes = this.Game.SupportedInfrastructures.Where(i => i.GameTechnology == technology).Select(i => i.PlatformType).ToArray();
            if (platformTypes.Length == 0)
                return string.Empty;
                        
            return string.Join(", ", platformTypes.Select(pt => pt.ToDescription()));

        }

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
