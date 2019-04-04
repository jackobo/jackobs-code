using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Interfaces.Events
{
    public class NewFeatureBranchEventData 
    {
        public NewFeatureBranchEventData(IFeatureBranch feature, IMainBranch owner)
        {
            this.Feature = feature;
            this.Owner = owner;
        }

        public IFeatureBranch Feature { get; private set; }
        public IMainBranch Owner { get; private set; }
    }
}
