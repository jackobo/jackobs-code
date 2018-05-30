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
    public class DevFeatureBranchExplorerBarItem : FeatureBranchExplorerBarItem<DevFeatureBranchWorkspaceItem>
    {
        public DevFeatureBranchExplorerBarItem(IFeatureBranch featureBranch, IExplorerBarItem parent, IServiceLocator serviceLocator) 
            : base(featureBranch, parent, serviceLocator)
        {
        }

        protected override DevFeatureBranchWorkspaceItem CreateWorkspaceItemViewModel()
        {
            return new DevFeatureBranchWorkspaceItem(this.FeatureBranch, this.ServiceLocator);
        }
    }
}
