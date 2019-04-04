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
    public class QAFeatureBranchExplorerBarItem : FeatureBranchExplorerBarItem<QAFeatureBranchWorkspaceItem>
    {
        public QAFeatureBranchExplorerBarItem(IFeatureBranch featureBranch, IExplorerBarItem parent, IServiceLocator serviceLocator) 
            : base(featureBranch, parent, serviceLocator)
        {
            
        }
        
        protected override QAFeatureBranchWorkspaceItem CreateWorkspaceItemViewModel()
        {
            return new QAFeatureBranchWorkspaceItem(this.FeatureBranch, ServiceLocator);
        }
    }
}
