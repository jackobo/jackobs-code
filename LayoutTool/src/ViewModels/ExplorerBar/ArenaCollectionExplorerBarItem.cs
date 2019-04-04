using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;

namespace LayoutTool.ViewModels
{
    public class ArenaCollectionExplorerBarItem : TreeViewItem<ArenaCollectionViewModel>
    {
        
        public ArenaCollectionExplorerBarItem(ArenaCollectionViewModel arenas, IServiceLocator serviceLocator)
            : base(arenas, serviceLocator)
        {
            


            this.Items = new SynchronizedTreeViewItemCollection<ArenaViewModel>(this.ViewModel, arenaViewModel => new ArenaExplorerBarItem(arenaViewModel, this, this.ServiceLocator));
            
        }

        public override string Caption
        {
            get
            {
                return ViewModel.Title;
            }
        }

        


    }
}
