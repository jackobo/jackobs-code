using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Interfaces
{
    public interface IFeatureBranch
    {
        string Name { get; }
        IEnumerable<ILogicalComponent> GetComponents();
        IEnumerable<ILogicalComponent> GetMissingComponentsFromMain();
        IEnumerable<IMergeSet> GetMergeSetsToMain();
        IEnumerable<IMergeSet> GetMergeSetsFromMain();
        void AddMissingComponents(ILogicalComponent[] components, Action<ProgressCallbackData> progressCallBack);
    }
}
