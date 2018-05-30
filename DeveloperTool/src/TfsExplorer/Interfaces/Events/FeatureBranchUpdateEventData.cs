using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Interfaces.Events
{
    public class FeatureBranchUpdateEventData
    {
        public FeatureBranchUpdateEventData(IFeatureBranch feature)
        {
            Feature = feature;
        }

        public IFeatureBranch Feature { get; private set; }
    }
}
