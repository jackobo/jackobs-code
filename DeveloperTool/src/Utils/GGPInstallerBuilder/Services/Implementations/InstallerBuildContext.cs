using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Logging;
using Spark.Infra.Windows;

namespace GGPInstallerBuilder
{
    public class InstallerBuildContext : IInstallerBuildContext
    {
        public InstallerBuildContext(ILoggerFactory loggerFactory, 
                                    IFileSystemAdapter fileSystemAdapter,
                                    ISourceControlAdapter sourceControlAdapter,
                                    IBuildConfiguration buildConfiguration)
        {
            this.Logger = loggerFactory.CreateLogger(this.GetType());
            this.FileSystemAdapter = fileSystemAdapter;
            this.SourceControlAdapter = sourceControlAdapter;
            this.BuildConfiguration = buildConfiguration;

        }
        
        public IBuildConfiguration BuildConfiguration { get; private set; }
        

        public IFileSystemAdapter FileSystemAdapter { get; private set; }

        public ILogger Logger { get; private set; }

        public ISourceControlAdapter SourceControlAdapter { get; private set; }
    }
}
