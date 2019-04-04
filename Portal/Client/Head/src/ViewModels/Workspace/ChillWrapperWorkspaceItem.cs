using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;

namespace GamesPortal.Client.ViewModels.Workspace
{
    public class ChillWrapperWorkspaceItem : GameVersionsWorkspaceItem
    {
        public ChillWrapperWorkspaceItem(Game game, GameInfrastructure infrastructure, IServiceLocator serviceLocator) 
            : base(game, infrastructure, serviceLocator)
        {
        }

        public override string Title
        {
            get
            {
                return this.Game.Name;
            }
        }
    }
}
