using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Workspace
{
    public abstract class InstallerWorkspaceItem<TInstaller> : WorkspaceItemBase, IBranchPublisherViewModel
        where TInstaller : IInstaller
    {
        public InstallerWorkspaceItem(TInstaller installer, IServiceLocator serviceLocator) : base(serviceLocator)
        {
            this.Installer = installer;
            StartBusyAction(() => LoadComponents(), "Loading components...");
        }

        public override string Title
        {
            get
            {
                return Installer.GetDescription();
            }
        }

        protected TInstaller Installer { get; private set; }
        protected abstract bool AllowCustomizedInstallerGeneration { get; }

        bool IBranchPublisherViewModel.AllowCustomizedInstallerGeneration
        {
            get
            {
                return AllowCustomizedInstallerGeneration;
            }
        }

        private void LoadComponents()
        {
            this.Components = new ComponentsExplorerBar(Installer.GetComponents(),
                                                        ServiceLocator);
        }

        public override IContextCommand[] Actions
        {
            get
            {
                var actions = new List<IContextCommand>();

                if (!Installer.IsBranched())
                    actions.Add(new Actions.CreateInstallerBranchAction(Installer, this.ServiceLocator, 
                        () => {
                                LoadComponents();
                                OnPropertyChanged(nameof(Actions));    
                              }));
                else
                    actions.Add(new Actions.PublishToQAAction(this, this.ServiceLocator, "Publish hotfix"));

                return actions.ToArray();
            }
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

        void IBranchPublisherViewModel.StartPublishing()
        {
            ShowSideBarItem(new PublishToQAViewModel(this.Components, this, this.ServiceLocator));
        }

        bool IBranchPublisherViewModel.IsPublishInProgress
        {
            get
            {
                return this.Installer.IsPublishInProgress();
            }
        }

        IEnumerable<IComponentPublisher> IBranchPublisherViewModel.GetPublishableComponents()
        {
            return Installer.GetHotfixPublishers();
        }

        void IBranchPublisherViewModel.Publish(IPublishPayload publishPayload)
        {
            Installer.Publish(publishPayload);
        }
    }
}
