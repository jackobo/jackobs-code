using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGPGameServer.ApprovalSystem.Common;

namespace GGPMockBootstrapper.ViewModels
{
    public class InstallationProgressViewModel : ViewModelBase
    {
        public InstallationProgressViewModel(Models.IInstallAction[] installActions, Models.IInstalationContext installationContext)
        {
            this.InstallActions = installActions;
            this.InstallationContext = installationContext;
        }

        Models.IInstallAction[] InstallActions { get; set; }

        Models.IInstalationContext InstallationContext { get; set; }

        public int ActionsCount
        {
            get { return InstallActions.Length; }
        }

        private int _currentActionNumber = 0;

        public int CurrentActionNumber
        {
            get { return _currentActionNumber; }
            set 
            { 
                _currentActionNumber = value;
                OnPropertyChanged(this.GetPropertyName(t => t.CurrentActionNumber));
            }
        }


        private InstallActionViewModel _currentAction;
        public InstallActionViewModel CurrentAction
        {
            get { return _currentAction; }
            set
            {
                _currentAction = value;
                OnPropertyChanged(this.GetPropertyName(t => t.CurrentAction));
            }
        }

        public void Run(bool async = true)
        {
            if (async)
            {
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(
                              () =>
                              {
                                  try
                                  {

                                      ExecuteActions();
                                  }
                                  catch (Exception ex)
                                  {
                                      this.InstallationContext.Logger.Exception(ex);
                                      try
                                      {
                                          this.UIServices.ShowMessage(ex.Message);
                                      }
                                      catch { }
                                      UIServices.GetCustomUIService<Models.IApplicationServices>().ShutDown();

                                  }
                              }));


                thread.Start();
            }
            else
            {
                ExecuteActions();
            }
        }


        private void ExecuteActions()
        {

            for (int i = 0; i < this.InstallActions.Length; i++ )
            {
                var action = this.InstallActions[i];

                this.CurrentAction = new InstallActionViewModel(action);
                this.CurrentActionNumber = i + 1;
                action.Execute(this.InstallationContext);

            }

            OnFinished();
        }


        public event EventHandler Finished;

        private void OnFinished()
        {
            var ev = Finished;

            if (ev != null)
                Finished(this, EventArgs.Empty);
        }

    }
}
