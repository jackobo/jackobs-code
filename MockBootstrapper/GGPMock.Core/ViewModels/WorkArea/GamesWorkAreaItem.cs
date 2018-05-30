using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GGPGameServer.ApprovalSystem.Common;
using GGPMockBootstrapper.GamesPortalService;

namespace GGPMockBootstrapper.ViewModels
{

    public abstract class GamesWorkAreaItem : WorkAreaItemBase, IGameLuncher, IGameEditor
    {
        public GamesWorkAreaItem(IWorkArea workArea)
            : base(workArea)
        {
            this.Games = new ObservableCollection<GameTypeViewModel>();
            ReloadGames();

            OpenCurrentGameSimulatorUrlInBrowserAction = new ActionViewModel("Open URL in browser", new Command(OpenCurrentGameSimulatorUrlInBrowser));
            this.AddGameFromArtifactoryAction = new ActionViewModel("Add game from Artifactory", new Command(AddGameFromArtifactory));
        }


        public ObservableCollection<GameTypeViewModel> Games { get; private set; }


        public System.Windows.Visibility AddGameFromArtifactoryVisible
        {
            get
            {
                if (GGPMockBootstrapper.Models.GlobalSettings.IsInternalDeployment)
                    return System.Windows.Visibility.Visible;
                else
                    return System.Windows.Visibility.Collapsed;
            }
        }


        protected void ReloadGames()
        {
            this.Games.Clear();

            Models.InstalledGame[] gameTypes = GetSupportedGameTypes();

            foreach (var game in GameTypeViewModel.CreateArray(gameTypes, this.WorkArea.GamesInformationProvider, this, GetSwfFilesProvider(), this))
            {
                Games.Add(game);
            }

        }

        private void AddGameFromArtifactory()
        {
            var gameSelector = CreateArtifactoryGameSelector(this.WorkArea.GamesInformationProvider.GetAllGames(), GetArtifactoryStorage(), GetGameTechnology());
            this.UIServices.GetCustomUIService<IDialogShowService>().ShowOkCancelDialog(gameSelector);

            if (!gameSelector.OKPressed)
                return;


            this.DownloadHandler = new ArtifactoryDownloadHandler(this, gameSelector);
            
            this.DownloadHandler.Start();
        }

        protected virtual ArtifactoryGameSelector CreateArtifactoryGameSelector(Models.GameInfoModel[] allGames, Artifactory.ArtifactoryStorage artifactoryStorage, GameTechnology gameTechnology)
        {
            return new ArtifactoryGameSelector(allGames, gameTechnology, artifactoryStorage);
        }


        private ArtifactoryDownloadHandler _downloadHandler;

        public ArtifactoryDownloadHandler DownloadHandler
        {
            get { return _downloadHandler; }
            set
            {
                _downloadHandler = value;
                OnPropertyChanged(this.GetPropertyName(t => t.DownloadHandler));
            }
        }

        public class ArtifactoryDownloadHandler : ViewModelBase
        {
            public ArtifactoryDownloadHandler(GamesWorkAreaItem parent, ArtifactoryGameSelector gameSelector)
            {
                _parent = parent;
                _gameSelector = gameSelector;
                _gameUrl = gameSelector.GameDownloadUrl;
                _chillUrl = gameSelector.ChillDownloadUrl;

                this.GameLocalFile = Path.Combine(Path.GetTempPath(), _gameSelector.SelectedGame.FriendlyName + ".zip");

                if (HasChill)
                    this.ChillLocalFile = Path.Combine(Path.GetTempPath(), "Chill " + _gameSelector.SelectedChillWrapperVersion.ToString().Replace(".", "_") + ".zip"); 
            }


            private string GameLocalFile { get; set; }
            private string ChillLocalFile { get; set; }
            

            string _gameUrl;
            string _chillUrl;
            ArtifactoryGameSelector _gameSelector;
            GamesWorkAreaItem _parent;
            

            public void Start()
            {
                DeleteFile(this.GameLocalFile);
                DeleteFile(this.ChillLocalFile);

                _parent.DownloadStarted = true;
                if (HasChill)
                {
                    var chillDownloader = new System.Net.WebClient();
                    chillDownloader.DownloadProgressChanged += chillDownloader_DownloadProgressChanged;
                    chillDownloader.DownloadFileCompleted += chillDownloader_DownloadFileCompleted;
                    chillDownloader.DownloadFileAsync(new Uri(_chillUrl), ChillLocalFile);
                }
                else
                {
                    this.ChillDownloadFinish = true;
                }

                var gameDownloader = new System.Net.WebClient();
                gameDownloader.DownloadProgressChanged += gameDownloader_DownloadProgressChanged;
                gameDownloader.DownloadFileCompleted += gameDownloader_DownloadFileCompleted;
                gameDownloader.DownloadFileAsync(new Uri(_gameUrl), GameLocalFile);


                OnPropertyChanged(this.GetPropertyName(t => t.ChillDownloadDescription));
                OnPropertyChanged(this.GetPropertyName(t => t.GameDownloadDescription));
            }

            private void DeleteFile(string file)
            {
                if (string.IsNullOrEmpty(file))
                    return;
                try
                {
                    if (File.Exists(file))
                        File.Delete(file);
                }
                catch
                {
                }
            }

            public string GameDownloadDescription
            {
                get
                {
                    return string.Format("Download {0} version {1} from {2}", _gameSelector.SelectedGame.FriendlyName, _gameSelector.SelectedGameVersion.ToString(), _gameUrl);

                }
            }

            public string ChillDownloadDescription
            {
                get
                {
                    if (!HasChill)
                        return string.Empty;

                    return string.Format("Download chill version {0} from {1} ", _gameSelector.SelectedChillWrapperVersion.ToString(), _chillUrl);
                }
            }

