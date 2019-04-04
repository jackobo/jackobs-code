using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class DevFolder : EnvironmentFolder
    {
        public DevFolder(RootBranchFolder logicalBranchFolder)
            : base("DEV", logicalBranchFolder)
        {

        }

        public DevMainFolder Main
        {
            get { return new DevMainFolder(this); }
        }

        public override IBranchFolder GetMain()
        {
            return this.Main;
        }
    }
}
