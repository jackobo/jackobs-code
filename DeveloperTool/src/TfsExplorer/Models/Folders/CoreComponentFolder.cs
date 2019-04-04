using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Folders
{
    public class CoreComponentFolder : ChildFolderWithBranchingSupport<CoreComponentFolder, CoreFolder>
    {
        public CoreComponentFolder(string name, CoreFolder parent)
            : base(name, parent)
        {
        }

        public static class WellKnownNames
        {
            public static readonly string GGPBootstrapper = "GGPBootstrapper";
        }

        public ComponentUniqueIdTxt<CoreComponentFolder> ComponentUniqueIdTxt
        {
            get { return new ComponentUniqueIdTxt<CoreComponentFolder>(this); }
        }
    }
}
