using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.Interfaces
{
    public interface IMergeSet
    {
        bool IsNew { get; }
        ILogicalComponent SourceComponent { get; }
        IEnumerable<IMergeableChangeSet> ChangeSets { get; }

        MergeResult Merge();

    }

    public interface IChangeSet
    {
        int Id { get; }
        DateTime Date { get; }
        string CommiterDisplayName { get; }
        string Comments { get; }
        string CommiterUserName { get; }
    }

    public interface IMergeableChangeSet : IChangeSet
    {
     
        MergeResult Merge();
        
    }

    public interface ISourceControlWorkItem
    {
        int Id { get; }
        string Title { get; }
        string AssignedTo { get; }
    }
}
