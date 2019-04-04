using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Models.MainProxy
{
    public class MainProxyInstallAction : ProductInstallAction<MainProxyProduct>
    {
        public MainProxyInstallAction(MainProxyProduct mainProxy)
            : base(mainProxy)
        {

        }


        private static readonly string MAIN_PROXY_MOCK_SERVICE_NAME = "MainProxyMock";

        public override string Description
        {
            get
            {
                return "Installing MainProxy mock";
            }
        }

        protected override string GetPackageName()
        {
            return "MainProxyMock.zip";
        }

        protected override void Install(IInstalationContext context, string zipFileName)
        {
            var mainProxyExe = GetMainProxyMockExe(context);
            var mainProxyFolder = Path.GetDirectoryName(mainProxyExe);

            this.SubActionsCount = 6;
            
            IncrementSubactionIndex("Stopping service");
            context.EnvironmentServices.StopService(MAIN_PROXY_MOCK_SERVICE_NAME);

            IncrementSubactionIndex("Extracting files");
            ExtractServerContent(context, mainProxyFolder, zipFileName);

            
            IncrementSubactionIndex("Add firewall exception");
            NetshAddAllowedProgram(context, mainProxyExe);

            IncrementSubactionIndex("Create service");
            context.EnvironmentServices.CreateService(MAIN_PROXY_MOCK_SERVICE_NAME,
                                                      mainProxyExe,
                                                      null);


            var mainProxyLogsFolder = Path.Combine(context.Parameters.GGPMockFolder, "MainProxyLogs");
            IncrementSubactionIndex("Create log folder");
            context.EnvironmentServices.FileSystem.CreateFolder(mainProxyFolder);

            IncrementSubactionIndex("Starting service");
            context.EnvironmentServices.StartService(MAIN_PROXY_MOCK_SERVICE_NAME, new string[0], TimeSpan.FromSeconds(10));
        }


        private void NetshAddAllowedProgram(IInstalationContext context, string program)
        {
            try
            {
                context.EnvironmentServices.NetshAddFirewallAllowedProgram(program);
            }
            catch (Exception ex)
            {
                context.Logger.Warning(string.Format("Failed to add firewall exception for program {0} ! Exception details {1}", program, ex.ToString()));
            }
        }


        private void ExtractServerContent(IInstalationContext context, string mainProxyFolder, string zipFileName)
        {

            using (Ionic.Zip.ZipFile zipFile = Ionic.Zip.ZipFile.Read(zipFileName))
            {
                this.SubActionsCount += zipFile.Count;
                foreach (var entry in zipFile)
                {
                    try
                    {
                        IncrementSubactionIndex(string.Format("Extracting file: {0}", entry.FileName));
                        entry.ExtractExistingFile = Ionic.Zip.ExtractExistingFileAction.OverwriteSilently;
                        entry.Extract(mainProxyFolder);
                    }
                    catch (Exception ex)
                    {
                        context.Logger.Warning(string.Format("GGPSimulatorServiceInstallAction.ExtractServerContent: Failed to extract '{0}' from '{1}'! Exception details: {2}", entry.FileName, zipFileName, ex.ToString()));
                    }
                }
            }

        }

        private static string GetMainProxyMockExe(IInstalationContext context)
        {
            var serviceInfo = context.EnvironmentServices.GetWindowsServiceInformationOrNull(MAIN_PROXY_MOCK_SERVICE_NAME);


            if (serviceInfo != null && !string.IsNullOrEmpty(serviceInfo.BinPath))
                return serviceInfo.BinPath;
                        
            return Path.Combine(context.Parameters.GGPMockFolder, "MainProxy", "MainProxyMock.exe");
            
        }
    }
}
