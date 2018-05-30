using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Design
{
    public interface IMainDevBranchBuilder
    {
        
        void Build(Folders.RootBranchFolder logicalBranchFolder, IEnumerable<ILogicalComponent> components, Action<ProgressCallbackData> progressCallback =  null);
        
        bool CanBuild(Folders.RootBranchFolder logicalBranchFolder);
    }
}
