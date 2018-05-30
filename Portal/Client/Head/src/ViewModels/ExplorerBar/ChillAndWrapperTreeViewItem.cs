using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;

namespace GamesPortal.Client.ViewModels.ExplorerBar
{
    public class ChillAndWrapperTreeViewItem : GamesTreeViewItemBase
    {
        public ChillAndWrapperTreeViewItem(IEnumerable<Game> components, IServiceLocator serviceLocator) 
            : base(components, serviceLocator, false, GamingComponentCategory.Chill, GamingComponentCategory.Wrapper)
        {
            this.Caption = "Chill & Wrapper";
        }

        protected override GameTreeViewItem CreateGameTreeViewItem(Game game)
        {
            return new ChillWrapperTreeViewItem(game, this, ServiceLocator);
        }

        public override IEnumerable<FoundItem> Search(string[] words)
        {
            return new FoundItem[0];
        }
    }
}
