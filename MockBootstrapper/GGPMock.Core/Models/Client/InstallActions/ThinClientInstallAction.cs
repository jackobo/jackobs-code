using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Web.Administration;

namespace GGPMockBootstrapper.Models.Client
{
    public class ThinClientInstallAction : IISAppInstallActionBase<ClientProduct>
    {
        public ThinClientInstallAction(ClientProduct clientProduct)
            : base(clientProduct, AppName, Microsoft.Web.Administration.ManagedPipelineMode.Integrated)
        {
            
        }

        public static readonly string AppName = "ThinClientForMock";

        protected override string GetPackageName()
        {
            return "ThinClient.zip";
        }

        public override string Description
        {
            get
            {
                return "Install sample Flash games for GGPMock";
            }
        }

        protected override void ConfigureApplicationCore(IInstalationContext context, Microsoft.Web.Administration.ServerManager iisManager)
        {
            base.ConfigureApplicationCore(context, iisManager);

            ChangeWrapperConfiguration(context, iisManager);

            AddMimeTypes(iisManager, 
                         new MimeTypeDefinition(".xmm", "text/xml"),
                         new MimeTypeDefinition(".api", "application/octet-stream"),
                         new MimeTypeDefinition(".lig", "application/octet-stream"));
            
        }

       

        private void ChangeWrapperConfiguration(IInstalationContext context, Microsoft.Web.Administration.ServerManager iisManager)
        {
            string wrapperConfigurationXmmPath= Path.Combine(this.GetApplicationPhysicalPath(iisManager), "versionX", "wrapper", "WrapperConfiguration.xmm");

            XDocument xmlDoc = XDocument.Load(wrapperConfigurationXmmPath);

            var serverConnectionTypeElement = xmlDoc.Root.Element("serverConnectionType");

            var updateBalanceIntervalAttribute = serverConnectionTypeElement.Attribute("updateBalanceInterval");
            if (updateBalanceIntervalAttribute == null)
            {
                updateBalanceIntervalAttribute = new XAttribute("updateBalanceInterval", "0");
                serverConnectionTypeElement.Add(updateBalanceIntervalAttribute);
            }
            else
            {
                updateBalanceIntervalAttribute.Value = "0";
            }

            var intervalAttribute = serverConnectionTypeElement.Attribute("interval");
            if (intervalAttribute == null)
            {
                intervalAttribute = new XAttribute("interval", "5");
                serverConnectionTypeElement.Add(intervalAttribute);
            }
            else
            {
                intervalAttribute.Value = "5";
            }


            var serverElement = serverConnectionTypeElement.Elements("server").First();

            serverElement.SetAttributeValue("name", "localhost");
            serverElement.SetAttributeValue("socket", "710");
            serverElement.SetAttributeValue("ssl", "false");


            


            xmlDoc.Save(wrapperConfigurationXmmPath);

        }


        public override void Execute(IInstalationContext context)
        {
            string settingsXmlFor888GamesBackupFile = BackupSettingsXml(context, "888Games");
            string settingsXmlFor888BingoBackupFile = BackupSettingsXml(context, "888Bingo");
            base.Execute(context);

            RestoreSettingsXml(context, settingsXmlFor888GamesBackupFile, "888Games");
            RestoreSettingsXml(context, settingsXmlFor888BingoBackupFile, "888Bingo");
        }

        private string BackupSettingsXml(IInstalationContext context, string operatorName)
        {
            using (ServerManager iisManager = new ServerManager())
            {

                var appPath = this.GetApplicationPhysicalPath(iisManager);
                if (string.IsNullOrEmpty(appPath))
                    return null;

                string settingsFile = Path.Combine(appPath, "versionX", "brand", operatorName, "settings.xml");
                
                if(!context.EnvironmentServices.FileSystem.FileExists(settingsFile))
                    return string.Empty;


                string backupFile = Path.Combine(this.GetApplicationPhysicalPath(iisManager), "versionX", "brand", operatorName, "backup_settings.xml");
                context.EnvironmentServices.FileSystem.CopyFile(settingsFile, backupFile);

                return backupFile;
                
            }
        }

        private void RestoreSettingsXml(IInstalationContext context, string backupFile, string operatorName)
        {
            if (string.IsNullOrEmpty(backupFile))
                return;

            using (ServerManager iisManager = new ServerManager())
            {
                if (string.IsNullOrEmpty(backupFile))
                    return;

                context.EnvironmentServices.FileSystem.CopyFile(backupFile,
                                                                Path.Combine(this.GetApplicationPhysicalPath(iisManager), "versionX", "brand", operatorName, "settings.xml"));

                context.EnvironmentServices.FileSystem.DeleteFile(backupFile);
            }
        }

        /*
        private void CopyBingoThinClientTo888(IInstalationContext context)
        {
            using (ServerManager iisManager = new ServerManager())
            {
                var brandBingoPath = Path.Combine(this.GetApplicationPhysicalPath(iisManager), "versionX", "brand", "888Bingo");
                var brand888Path = Path.Combine(this.GetApplicationPhysicalPath(iisManager), "versionX", "brand", "888Games");
                var brand888BackupFolder = Path.Combine(brand888Path, "backup");


                context.EnvironmentServices.FileSystem.CreateFolder(brand888BackupFolder);

                var thinClientSwf = "ThinClient.swf";
                var thinClientDebugSwf = "ThinClientDebug.swf";

                context.EnvironmentServices.FileSystem.CopyFile(Path.Combine(brand888Path, thinClientSwf), Path.Combine(brand888BackupFolder, thinClientSwf));
                context.EnvironmentServices.FileSystem.CopyFile(Path.Combine(brand888Path, thinClientDebugSwf), Path.Combine(brand888BackupFolder, thinClientDebugSwf));

                context.EnvironmentServices.FileSystem.CopyFile(Path.Combine(brandBingoPath, thinClientSwf), Path.Combine(brand888Path, thinClientSwf));
                context.EnvironmentServices.FileSystem.CopyFile(Path.Combine(brandBingoPath, thinClientDebugSwf), Path.Combine(brand888Path, thinClientDebugSwf));


            }
         
        }
        */

    }
}
