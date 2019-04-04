using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;

namespace LayoutTool.ViewModels
{
    public class LobbyCollectionExplorerBarItem : TreeViewItem<LobbyCollectionViewModel>
    {
        public LobbyCollectionExplorerBarItem(LobbyCollectionViewModel lobbyCollection, IServiceLocator serviceLocator)
            : base(lobbyCollection, serviceLocator)
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
