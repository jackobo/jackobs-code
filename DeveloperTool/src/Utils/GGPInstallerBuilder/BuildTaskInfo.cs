using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace GGPInstallerBuilder
{
    public class BuildTaskInfo
    {
        public BuildTaskInfo(string branchName, DeployEnvironment environment, string installerContentRootServerPath)
        {
            this.BranchName = branchName;
            this.Environment = environment;
            this.InstallerContentRootServerPath = installerContentRootServerPath;
        }

        public string BranchName { get; private set; }
        public DeployEnvironment Environment { get; private set; }
        public string InstallerContentRootServerPath { get; private set; }
    }
}
