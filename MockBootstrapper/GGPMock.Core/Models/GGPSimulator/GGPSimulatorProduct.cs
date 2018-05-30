using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGPGameServer.ApprovalSystem.Common;

namespace GGPMockBootstrapper.Models.GGPSimulator
{ 
    public class GGPSimulatorProduct : Product
    {
        public GGPSimulatorProduct(IInstalationContext installationContext)
            : base("GGP Simulator", installationContext)
        {
        }

        public override IInstallAction[] GetInstallActions()
        {
            if (IISDetection.IsInstalled)
            {
                return new IInstallAction[]
                {
                    new GGPSimulatorServiceInstallAction(this),
                    new GGPSimulatorClientInstallAction(this)
                };
            }
            else
            {
                return new IInstallAction[]
                {
                    new GGPSimulatorServiceInstallAction(this),
                };
            }
        }

        protected override string GetContentSectionName()
        {
            return "Simulator";
        }

        public string GGPSimulatorIISAppName
        {
            get
            {
                return "GGPSimulator";
            }
        }

        public string GetSimulatorURL(int? gameType = null)
        {
            if (GGPMockDataManager.Singleton.MockData != null)
            {
                return string.Format("http://localhost:{0}/{1}/?cid={2}{3}",
                                        GetSimulatorPort(),
                                        this.GGPSimulatorIISAppName,
                                        GGPMockDataManager.Singleton.MockData.CID, 
                                        gameType != null ? "&gameType=" + gameType.Value.ToString() : string.Empty);
            }
            else
            {
                return string.Format("http://localhost:{0}/{1}", GetSimulatorPort(), this.GGPSimulatorIISAppName);
            }
        }


        private int GetSimulatorPort()
        {
            return ServerManagerExtensionMethods.GetApplicationHttpPort(GGPSimulatorIISAppName);
        }



        
    }
}
