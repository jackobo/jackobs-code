using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Workspace
{
   
    public interface IMainBranchWorkspaceItem : IViewModel
    {
        IFeatureBranchBuilderViewModel StartNewFeatureBranch();
        void MergeToPairEnvironmentMain(Action<IMergeBuilderViewModel> onDone);
        
    }

    public abstract class MainBranchWorkspaceItem<TMainBranch> : WorkspaceItemBase, 
                                                                IMainBranchWorkspaceItem,
                                                                IComponentRenameDeleteHandler
        where TMainBranch : IMainBranch
    {
        public MainBranchWorkspaceItem(TMainBranch mainBranch, IServiceLocator serviceLocator) : base(serviceLocator)
        {
            this.MainBranch = mainBranch;
            StartBusyAction(() => LoadComponents(), "Loading components...");
        }

        public abstract void MergeToPairEnvironmentMain(Action<IMergeBuilderViewModel> onDone);

        protected TMainBranch MainBranch { get; private set; }

        private void LoadComponents()
        {
            this.Components = CreateComponentsExplorerBar();
        }

        protected virtual ComponentsExplorerBar CreateComponentsExplorerBar()
        {
            return new ComponentsExplorerBar(this.MainBranch.GetComponents(),
                                            this.ServiceLocator,
                                            this);
        }


        bool _renameDeleteInProgress = false;
        bool IComponentRenameDeleteHandler.InProgress
        {
            get
            {
                return _renameDeleteInProgress;
            }
        }


        void IComponentRenameDeleteHandler.StartRenaming(ILogicalComponent component)
        {
            ShowSideBarItem(new RenameComponentViewModel(this.MainBranch, 
                                                        component, 
                                                        this.ServiceLocator, 
                                                        OnRenameDeleteInProgress));
        }

       

        void IComponentRenameDeleteHandler.StartDeleting(ILogicalComponent component)
        {
            ShowSideBarItem(new DeleteComponentViewModel(this.MainBranch,
                                                      component,
                                                      this.ServiceLocator,
                                                      OnRenameDeleteInProgress));
        }

        private void OnRenameDeleteInProgress(bool inProgress)
        {
            _renameDeleteInProgress = inProgress;
            OnPropertyChanged(nameof(IComponentRenameDeleteHandler.InProgress));
        }

        ComponentsExplorerBar _components;

        public ComponentsExplorerBar Components
        {
            get { return _components; }
            private set
            {
                SetProperty(ref _components, value);
            }
        }



        public IFeatureBranchBuilderViewModel StartNewFeatureBranch()
        {            
            return ShowSideBarItem(new FeatureBranchBuilderViewModel(this.Components, MainBranch, this.ServiceLocator));
        }

        
    }
}
