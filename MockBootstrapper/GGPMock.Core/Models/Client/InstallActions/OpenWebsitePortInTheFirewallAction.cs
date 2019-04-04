using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.Administration;
using GGPGameServer.ApprovalSystem.Common;
using System.ComponentModel;

namespace GGPMockBootstrapper.Models.Client
{
    public class OpenWebsitePortInTheFirewallAction : IInstallAction
    {
        public OpenWebsitePortInTheFirewallAction()
        {
            using (var iisManager = new ServerManager())
            {
                this.Ports = new PortInFirewall[]
                {
                    new PortInFirewall(Html5ClientInstallAction.AppName, iisManager.GetApplicationHttpPort(Html5ClientInstallAction.AppName)),
                    new PortInFirewall(ThinClientInstallAction.AppName, iisManager.GetApplicationHttpPort(ThinClientInstallAction.AppName)),
                };

            }

        }


        private PortInFirewall[] Ports { get; set; }

        private class PortInFirewall
        {
            public PortInFirewall(string appName, int port)
            {
                this.AppName = appName;
                this.Port = port;
            }
            public string AppName { get; set; }
            public int Port { get; set; }
        }

        #region IInstallAction Members

        public string Description
        {
            get
            {
                return "Open ports in the firewall";
            }
        }

        public void Execute(IInstalationContext context)
        {
            foreach (var port in this.Ports)
            {
                IncrementSubactionIndex(string.Format("Add firewall port {0} for {1}", port.Port, port.AppName));

                try
                {
                    context.EnvironmentServices.NetshAddFirewallAllowedTcpPort(port.AppName, port.Port, FirewallRuleDirection.Inbound);
                }
                catch (Exception ex)
                {
                    context.Logger.Warning(string.Format("Could not open {0} port for {1} in the firewall. Exception details {2}", port.Port, port.AppName, ex.ToString()));
                }
                
            }

        }

        private int _subActionsCount = 0;
        public int SubActionsCount
        {
            get { return _subActionsCount; }
            set
            {
                _subActionsCount = value;
                OnPropertyChanged(this.GetPropertyName(t => t.SubActionsCount));
            }
        }

        private int _currentActionIndex = 0;
        public int CurrentSubActionIndex
        {
            get { return _currentActionIndex; }
            set
            {
                _currentActionIndex = value;
                OnPropertyChanged(this.GetPropertyName(t => t.CurrentSubActionIndex));
            }
        }

        private string _currentSubActionDescription;
        public string CurrentSubActionDescription
        {
            get { return _currentSubActionDescription; }
            set
            {
                _currentSubActionDescription = value;
                OnPropertyChanged(this.GetPropertyName(t => t.CurrentSubActionDescription));
            }
        }


        protected virtual void IncrementSubactionIndex(string description)
        {
            this.CurrentSubActionIndex = Math.Min(this.CurrentSubActionIndex + 1, this.SubActionsCount);
            this.CurrentSubActionDescription = description;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var ev = PropertyChanged;
            if (ev != null)
                ev(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
