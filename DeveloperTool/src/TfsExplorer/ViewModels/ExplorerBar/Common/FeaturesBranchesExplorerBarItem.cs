using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    public abstract class FeaturesBranchesExplorerBarItem<TWorkspaceItem, TMainBranch>
        : NavigationAwareItem<TWorkspaceItem>
        where TWorkspaceItem : IWorkspaceItem
        where TMainBranch : IMainBranch
    {
        public FeaturesBranchesExplorerBarItem(TMainBranch mainBranch, IExplorerBarItem parent, IServiceLocator serviceLocator) : base(parent, serviceLocator)
        {
            this.MainBranch = mainBranch;
            LoadFeatureBranches();
            SubscribeToEvent<NewFeatureBranchEventData>(NewFeatureBranchEvent);
        }

        protected TMainBranch MainBranch { get; private set; }



        public override string Caption
        {
            get
            {
                return "Features";
            }
        }

        private void NewFeatureBranchEvent(NewFeatureBranchEventData eventData)
        {
            if (this.MainBranch.Equals(eventData.Owner))
            {
                var newItem = CreateFeatureBranchItem(eventData.Feature);
                this.Items.Add(newItem);
                newItem.EnsureExpanded();
            }
        }

        


        private void LoadFeatureBranches()
        {
            foreach (var f in this.MainBranch.GetFeatureBranches())
            {
                this.Items.Add(CreateFeatureBranchItem(f));
            }
        }

        protected abstract IExplorerBarItem CreateFeatureBranchItem(IFeatureBranch featureBranch);
    }
}
