using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;

namespace LayoutTool.ViewModels
{
    public class ArenaExplorerBarItem :  TreeViewItem<ArenaViewModel>
    {
        public ArenaExplorerBarItem(ArenaViewModel arenaViewModel, TreeViewItem parent, IServiceLocator serviceLocator)
            : base(arenaViewModel, parent, serviceLocator)
        {
        }

        public override string Caption
        {
            get
            {
                return this.ViewModel.Name;
            }
        }
    }
}
