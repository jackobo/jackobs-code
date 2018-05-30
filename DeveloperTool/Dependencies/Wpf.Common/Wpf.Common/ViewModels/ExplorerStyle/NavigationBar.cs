using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;


namespace Spark.Wpf.Common.ViewModels
{
    public class NavigationBar : ServicedViewModelBase, INavigationBar
    {
        public NavigationBar(IServiceLocator serviceLocator) 
            : base(serviceLocator)
        {
        }

        IContextCommand[] _navigationList = new IContextCommand[0];
        public IContextCommand[] NavigationList
        {
            get
            {
                return _navigationList;
            }
            private set
            {
                SetProperty(ref _navigationList, value);
            }
        }

        public void Navigate(IExplorerBarItem explorerBarItem)
        {
            var navigationList = explorerBarItem.GetAccessorsList();

            this.NavigationList = navigationList.Select(item => new ContextCommand(item.Caption, () => item.IsSelected = true))
                                                .ToArray();

        }
    }
}
