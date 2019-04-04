using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Logging;
using Spark.TfsExplorer.Interfaces;

namespace GGPInstallerBuilder
{
    public interface IInstallerBuildContext
    {
        IFileSystemAdapter FileSystemAdapter { get; }
        ILogger Logger { get; }
        ISourceControlAdapter SourceControlAdapter { get; }

        IBuildConfiguration BuildConfiguration { get; }
    }
}
