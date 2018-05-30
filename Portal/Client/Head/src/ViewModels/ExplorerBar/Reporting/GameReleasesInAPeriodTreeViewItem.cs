using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;

namespace GamesPortal.Client.ViewModels.ExplorerBar
{
    public class GameReleasesInAPeriodTreeViewItem : TreeViewItem
    {
        public GameReleasesInAPeriodTreeViewItem(TreeViewItem parent, IServiceLocator serviceLocator)
            : base(parent, serviceLocator)
        {
            this.Caption = "Games versions releases in a specific period";
        }

        public override IWorkspaceItem CreateWorkspaceItem(IServiceLocator serviceLocator)
        {
            return new Workspace.GameReleasesInAPeriodWorkspaceItem(serviceLocator);
        }

    }
}
