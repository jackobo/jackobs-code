using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;
using Spark.Infra.Types;

namespace Spark.Wpf.Common.ViewModels
{
    internal class Sidebar : ServicedViewModelBase, ISidebar
    {
        public Sidebar(IServiceLocator serviceLocator) 
            : base(serviceLocator)
        {
        }

        public void Navigate<TViewModel>(TViewModel viewModel) where TViewModel : ISideBarItem, IActivationAware
        {
            _currentItem.Do(item => item.Deactivate());

            Optional<ISideBarItem> _previousItem = _currentItem;

            ServiceLocator.GetInstance<IRegionManager>().NavigateToViewModel(GetSidebarRegionName(), viewModel);
            
            _currentItem = Optional<ISideBarItem>.Some(viewModel);

            _previousItem.Do(item => item.Dispose());
            
            this.IsCollapsed = false;
        }

        public void Hide()
        {
            _currentItem = Optional<ISideBarItem>.None();
            this.IsCollapsed = true;
        }

        Optional<ISideBarItem> _currentItem = Optional<ISideBarItem>.None();

        bool _isCollapsed = true;
        public bool IsCollapsed
        {
            get
            {
                return _isCollapsed;
            }

            set
            {
                SetProperty(ref _isCollapsed, value);
            }
        }

        protected virtual string GetSidebarRegionName()
        {
            return WellKnowRegionNames.Sidebar;
        }

        
    }
}
