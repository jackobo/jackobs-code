using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.Services;

using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Exceptions;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.Dialogs
{
    public class DownloadGameVersionDialog : ViewModelBase, IOkCancelDialogBoxViewModel
    {
        public DownloadGameVersionDialog(Game game, GameVersion gameVersion, IServiceLocator serviceLocator)
        {
            this.Game = game;
            this.GameVersion = gameVersion;
            this.ServiceLocator = serviceLocator;
            this.DestinationFolder = LastUsedDestinationFolder;

            this.SupportedRegulations = new ObservableCollection<SupportedRegulation>
                                                (
                                                    gameVersion.GetSupportedRegulations()
                                                                            .OrderBy(r => r.Name)
                                                                            .Select(r => new SupportedRegulation(r) { Selected = true })
                                                );


            this.SelectDestinationFolderCommand = new Command(SelectFolder);
            this.SelectAllRegulationsCommand = new Command(() => SelectAllRegulations(true));
            this.UnselectAllRegulationsCommand = new Command(() => SelectAllRegulations(false));
            
        }

        private static string LastUsedDestinationFolder { get; set; }

        public string Title
        {
            get
            {
                return string.Format("Download {0} [{1}] version {2}", this.Game.Name, this.Game.MainGameType, this.GameVersion.Version.ToString());
            }
        }

        private void SelectAllRegulations(bool selected)
        {
            foreach (var r in this.SupportedRegulations)
            {
                r.Selected = selected;
            }
        }


        public ObservableCollection<SupportedRegulation> SupportedRegulations { get; private set; }

        public ICommand SelectDestinationFolderCommand { get; private set; }
        public ICommand SelectAllRegulationsCommand { get; private set; }
        public ICommand UnselectAllRegulationsCommand { get; private set; }
        

        public Game Game { get; private set; }
        public GameVersion GameVersion { get; private set; }

        IServiceLocator ServiceLocator { get; set; }

        IDialogServices DialogServices
        {
            get { return ServiceLocator.GetInstance<IDialogServices>(); }
        }

        NotificationArea.IBackgroundActionsArea BackgroundActionsArea
        {
            get { return this.ServiceLocator.GetInstance<NotificationArea.IBackgroundActionsArea>(); }
        }
               

        private bool _extractFileContent = true;

        public bool ExtractFileContent
        {
            get { return _extractFileContent; }
            set
            {
                SetProperty(ref _extractFileContent, value);
            }
        }


        private void SelectFolder()
        {
            var selectedFolder = DialogServices.SelectFolder();
            if (!string.IsNullOrEmpty(selectedFolder))
            {
                this.DestinationFolder = selectedFolder;
            }
        }


        string _destinationFolder;
        public string DestinationFolder
        {
            get { return _destinationFolder; }
            set
            {
                if (value != null)
                    value = value.Trim();
                SetProperty(ref _destinationFolder, value);
            }
        }

        public void ExecuteOk()
        {
            if(string.IsNullOrEmpty(this.DestinationFolder))
                throw new ValidationException("You must provide the destination folder!");

            if (SelectedRegulations.Length == 0)
                throw new ValidationException("You must select at least one regulation!");
                     
            foreach(var regulation in this.GameVersion.Regulations.Where(r => this.SelectedRegulations.Contains(r.RegulationType)))
            {
                this.BackgroundActionsArea.AddAction(CreateAction(regulation));
            }
            

            LastUsedDestinationFolder = this.DestinationFolder;
            
        }

        private NotificationArea.IBackgroundAction CreateAction(GameVersionRegulation regulation)
        {
            if (this.ExtractFileContent)
            {
                var zipFileName = Path.Combine(this.DestinationFolder, "Downloads", regulation.RegulationType.Name, this.Game.Name + "_" + regulation.DownloadInfo.FileName);
                return new NotificationArea.DownloadAndUnzipFileAction(new NotificationArea.DownloadFileAction(this.ServiceLocator,
                                                                                                              new Uri(regulation.DownloadInfo.Uri),
                                                                                                              zipFileName),
                                                                       new NotificationArea.UnzipFileAction(zipFileName, 
                                                                                                            Path.Combine(this.DestinationFolder, "Unzip", regulation.RegulationType.Name),
                                                                                                            this.ServiceLocator));
            }
            else
            {
                return new NotificationArea.DownloadFileAction(
                                                    this.ServiceLocator,
                                                    new Uri(regulation.DownloadInfo.Uri),
                                                    Path.Combine(this.DestinationFolder, regulation.RegulationType.Name, this.Game.Name + "_" + regulation.DownloadInfo.FileName));
            }
        }

        


        RegulationType[] SelectedRegulations
        {
            get
            {
                return this.SupportedRegulations.Where(r => r.Selected).Select(r => r.Regulation).ToArray();
            }
        }

        public void ExecuteCancel()
        {

        }


        public class SupportedRegulation : ViewModelBase
        {
            public SupportedRegulation(RegulationType regulation)
            {
                this.Regulation = regulation;
            }

            private bool _selected;

            public bool Selected
            {
                get { return _selected; }
                set
                {
                    SetProperty(ref _selected, value);
                }
            }
            public RegulationType Regulation { get; private set; }
        }




        
    }
}
