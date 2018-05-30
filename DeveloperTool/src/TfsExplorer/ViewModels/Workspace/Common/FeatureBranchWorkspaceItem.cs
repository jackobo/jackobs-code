using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Workspace
{
    public interface IFeatureBranchWorkspaceItem : IWorkspaceItem
    {
        void MergeToMain(Action<IMergeBuilderViewModel> onDone);
        void MergeFromMain(Action<IMergeBuilderViewModel> onDone);
        void AddMissingComponent();
    }
    public abstract class FeatureBranchWorkspaceItem : WorkspaceItemBase, IFeatureBranchWorkspaceItem
    {
        public FeatureBranchWorkspaceItem(IFeatureBranch featureBranch, IServiceLocator serviceLocator) 
            : base(serviceLocator)
        {
            
            this.FeatureBranch = featureBranch;
            StartBusyAction(() => LoadComponents(), "Loading components...");
            SubscribeToEvent<FeatureBranchUpdateEventData>(FeatureBranchUpdateEventHandler, (data) => this.FeatureBranch.Equals(data.Feature));
        }

        private void FeatureBranchUpdateEventHandler(FeatureBranchUpdateEventData eventData)
        {
            LoadComponents(); 
        }

        private void LoadComponents()
        {
            this.Components = new ComponentsExplorerBar(FeatureBranch.GetComponents(),
                                                        ServiceLocator);
        }

        public void MergeToMain(Action<IMergeBuilderViewModel> onDone)
        {
            StartBusyAction(() =>
            {
                var fromMainMergeSets = this.FeatureBranch.GetMergeSetsFromMain();

                if (fromMainMergeSets.Any())
                {
                    MessageBoxResponse msgResponse = MessageBoxResponse.None;
                    this.ExecuteOnUIThread(() =>
                    {
                        msgResponse = this.ServiceLocator.GetInstance<IMessageBox>().ShowYesNoMessage("There are some changes on the Main branch. It is recomanded to merge the changes from Main first."
                                                                                    + Environment.NewLine + Environment.NewLine
                                                                                    + "Do you want to merge changes from Main first?");

                    });

                    if (MessageBoxResponse.Yes == msgResponse)
                    {
                        MergeFromMain(onDone);
                        return;
                    }
                }

                onDone(ShowSideBarItem(new MergeBuilderViewModel("Merge TO Main", 
                                                                 () => this.FeatureBranch.GetMergeSetsToMain(),
                                                                 this.ServiceLocator)));

            },
            "Checking merge sets from Main...");

        
        }

        public void MergeFromMain(Action<IMergeBuilderViewModel> onDone)
        {
            onDone(ShowSideBarItem(new MergeBuilderViewModel("Merge FROM Main", () => this.FeatureBranch.GetMergeSetsFromMain(),
                                   this.ServiceLocator)));
        }

        public void AddMissingComponent()
        {
            ShowSideBarItem(new AddComponentsToFeatureViewModel(this.FeatureBranch, this.ServiceLocator));
        }

      

        protected IFeatureBranch FeatureBranch { get; private set; }
        protected IMainBranch MainBranch { get; private set; }

        public override string Title
        {
            get
            {
                return FeatureBranch.Name;
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

        IContextCommand[] _actions = null;

        public override IContextCommand[] Actions
        {
            get
            {
                if (_actions == null)
                {
                    _actions = new IContextCommand[]
                        {
                            new Actions.FeatureMergeFromMainAction(this, this.ServiceLocator.GetInstance<IApplicationServices>()),
                            new Actions.FeatureMergeToMainAction(this, this.ServiceLocator.GetInstance<IApplicationServices>()),
                            null, //separator
                            new ContextCommand("Add missing components", this.AddMissingComponent)
                        };
                }

                return _actions;
            }
        }
    }
}
