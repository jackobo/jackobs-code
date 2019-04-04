using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GGPMockBootstrapper.InstallationProgressFeedbackService;

namespace GGPMockBootstrapper.Models.GGP
{
    public class RunGGPInstallerAction : ProductInstallAction<GGPProduct>, IInstallationProgressFeedbackServiceCallback
    {
        public RunGGPInstallerAction(GGPProduct ggpProduct)
            : base(ggpProduct)
        {
            
        }

        #region IInstallAction Members


        public override string Description
        {
            get { return "Installing GGP Game Server"; }
        }

        const int NUMBER_OF_ACTIONS_BEFORE_EXECUTING_INSTALLER = 5;


        private Process RunningGGPInstaller;

        protected override void Install(IInstalationContext context, string tempZipFileName)
        {
            this.SubActionsCount = 1000;
            this.CurrentSubActionIndex = 0;

            var ggpInstallerTempFolder = Path.Combine(context.EnvironmentServices.FileSystem.GetTempFolder(), "GGPInstaller");

            IncrementSubactionIndex("Check if any other installer is running");
            KillAnyExistingInstallerProcess();

            if (context.EnvironmentServices.FileSystem.FolderExists(ggpInstallerTempFolder))
            {
                context.EnvironmentServices.FileSystem.DeleteFolder(ggpInstallerTempFolder);
            }

            IncrementSubactionIndex("Extracting installer files");

            context.EnvironmentServices.UnzipFile(tempZipFileName, ggpInstallerTempFolder);
            context.EnvironmentServices.UnzipFile(Product.GetEmbededResourceFullPath("GGPConfigs.zip"), ggpInstallerTempFolder);

            IncrementSubactionIndex("Setup command line parameters");

            CreateCommandLineFile(context, ggpInstallerTempFolder);

            IncrementSubactionIndex("Creating logging.xml file");

            WriteLoggingXml(context);

            var exeFileName = Path.GetFileName(Directory.EnumerateFiles(ggpInstallerTempFolder)
                                                        .First(file => Path.GetExtension(file) == ".exe" && file.Contains("Installer")));

            RunningGGPInstaller = StartGGPInstaller(ggpInstallerTempFolder, exeFileName);

            IncrementSubactionIndex("Subscribe to installer progress");
            SubscribeToInstallerProgress(context);

            IncrementSubactionIndex("Preparing the installer");

            SubscribeToApplicationShutdown(context);

            var exitOnTime = RunningGGPInstaller.WaitForExit(3 * 60 * 1000); //3 minutes

            UnsubscribeFromApplicationShutdown(context);

            try
            {
                if (!exitOnTime)
                {
                    throw new ApplicationException("Running GGP Installer timed out");
                }

                if (RunningGGPInstaller.ExitCode != 0)
                {
                    throw new ApplicationException(string.Format("GGP Installer failed! Exit code = {0}", RunningGGPInstaller.ExitCode));
                }
            }
            finally
            {
                RunningGGPInstaller = null;
            }
        }

        private Process StartGGPInstaller(string ggpInstallerTempFolder, string exeFileName)
        {
            var pstartInfo = new ProcessStartInfo(Path.Combine(ggpInstallerTempFolder, exeFileName));
            pstartInfo.Arguments = "\"" + RemoveTrailingBackslash(Path.Combine(ggpInstallerTempFolder, CommandLineFileName)) + "\"";
            pstartInfo.UseShellExecute = true;
            pstartInfo.Verb = "runas";
            return Process.Start(pstartInfo);
        }

        private static string RemoveTrailingBackslash(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            if (!path.EndsWith("\\"))
                return path;

            return path.Substring(0, path.Length - 1);
        }

        private void SubscribeToApplicationShutdown(IInstalationContext context)
        {
            context.EnvironmentServices.UI.GetCustomUIService<Models.IApplicationServices>().ShuttingDown += Application_ShuttingDownHandler;
        }

        private void UnsubscribeFromApplicationShutdown(IInstalationContext context)
        {
            context.EnvironmentServices.UI.GetCustomUIService<Models.IApplicationServices>().ShuttingDown -= Application_ShuttingDownHandler;
        }

        void Application_ShuttingDownHandler(object sender, EventArgs e)
        {
            KillProcess(this.RunningGGPInstaller);
        }

        private void KillAnyExistingInstallerProcess()
        {
            foreach (var process in Process.GetProcesses())
            {
                if (process.ProcessName.StartsWith("GGPGameServer.Installer."))
                {
                    KillProcess(process);
                }
            }
        }

        private static void KillProcess(Process process)
        {

            if (!process.HasExited)
            {
                process.Kill();
                process.WaitForExit(5000);
            }
        }

