using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Build
{
    public interface IBuildAction
    {
        void Execute(IBuildContext buildContext);
    }
}
