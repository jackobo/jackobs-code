using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GGPMockBootstrapper.Models;
using GGPGameServer.ApprovalSystem.Common;
using GGPMockBootstrapper.GamesPortalService;

namespace GGPMockBootstrapper.ViewModels
{
    public class ArtifactoryGameSelector : OkCancelDialogViewModel
    {
        public ArtifactoryGameSelector(IEnumerable<GameInfoModel> availableGames, GameTechnology gameTechnology, Artifactory.ArtifactoryStorage artifactoryStorage)
        {
            Title = "Select game from Artifactory";
            _gameTechnology = gameTechnology;
            this.ArtifactoryStorage = artifactoryStorage;
            this.Games = availableGames.Where(g => !g.IsSubGame && g.OperatorId == 0)
                                       .GroupBy(g => g.GameType)
                                       .Select(g => g.First())
                                       .OrderBy(g => g.FriendlyName)
                                       .ToArray();

            

            
        }

        GameTechnology _gameTechnology;

        Artifactory.ArtifactoryStorage ArtifactoryStorage { get; set; }
        public GameInfoModel[] Games { get; private set; }

        private GameInfoModel _selectedGame;

        public GameInfoModel SelectedGame
        {
            get { return _selectedGame; }
            set
            {
                _selectedGame = value;
                OnPropertyChanged(this.GetPropertyName(t => t.SelectedGame));

                using (var proxy = CreateGamesPortalServiceProxy())
                {
                    var game = proxy.GetAllGames().Games.FirstOrDefault(g => g.MainGameType == this.SelectedGame.GameType);
                    _selectedGameVersions = proxy.GetGameVersions(game.Id).GameVersions;
                }

                ReloadRegulations();
            }
        }


        GamesPortalService.GameVersionDTO[] _selectedGameVersions = new GamesPortalService.GameVersionDTO[0];

        protected override bool OK()
        {
            if (this.SelectedGame == null)
            {
                this.UIServices.ShowMessage("You must select a game");
                return false;
            }

            if (this.SelectedRegulation == null)
            {
                this.UIServices.ShowMessage("You must select a regulation");
                return false;
            }

            if (this.SelectedGameVersion == null)
            {
                this.UIServices.ShowMessage("You must select a game version");
                return false;
            }

            if (this.SelectedChillWrapperVersion == null)
            {
                this.UIServices.ShowMessage("You must select a chill version");
                return false;
            }

            return true;
        }

        private void ReloadRegulations()
        {

            if (this.SelectedGame == null)
            {
                this.SelectedRegulation = null;
                this.Regulations = new string[0];

            }
            else
            {
                this.Regulations = _selectedGameVersions.SelectMany(v => v.Regulations.Select(p => p.Regulation).Distinct()).Distinct().OrderBy(r => r).ToArray();
            }
        }

        private static GamesPortalService.GamesPortalServiceClient CreateGamesPortalServiceProxy()
        {
            return new GamesPortalService.GamesPortalServiceClient();
        }

        private string[] _regulations;

        public string[] Regulations
        {
            get { return _regulations; }
            set
            {
                _regulations = value;
                OnPropertyChanged(this.GetPropertyName(t => t.Regulations));
            }
        }

        private string _selectedRegulation;
        public string SelectedRegulation
        {
            get { return _selectedRegulation; }
            set
            {
                _selectedRegulation = value;
                OnPropertyChanged(this.GetPropertyName(t => t.SelectedRegulation));

                ReloadGameVersions();
                ReloadWrapperVersions();
            }
        }

      

        private void ReloadGameVersions()
        {
            if (string.IsNullOrEmpty(this.SelectedRegulation))
            {
                this.SelectedGameVersion = null;
                this.GameVersions = new ArtifactoryGameVersion[0];
            }
            else
            {
                this.GameVersions = _selectedGameVersions.Where(v => v.GameInfrastructure.GameTechnology == _gameTechnology 
                                                                     && v.Regulations.Select(p => p.Regulation).Contains(this.SelectedRegulation))
                                                                .Select(v => new ArtifactoryGameVersion(v.VersionId, new VersionNumber(v.VersionAsNumber), v.GameInfrastructure.PlatformType))
                                                                .OrderByDescending(v => v.Version)
                                                                .ToArray();
                this.SelectedGameVersion = this.GameVersions.FirstOrDefault();
                
            }

        }


