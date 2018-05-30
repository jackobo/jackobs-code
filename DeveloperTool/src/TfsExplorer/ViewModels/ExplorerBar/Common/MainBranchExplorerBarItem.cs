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
    public abstract class MainBranchExplorerBarItem<TWorkspaceItem, TMainBranch> : NavigationAwareItem<TWorkspaceItem>
        where TWorkspaceItem : MainBranchWorkspaceItem<TMainBranch>
        where TMainBranch : IMainBranch
    {
        public MainBranchExplorerBarItem(TMainBranch mainBranch, IExplorerBarItem parent, IServiceLocator serviceLocator) 
            : base(parent, serviceLocator)
        {
            this.MainBranch = mainBranch;
        }

        protected TMainBranch MainBranch { get; private set; }


        public override string Caption
        {
            get
            {
                return "Main";
            }
        }

       

        protected abstract TWorkspaceItem CreateWorkspaceItem(TMainBranch mainBranch, IServiceLocator serviceLocator);

        
        protected override TWorkspaceItem CreateWorkspaceItemViewModel()
        {
            return CreateWorkspaceItem(MainBranch, ServiceLocator);
        }
        
        protected virtual void CreateNewFeatureBranch()
        {
            this.IsSelected = true;
            WorkspaceItemViewModel.StartNewFeatureBranch();

        }
    }
}
