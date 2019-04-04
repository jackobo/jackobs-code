using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.ViewModels.Workspace;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    public abstract class FeatureBranchExplorerBarItem<TWorkspaceItem> : NavigationAwareItem<TWorkspaceItem>
        where TWorkspaceItem : IFeatureBranchWorkspaceItem
    {
        public FeatureBranchExplorerBarItem(IFeatureBranch featureBranch, IExplorerBarItem parent, 
            IServiceLocator serviceLocator) : base(parent, serviceLocator)
        {
            this.FeatureBranch = featureBranch;
        }

        protected IFeatureBranch FeatureBranch { get; private set; }

        public override string Caption
        {
            get
            {
                return FeatureBranch.Name;
            }
        }
        
    }
}