        private void ReloadWrapperVersions()
        {
            if (string.IsNullOrEmpty(this.SelectedRegulation))
            {
                this.SelectedChillWrapperVersion = null;
                this.ChillWrapperVersions = new VersionNumber[0];
            }
            else
            {

                this.ChillWrapperVersions = this.ArtifactoryStorage.GetChillWrapperVersions(this.SelectedRegulation).OrderByDescending(v => v).ToArray();
                this.SelectedChillWrapperVersion = this.ChillWrapperVersions.FirstOrDefault();

            }
        }

        private ArtifactoryGameVersion[] _gameVersions;

        public ArtifactoryGameVersion[] GameVersions
        {
            get { return _gameVersions; }
            set
            {
                _gameVersions = value;
                OnPropertyChanged(this.GetPropertyName(t => t.GameVersions));
            }
        }


        private ArtifactoryGameVersion _selectedGameVersion;

        public ArtifactoryGameVersion SelectedGameVersion
        {
            get { return _selectedGameVersion; }
            set
            {
                _selectedGameVersion = value;
                OnPropertyChanged(this.GetPropertyName(t => t.SelectedGameVersion));
            }
        }

        public string GameDownloadUrl
        {
            get
            {

                var gameVersion = _selectedGameVersions.FirstOrDefault(v => v.VersionId == this.SelectedGameVersion.VersionID);
                if (gameVersion == null)
                    return null;

                var regulation = gameVersion.Regulations.FirstOrDefault(r => r.Regulation == this.SelectedRegulation);
                if (regulation == null)
                    return null;

                return regulation.DownloadInfo.Uri;
                
            }
        }


        public string ChillDownloadUrl
        {
            get
            {
                return this.ArtifactoryStorage.GetWrapperChillDownloadUrl(this.SelectedRegulation, this.SelectedChillWrapperVersion);
            }
        }


        private VersionNumber[] _chillWrapperVersions;

        public VersionNumber[] ChillWrapperVersions
        {
            get { return _chillWrapperVersions; }
            set
            {
                _chillWrapperVersions = value;
                OnPropertyChanged(this.GetPropertyName(t => t.ChillWrapperVersions));
            }
        }

        private System.Windows.Visibility _wrapperChillSelectionVisible = System.Windows.Visibility.Visible;

        public System.Windows.Visibility WrapperChillSelectionVisible
        {
            get { return _wrapperChillSelectionVisible; }
            set
            {
                _wrapperChillSelectionVisible = value;
                OnPropertyChanged(this.GetPropertyName(t => t.WrapperChillSelectionVisible));
            }
        }
        
        private VersionNumber _selectedChillWrapperVersion;

        public VersionNumber SelectedChillWrapperVersion
        {
            get { return _selectedChillWrapperVersion; }
            set
            {
                _selectedChillWrapperVersion = value;
                OnPropertyChanged(this.GetPropertyName(t => t.SelectedChillWrapperVersion));
            }
        }



        
    }


    public class ArtifactoryGameVersion
    {
        public ArtifactoryGameVersion(Guid versionID, VersionNumber version, PlatformType platformType)
        {
            this.VersionID = versionID;
            this.Version = version;
            this.PlatformType = platformType;
        }

        public Guid VersionID { get; set; }

        public VersionNumber Version { get; set; }
        public PlatformType PlatformType { get; set; }

        public override bool Equals(object obj)
        {
            var theOther = obj as ArtifactoryGameVersion;

            if (theOther == null)
                return false;

            return this.VersionID == theOther.VersionID;
        }

        public override string ToString()
        {
            return Version.ToString() + " - " + PlatformType;
        }

        public override int GetHashCode()
        {
            return this.VersionID.GetHashCode();
        }
    }
}
