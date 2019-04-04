using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;

namespace Spark.TfsExplorer.ViewModels.Workspace
{
    public class DevFeaturesBranchesWorkspaceItem : FeaturesBranchesWorkspaceItem
    {
        public DevFeaturesBranchesWorkspaceItem(IServiceLocator serviceLocator) : base(serviceLocator)
        {
        }

        public override string Title
        {
            get
            {
                return "DEV Features Branches";
            }
        }
    }
}
