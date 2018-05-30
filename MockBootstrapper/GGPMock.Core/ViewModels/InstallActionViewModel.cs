using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGPGameServer.ApprovalSystem.Common;

namespace GGPMockBootstrapper.ViewModels
{
    public class InstallActionViewModel : ViewModelBase
    {
        public InstallActionViewModel(Models.IInstallAction installAction)
        {
            this.InstallAction = installAction;
            this.InstallAction.PropertyChanged += InstallAction_PropertyChanged;
        }

        void InstallAction_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        
        Models.IInstallAction InstallAction { get; set; }

        public string Caption
        {
            get
            {
                return this.InstallAction.Description;
            }
        }

        public int SubActionsCount
        {
            get { return this.InstallAction.SubActionsCount; }
        }
        public int CurrentSubActionIndex
        {
            get { return this.InstallAction.CurrentSubActionIndex; }
        }

        public string CurrentSubActionDescription
        {
            get { return this.InstallAction.CurrentSubActionDescription; }
        }

    }
}
