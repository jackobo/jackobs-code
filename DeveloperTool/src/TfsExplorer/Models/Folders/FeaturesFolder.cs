using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class FeaturesFolder : ChildFolderHolder<FeaturesFolder, EnvironmentFolder>
    {
        public FeaturesFolder(EnvironmentFolder environment)
            : base("Features", environment)
        {
        }

        public FeatureFolder Feature(string name)
        {
            return new FeatureFolder(name, this);
        }

        public IEnumerable<FeatureFolder> Features()
        {

            if (!Exists())
                return new FeatureFolder[0];


            return ToSourceControlFolder()
                    .GetSubfolders()
                    .Select(f => Feature(f.Name))
                   .ToList();
        }
    }
}
