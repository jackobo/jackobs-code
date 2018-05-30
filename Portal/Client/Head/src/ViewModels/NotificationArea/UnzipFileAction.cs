using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Services;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.NotificationArea
{
    public class UnzipFileAction : ViewModelBase, IBackgroundAction
    {
        public UnzipFileAction(string zipFile, string destinationFolder, IServiceLocator serviceLocator)
        {
            this.ZipFile = zipFile;
            this.DestinationFolder = destinationFolder;
            this.ServiceLocator = serviceLocator;

        }

        string ZipFile{get;set;}
        string DestinationFolder{get;set;}
        IServiceLocator ServiceLocator { get; set; }

        #region IBackgroundAction Members

        BackgroundActionStatus _status = BackgroundActionStatus.Waiting;
        public BackgroundActionStatus Status
        {
            get
            {
                return _status;
            }
            private set
            {
                SetProperty(ref _status, value);
            }
        }

        public string Caption
        {
            get
            {
                return string.Format("Unzip file {0} to {1}", Path.GetFileName(this.ZipFile), this.DestinationFolder);
            }
        }

        int _progressPercentage = 0;
        public int ProgressPercentage
        {
            get
            {
                return _progressPercentage;
            }
            set
            {
                SetProperty(ref _progressPercentage, value);
            }
        }

        public void Start()
        {
            if (this.Status == BackgroundActionStatus.InProgress)
            {
                throw new InvalidOperationException(string.Format("Unziping file {0} is already in progress", this.ZipFile));
            }

            this.Status = BackgroundActionStatus.InProgress;

            InitZipFileExtractor();

            ZipFileExtractor.Unzip(this.ZipFile, this.DestinationFolder);
        }


        IZipFileExtractor ZipFileExtractor
        {
            get;
            set;
        }

        private void InitZipFileExtractor()
        {
            if (ZipFileExtractor == null)
            {
                var zipFileExtractor = this.ServiceLocator.GetInstance<IZipFileExtractor>();
                zipFileExtractor.UnzipProgressChanged += (sender, args) => this.ProgressPercentage = args.Percentage;
                zipFileExtractor.UnzipCompleted += (sender, args) =>
                {
                    if (args.Cancelled)
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
                };

                ZipFileExtractor = zipFileExtractor;
            }
        }

    

        public void Cancel()
        {
            if (ZipFileExtractor != null)
            {
                ZipFileExtractor.Cancel();
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if (ZipFileExtractor != null)
                {
                    ZipFileExtractor.Dispose();
                }
            }
            base.Dispose(disposing);
        }
       
        #endregion

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



        string _errorDetails;
        public string ErrorDetails
        {
            get { return _errorDetails; }
            set
            {
                SetProperty(ref _errorDetails, value);
            }
        }
    }
}
