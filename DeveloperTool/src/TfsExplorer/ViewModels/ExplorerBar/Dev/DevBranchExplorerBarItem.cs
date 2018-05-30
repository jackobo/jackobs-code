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
    public class DevBranchExplorerBarItem : EnvironmentBranchExplorerBarItem<DevBranchWorkspaceItem, IDevBranch> 
    {
        public DevBranchExplorerBarItem(IDevBranch devBranch, IExplorerBarItem parent, IServiceLocator serviceLocator) 
            : base(devBranch, parent, serviceLocator)
        {
            
        }

        public override string Caption
        {
            get
            {
                return "DEV";
            }
        }

        protected override IExplorerBarItem CreateMainExplorerBarItem(IDevBranch mainBranch)
        {
            return this.ItemsFactory.CreateDevMainBranchItem(mainBranch);
        }


        protected override IExplorerBarItem CreateFaturesBranchesExplorerBarItem(IDevBranch mainBranch)
        {
            return this.ItemsFactory.CreateDevFeaturesBranchesItem(mainBranch);
        }

        protected override DevBranchWorkspaceItem CreateWorkspaceItemViewModel()
        {
            return new DevBranchWorkspaceItem(ServiceLocator);
        }
    }
}
