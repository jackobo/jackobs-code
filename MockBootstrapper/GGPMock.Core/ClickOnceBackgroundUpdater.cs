using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Deployment.Application;
using System.ComponentModel;


namespace GGPMockBootstrapper
{
    public class ClickOnceBackgroundUpdater
    {
       

        public ClickOnceBackgroundUpdater(TimeSpan checkInterval)
        {
                        

            if (SubscribeToUpdateApplicationEvents())
                StartTimer(checkInterval);
        }

     
        
        
        private bool SubscribeToUpdateApplicationEvents()
        {
            if (!ApplicationDeployment.IsNetworkDeployed)
                return false;
            ApplicationDeployment applicationDeployment = ApplicationDeployment.CurrentDeployment;
            applicationDeployment.CheckForUpdateCompleted += new CheckForUpdateCompletedEventHandler(CheckForApplicationUpdateCompleted);
            applicationDeployment.UpdateCompleted += new AsyncCompletedEventHandler(ApplicationUpdateCompleted);

            return true;
        }

       

        System.Timers.Timer _updateCheckerTimer;
        private void StartTimer(TimeSpan checkInterval)
        {
            _updateCheckerTimer = new System.Timers.Timer(checkInterval.TotalMilliseconds);
            _updateCheckerTimer.Elapsed += new System.Timers.ElapsedEventHandler(UpdateCheckerTimer_Elapsed);
            _updateCheckerTimer.Start();
        }

        private void UpdateCheckerTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdateApplication();
        }


        public event EventHandler ApplicationUpdated;

        private void OnApplicationUpdated()
        {
            var ev = ApplicationUpdated;
            if (ev != null)
                ev(this, EventArgs.Empty);
        }

        private void UpdateApplication()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment.CurrentDeployment.CheckForUpdateAsync();  
            }
        }


        private bool Downloading { get; set; }

        private void CheckForApplicationUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled)
            {
                return;
            }

            if(!this.Downloading && e.UpdateAvailable && e.AvailableVersion != ApplicationDeployment.CurrentDeployment.UpdatedVersion)
            {   
                ApplicationDeployment.CurrentDeployment.UpdateAsync();
                this.Downloading = true;
            }
        }

        private void ApplicationUpdateCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && !e.Cancelled)
                {
                    OnApplicationUpdated();
                }
            }
            finally
            {
                this.Downloading = false;
            }
        }


        public void Stop()
        {
            if(_updateCheckerTimer != null)
                _updateCheckerTimer.Stop();
        }
    }

}