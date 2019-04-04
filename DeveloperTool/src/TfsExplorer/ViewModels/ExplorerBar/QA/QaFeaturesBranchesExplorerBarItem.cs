using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.TfsExplorer.ViewModels.Workspace;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    public class QaFeaturesBranchesExplorerBarItem : FeaturesBranchesExplorerBarItem<QaFeaturesBranchesWorkspaceItem, IQaBranch>
    {
        public QaFeaturesBranchesExplorerBarItem(IQaBranch qaBranch, IExplorerBarItem parent, IServiceLocator serviceLocator) 
            : base(qaBranch, parent, serviceLocator)
        {
        }
        
        protected override IExplorerBarItem CreateFeatureBranchItem(IFeatureBranch featureBranch)
        {
            return this.ItemsFactory.CreateQAFeatureBranchItem(featureBranch);
        }


        protected override QaFeaturesBranchesWorkspaceItem CreateWorkspaceItemViewModel()
        {
            return new QaFeaturesBranchesWorkspaceItem(ServiceLocator);
        }

    }
}
