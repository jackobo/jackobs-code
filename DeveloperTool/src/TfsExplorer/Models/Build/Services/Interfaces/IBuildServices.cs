using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Logging;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.Build
{
    public interface IBuildServices
    {
        ILoggerFactory LoggerFactory { get; }
        IOperatingSystemServices OperatingSystemServices { get; }
        IPublishPayloadSerializer CreatePublishPayloadSerializer();
        IComponentsBuildersReader CreateComponentsBuildersReader();
        TFS.ITfsGateway TfsGateway { get; }
        IApprovalSystemAdapter CreateApprovalSystemAdapter();

        ISourceControlAdapter CreateSourceControlAdapter();
    }
}
