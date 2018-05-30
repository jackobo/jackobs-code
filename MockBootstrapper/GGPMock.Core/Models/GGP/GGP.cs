using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GGPGameServer.ApprovalSystem.Common;

namespace GGPMockBootstrapper.Models.GGP
{
    public class GGPProduct : Product
    {
        public GGPProduct(IInstalationContext installationContext)
            : base("Game server", installationContext)
        {

        }

        public override IInstallAction[] GetInstallActions()
        {
            var actions = new List<IInstallAction>();

            actions.Add(new RunGGPInstallerAction(this));


            if (IISDetection.IsInstalled)
            {
                actions.Add(new HistoryHandlerInstallAction(this));
                actions.Add(new AddGGPHttpHandlerFirewallException());
            }
                        
            
            return actions.ToArray();
        }


        protected override string GetContentSectionName()
        {
            return "GGP";
        }
        
        
        public string GameServerInstallationPath(IInstalationContext context)
        {
            var folder = ReadProductInstallationFolder(context, GGPGameServer.Installer.Models.Constants.GGP_PRODUCT_NAME);
            if (string.IsNullOrEmpty(folder))
                return Path.Combine(context.Parameters.GGPMockFolder, GGPGameServer.Installer.Models.Constants.GGP_PRODUCT_NAME);
            else
                return folder;

        }

        public string GetGGPHttpHandlerRelativeUrl()
        {
            return string.Format("{0}:{1}/GGPHttpHandler/", GetMachineHostName(), GGP.AddGGPHttpHandlerFirewallException.GGP_HTTP_HANDLER_PORT);
        }
        
       public string GetGGPHistoryHandlerRelativeUrl()
        {
            return string.Format("{0}:{1}/{2}/",
                          GetMachineHostName(),
                          ServerManagerExtensionMethods.GetApplicationHttpPort(HistoryHandlerInstallAction.GGPHistoryHandlerAppName),
                          HistoryHandlerInstallAction.GGPHistoryHandlerAppName);
        }

        private string ReadProductInstallationFolder(IInstalationContext context, string productName)
        {
            var installationLog = context.EnvironmentServices.ReadInstallationLog(InstallerFolders.CurrentVersionLogFile);
            if (installationLog == null)
                return null;

            var product = installationLog.Products.FirstOrDefault(p => p.ProductName == productName);
            if (product != null)
            {
                if(context.EnvironmentServices.FileSystem.FolderExists(product.InstallationFolder))
                    return product.InstallationFolder;
            }

            return null;
        }



        public GGPInfo GetGGPInfo()
        {
            var installationLog = this.InstallationContext.EnvironmentServices.ReadInstallationLog(GGPGameServer.ApprovalSystem.Common.InstallerFolders.CurrentVersionLogFile);
            if (installationLog == null)
                return null;

            var result = new GGPInfo();
            result.GGPVersion = installationLog.Components.First(c => c.ComponentType == ComponentType.GGP).Version;
            result.InstallerVersion = installationLog.InstallerVersion;

            return result;
        }


        public class GGPInfo
        {
            public VersionNumber GGPVersion { get; set; }
            public VersionNumber InstallerVersion { get; set; }
        }

    }
}
