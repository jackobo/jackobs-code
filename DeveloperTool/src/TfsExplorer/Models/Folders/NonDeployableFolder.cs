using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class NonDeployableFolder : ChildFolderWithBranchingSupport<NonDeployableFolder, ComponentsFolder>
    {
        public NonDeployableFolder(ComponentsFolder parent) 
            : base(WellKnownName, parent)
        {
        }

        public static readonly string WellKnownName = "NonDeployable";
    }
}
