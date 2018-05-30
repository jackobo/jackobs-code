using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamesPortal.Client.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;

namespace GamesPortal.Client.ViewModels.ExplorerBar
{
    public class GameTechnologyTreeViewItem : TreeViewItem
    {
        public GameTechnologyTreeViewItem(GamesPortal.Client.Interfaces.Entities.Game game, GameInfrastructure infrastructure, TreeViewItem parent, IServiceLocator serviceLocator)
            : base(parent, serviceLocator)
        {
            this.Game = game;
            this.Infrastructure = infrastructure;
            this.Caption = infrastructure.ToString();
        }

        GamesPortal.Client.Interfaces.Entities.Game Game { get; set; }

        public GameInfrastructure Infrastructure { get; private set; }

        public override IWorkspaceItem CreateWorkspaceItem(IServiceLocator serviceLocator)
        {
            return new Workspace.GameVersionsWorkspaceItem(this.Game, this.Infrastructure, this.ServiceLocator);
        }
    }
}
