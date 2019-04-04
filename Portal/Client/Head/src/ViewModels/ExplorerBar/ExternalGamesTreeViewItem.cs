using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.PubSubEvents;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;

namespace GamesPortal.Client.ViewModels.ExplorerBar
{
    public class ExternalGamesTreeViewItem : GamesTreeViewItemBase
    {
        public ExternalGamesTreeViewItem(IEnumerable<GamesPortal.Client.Interfaces.Entities.Game> games, IServiceLocator serviceLocator)
            : base(games, serviceLocator, true, GamingComponentCategory.Game)
        {
            this.Caption = "External games";
         
        }

        public override IWorkspaceItem CreateWorkspaceItem(IServiceLocator serviceLocator)
        {
            return new Workspace.ExternalGamesWorkspaceItem(this.Games, serviceLocator);
        }





       
    }
}
