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
    public class DevMainBranchExplorerBarItem : MainBranchExplorerBarItem<DevMainBranchWorkspaceItem, IDevBranch> 
    {
        public DevMainBranchExplorerBarItem(IDevBranch devBranch, IExplorerBarItem parent, IServiceLocator serviceLocator) 
            : base(devBranch, parent, serviceLocator)
        {
        }
        
        protected override DevMainBranchWorkspaceItem CreateWorkspaceItem(IDevBranch mainBranch, IServiceLocator serviceLocator)
        {
            return new DevMainBranchWorkspaceItem(mainBranch, serviceLocator);
        }

       
    }
}
