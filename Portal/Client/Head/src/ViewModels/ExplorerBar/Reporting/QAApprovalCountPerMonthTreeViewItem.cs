using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;

namespace GamesPortal.Client.ViewModels.ExplorerBar
{
    public class QAApprovalCountPerMonthTreeViewItem : TreeViewItem
    {
        public QAApprovalCountPerMonthTreeViewItem(TreeViewItem parent, IServiceLocator serviceLocator) 
            : base(parent, serviceLocator)
        {
            this.Caption = "QA Approval Count";
        }
        public override IWorkspaceItem CreateWorkspaceItem(IServiceLocator serviceLocator)
        {
            return new Workspace.QAApprovalCountPerMonth(serviceLocator);
        }
    }
}
