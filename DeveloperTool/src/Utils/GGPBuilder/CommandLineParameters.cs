using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Exceptions;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Build;

namespace GGPBuilder
{

   

    public class CommandLineParameters 
    {
      
        private CommandLineParameters(RootBranchVersion branchName, BuildType buildType)
        {
            this.BranchName = branchName;
            this.BuildType = buildType;
        }
        
        private static readonly string BRANCH_ARG = "branch";
        private static readonly string BUILD_TYPE_ARG = "buildType";
        
        private static string GetUsageInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("USAGE:");
            var buildTypes = string.Join(" | ", Enum.GetNames(typeof(BuildType)));
            sb.AppendLine($"{ExeName} {BRANCH_ARG}=<branchName> {BUILD_TYPE_ARG}=<{buildTypes}>");

            sb.AppendLine();
            sb.AppendLine("Example:");
            sb.AppendLine($"{ExeName} {BRANCH_ARG}=3.x");
            return sb.ToString();
        }

        protected static string ExeName
        {
            get { return Path.GetFileName(Assembly.GetExecutingAssembly().Location); }
        }

        public RootBranchVersion BranchName { get; private set; }
        public BuildType BuildType { get; private set; }
        
        public static CommandLineParameters Parse(string[] args)
        {
            if (args.Length == 0)
                throw new InvalidCommandLineArgumnentsException(GetUsageInfo());

            var parser = new CommandLineParametersParser(args, GetUsageInfo());

            var branchName = RootBranchVersion.Parse(parser.GetMandatoryParameterValue<string>(BRANCH_ARG));
            var buildType = parser.GetMandatoryParameterValue<BuildType>(BUILD_TYPE_ARG);
            
            return new CommandLineParameters(branchName, buildType);
        }
        
        
    }
    
}
