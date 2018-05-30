using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.TfsExplorer.ViewModels.Workspace;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    public class QaMainBranchExplorerBarItem : MainBranchExplorerBarItem<QAMainBranchWorkspaceItem, IQaBranch>
    {
        public QaMainBranchExplorerBarItem(IQaBranch qaBranch, 
                                           IExplorerBarItem parent, 
                                           IServiceLocator serviceLocator)
            : base(qaBranch, parent, serviceLocator)
        {
            
            
        }

        protected override QAMainBranchWorkspaceItem CreateWorkspaceItem(IQaBranch mainBranch, IServiceLocator serviceLocator)
        {
            return new QAMainBranchWorkspaceItem(mainBranch, serviceLocator);
        }

      
    }
}
