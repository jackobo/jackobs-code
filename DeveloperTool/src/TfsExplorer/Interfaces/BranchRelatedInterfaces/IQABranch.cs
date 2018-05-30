using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.Interfaces
{
    public interface IQaBranch : IMainBranch
    {
        

        IEnumerable<IMergeSet> GetMergeSetsToDev();
        IEnumerable<IComponentPublisher> GetComponentsToPublish();
        bool IsPublishInProgress();
        void Publish(IPublishPayload payload);
        IEnumerable<IQAInstaller> GetInstallers();
    }

    
}
