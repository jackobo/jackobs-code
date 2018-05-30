using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace LayoutTool.Interfaces
{
    public class Layout
    {
        public Layout()
        {
            this.AvailableFilters = new ObservableCollection<ArenaFilter>();
            this.Arenas = new ObservableCollection<NavigationPlanArena>();
            this.TopGames = new ObservableCollection<Game>();
            this.VipGames = new ObservableCollection<Game>();
        }

        public ObservableCollection<ArenaFilter> AvailableFilters { get; set; }

        public ObservableCollection<NavigationPlanArena> Arenas { get; set; }
        
        public ObservableCollection<Game> TopGames { get; set; }
        public ObservableCollection<Game> VipGames { get; set; }
    }
    
}
