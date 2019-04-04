using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Models.GGP
{
    public class AddGGPHttpHandlerFirewallException : IInstallAction
    {
        #region IInstallAction Members

        public string Description
        {
            get
            {
                return string.Format("Allow GGPHttpHandler port {0} in the firewall", GGP_HTTP_HANDLER_PORT);
            }
        }


        public static readonly int GGP_HTTP_HANDLER_PORT = 8687;

        public void Execute(IInstalationContext context)
        {
            try
            {
                context.EnvironmentServices.NetshAddFirewallAllowedTcpPort("GGPHttpHandler", GGP_HTTP_HANDLER_PORT, GGPGameServer.ApprovalSystem.Common.FirewallRuleDirection.Inbound);
            }
            catch (Exception ex)
            {
                context.Logger.Warning(string.Format("Could not open {0} port in the firewall. Exception details {1}", GGP_HTTP_HANDLER_PORT, ex.ToString()));
            }
        }

        public int SubActionsCount
        {
            get
            {
                return 0;
            }
        }

        public int CurrentSubActionIndex
        {
            get
            {
                return 0;
            }
        }

        public string CurrentSubActionDescription
        {
            get
            {
                return string.Empty;
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
