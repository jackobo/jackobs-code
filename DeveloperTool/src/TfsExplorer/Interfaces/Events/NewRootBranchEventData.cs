using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Interfaces.Events
{
    public class NewRootBranchEventData
    {
        public NewRootBranchEventData(IRootBranch rootBranch)
        {
            this.RootBranch = rootBranch;
        }

        public IRootBranch RootBranch { get; private set; }
    }
}
