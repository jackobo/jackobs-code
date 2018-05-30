using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;

namespace LayoutTool.ViewModels
{
    public class GameGroupLayoutCollectionExplorerBarItem : TreeViewItem<GameGroupLayoutCollectionViewModel>
    {
        public GameGroupLayoutCollectionExplorerBarItem(GameGroupLayoutCollectionViewModel collection, IServiceLocator serviceLocator)
            : base(collection, serviceLocator)
        {
            
            
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