            void chillDownloader_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
            {
                this.ChillDownloadFinish = true;
                StartCreate();
            }

            void chillDownloader_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
            {
                this.ChillDownloadProgress = e.ProgressPercentage;
            }


            private bool HasChill
            {
                get { return !string.IsNullOrEmpty(_chillUrl); }
            }


            void gameDownloader_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
            {
                this.GameDownloadFinish = true;
                StartCreate();
            }

            private void StartCreate()
            {
                if (this.ChillDownloadFinish && this.GameDownloadFinish)
                {
                    try
                    {
                        _parent.CreateGameFromArtifactory(_gameSelector.SelectedGame.GameType, this.GameLocalFile, this.ChillLocalFile);
                    }
                    finally
                    {
                        _parent.DownloadStarted = false;
                    }
                }
            }

            void gameDownloader_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
            {
                this.GameDownloadProgress = e.ProgressPercentage;
            }

            private int _gameDownloadProgress;

            public int GameDownloadProgress
            {
                get { return _gameDownloadProgress; }
                set
                {
                    _gameDownloadProgress = value;
                    OnPropertyChanged(this.GetPropertyName(t => t.GameDownloadProgress));
                }
            }

            private int _chillDownloadProgress;

            public int ChillDownloadProgress
            {
                get { return _chillDownloadProgress; }
                set
                {
                    _chillDownloadProgress = value;
                    OnPropertyChanged(this.GetPropertyName(t => t.ChillDownloadProgress));
                }
            }
            
            private bool GameDownloadFinish { get; set; }
            private bool ChillDownloadFinish { get; set; }

            public System.Windows.Visibility ChillDownloadVisible
            {
                get
                {
                    if (HasChill)
                        return System.Windows.Visibility.Visible;
                    else
                        return System.Windows.Visibility.Collapsed;
                }
            }
        }


        private bool _downloadStarted;

        public bool DownloadStarted
        {
            get { return _downloadStarted; }
            set
            {
                _downloadStarted = value;
                OnPropertyChanged(this.GetPropertyName(t => t.DownloadStarted));
            }
        }

        protected abstract Artifactory.ArtifactoryStorage GetArtifactoryStorage();

        protected abstract GameTechnology GetGameTechnology();

        public abstract bool AllowMyGamesTab { get; }


        protected abstract Models.ISwfFilesProvider GetSwfFilesProvider();

        protected Models.Client.ClientProduct ClientProduct
        {
            get
            {
                return this.WorkArea.GetProduct<Models.Client.ClientProduct>();
            }
        }

        protected abstract void CreateGameFromArtifactory(int gameType, string gameZipFile, string wrapperChillZipFile);


        protected Models.GGP.GGPProduct GGPProduct
        {
            get
            {
                return this.WorkArea.GetProduct<Models.GGP.GGPProduct>();
            }
        }

        protected abstract Models.InstalledGame[] GetSupportedGameTypes();

        #region IGameLuncher Members

        void IGameLuncher.OpenGame(OpenGameParametersViewModel game)
        {

            game.OKPressed = false;

            UIServices.GetCustomUIService<IDialogShowService>().ShowOkCancelDialog(game);

            if (game.OKPressed)
            {
                PlayGame(game);
            }

        }

        protected virtual void PlayGame(OpenGameParametersViewModel game)
        {

            if (game.Parameters.IsFreePlay)
            {
                if (Models.GGPMockDataManager.Singleton.MockData.FreePlay == null || Models.GGPMockDataManager.Singleton.MockData.FreePlay.Length == 0)
                {
                    UIServices.ShowMessage("You don't have any free play voucher defined!");
                    return;
                }
            }

            if (game.SelectedBrowser == null)
                Models.InstalledBrowsersProvider.OpeneWithDefaultBrowser(BuildGameUrl(game));
            else
                game.SelectedBrowser.OpenUrl(BuildGameUrl(game));
        }

        protected abstract string BuildGameUrl(OpenGameParametersViewModel game);

        #endregion

        #region IGameLuncher Members


        public GGPMockDataProvider.LanguageMock[] GetLanguages()
        {
            return Models.GGPMockDataManager.Singleton.Languages;
        }

        #endregion

        #region IGameLuncher Members


        public void OpenSimulator(int gameType)
        {
            this.CurrentGameSimulatorUrl = this.WorkArea.GetProduct<Models.GGPSimulator.GGPSimulatorProduct>().GetSimulatorURL(gameType);
            //Models.InstalledBrowsersProvider.OpenWithChromeIfPossible(this.WorkArea.GetProduct<Models.GGPSimulator.GGPSimulatorProduct>().GetSimulatorURL(gameType));
        }

        private void OpenCurrentGameSimulatorUrlInBrowser()
        {
            Models.InstalledBrowsersProvider.OpenWithChromeIfPossible(CurrentGameSimulatorUrl);
        }



        public IActionViewModel OpenCurrentGameSimulatorUrlInBrowserAction { get; private set; }


        public ActionViewModel AddGameFromArtifactoryAction { get; private set; }

        

        private string _currentGameSimulatorUrl;

        public string CurrentGameSimulatorUrl
        {
            get { return _currentGameSimulatorUrl; }
            set
            {
                _currentGameSimulatorUrl = value;
                OnPropertyChanged(this.GetPropertyName(t => t.CurrentGameSimulatorUrl));
            }
        }

        #endregion

        #region IGameEditor Members

        public abstract void RemoveGame(GameTypeViewModel gameType);

        #endregion
    }


    public interface IGameEditor
    {
        void RemoveGame(GameTypeViewModel gameType);
    }

}
