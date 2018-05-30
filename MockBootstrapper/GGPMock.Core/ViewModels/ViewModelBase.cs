using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.ViewModels
{
    public class ViewModelBase : IViewModel
    {

        private GGPGameServer.ApprovalSystem.Common.IUserInterfaceServices _uiServices = null;
        public GGPGameServer.ApprovalSystem.Common.IUserInterfaceServices UIServices
        {
            get
            {
                if (_uiServices == null)
                    return MainViewModel.Singletone.UIServices;
                else
                    return _uiServices;
            }
            set
            {
                _uiServices = value;
            }


        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
