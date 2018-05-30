using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;

namespace GamesPortal.Client.ViewModels.ExplorerBar
{
    public class NeverApprovedGamesTreeViewItem : TreeViewItem
    {
        public NeverApprovedGamesTreeViewItem(TreeViewItem parent, IServiceLocator serviceLocator)
            : base(parent, serviceLocator)
        {
            this.Caption = "Never approved games";
        }

        public override IWorkspaceItem CreateWorkspaceItem(IServiceLocator serviceLocator)
        {
            return new Workspace.NeverApprovedGamesWorkspaceItem(serviceLocator);
        }
    }
}
