using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Interfaces
{
    public interface IMainBranch
    {
        IEnumerable<ILogicalComponent> GetComponents();
        void CreateFeatureBranch(string name, IEnumerable<ILogicalComponent> components, Action<ProgressCallbackData> progressCallback);
        IEnumerable<IFeatureBranch> GetFeatureBranches();
        IEnumerable<ILogicalComponent> ScanForSimilarComponents(ILogicalComponent component);
        void RenameComponents(IEnumerable<ILogicalComponent> sameComponents, string newName);
        void DeleteComponents(IEnumerable<ILogicalComponent> sameComponents);
    }

    
}
