using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GGPGameServer.ApprovalSystem.Common;
using GGPMockBootstrapper.GGPMockDataProvider;
using GGPMockBootstrapper.Models.Client;

namespace GGPMockBootstrapper.ViewModels
{
    public class OpenSessionsWorkAreaItem : WorkAreaItemBase
    {
        public OpenSessionsWorkAreaItem(IWorkArea workArea)
            : base(workArea)
        {

            this.SaveOpenSessionsToFileAction = new ActionViewModel("Save open sessions", new Command(SaveOpenSessionsToFile), ResourcesProvider.CreateBitmapImageSource("Save24x24.png"));
            this.LoadOpenSessionsFromFileAction = new ActionViewModel("Load open sessions", new Command(LoadOpenSessionsFromFile), ResourcesProvider.CreateBitmapImageSource("OpenFile24x24.png"));
            this.RemoveAllSessionsAction = new ActionViewModel("Remove all sessions", new Command(RemoveAllSessions), ResourcesProvider.CreateBitmapImageSource("Excluded.png"));
            

            this.OpenSessions = new ObservableCollection<OpenSessionViewModel>();
            RefreshOpenSessions();
            Models.GGPMockDataManager.Singleton.PropertyChanged += Singleton_PropertyChanged;
        }


        public ActionViewModel SaveOpenSessionsToFileAction { get; private set; }
        public ActionViewModel LoadOpenSessionsFromFileAction { get; private set; }
        public ActionViewModel RemoveAllSessionsAction { get; private set; }

        private void RemoveAllSessions()
        {
            if (MessageBoxResponse.Yes != UIServices.ShowMessage("Are you sure?", "Confirmation", MessageBoxButtonType.YesNo))
                return;


            foreach (var s in this.OpenSessions.ToArray())
            {
                s.RemoveSession();
            }
        }

        private void SaveOpenSessionsToFile()
        {
            Models.GGPMockDataManager.Singleton.SaveOpenSessionsToFile();
            UIServices.ShowMessage("Sessions successfully saved!");
        }

        private void LoadOpenSessionsFromFile()
        {
            if (!Models.GGPMockDataManager.Singleton.CanLoadSavedSessions)
            {
                UIServices.ShowMessage("There are no saved open sessions to load from!");
                return;
            }


            Models.GGPMockDataManager.Singleton.LoadOpenSessionsFromFile();
        }

