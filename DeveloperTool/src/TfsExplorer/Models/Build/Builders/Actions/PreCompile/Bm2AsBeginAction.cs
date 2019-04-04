using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Build
{
    public class Bm2AsBeginAction : IBuildAction
    {
        public void Execute(IBuildContext buildContext)
        {
            buildContext.Logger.Info($"Begin BM2AS");
            buildContext.ApprovalSystemAdapter.BeginBuild(buildContext.DeploymentContentBuilder.Build());
        }
    }
}
