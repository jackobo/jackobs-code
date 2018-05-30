using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Web.Administration;


namespace GGPMockBootstrapper.Models.GGPSimulator
{
    public class GGPSimulatorClientInstallAction : IISAppInstallActionBase<GGPSimulatorProduct>
    {
        public GGPSimulatorClientInstallAction(GGPSimulatorProduct ggpSimulatorProduct)
            : base(ggpSimulatorProduct, ggpSimulatorProduct.GGPSimulatorIISAppName, ManagedPipelineMode.Integrated)
        {
        }


        public override string Description
        {
            get 
            {
                return "Install GGP Simulator User Interface";
            }
        }


      
        protected override string GetPackageName()
        {
            return "Client.zip";
        }

        protected override void ConfigureApplicationCore(IInstalationContext context, ServerManager iisManager)
        {
            base.ConfigureApplicationCore(context, iisManager);

            CreateConfigJSFile(iisManager, context);
        }

        
        private void CreateConfigJSFile(ServerManager iisManager, IInstalationContext context)
        {
            var jsTemplate = Path.Combine(GetApplicationPhysicalPath(iisManager), "js", "config.js.template");
            var jsConfigContent = context.EnvironmentServices.FileSystem.ReadAllText(jsTemplate);

            /*
            context.EnvironmentServices.FileSystem.WriteAllText(Path.Combine(FindApplication(iisManager).VirtualDirectories["/"].PhysicalPath, "js", "config.js"),
                                                                jsConfigContent.Replace("<<server_ip>>", this.Product.GetMachineHostName()).Replace("<<server_port>>", GGP.AddGGPHttpHandlerFirewallException.GGP_HTTP_HANDLER_PORT.ToString()));
             */

            context.EnvironmentServices.FileSystem.WriteAllText(Path.Combine(FindApplication(iisManager).VirtualDirectories["/"].PhysicalPath, "js", "config.js"),
                                                                jsConfigContent.Replace("<<server_ip>>", "localhost").Replace("<<server_port>>", GGP.AddGGPHttpHandlerFirewallException.GGP_HTTP_HANDLER_PORT.ToString()));
        }
    }
}
