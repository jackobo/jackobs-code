using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    public abstract class NavigationAwareItem<TViewModel> : WorkspaceAwareExplorerBarItem<TViewModel>
        where TViewModel : IWorkspaceItem
    {

        public NavigationAwareItem(IExplorerBar explorerBar, IServiceLocator serviceLocator)
          : base(explorerBar, serviceLocator)
        {

        }

        public NavigationAwareItem(IExplorerBarItem parent, IServiceLocator serviceLocator)
            : base(parent, serviceLocator)
        {

        }
        
        protected IExplorerBarItemsRepository ItemsFactory
        {
            get
            {
                return ServiceLocator.GetInstance<IExplorerBarItemsRepositoryFactory>().GetRepository(this);
            }
        }
    }
}
