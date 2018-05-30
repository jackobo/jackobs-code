using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGPGameServer.ApprovalSystem.Common;
using GGPMockBootstrapper.Models;

namespace GGPMockBootstrapper.ViewModels
{
    public class OpenGameParametersViewModel : OkCancelDialogViewModel
    {
        public OpenGameParametersViewModel(GameTypeViewModel game, GGPMockDataProvider.LanguageMock[] languages)
        {
            this.Title = "Open game";
            this.Currencies = new string[] { "USD", "EUR", "GBP", "CAD", "AUD", "DKK", "NZD", "BRL", "SEK", "BPT", "RON" };
            this.Game = game;
            this.Languages = languages.OrderBy(l => l.Name).ToArray();
            
            this.Parameters = new OpenGameParameters(game.Id, game.Name, game.PhysicalPath);
            this.Parameters.Language = languages.FirstOrDefault(l => l.Iso3 == "eng");
            this.Parameters.Currency = this.Currencies.FirstOrDefault();
            this.Parameters.Brand = this.GetCurrentBrands().FirstOrDefault();

            Models.GGPMockDataManager.Singleton.PropertyChanged += Singleton_PropertyChanged;
        }

        private void Singleton_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(GGPMockDataManager.MockData))
            {
                this.Parameters.Brand = this.GetCurrentBrands().FirstOrDefault();
            }
        }

        static OpenGameParametersViewModel()
        {
            LoadBrowsers();
        }

        private static void LoadBrowsers()
        {
            List<BrowserInfo> browsers = new List<BrowserInfo>();

            browsers.Add(BrowserInfo.DefaultBrowser);
            browsers.AddRange(InstalledBrowsersProvider.GetBrowsers());

            _installedBrowsers = browsers.ToArray();
        }


        public GameTypeViewModel Game { get; private set; }


        public string[] Currencies { get; private set; }

        public BrandInfo[] Brands
        {
            get
            {
                return this.GetCurrentBrands();
            }
        }

        private BrandInfo[] GetCurrentBrands()
        {
            return BrandInfo.GetBrandsForRegulation(GGPMockDataManager.Singleton.MockData.Regulation.Id)
                    .OrderBy(b => b.BrandId)
                    .ToArray();
        }

        public string CurrentRegulation
        {
            get
            {
                if (Models.GGPMockDataManager.Singleton.MockData == null || Models.GGPMockDataManager.Singleton.MockData.Regulation == null)
                    return "?????";

                return Models.GGPMockDataManager.Singleton.MockData.Regulation.Name;
            }
        }

        GGPMockDataProvider.LanguageMock[] _languages;
        public GGPMockDataProvider.LanguageMock[] Languages
        {
            get { return _languages; }
            set
            {
                _languages = value;
                OnPropertyChanged(this.GetPropertyName(t => t.Languages));
            }
        }



        public OpenGameParameters Parameters { get; private set; }

       
        protected override bool OK()
        {

            if (string.IsNullOrEmpty(this.Parameters.Currency))
            {
                UIServices.ShowMessage("You must choose a currency!");
                return false;
            }

            return base.OK();
        }

        JoinTypeEnum[] _joinTypes = new JoinTypeEnum[] { JoinTypeEnum.Regular, JoinTypeEnum.Anonymous };

        public JoinTypeEnum[] JoinTypes
        {
            get { return _joinTypes; }
        }


        private static BrowserInfo[] _installedBrowsers;
        public BrowserInfo[] InstalledBrowsers
        {
            get { return _installedBrowsers; }
        }


        private static Models.BrowserInfo _selectedbrowser = BrowserInfo.DefaultBrowser;

        public Models.BrowserInfo SelectedBrowser
        {
            get { return _selectedbrowser; }
            set
            {
                _selectedbrowser = value;
                OnPropertyChanged(this.GetPropertyName(t => t.SelectedBrowser));
            }
        }
    }

}
