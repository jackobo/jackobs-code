using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamesPortal.Client.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;

namespace GamesPortal.Client.ViewModels.ExplorerBar
{
    public class InternalGamesTreeViewItem : GamesTreeViewItemBase
    {
        public InternalGamesTreeViewItem(IEnumerable<Game> games, IServiceLocator serviceLocator)
            : base(games, serviceLocator, false, GamingComponentCategory.Game)
        {
            this.Caption = "Internal games";
        }


        public override IWorkspaceItem CreateWorkspaceItem(IServiceLocator serviceLocator)
        {
            return new Workspace.InternalGamesWorkspaceItem(this.Games, this.ServiceLocator);
        }

     
    }
}
