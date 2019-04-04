using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Models
{
    public class InstallationContext : IInstalationContext
    {

        public InstallationContext(GGPGameServer.ApprovalSystem.Common.IUserInterfaceServices ui)
        {
            this.EnvironmentServices = new GGPGameServer.ApprovalSystem.Common.EnvironmentServices(ui);
            this.Parameters = new InstallationParameters();
            this.Logger = GGPGameServer.ApprovalSystem.Common.Logger.LoggerFactoryManager.CreateLogNotifier(this.GetType());
        }

        #region IInstalationContext Members

        public GGPGameServer.ApprovalSystem.Common.IEnvironmentServices EnvironmentServices
        {
            get;
            private set;
        }

        #endregion

        #region IInstalationContext Members


        public InstallationParameters Parameters
        {
            get;
            private set;
        }

        #endregion

        #region IInstalationContext Members


        public GGPGameServer.ApprovalSystem.Common.Logger.ILogNotifier Logger
        {
            get;
            private set;
        }

        #endregion
    }
}
