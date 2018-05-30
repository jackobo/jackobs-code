using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;
using Spark.Infra.Exceptions;
using Spark.Infra.Logging;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models;
using Spark.TfsExplorer.Models.Build;
using Spark.TfsExplorer.Models.Folders;
using Spark.TfsExplorer.Models.TFS;

namespace GGPBuilder
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
                
                string distributionFolder = ConfigurationManager.AppSettings["distributionFolder"];
                
                var buildServices = GetBuildServices();
              
                var ggpBuilder = new GGPSolutionBuilder(cliParams.BranchName,
                                                        cliParams.BuildType,
                                                        buildServices,
                                                        distributionFolder);
                ggpBuilder.Build();

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
            catch (Exception ex)
            {
                Logger.Exception(ex);
                Logger.Error("BUILD FAILED!");
                ExitWithError(ex);
            }

        }

        private static void ExitWithError(Exception ex)
        {
            if (ex.HResult == 0)
                Environment.Exit(1);
            else
                Environment.Exit(ex.HResult);
        }

        private static IBuildServices GetBuildServices()
        {
           return new BuildServices(LoggerFactory, new AutomaticWorkspaceSelector(TfsGateway.ROOT_FOLDER));   
        }
    }
}
