using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Folders
{
    public interface IFolderWithBranchingSupport : IFolderHolder
    {
        void Branch(IFolderWithBranchingSupport targetFolder);
        IEnumerable<IMergeableChangeSet> GetMergeChangeSets(IFolderWithBranchingSupport targetFolder);
        MergeResult Merge(IFolderWithBranchingSupport targetFolder);
    }
}
