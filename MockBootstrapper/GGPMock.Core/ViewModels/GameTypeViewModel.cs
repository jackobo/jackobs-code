using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGPMockBootstrapper.Models;
using GGPGameServer.ApprovalSystem.Common;
using System.Diagnostics;

namespace GGPMockBootstrapper.ViewModels
{

    public class GameTypeViewModel : ViewModelBase
    {
        public GameTypeViewModel(int id, IGamesInformationProvider gamesInformationProvider, IGameLuncher gameLuncher, string physicalPath, Models.ISwfFilesProvider swfFilesProvider, bool allowRemove, IGameEditor gameEditor)
        {
            this.Id = id;
            this.GamesInformationProvider = gamesInformationProvider;
            this.GameLuncher = gameLuncher;
            this.PhysicalPath = physicalPath;

            this.PlayGameAction = new ActionViewModel("Play game", new Command(PlayGame), ResourcesProvider.CreateBitmapImageSource("Resume24x24.png"));
            this.OpenPhysicalFolderAction = new ActionViewModel("Open", new Command(OpenPhysicalFolder));
            this.OpenSimulatorAction = new ActionViewModel("Simulate", new Command(OpenSimulator));
            this.SwfFilesProvider = swfFilesProvider;
            this.AllowRemove = allowRemove;
            this.RemoveGameAction = new ActionViewModel("Remove", new Command(RemoveGame), ResourcesProvider.CreateBitmapImageSource("Excluded.png"));
            this.GameEditor = gameEditor;

        }


        IGameEditor GameEditor { get; set; }
     
        bool _allowRemove;
        public bool AllowRemove
        {
            get { return _allowRemove && this.GameEditor != null; }
            set
            {
                _allowRemove = value;
                OnPropertyChanged(this.GetPropertyName(t => t.AllowRemove));
            }
        }

        public int Id { get; set; }
        public string Name
        {
            get
            {
                var gameInfo = this.GamesInformationProvider.GetGameInfoOrNull(this.Id);

                if (gameInfo == null)
                    return "N/A";

                return gameInfo.FriendlyName;

                
            }
        }


        public ActionViewModel RemoveGameAction { get; private set; }
        public void RemoveGame()
        {
            if(MessageBoxResponse.Yes == UIServices.ShowMessage("Are you sure?", "Confirmation", MessageBoxButtonType.YesNo))
            {
                this.GameEditor.RemoveGame(this);
            }
        }

        private bool IsGameTypeSupported
        {
            get
            {
                return null != this.GamesInformationProvider.GetGameInfoOrNull(this.Id);
            }
        }


        Models.ISwfFilesProvider SwfFilesProvider { get; set; }


        SwfFile[] _availableSwfFiles = null;
        public SwfFile[] AvailableSwfFiles
        {
            get
            {
                if (SwfFilesProvider == null)
                    return new SwfFile[0];

                if(_availableSwfFiles == null)
                    _availableSwfFiles = SwfFilesProvider.GetSwfFiles(this.Id);

                return _availableSwfFiles;
            }
        }


        

        public SwfFile SelectedSwfFile
        {
            get
            {
                var selected = this.AvailableSwfFiles.FirstOrDefault(swf => swf.IsSelected);

                return selected;
            }
            set
            {
                foreach (var swf in this.AvailableSwfFiles)
                {
                    swf.IsSelected = swf.Equals(value);
                    if (swf.IsSelected)
                    {
                        SwfFilesProvider.UpdateSelectedSwf(this.Id, swf);
                    }
                }


                OnPropertyChanged(this.GetPropertyName(t => t.SelectedSwfFile));
            }
        }

        public string PhysicalPath { get; private set; }

        public IActionViewModel OpenPhysicalFolderAction { get; private set; }

        private void OpenPhysicalFolder()
        {
            Process.Start(this.PhysicalPath);
        }

        private IGamesInformationProvider GamesInformationProvider { get; set; }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", this.Name, this.Id);
        }


        IGameLuncher GameLuncher { get; set; }

        public override bool Equals(object obj)
        {
            var theOther = obj as GameTypeViewModel;

            if (theOther == null)
                return false;

            return this.Id == theOther.Id;
        }

        public IActionViewModel PlayGameAction { get; private set; }

        

        public void PlayGame()
        {
            if (!IsGameTypeSupported)
            {
                UIServices.ShowMessage(string.Format("GameType {0} is not supported by the server", this.Id));
                return;
            }

            if (this.SwfFilesProvider != null && this.SelectedSwfFile == null)
            {
                UIServices.ShowMessage("You need to provide the main flash file for this game before playing it");
                return;
            }

            this.GameLuncher.OpenGame(this.OpenParameters);
        }


        public IActionViewModel OpenSimulatorAction { get; private set; }

        private void OpenSimulator()
        {
            if (!IsGameTypeSupported)
            {
                UIServices.ShowMessage(string.Format("GameType {0} is not supported by the server", this.Id));
                return;
            }

            this.GameLuncher.OpenSimulator(this.Id);
            
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }


        private OpenGameParametersViewModel _openParameters;

        public OpenGameParametersViewModel OpenParameters
        {
            get 
            {
                if (_openParameters == null)
                    _openParameters = new OpenGameParametersViewModel(this, this.GameLuncher.GetLanguages());

                return _openParameters; 
            }
            
        }

        public static GameTypeViewModel[] CreateArray(Models.InstalledGame[] gameTypes, IGamesInformationProvider gamesInformationProvider, IGameLuncher gameLuncher, ISwfFilesProvider swfFilesProvider, IGameEditor gameEditor)
        {
            return gameTypes.Select(gt => new GameTypeViewModel(gt.GameType, gamesInformationProvider, gameLuncher, gt.PhysicalPath, swfFilesProvider, gt.IsCustomGame, gameEditor)).OrderBy(g => g.Name).ToArray();
            
        }

       
    }
}
