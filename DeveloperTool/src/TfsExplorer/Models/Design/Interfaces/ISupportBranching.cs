using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.Design
{
    
    public interface ISupportBranching
    {
        void Branch(Folders.ComponentsFolder targetComponentsFolder);
        Optional<IMergeSet> GetMergeSet(Folders.ComponentsFolder targetComponentsFolder);
        MergeResult Merge(ComponentsFolder targetComponentsFolder);
    }
}
