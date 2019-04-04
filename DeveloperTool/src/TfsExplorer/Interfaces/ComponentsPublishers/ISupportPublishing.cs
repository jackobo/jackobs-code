using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Interfaces
{
    public interface ISupportPublishing
    {
        IEnumerable<IComponentPublisher> GetPublishers();
        IEnumerable<IComponentPublisher> GetPublishers(IChangeSet sinceThisChangeSet);
    }
}
