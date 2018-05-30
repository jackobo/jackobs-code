using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Logging;
using Spark.Infra.Types;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;
using Spark.TfsExplorer.Models.Publish;

namespace Spark.TfsExplorer.Models.Build
{
    public class BuildServices : IBuildServices
    {
        public BuildServices(ILoggerFactory loggerFactory, 
                             IWorkspaceSelector workspaceSelector)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            if (workspaceSelector == null)
                throw new ArgumentNullException(nameof(workspaceSelector));

            this.LoggerFactory = loggerFactory;
            this.OperatingSystemServices = new OperatingSystemServices(loggerFactory);
            this.TfsGateway = new TFS.TfsGateway(this.OperatingSystemServices.FileSystem, workspaceSelector, this.OperatingSystemServices.ThreadingServices);
        }

        
        public IComponentsBuildersReader CreateComponentsBuildersReader()
        {
            return new ComponentsBuildersReader();
        }


        public TFS.ITfsGateway TfsGateway { get; private set; }

        public ILoggerFactory LoggerFactory { get; private set; }

        public IOperatingSystemServices OperatingSystemServices { get; private set; }

        public IPublishPayloadSerializer CreatePublishPayloadSerializer()
        {
            return new PublishPayloadXmlSerializer();
        }
        
        public IApprovalSystemAdapter CreateApprovalSystemAdapter()
        {
            return new ApprovalSystemAdapter();
        }

        public ISourceControlAdapter CreateSourceControlAdapter()
        {
            if (System.Configuration.ConfigurationManager.AppSettings["useMockTfsAdapter"] == "true")
                return new MockSourceControlAdapter();
            else
                return new TfsSourceControlAdapter(this.TfsGateway);
        }
    }
}
