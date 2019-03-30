using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Build
{
    public class AfterCompile : IBuildAction
    {
        public void Execute(IBuildContext buildContext)
        {
            PrepareFiles.RollbackAllFiles();
        }
    }
}
