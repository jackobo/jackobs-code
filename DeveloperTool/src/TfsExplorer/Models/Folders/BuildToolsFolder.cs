using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class BuildToolsFolder : ChildFolderHolder<BuildToolsFolder, IBranchFolder>
    {
        public BuildToolsFolder(IBranchFolder parent) 
            : base("BuildTools", parent)
        {
        }
    }
}
