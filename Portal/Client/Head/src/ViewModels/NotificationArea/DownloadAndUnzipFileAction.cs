using System;
using Spark.Infra.Types;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.NotificationArea
{
    public class DownloadAndUnzipFileAction : ViewModelBase, IBackgroundAction
    {
        

  
        public DownloadAndUnzipFileAction(IBackgroundAction downloadAction, IBackgroundAction unzipAction)
        {
            this.DownloadAction = downloadAction;
            this.DownloadAction.PropertyChanged += DownloadAction_PropertyChanged;

            this.UnzipAction = unzipAction;
            this.UnzipAction.PropertyChanged += UnzipAction_PropertyChanged;

            this.CurrentAction = downloadAction;
        }

     
      

        IBackgroundAction DownloadAction { get; set; }
        IBackgroundAction UnzipAction { get; set; }


        public IBackgroundAction CurrentAction
        {
            get;
            private set;
        }


        void DownloadAction_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!object.ReferenceEquals(this.CurrentAction, this.DownloadAction))
                return;

            if (e.PropertyName == this.GetPropertyName(t => t.Status) 
                && DownloadAction.Status == BackgroundActionStatus.Done)
            {
                this.UnzipAction.Start();
                this.CurrentAction = this.UnzipAction;
                OnPropertyChanged(() => Caption);
            }
            else 
            {
                OnPropertyChanged(e.PropertyName);
            }
        }


        void UnzipAction_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (object.ReferenceEquals(this.CurrentAction, this.UnzipAction))
            {
                OnPropertyChanged(e.PropertyName);
            }
        }


      
        #region IBackgroundAction Members

        
        public BackgroundActionStatus Status
        {
            get
            {
                return this.CurrentAction.Status;
            }
            
        }

        public string Caption
        {
            get
            {
                return CurrentAction.Caption;
            }
        }

        public int ProgressPercentage
        {
            get
            {
                return (int)Math.Round(((decimal)(this.DownloadAction.ProgressPercentage + this.UnzipAction.ProgressPercentage) / 200m) * 100m, 0);
            }
        }

        public void Start()
        {
            if (this.Status == BackgroundActionStatus.InProgress)
            {
                throw new InvalidOperationException(string.Format("Download already in progress"));
            }


            this.CurrentAction.Start();
        }

        public void Cancel()
        {
            if (this.CurrentAction != null)
                this.CurrentAction.Cancel();

        }


        public string ErrorMessage
        {
            get
            {
                return this.CurrentAction.ErrorMessage;
            }
        }

        public string ErrorDetails
        {
            get
            {
                return this.CurrentAction.ErrorDetails;
            }
        }



        #endregion


        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if (this.DownloadAction != null)
                {
                    this.DownloadAction.Dispose();
                }
                if (this.UnzipAction != null)
                {
                    this.UnzipAction.Dispose();
                }
            }
            base.Dispose(disposing);
        }

     
    }
}
