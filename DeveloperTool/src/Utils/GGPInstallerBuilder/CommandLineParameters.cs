using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Exceptions;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace GGPInstallerBuilder
{
    public class CommandLineParameters
    {
        private CommandLineParameters(DeployEnvironment environment, string branchName)
        {
            this.Environment = environment;
            this.BranchName = branchName;
        }
        public DeployEnvironment Environment { get; private set; }
        public string BranchName { get; private set; }

        public string InstallerDistributionPath { get; private set; }
        
        private static readonly string BRANCH_ARG = "branch";
        private static readonly string ENVIRONMENT_ARG = "environment";
        private static readonly string INSTALLER_DISTRIBUTION_PATH_ARG = "installerDistributionPath";

        private static string GetUsageInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("USAGE:");
            sb.AppendLine($"{ExeName} {ENVIRONMENT_ARG}=<{DeployEnvironment.QA} | {DeployEnvironment.Production}> {BRANCH_ARG}=<branchName> [{INSTALLER_DISTRIBUTION_PATH_ARG}=<Path>]");

            sb.AppendLine();
            sb.AppendLine("Example:");
            sb.AppendLine($"{ExeName} {ENVIRONMENT_ARG}={DeployEnvironment.QA} {BRANCH_ARG}=3.x");
            sb.AppendLine($"{ExeName} {ENVIRONMENT_ARG}={DeployEnvironment.Production} {BRANCH_ARG}=GIB");
            sb.AppendLine($"{ExeName} {ENVIRONMENT_ARG}={DeployEnvironment.QA} {BRANCH_ARG}=3.x \"{INSTALLER_DISTRIBUTION_PATH_ARG}=R:\\Gaming Releases\\GGP1\\GGPInstaller\"");
            return sb.ToString();
        }
        
        public static CommandLineParameters Parse(string[] args)
        {
            if (args.Length == 0)
                throw new InvalidCommandLineArgumnentsException(GetUsageInfo());

            var parser = new CommandLineParametersParser(args, GetUsageInfo());
            
            var environment = parser.GetMandatoryParameterValue<DeployEnvironment>(ENVIRONMENT_ARG); 
            var branchName = parser.GetMandatoryParameterValue<string>(BRANCH_ARG);



            var result = new CommandLineParameters(environment, branchName);

            parser.GetOptionalParameterValue<string>(INSTALLER_DISTRIBUTION_PATH_ARG).Do(val => result.InstallerDistributionPath = val);

            return result;
        }

        protected static string ExeName
        {
            get { return Path.GetFileName(Assembly.GetExecutingAssembly().Location); }
        }


    }

    
}
