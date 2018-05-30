using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using GGPGameServer.ApprovalSystem.Common;
using GGPMockBootstrapper.GamesPortalService;

namespace GGPMockBootstrapper.ViewModels
{
    public class FlashGamesWorkAreaItem : GamesWorkAreaItem
    {
        public FlashGamesWorkAreaItem(IWorkArea workArea)
            : base(workArea)
        {
            this.ClientProduct.PropertyChanged += ClientProduct_PropertyChanged;
        }

        void ClientProduct_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.ClientProduct.GetPropertyName(t => t.SupportedFlashGameTypes))
            {
                UIServices.InvokeOnMainThread(ReloadGames);
                
                
            }
        }

        protected override GameTechnology GetGameTechnology()
        {
            return GameTechnology.Flash;
        }

        protected override void CreateGameFromArtifactory(int gameType, string gameZipFile, string wrapperChillZipFile)
        {
            this.ClientProduct.CreateFlashGameFromArtifactoryZip(gameType, gameZipFile, this.WorkArea.EnvironmentServices);
        }

        protected override ArtifactoryGameSelector CreateArtifactoryGameSelector(Models.GameInfoModel[] allGames,  Artifactory.ArtifactoryStorage artifactoryStorage, GamesPortalService.GameTechnology gameTechnology)
        {
            var selector = base.CreateArtifactoryGameSelector(allGames, artifactoryStorage, gameTechnology);
            selector.WrapperChillSelectionVisible = Visibility.Collapsed;
            return selector;
        }

        protected override Artifactory.ArtifactoryStorage GetArtifactoryStorage()
        {
            return new Artifactory.ArtifactoryStorage("modernGame-local");
        }

        protected override Models.InstalledGame[] GetSupportedGameTypes()
        {
            return this.ClientProduct.SupportedFlashGameTypes;
        }

        protected override string BuildGameUrl(OpenGameParametersViewModel game)
        {
            return ClientProduct.GetFlashGameURL(game.Parameters);
        }

        protected override Models.ISwfFilesProvider GetSwfFilesProvider()
        {
            return ClientProduct;
        }


        public override bool AllowMyGamesTab
        {
            get
            {
                return false;
            }
        }

        public override void RemoveGame(GameTypeViewModel gameType)
        {
            throw new NotSupportedException("Flash games removal is not supported!");
        }
    }
}
