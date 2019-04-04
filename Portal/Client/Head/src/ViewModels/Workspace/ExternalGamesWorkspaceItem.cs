using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.PubSubEvents;
using GamesPortal.Client.Interfaces.Services;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;

namespace GamesPortal.Client.ViewModels.Workspace
{
    public class ExternalGamesWorkspaceItem : GamesWorkspaceItemBase
    {
        public ExternalGamesWorkspaceItem(IEnumerable<Game> games, IServiceLocator serviceLocator)
            : base(games, serviceLocator)
        {
          
        }


        protected override bool IsExternal
        {
            get { return true; }
        }
       

        public override string Title
        {
            get { return "External games"; }
        }
    }
}
