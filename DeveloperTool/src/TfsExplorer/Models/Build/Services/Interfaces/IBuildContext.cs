using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Logging;
using Spark.Infra.Types;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public interface IBuildContext
    {
        ILogger Logger { get; }
        IFileSystemAdapter FileSystemAdapter { get; }
        
        IDeploymentContentBuilder DeploymentContentBuilder { get; }

        IBuildConfiguration BuildConfiguration { get; }

        IApprovalSystemAdapter ApprovalSystemAdapter { get; }
        ISourceControlAdapter SourceControlAdapter { get; }
    }
        

    public interface IBuildConfiguration
    {
        ILocalPath OutputFolder { get; }
        ILocalPath SolutionFile { get; }
        ILocalPath DistributionLocalPath { get; }
        IServerPath DistributionServerPath { get; }
        IDictionary<string, string> GlobalProperties { get; }
        
    }
    
    public class BuildConfiguration : IBuildConfiguration
    {
        public BuildConfiguration(ILocalPath outputFolder, ILocalPath distributionLocalFolder, IServerPath distributionServerPath, ILocalPath solutionFile)
        {
            this.OutputFolder = outputFolder;
            this.DistributionLocalPath = distributionLocalFolder;
            this.DistributionServerPath = distributionServerPath;
            this.SolutionFile = solutionFile;
        }

        public ILocalPath OutputFolder { get; private set; }

        public ILocalPath DistributionLocalPath { get; private set; }

        public IServerPath DistributionServerPath { get; private set; }

        public ILocalPath SolutionFile { get; private set; }

        public IDictionary<string, string> GlobalProperties
        {
            get
            {
                var buildProperties = new Dictionary<string, string>();
                buildProperties.Add("Configuration", "Release");
                return buildProperties;
            }
        }
    }
}
