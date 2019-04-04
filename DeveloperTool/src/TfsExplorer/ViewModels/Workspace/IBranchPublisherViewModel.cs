using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.ViewModels.Workspace
{
    public interface IBranchPublisherViewModel
    {
        bool AllowCustomizedInstallerGeneration { get; }
        bool IsPublishInProgress { get; }
        void StartPublishing();
        IEnumerable<IComponentPublisher> GetPublishableComponents();
        void Publish(IPublishPayload publishPayload);
    }
}
