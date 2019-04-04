using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Workspace
{
    public class RootBranchWorkspaceItem : WorkspaceItemBase
    {
        public RootBranchWorkspaceItem(IRootBranch rootBranch, IServiceLocator serviceLocator) 
            : base(serviceLocator)
        {
            _rootBranch = rootBranch;
        }

       

        IRootBranch _rootBranch;

        public override string Title
        {
            get
            {
                return "Branch " + _rootBranch.Version;
            }
        }
        
    }
}
