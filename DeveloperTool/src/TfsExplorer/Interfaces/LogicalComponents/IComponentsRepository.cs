using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.Interfaces
{
    public interface IComponentsRepository
    {
        IEnumerable<IRootBranch> GetRootBranches();
        IEnumerable<ISourceControlWorkItem> GetRelatedWorkItems(IEnumerable<IChangeSet> changeSet);
    }

    
    
}
