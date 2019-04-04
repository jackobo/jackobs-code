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
    public abstract class WorkspaceAwareExplorerBarItem<TWorkspaceItemViewModel> : ExplorerBarItem
        where TWorkspaceItemViewModel : IWorkspaceItem
    {
        public WorkspaceAwareExplorerBarItem(IExplorerBar explorerBar, IServiceLocator serviceLocator)
          : base(explorerBar, serviceLocator)
        {

        }

        public WorkspaceAwareExplorerBarItem(IExplorerBarItem parent, IServiceLocator serviceLocator)
            : base(parent, serviceLocator)
        {

        }

        Optional<TWorkspaceItemViewModel> _workspaceItemViewModel = Optional<TWorkspaceItemViewModel>.None();

        public virtual TWorkspaceItemViewModel WorkspaceItemViewModel
        {
            get
            {
                if (!_workspaceItemViewModel.Any())
                {
                    _workspaceItemViewModel = Optional<TWorkspaceItemViewModel>.Some(CreateWorkspaceItemViewModel());
                    _workspaceItemViewModel.First().PropertyChanged += WorkspaceItem_PropertyChanged;
                }

                return _workspaceItemViewModel.First();
            }
        }

        public override IContextCommand[] Actions
        {
            get
            {
                return this.WorkspaceItemViewModel.Actions;
            }
        }

        private void WorkspaceItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(IWorkspaceItem.Actions))
            {
                OnPropertyChanged(nameof(Actions));
            }
        }

        protected abstract TWorkspaceItemViewModel CreateWorkspaceItemViewModel();
        
        protected override void OnSelected()
        {
            base.OnSelected();
            OnNavigate();
        }

        protected virtual void OnNavigate()
        {
            ServiceLocator.GetInstance<IWorkspace>().Navigate(WorkspaceItemViewModel);
            ServiceLocator.GetInstance<INavigationBar>().Navigate(this);
        }
        
    }
}
