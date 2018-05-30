using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GGPGameServer.ApprovalSystem.Common;
using GGPMockBootstrapper.GamesPortalService;

namespace GGPMockBootstrapper.ViewModels
{
    public class Html5GamesWorkAreaItem : GamesWorkAreaItem
    {
        public Html5GamesWorkAreaItem(IWorkArea workArea)
            : base(workArea)
        {
            this.AddNewGameAction = new ActionViewModel("Add new game", new Command(AddNewGame));
            
            this.ClientProduct.PropertyChanged += ClientProduct_PropertyChanged;
        }


        protected override void CreateGameFromArtifactory(int gameType, string gameZipFile, string wrapperChillZipFile)
        {
            this.ClientProduct.CreateHtml5GameFromArtifactoryZip(gameType, gameZipFile, wrapperChillZipFile, this.WorkArea.EnvironmentServices, this.GGPProduct.GetGGPHttpHandlerRelativeUrl(), this.GGPProduct.GetGGPHistoryHandlerRelativeUrl());
        }

        protected override Artifactory.ArtifactoryStorage GetArtifactoryStorage()
        {
            return new Artifactory.ArtifactoryStorage("HTML5Game-local");
        }

        protected override GameTechnology GetGameTechnology()
        {
            return GameTechnology.Html5;
        }

        private void ClientProduct_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.ClientProduct.GetPropertyName(t => t.SupportedHtml5GameTypes))
            {
                UIServices.InvokeOnMainThread(ReloadGames);
            }
        }


        public override void RemoveGame(GameTypeViewModel gameType)
        {
            this.ClientProduct.RemoveCustomHtml5Game(gameType.Id);
        }
        

        protected override Models.InstalledGame[] GetSupportedGameTypes()
        {
            return this.ClientProduct.SupportedHtml5GameTypes;
        }

        public string GGPHttpHandlerRelativeUrl
        {
            get
            {
                return this.GGPProduct.GetGGPHttpHandlerRelativeUrl();
            }
        }

        protected override string BuildGameUrl(OpenGameParametersViewModel game)
        {
            var url = ClientProduct.GetHtml5GameUrl(game.Parameters);

            try
            {
                ClientProduct.GenerateMobileDeviceHelperPage(game.Game.Name, url);
            }
            catch (Exception ex)
            {
                UIServices.ShowMessage("Unable the generate GGP Mock - Mobile device lunch page" + Environment.NewLine + ex.ToString());
            }

            return url;
        }


        public ActionViewModel AddNewGameAction { get; private set; }

        

      

        private void AddNewGame()
        {
            AddNewHtml5GameViewModel dialogViewModel = new AddNewHtml5GameViewModel(this.ClientProduct, WorkArea.GamesInformationProvider);
            UIServices.GetCustomUIService<IDialogShowService>().ShowOkCancelDialog(dialogViewModel);
        }
        
        protected override Models.ISwfFilesProvider GetSwfFilesProvider()
        {
            return null;
        }


        public override bool AllowMyGamesTab
        {
            get
            {
                return true;
            }
        }
    }
}
