using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GamesPortal.Client.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;

namespace GamesPortal.Client.ViewModels.Workspace
{
    public class InternalGamesWorkspaceItem : GamesWorkspaceItemBase
    {
        public InternalGamesWorkspaceItem(IEnumerable<Game> games, IServiceLocator serviceLocator)
            : base(games, serviceLocator)
        {
            
        }


        protected override bool IsExternal
        {
            get { return false; }
        }

        
        public override string Title
        {
            get
            {
                return "Internal Games";
            }
        }

        
    }
}
