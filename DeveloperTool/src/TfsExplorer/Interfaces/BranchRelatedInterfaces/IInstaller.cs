using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.Interfaces
{
    public interface IInstaller
    {
        VersionNumber Version { get; }
        string GetDescription();
        bool IsBranched();
        void CreateBranch(Action<ProgressCallbackData> progressCallback = null);
        IEnumerable<ILogicalComponent> GetComponents();
        IEnumerable<IComponentPublisher> GetHotfixPublishers();
        void Publish(IPublishPayload publishPayload);
        bool IsPublishInProgress();
    }

    public interface IProductionInstaller : IInstaller
    {
        bool IsOwnedBy(IProductionEnvironment environment);
    }

    public interface IQAInstaller : IInstaller
    {
        
        
    }
}
