using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Logging;

namespace GGPInstallerBuilder
{
    public interface IInstallerBuildServices
    {
        ILoggerFactory LoggerFactory { get; }
        ISourceControlAdapter SourceControlAdapter { get; }
        IInstallerDefinitionReader InstallerDefinitionReader { get; }
        IFileSystemAdapter FileSystemAdapter { get; }
        
    }
}
