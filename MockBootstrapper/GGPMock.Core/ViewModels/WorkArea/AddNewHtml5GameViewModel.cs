using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGPMockBootstrapper.Models.Client;

namespace GGPMockBootstrapper.ViewModels
{
    public class AddNewHtml5GameViewModel : OkCancelDialogViewModel
    {
        public AddNewHtml5GameViewModel(ClientProduct clientProduct, Models.IGamesInformationProvider gamesInformationProvider)
        {
            this.Title = "Add new HTML5 game";
            this.ClientProduct = clientProduct;
            this.GamesInformationProvider = gamesInformationProvider;
            
        }

        ClientProduct ClientProduct { get; set; }

        Models.IGamesInformationProvider GamesInformationProvider { get; set; }

        public int? GameType { get; set; }
        public string BaseUrl { get; set; }

        protected override bool OK()
        {
            if (!this.GameType.HasValue)
            {
                UIServices.ShowMessage("Please fill in the Game Type");
                return false;
            }

            if (string.IsNullOrEmpty(this.BaseUrl))
            {
                UIServices.ShowMessage("Please fill in the Base URL");
                return false;
            }


            if (!IsValidBaseUrl())
            {
                UIServices.ShowMessage("You must enter a valid absolut URL");
                return false;
            }

            var gameInfo = this.GamesInformationProvider.GetGameInfoOrNull(this.GameType.Value);

            if (gameInfo == null)
            {
                UIServices.ShowMessage(string.Format("Game type {0} is not supported by the Game Server!", this.GameType.Value));
                return false;
            }


            this.ClientProduct.AddCustomHtml5Game(new CustomHtml5Game(this.GameType.Value, this.BaseUrl));
            return true;
        }

        private bool IsValidBaseUrl()
        {
            Uri baseUri;
            if (!Uri.TryCreate(this.BaseUrl, UriKind.Absolute, out baseUri))
                return false;

            return baseUri.Scheme == Uri.UriSchemeHttp || baseUri.Scheme == Uri.UriSchemeHttps;

        }
    }
}
