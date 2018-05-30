using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.ViewModels.Workspace;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    public class DevFeaturesBranchesExplorerBarItem : FeaturesBranchesExplorerBarItem<DevFeaturesBranchesWorkspaceItem, IDevBranch>
    {
        public DevFeaturesBranchesExplorerBarItem(IDevBranch mainBranch, IExplorerBarItem parent, IServiceLocator serviceLocator) 
            : base(mainBranch, parent, serviceLocator)
            
        {
        }

        protected override IExplorerBarItem CreateFeatureBranchItem(IFeatureBranch featureBranch)
        {
            return ItemsFactory.CreateDevFeatureBrancheItem(featureBranch);
        }

        protected override DevFeaturesBranchesWorkspaceItem CreateWorkspaceItemViewModel()
        {
            return new DevFeaturesBranchesWorkspaceItem(ServiceLocator);
        }
    }
}
