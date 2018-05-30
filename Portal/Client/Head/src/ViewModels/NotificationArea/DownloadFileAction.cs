using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Services;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Spark.Infra.Windows;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.NotificationArea
{
    public class DownloadFileAction : ViewModelBase, IBackgroundAction
    {
        
        public DownloadFileAction(IServiceLocator serviceLocator, Uri uri, string destinationPath)
        {
            this.Status = BackgroundActionStatus.Waiting;
            this.ServiceLocator = serviceLocator;
            this.Uri = uri;
            this.DestinationPath = destinationPath;
            
        }

        private IServiceLocator ServiceLocator { get; set; }


        BackgroundActionStatus _status;
        public BackgroundActionStatus Status
        {
            get { return _status; }
            set
            {
                SetProperty(ref _status, value);
                OnPropertyChanged(() => Caption);
            }
        }

        public string Caption
        {
            get
            {
                if (this.Status == BackgroundActionStatus.Waiting)
                    return string.Format("Waiting for downloading {0}", this.Uri);

                if(this.Status == BackgroundActionStatus.InProgress)
                    return string.Format("Downloading {0}", this.Uri);

                if (this.Status == BackgroundActionStatus.Error)
                    return string.Format("Error downloading {0}", this.Uri);

                if (this.Status == BackgroundActionStatus.Canceled)
                    return string.Format("Download canceled for {0}", this.Uri);

                return string.Format("Finished downloading {0}", this.Uri);
            }
        }


        const int MAX_PERCENTAGE = 100;

        int _progressPercentage;
        public int ProgressPercentage
        {
            get { return _progressPercentage; }
            private set
            {
                SetProperty(ref _progressPercentage, value);
            }
        }

        public Uri Uri { get; set; }
        public string DestinationPath { get; set; }


        string _errorMessage;
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                SetProperty(ref _errorMessage, value);
            }
        }

        string _errorDetails = null;
        public string ErrorDetails
        {
            get
            {
                return _errorDetails;
            }
            set
            {
                SetProperty(ref _errorDetails, value);
            }
        }

        IDownloadManager DownloadManager { get; set; }

        public void Start()
        {
            
            if (this.Status == BackgroundActionStatus.InProgress)
            {
                throw new InvalidOperationException(string.Format("Download already in progress {0}", this.Uri));
            }

            this.ProgressPercentage = 0;
            this.Status = BackgroundActionStatus.InProgress;
                        

            InitDownloadManager();

            if (FileSystemManager.FileExists(this.DestinationPath))
                FileSystemManager.DeleteFile(this.DestinationPath);
            else
                FileSystemManager.CreateFolder(Path.GetDirectoryName(this.DestinationPath));
            
            DownloadManager.DownloadFile(this.Uri, this.DestinationPath);

            
        }

        

        private IFileSystemManager FileSystemManager
        {
            get
            {
                return this.ServiceLocator.GetInstance<IFileSystemManager>();
            }
        }

        private void InitDownloadManager()
        {
            if (DownloadManager == null)
            {
                DownloadManager = this.ServiceLocator.GetInstance<IDownloadManager>();
                DownloadManager.DownloadProgressChanged += (mgr, args) => this.ProgressPercentage = args.Percentage;
                DownloadManager.DownloadCompleted += (mgr, args) =>
                    {
                        this.ProgressPercentage = MAX_PERCENTAGE;
                        if (args.Cancelled == true)
                        {
                            this.Status = BackgroundActionStatus.Canceled;
                        }
                        else if (args.Error != null)
                        {
                            this.Status = BackgroundActionStatus.Error;
                            this.ErrorMessage = args.Error.Message;
                            this.ErrorDetails = args.Error.ToString();
                            
                        }
                        else
                        {
                            this.Status = BackgroundActionStatus.Done;
                        }

                        DownloadManager.Dispose();
                        DownloadManager = null;
                    };
            }
        }

        public void Cancel()
        {
            if (this.Status == BackgroundActionStatus.InProgress && this.DownloadManager != null)
            {
                DownloadManager.Cancel();
                this.Status = BackgroundActionStatus.Canceled;
            }
            
        }


        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if (DownloadManager != null)
                    DownloadManager.Dispose();
            }
            base.Dispose(disposing);
        }
        

     
    }
}