        void Singleton_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Models.GGPMockDataManager.Singleton.GetPropertyName(t => t.OpenSessions))
            {
                this.UIServices.InvokeOnMainThread(() =>
                    {
                        RefreshOpenSessions();
                    });
            }
        }

        private void RefreshOpenSessions()
        {
            this.OpenSessions.Clear();
            foreach (var session in Models.GGPMockDataManager.Singleton.OpenSessions.OrderBy(s => s.GameType))
            {
                this.OpenSessions.Add(new OpenSessionViewModel(session, this.WorkArea));
            }
        }

        public ObservableCollection<OpenSessionViewModel> OpenSessions { get; private set; }


    }

    public class OpenSessionViewModel : ViewModelBase
    {
        public OpenSessionViewModel(OpenSessionProviderService.GetOpenSessionResponse openSession, IWorkArea workArea)
        {
            WorkArea = workArea;
            
            this.RemoveSessionAction = new ActionViewModel("Remove", new Command(RemoveSession), ResourcesProvider.CreateBitmapImageSource("Excluded.png"));
            LoadContextActions();
            this.OpenSession = openSession;
        }

        private void LoadContextActions()
        {
            this.Actions = new List<ActionViewModel>();

            this.Actions.Add(new ActionViewModel("Play with Flash", new Command(PlayWithFlash), ResourcesProvider.CreateBitmapImageSource("Flash24x24.png")));
            this.Actions.Add(new ActionViewModel("Play with HTML5", new Command(PlayWithHtml5), ResourcesProvider.CreateBitmapImageSource("HTML5.png")));

        }

        IWorkArea WorkArea;

        OpenSessionProviderService.GetOpenSessionResponse OpenSession { get; set; }


        public long PlayerSessionID
        {
            get { return this.OpenSession.PlayerSessionID; }
        }

        public string GameState
        {
            get { return this.OpenSession.GameState; }
        }

        public DateTime TimeStamp
        {
            get { return this.OpenSession.TimeStamp; }
        }

        public int GameType
        {
            get { return this.OpenSession.GameType; }
        }

        public string GameString
        {
            get { return this.OpenSession.GameString; }
        }

        public int SubGameType
        {
            get { return this.OpenSession.SubGameType; }
        }

        public string SubGameString
        {
            get { return this.OpenSession.SubGameString; }
        }
        

        public string GameCurrencyCode
        {
            get { return this.OpenSession.GameCurrencyCode; }
        }

        
        public ActionViewModel RemoveSessionAction { get; private set; }

        public void RemoveSession()
        {
            Models.GGPMockDataManager.Singleton.RemoveSession(this.PlayerSessionID);
        }

        public List<ActionViewModel> Actions { get; private set; }

        private void PreparePlayerData()
        {
            Models.GGPMockDataManager.Singleton.MockData.RealMoneyBalance = OpenSession.BankrollBalance;
            Models.GGPMockDataManager.Singleton.MockData.BankrollCurrency = OpenSession.GameCurrencyCode;
            Models.GGPMockDataManager.Singleton.MockData.Regulation = Models.GGPMockDataManager.Singleton.SupportedRegulations.FirstOrDefault(item => item.Id == OpenSession.RegulationId);
            if (OpenSession.FreePlayMockData != null)
            {
                var newFreePlayData = new FreePlayMockData
                {
                    AlternativeGames = OpenSession.FreePlayMockData.AlternativeGames,
                    Balance = OpenSession.FreePlayMockData.Balance,
                    ExpirationDate = OpenSession.FreePlayMockData.ExpirationDate,
                    FreePlayID = OpenSession.FreePlayMockData.FreePlayID,
                    FreePlayState = new FreePlayStateMock
                    {
                        Id = OpenSession.FreePlayMockData.FreePlayState.Id,
                        Name = OpenSession.FreePlayMockData.FreePlayState.Name
                    },
                    FreePlayType = OpenSession.FreePlayMockData.FreePlayType,
                    GamesTypes = OpenSession.FreePlayMockData.GamesTypes,
                    MaxBetAmount = OpenSession.FreePlayMockData.MaxBetAmount,
                    MaxWinningCap = OpenSession.FreePlayMockData.MaxWinningCap,
                    TotalWinnings = OpenSession.FreePlayMockData.TotalWinnings
                };

                var allFreePlays = Models.GGPMockDataManager.Singleton.MockData.FreePlay.ToList();
                var existingFreePlay =
                    allFreePlays.FirstOrDefault(item => item.FreePlayID == OpenSession.FreePlayMockData.FreePlayID);
                if (existingFreePlay == null)
                {
                    allFreePlays.Add(newFreePlayData);
                }
                else
                {
                    allFreePlays[allFreePlays.IndexOf(existingFreePlay)] = newFreePlayData;
                }

                Models.GGPMockDataManager.Singleton.MockData.FreePlay = allFreePlays.ToArray();
            }

            Models.GGPMockDataManager.Singleton.SendMockData();
        }

        private void PlayWithFlash()
        {
            PlayGame(new FlashGameLuncher(this.ClientProduct), this.ClientProduct.SupportedFlashGameTypes, "Flash");
        }


        private void PlayWithHtml5()
        {

            PlayGame(new Html5GameLuncher(this.ClientProduct), this.ClientProduct.SupportedHtml5GameTypes, "HTML5");
        }

        private void PlayGame(IGameLuncher launcher, Models.InstalledGame[] supportedGame, string gameTechnologyName)
        {
            var installedGame = supportedGame.FirstOrDefault(t => t.GameType == this.GameType);

            if (installedGame == null)
            {
                UIServices.ShowMessage(string.Format("This {0} game is not installed!", gameTechnologyName));
                return;
            }

            var gameViewModel = new GameTypeViewModel(this.GameType, WorkArea.GamesInformationProvider, launcher, installedGame.PhysicalPath, this.ClientProduct, installedGame.IsCustomGame, null);

            gameViewModel.OpenParameters.Parameters.Currency = this.GameCurrencyCode;
            gameViewModel.OpenParameters.Parameters.IsFreePlay = OpenSession.FreePlayMockData != null;
            gameViewModel.OpenParameters.OKPressed = false;


            UIServices.GetCustomUIService<IDialogShowService>().ShowOkCancelDialog(gameViewModel.OpenParameters);

            if (gameViewModel.OpenParameters.OKPressed)
            {
                PreparePlayerData();
                launcher.OpenGame(gameViewModel.OpenParameters);
            }
        }

        protected Models.Client.ClientProduct ClientProduct
        {
            get
            {
                return this.WorkArea.GetProduct<Models.Client.ClientProduct>();
            }
        }

        private class Html5GameLuncher : IGameLuncher
        {
            public Html5GameLuncher(ClientProduct clientProduct)
            {
                _clientProduct = clientProduct;
            }

            ClientProduct _clientProduct;

            #region IGameLuncher Members

            public void OpenGame(OpenGameParametersViewModel game)
            {
                if (game.SelectedBrowser == null)
                    Models.InstalledBrowsersProvider.OpeneWithDefaultBrowser(_clientProduct.GetHtml5GameUrl(game.Parameters));
                else
                    game.SelectedBrowser.OpenUrl(_clientProduct.GetHtml5GameUrl(game.Parameters));
            }

            public GGPMockDataProvider.LanguageMock[] GetLanguages()
            {
                return Models.GGPMockDataManager.Singleton.Languages;
            }

            public void OpenSimulator(int gameType)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        private class FlashGameLuncher : IGameLuncher
        {
            public FlashGameLuncher(ClientProduct clientProduct)
            {
                _clientProduct = clientProduct;
            }

            ClientProduct _clientProduct;

            #region IGameLuncher Members

            public void OpenGame(OpenGameParametersViewModel game)
            {
                if (game.SelectedBrowser == null)
                    Models.InstalledBrowsersProvider.OpeneWithDefaultBrowser(_clientProduct.GetFlashGameURL(game.Parameters));
                else
                    game.SelectedBrowser.OpenUrl(_clientProduct.GetFlashGameURL(game.Parameters));


            }

            public GGPMockDataProvider.LanguageMock[] GetLanguages()
            {
                return Models.GGPMockDataManager.Singleton.Languages;
            }

            public void OpenSimulator(int gameType)
            {
                
            }

            #endregion
        }

    }
}
