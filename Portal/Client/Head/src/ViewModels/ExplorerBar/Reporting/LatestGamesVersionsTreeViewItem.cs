using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;

namespace GamesPortal.Client.ViewModels.ExplorerBar
{
    public class LatestGamesVersionsTreeViewItem : TreeViewItem
    {
        public LatestGamesVersionsTreeViewItem(TreeViewItem parent,  IServiceLocator serviceLocator)
            : base(parent, serviceLocator)
        {
            
            this.Caption = "Latest version for each game";
        }

        public override IWorkspaceItem CreateWorkspaceItem(IServiceLocator serviceLocator)
        {
            return new Workspace.LatestGamesVersionsWorkspaceItem(serviceLocator);
        }
    }
}
