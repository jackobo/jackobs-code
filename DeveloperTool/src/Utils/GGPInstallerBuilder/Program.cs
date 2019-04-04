using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Exceptions;
using Spark.Infra.Logging;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models;
using Spark.TfsExplorer.Models.TFS;

namespace GGPInstallerBuilder
{
    class Program
    {
        static ILoggerFactory LoggerFactory { get; set; }
        static ILogger Logger
        {
            get { return LoggerFactory.CreateLogger(typeof(Program)); }
        }

        static void Main(string[] args)
        {
            

            LoggerFactory = Log4NetNotifierFactory.FromConfigurationFileInApplicationFolder();
            try
            {
                var cliParams = CommandLineParameters.Parse(args);

                var argsDescription = string.Join("; ", args);
                Logger.Info($"BEGIN BUILD {argsDescription}");

                string installerDefinitionServerPath = ConfigurationManager.AppSettings[ConfigurationKeys.installerDefinitionServerPath];
                string ggpApprovalSystemSourceCodeServerPath = ConfigurationManager.AppSettings[ConfigurationKeys.ggpApprovalSystemSourceCodeServerPath];
                string installerDistributionPath = GetInstallerDistributionPath(cliParams);



                var builder = new InstallerBuilder(GetServices());
                builder.Build(new BuildTaskInfo(cliParams.BranchName, 
                                                cliParams.Environment, 
                                                installerDefinitionServerPath),
                              ggpApprovalSystemSourceCodeServerPath,
                              installerDistributionPath);

                Logger.Info("BUILD SUCCESSFULL!");
            }
            catch(InvalidCommandLineArgumnentsException ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                {
                    if (ex.InnerException != null)
                        Logger.Exception(ex);
                    else
                        Logger.Error(ex.Message);
                }

                Logger.Info(ex.UsageInfo);
                ExitWithError(ex);
            }
            catch(Exception ex)
            {
                Logger.Exception(ex);
                Logger.Error("BUILD FAILED!");
                ExitWithError(ex);
            }

        }

        private static string GetInstallerDistributionPath(CommandLineParameters cliParams)
        {
            if (!string.IsNullOrEmpty(cliParams.InstallerDistributionPath))
                return cliParams.InstallerDistributionPath;

            return ConfigurationManager.AppSettings[ConfigurationKeys.installerDistributionPath];
        }


        private static void ExitWithError(Exception ex)
        {
            if (ex.HResult == 0)
                Environment.Exit(1);
            else
                Environment.Exit(ex.HResult);
        }


        private static IInstallerBuildServices GetServices()
        {
            if (System.Configuration.ConfigurationManager.AppSettings["useMockServices"] == "true")
                return new Mocks.MockInstallerBuilderServices(LoggerFactory);
            else
                return new InstallerBuilderServices(LoggerFactory, new AutomaticWorkspaceSelector(ConfigurationManager.AppSettings[ConfigurationKeys.ggpApprovalSystemSourceCodeServerPath]));

        }
    }
}
