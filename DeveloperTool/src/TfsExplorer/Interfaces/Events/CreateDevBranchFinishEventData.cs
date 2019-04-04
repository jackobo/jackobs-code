using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Interfaces.Events
{
    public class CreateDevBranchFinishEventData
    {
        public CreateDevBranchFinishEventData(IRootBranch logicalBranch)
        {
            this.LogicalBranch = logicalBranch;
        }
        
        public IRootBranch LogicalBranch { get; private set; }
    } 
}
