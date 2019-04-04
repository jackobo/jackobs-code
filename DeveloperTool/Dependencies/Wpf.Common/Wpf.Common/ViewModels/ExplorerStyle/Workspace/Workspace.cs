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
    public class Workspace : ServicedViewModelBase, IWorkspace
    {
        public Workspace(IServiceLocator serviceLocator) 
            : base(serviceLocator)
        {
        }

        public void Navigate<TViewModel>(TViewModel viewModel) where TViewModel : IWorkspaceItem
        {
            _currentItem.Do(item => item.Deactivate());
            
            ServiceLocator.GetInstance<IRegionManager>().NavigateToViewModel(GetWorkspaceRegionName(), viewModel);

            _currentItem = Optional<IWorkspaceItem>.Some(viewModel);

            
        }

        Optional<IWorkspaceItem> _currentItem = Optional<IWorkspaceItem>.None();

        protected virtual string GetWorkspaceRegionName()
        {
            return WellKnowRegionNames.Workspace;
        }
    }
}
