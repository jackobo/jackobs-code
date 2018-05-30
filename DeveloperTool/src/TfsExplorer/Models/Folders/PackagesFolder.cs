using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class PackagesFolder : ChildFolderWithBranchingSupport<PackagesFolder, ComponentsFolder>
    {
        public PackagesFolder(ComponentsFolder parent) 
            : base(WellKnownName, parent)
        {
        }

        public static readonly string WellKnownName = "packages";

        public ComponentUniqueIdTxt<PackagesFolder> ComponentUniqueIdTxt
        {
            get { return new ComponentUniqueIdTxt<PackagesFolder>(this); }
        }
    }
}
