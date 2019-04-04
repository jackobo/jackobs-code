using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class FeatureFolder : BranchFolder<FeatureFolder, FeaturesFolder>
    {
        public FeatureFolder(string name, FeaturesFolder features)
            : base(name, features)
        {
        }
    }
}
