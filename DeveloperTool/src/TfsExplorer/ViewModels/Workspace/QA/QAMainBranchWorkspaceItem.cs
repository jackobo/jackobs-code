using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Workspace
{
    public class QAMainBranchWorkspaceItem : MainBranchWorkspaceItem<IQaBranch>, 
                                             IBranchPublisherViewModel
                                             
    {
        public QAMainBranchWorkspaceItem(IQaBranch qaBranch, IServiceLocator serviceLocator) 
            : base(qaBranch, serviceLocator)
        {
            
            
        }
        
        public override string Title
        {
            get
            {
                return "QA Main Branch";
            }
        }

        public override void MergeToPairEnvironmentMain(Action<IMergeBuilderViewModel> onDone)
        {
            var sideBarItem = ShowSideBarItem(new MergeBuilderViewModel("Merge To DEV", () => this.MainBranch.GetMergeSetsToDev(),
                                   this.ServiceLocator));
            onDone(sideBarItem);
        }

        void IBranchPublisherViewModel.StartPublishing()
        {
            ShowSideBarItem(new PublishToQAViewModel(this.Components, this, this.ServiceLocator));
        }

        bool IBranchPublisherViewModel.IsPublishInProgress
        {
            get
            {
                return this.MainBranch.IsPublishInProgress();
            }
        }

        IEnumerable<IComponentPublisher> IBranchPublisherViewModel.GetPublishableComponents()
        {
            return this.MainBranch.GetComponentsToPublish();
        }

        void IBranchPublisherViewModel.Publish(IPublishPayload publishPayload)
        {
            this.MainBranch.Publish(publishPayload);
        }

        bool IBranchPublisherViewModel.AllowCustomizedInstallerGeneration
        {
            get
            {
                return false;
            }
        }

       

        IContextCommand[] _actions = null;

        public override IContextCommand[] Actions
        {
            get
            {
                if (_actions == null)
                {
                    var actions = new List<IContextCommand>(base.Actions);

                    actions.Add(new Actions.MainMergeAction("Merge To DEV", this, this.ServiceLocator.GetInstance<IApplicationServices>()));
                    actions.Add(new Actions.PublishToQAAction(this, this.ServiceLocator));
                    actions.Add(null); //separator
                    actions.Add(new Actions.CreateFeatureBranchAction(this));

                    _actions = actions.ToArray();
                }

                return _actions;
            }
        }

      
    }
}
