using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.Services;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.ExplorerBar
{
    public class ExplorerBarViewModel : ViewModelBase
    {
        public ExplorerBarViewModel(IServiceLocator serviceLocator)
        {
            this.Items = new ObservableCollection<TreeViewItem>();
            var gamesRepository = serviceLocator.GetInstance<IGamesRepository>();
            var allGames = gamesRepository.GetAllGames();

            this.Items.Add(new ChillAndWrapperTreeViewItem(allGames.Where(g => g.Category != GamingComponentCategory.Game), serviceLocator));
            this.Items.Add(new InternalGamesTreeViewItem(allGames.Where(g => !g.IsExternal && g.Category == GamingComponentCategory.Game), serviceLocator));
            this.Items.Add(new ExternalGamesTreeViewItem(allGames.Where(g => g.IsExternal && g.Category == GamingComponentCategory.Game), serviceLocator));
            
            this.Items.Add(new ReportingTreeViewItem(serviceLocator));

            this.GameSearch = new GamesSearchViewModel(this.Items);
        }


      
        public GamesSearchViewModel GameSearch { get; private set; }

        public ObservableCollection<TreeViewItem> Items { get; private set; }


    }
}
