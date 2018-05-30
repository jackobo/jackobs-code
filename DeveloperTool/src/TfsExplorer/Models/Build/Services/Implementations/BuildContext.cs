using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Logging;
using Spark.Infra.Types;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public class BuildContext : IBuildContext
    {
        public BuildContext(IBuildServices services, 
                            IBuildConfiguration buildConfiguration, 
                            IDeploymentContentBuilder deploymentContentBuilder,
                            ISourceControlAdapter sourceControlAdapter)
        {
            this.Services = services;
            this.Logger = services.LoggerFactory.CreateLogger(this.GetType());
            this.BuildConfiguration = buildConfiguration;
            this.DeploymentContentBuilder = deploymentContentBuilder;
            this.ApprovalSystemAdapter = services.CreateApprovalSystemAdapter();
            this.SourceControlAdapter = sourceControlAdapter;
            this.FileSystemAdapter = new FileSystemAdapter(services.OperatingSystemServices.FileSystem);
        }


        IBuildServices Services { get; set; }

        public ILogger Logger { get; private set; }

        public IFileSystemAdapter FileSystemAdapter { get; private set; }

        public IDeploymentContentBuilder DeploymentContentBuilder { get; private set; }
        
        public IBuildConfiguration BuildConfiguration { get; private set; }

        public IApprovalSystemAdapter ApprovalSystemAdapter { get; private set; }

        public ISourceControlAdapter SourceControlAdapter { get; private set; }


    }
}
