using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Build
{
    public class Bm2AsEndAction : IBuildAction
    {
        public void Execute(IBuildContext buildContext)
        {
            
            buildContext.Logger.Info($"End BM2AS");

            buildContext.ApprovalSystemAdapter.EndBuild(buildContext.SourceControlAdapter.GetChangeSetId());
        }
    }
}
