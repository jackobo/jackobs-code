using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GGPGameServer.Simulations.MongoDB;


namespace GGPMockBootstrapper.Models.GGPSimulator
{
    public class GGPSimulatorServiceInstallAction : ProductInstallAction<GGPSimulatorProduct>
    {

        public GGPSimulatorServiceInstallAction(GGPSimulatorProduct ggpSimulatorProduct)
            : base(ggpSimulatorProduct)
        {
            
        }
        public override string Description
        {
            get 
            {
                return "Install GGP Simulator Service";
            }
        }


        protected override string GetPackageName()
        {
            return "Server.zip";
        }



        protected override void Install(GGPMockBootstrapper.Models.IInstalationContext context, string zipFileName)
        {
            this.SubActionsCount = 8;

            if (!context.EnvironmentServices.IsX64OperatingSystem)
                this.SubActionsCount++;

            string simulatorServiceFolder = GGPSimulatorServiceFolder(context);

            IncrementSubactionIndex(string.Format("Stopping {0} service", SIMULATOR_SERVICE_NAME));
            context.EnvironmentServices.StopService(SIMULATOR_SERVICE_NAME);


            IncrementSubactionIndex("Stopping MongoDB service");
            var mongoWrapper = new MongoWrapper(simulatorServiceFolder);
            mongoWrapper.StopService();

            IncrementSubactionIndex("Extracting simulator service content");
            ExtractServerContent(context, simulatorServiceFolder, zipFileName);

            if (!context.EnvironmentServices.IsX64OperatingSystem)
            {
                IncrementSubactionIndex("Extract MongoDB x86 version");
                ExtractMongoDbX86(context, simulatorServiceFolder);
            }
            
            IncrementSubactionIndex("Installing MongoDB service");


            InstallMongoDB(mongoWrapper, context, simulatorServiceFolder);

            
    
            var simulatorServiceExeFullPath = Path.Combine(simulatorServiceFolder, SIMULATOR_EXE_NAME);

            IncrementSubactionIndex(string.Format("Creating {0} service", SIMULATOR_SERVICE_NAME));

            context.EnvironmentServices.CreateService(SIMULATOR_SERVICE_NAME,
                                                      simulatorServiceExeFullPath, 
                                                      null,
                                                      new MongoInfo(simulatorServiceFolder).ServiceName);

            IncrementSubactionIndex(string.Format("Add firewall exception for {0} service", SIMULATOR_SERVICE_NAME));
            NetshAddAllowedProgram(context, simulatorServiceExeFullPath);

            IncrementSubactionIndex(string.Format("Creating {0} service configuration file", SIMULATOR_SERVICE_NAME));

            context.EnvironmentServices.FileSystem.CopyFile(GetEmbededAppConfigFile(), GetDestinationAppConfig(simulatorServiceFolder));


            IncrementSubactionIndex(string.Format("Starting {0} service", SIMULATOR_SERVICE_NAME));
            context.EnvironmentServices.StartService(SIMULATOR_SERVICE_NAME, new string[0], false);

        }

        private void ExtractMongoDbX86(IInstalationContext context, string simulatorServiceFolder)
        {
            var tempZipFileName = context.EnvironmentServices.FileSystem.GetTempFileName();
            context.EnvironmentServices.FileSystem.CopyFile(Product.GetEmbededResourceFullPath("MongoX86.zip"), tempZipFileName);
            context.EnvironmentServices.UnzipFile(tempZipFileName, Path.Combine(simulatorServiceFolder, "MongoDB", "bin"));

            context.EnvironmentServices.FileSystem.DeleteFile(tempZipFileName);
            
        }



        private static string GetDestinationAppConfig(string simulatorServiceFolder)
        {
            return Path.Combine(simulatorServiceFolder, "GGPSimulatorService.exe.config");
        }

        private string GetEmbededAppConfigFile()
        {
            return Product.GetEmbededResourceFullPath("GGPSimulatorService.exe.config");
        }

        private void InstallMongoDB(MongoWrapper mongoDBWrapper, IInstalationContext context, string simulatorServiceFolder)
        {
            
            mongoDBWrapper.InstallService();

            
            context.EnvironmentServices.UnzipFile(Product.GetEmbededResourceFullPath("data.zip"), mongoDBWrapper.MongoInfo.DataFolder);
            

            NetshAddAllowedProgram(context, mongoDBWrapper.MongoInfo.MongodExe);
            NetshAddAllowedProgram(context, mongoDBWrapper.MongoInfo.MongoExe);
            
            mongoDBWrapper.StartService();
            
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

        private void ExtractServerContent(IInstalationContext context, string simulatorServiceFolder, string zipFileName)
        {
            
            using (Ionic.Zip.ZipFile zipFile = Ionic.Zip.ZipFile.Read(zipFileName))
            {
                
                foreach (var entry in zipFile)
                {
                    try
                    {
                        entry.ExtractExistingFile = Ionic.Zip.ExtractExistingFileAction.OverwriteSilently;
                        entry.Extract(simulatorServiceFolder);
                    }
                    catch(Exception ex)
                    {
                        context.Logger.Warning(string.Format("GGPSimulatorServiceInstallAction.ExtractServerContent: Failed to extract '{0}' from '{1}'! Exception details: {2}", entry.FileName, zipFileName, ex.ToString()));
                    }
                }
            }



        }

      

        void zipFile_ExtractProgress(object sender, Ionic.Zip.ExtractProgressEventArgs e)
        {
            if (e.CurrentEntry != null && (e.CurrentEntry.FileName.Contains("mongo.exe") || e.CurrentEntry.FileName.Contains("mongod.exe")))
                e.CurrentEntry.ExtractExistingFile = Ionic.Zip.ExtractExistingFileAction.OverwriteSilently;
            
        }

        void zipFile_ReadProgress(object sender, Ionic.Zip.ReadProgressEventArgs e)
        {
            if (e.CurrentEntry != null && (e.CurrentEntry.FileName == "mongo.exe" || e.CurrentEntry.FileName == "mongod.exe"))
                e.CurrentEntry.ExtractExistingFile = Ionic.Zip.ExtractExistingFileAction.DoNotOverwrite;
        }

        void zipFile_ZipError(object sender, Ionic.Zip.ZipErrorEventArgs e)
        {
            
        }   
        
        private static string GGPSimulatorServiceFolder(IInstalationContext context)
        {
            var serviceInfo = context.EnvironmentServices.GetWindowsServiceInformationOrNull(SIMULATOR_SERVICE_NAME);


            string servicePath = null;
            if (serviceInfo != null && !string.IsNullOrEmpty(serviceInfo.BinPath))
               servicePath = Path.GetDirectoryName(serviceInfo.BinPath);

            if (string.IsNullOrEmpty(servicePath))
                return Path.Combine(context.Parameters.GGPMockFolder, "GGPSimulatorService");
            else
                return servicePath;
        }

        private static readonly string SIMULATOR_SERVICE_NAME = "GGPSimulator";
        private static readonly string SIMULATOR_EXE_NAME = "GGPSimulatorService.exe";


        protected override bool ShouldForceInstall(IInstalationContext context)
        {
            var simulatorServiceFolder = GGPSimulatorServiceFolder(context);
            if (!File.Exists(GetDestinationAppConfig(simulatorServiceFolder)))
                return true;


            //if (!context.EnvironmentServices.FileSystem.FilesAreEquals(GetEmbededAppConfigFile(), GetDestinationAppConfig(simulatorServiceFolder)))
            //    return true;


            return base.ShouldForceInstall(context);
            
            
        }



      
    }
}
