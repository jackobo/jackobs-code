using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Models.FlashPolicyServer
{
    public class FlashPolicyServerInstallAction : ProductInstallAction<FlashPolicyServerProduct>
    {
        public FlashPolicyServerInstallAction(FlashPolicyServerProduct product)
            : base(product)
        {

        }

        #region IInstallAction Members

        public override string Description
        {
            get { return "Install FlashPolicyServer"; }
        }

        protected override void Install(IInstalationContext context, string zipFileName)
        {
            if (context.EnvironmentServices.IsServiceInstalled(this.ServiceName))
            {
                AddFirewallException(context, GetServiceBinPath(context));
                return;
            }

            this.SubActionsCount = 4;

            IncrementSubactionIndex("Extracting files");
            string servicePath = GetServicePath(context);
            context.EnvironmentServices.UnzipFile(zipFileName, servicePath);

            IncrementSubactionIndex("Add firewall exception");
            var serviceExeFile = Directory.EnumerateFiles(servicePath).First(f => Path.GetExtension(f).ToLower() == ".exe");
            AddFirewallException(context, serviceExeFile);

            IncrementSubactionIndex("Creating service");
            context.EnvironmentServices.CreateService(this.ServiceName, serviceExeFile, null);
            IncrementSubactionIndex("Starting service");
            context.EnvironmentServices.StartService(this.ServiceName, new string[0], false);

        }

      

        private static void AddFirewallException(IInstalationContext context, string serviceExeFile)
        {
            try
            {
                context.EnvironmentServices.NetshAddFirewallAllowedProgram(serviceExeFile);
            }
            catch (Exception ex)
            {
                context.Logger.Warning(string.Format("Failed to add firewall exception for program {0} ! Exception details {1}", serviceExeFile, ex.ToString()));
            }
        }


        private string ServiceName
        {
            get { return "FlashPolicyServer"; }
        }

        private string GetServiceBinPath(IInstalationContext context)
        {
            var serviceInfo = context.EnvironmentServices.GetWindowsServiceInformationOrNull(this.ServiceName);
            if (serviceInfo != null)
                return serviceInfo.BinPath;
            else
                return Directory.EnumerateFiles(GetServicePath(context)).FirstOrDefault(f => Path.GetExtension(f).ToLower() == ".exe");
                

        }

        private string GetServicePath(IInstalationContext context)
        {

            var serviceInfo = context.EnvironmentServices.GetWindowsServiceInformationOrNull(this.ServiceName);

            string folder = null;
            if (serviceInfo != null && !string.IsNullOrEmpty(serviceInfo.BinPath))
                folder = Path.GetDirectoryName(serviceInfo.BinPath);

            if (!string.IsNullOrEmpty(folder))
                return folder;

            return Path.Combine(context.Parameters.GGPMockFolder, this.ServiceName);
        }


        protected override string GetPackageName()
        {
            return "FlashPolicyServer.zip";
        }

        #endregion
    }
}
