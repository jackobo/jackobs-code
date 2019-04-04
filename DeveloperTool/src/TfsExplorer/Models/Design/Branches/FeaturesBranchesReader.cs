using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Design
{
    public class FeaturesBranchesReader : IFeaturesBranchesReader
    {
        public FeaturesBranchesReader(ILogicalBranchComponentFactory componentsFactory)
        {
            _componentsFactory = componentsFactory;
        }

        ILogicalBranchComponentFactory _componentsFactory;

        public IEnumerable<IFeatureBranch> GetFeatureBranches(Folders.EnvironmentFolder environmentSpecificFolder, IMainBranch owner)
        {
            var featuresBranches = new List<IFeatureBranch>();

            foreach(var featureFolder in environmentSpecificFolder.Features.Features())
            {
                featuresBranches.Add(_componentsFactory.CreateFeatureBranch(featureFolder, owner));
            }

            return featuresBranches;
        }
    }
}
