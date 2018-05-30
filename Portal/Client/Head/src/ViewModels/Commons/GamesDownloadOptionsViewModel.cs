using System;
using System.Windows.Input;

using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Exceptions;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels
{
    public class GamesDownloadOptionsViewModel : ServicedViewModelBase
    {

        public GamesDownloadOptionsViewModel(Action downloadAction, IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            this.DownloadAction = downloadAction;
            this.DownloadCommand = new Command(Download);
            this.SelectDestinationFolderCommand = new Command(SelectFolder);
        }

        private void Download()
        {
            if (string.IsNullOrEmpty(this.DestinationFolder))
                throw new ValidationException("Please provide the destination folder!");

            DownloadAction();
        }

        Action DownloadAction { get; set; }

        private static string _destinationFolder;

        public string DestinationFolder
        {
            get { return _destinationFolder; }
            set
            {
                SetProperty(ref _destinationFolder, value);
            }
        }


        public ICommand SelectDestinationFolderCommand { get; private set; }

        private void SelectFolder()
        {
            var selectedFolder = this.ServiceLocator.GetInstance<IDialogServices>().SelectFolder();
            if (!string.IsNullOrEmpty(selectedFolder))
            {
                this.DestinationFolder = selectedFolder;
            }
        }

        public ICommand DownloadCommand { get; private set; }


    }
}
