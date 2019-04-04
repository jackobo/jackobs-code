using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGPGameServer.ApprovalSystem.Common;

namespace GGPMockBootstrapper.ViewModels
{
    public class OkCancelDialogViewModel : ViewModelBase, IOkCancelDialogViewModel
    {
        public OkCancelDialogViewModel()
        {
            this.OKPressed = false;
            this.OKAction = new ActionViewModel("OK", new Command(OkCore));
            this.CancelAction = new ActionViewModel("Cancel", new Command(CancelCore));
        }
        #region IOkCancelDialogViewModel Members

        Action _closeAction;
        public Action Close
        {
            get
            {
                return _closeAction;
            }
            set
            {
                _closeAction = value;

            }
        }

        public bool OKPressed
        {
            get;
            set;
        }

        public IActionViewModel OKAction { get; private set; }


        public IActionViewModel CancelAction { get; private set; }

        protected virtual bool OK()
        {
            return true;
        }


        protected virtual void Cancel()
        {

        }


        private void OkCore()
        {
            if (this.OK())
            {
                this.OKPressed = true;
                this.Close();
            }
        }

        private void CancelCore()
        {
            this.Cancel();
            this.Close();
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set 
            {
                _title = value;
                OnPropertyChanged(this.GetPropertyName(t => t.Title));
            }
        }
        
        
        
        #endregion
    }
}
