using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public abstract class EnvironmentFolder : ChildFolderHolder<EnvironmentFolder, RootBranchFolder>
    {
        public EnvironmentFolder(string name, RootBranchFolder logicalBranchFolder)
            : base(name, logicalBranchFolder)
        {
        }

      

        public FeaturesFolder Features
        {
            get { return new FeaturesFolder(this); }
        }

        public abstract IBranchFolder GetMain();
    }
    
}