        InstallationProgressFeedbackService.InstallationProgressFeedbackServiceClient _feedbackProxy;
        private bool SubscribeToInstallerProgress(IInstalationContext context)
        {
            _feedbackProxy = new InstallationProgressFeedbackService.InstallationProgressFeedbackServiceClient(new System.ServiceModel.InstanceContext(this));

            int retryCount = 30;

            for (int i = 1; i <= retryCount; i++)
            {
                try
                {
                    _feedbackProxy.Subscribe();
                    return true;
                }
                catch(Exception ex)
                {
                    if (_feedbackProxy != null)
                    {
                        try
                        {
                            if (_feedbackProxy.State == System.ServiceModel.CommunicationState.Faulted)
                                _feedbackProxy.Abort();
                            else
                                _feedbackProxy.Close();
                        }
                        catch
                        {
                        }
                    }
                    _feedbackProxy = new InstallationProgressFeedbackService.InstallationProgressFeedbackServiceClient(new System.ServiceModel.InstanceContext(this));
                    if (i == retryCount)
                    {
                        context.Logger.Warning(ex.ToString());
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                    }
                }
            }

            return false;
            
        }

        private void WriteLoggingXml(IInstalationContext context)
        {
            
            var ggpLogsFolder = Path.Combine(context.Parameters.GGPMockFolder, "GGPLogs");

            var tempLoggingXml = Product.GetEmbededResourceFullPath("Logging.xml");

            var doc = XDocument.Load(tempLoggingXml);

            var rollingFileListener = doc.Root.Element("listeners").Elements("add").First(elem => elem.Attribute("name").Value == "Rolling File Listener");

            rollingFileListener.Attribute(@"fileName").Value = Path.Combine(ggpLogsFolder, "trace.log");

            if (!context.EnvironmentServices.FileSystem.FolderExists(ggpLogsFolder))
                context.EnvironmentServices.FileSystem.CreateFolder(ggpLogsFolder);

            var installationFolder = Product.GameServerInstallationPath(context);
            context.EnvironmentServices.FileSystem.CreateFolder(installationFolder);
            doc.Save(Path.Combine(installationFolder, "logging.xml"));

        }

        protected override string GetPackageName()
        {
            return "GGPInstaller.zip";   
        }

        private void CreateCommandLineFile(IInstalationContext context, string ggpInstallerTempFolder)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("IsContinuousDeploymentEnabled = false");
            sb.AppendLine("RegisterFeedbackService = true");
            sb.AppendLine("Products = GGP, HistoryHandlerService, RoundHistoryReaderService");
            sb.AppendLine("GGP.Action = Install");
            sb.AppendLine("HistoryHandlerService.Action = Skip");
            sb.AppendLine("RoundHistoryReaderService.Action = Skip");
            sb.AppendLine(string.Format("GGP.Path = {0}", Product.GameServerInstallationPath(context)));

            sb.AppendLine(string.Format("GGP.GameServerId = {0}", GetValueFromMachineConfig("gameServerId", "999")));
            sb.AppendLine(string.Format("GGP.PlatformServerId = {0}", GetValueFromMachineConfig("platformServerId", "999")));
            sb.AppendLine(string.Format("GGP.LogServerType = {0}", GetValueFromMachineConfig("logServerType", "5")));
            sb.AppendLine(string.Format("GGP.GGPMockEnabled = true"));
            sb.AppendLine(string.Format("GGP.StartTheServiceAfterUpload = true"));
            

            
                        
            context.EnvironmentServices.FileSystem.WriteAllText(Path.Combine(ggpInstallerTempFolder, CommandLineFileName), sb.ToString());

        }


        private string GetValueFromMachineConfig(string key, string defaultValue)
        {
            var value = System.Configuration.ConfigurationManager.AppSettings[key];

            if (string.IsNullOrEmpty(value))
                return defaultValue;

            return value;
        }

        
        private string CommandLineFileName
        {
            get { return "parameters.txt"; }
        }


        #endregion

        #region IInstallationProgressFeedbackServiceCallback Members

        public void BeginInstallation(BeginInstallationRequest request)
        {
            this.SubActionsCount = NUMBER_OF_ACTIONS_BEFORE_EXECUTING_INSTALLER + request.ActionsCount;
        }

        public void BeginExecuteAction(BeginExecuteActionRequest request)
        {
            this.SubActionsCount = NUMBER_OF_ACTIONS_BEFORE_EXECUTING_INSTALLER + request.ActionsCount;
            this.CurrentSubActionIndex = request.CurrentActionIndex;
            this.CurrentSubActionDescription = request.CurrentActionDescription;
        }

        public void EndExecuteAction(EndExecuteActionRequest request)
        {
            this.SubActionsCount = NUMBER_OF_ACTIONS_BEFORE_EXECUTING_INSTALLER + request.ActionsCount;
            this.CurrentSubActionDescription = request.CurrentActionDescription;
            this.CurrentSubActionIndex = request.CurrentActionIndex;
        }

        public void EndInstallation(EndInstallationRequest request)
        {
            if (request.Success)
            {
                this.SubActionsCount = 0;
                this.CurrentSubActionIndex = 0;
            }
           
        }

        

        #endregion
    }
}
