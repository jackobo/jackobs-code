using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    public abstract class EnvironmentBranchExplorerBarItem<TWorkspaceItem, TMainBranch> : NavigationAwareItem<TWorkspaceItem>
        where TWorkspaceItem : IWorkspaceItem
        where TMainBranch : IMainBranch
    {
        public EnvironmentBranchExplorerBarItem(TMainBranch mainBranch, IExplorerBarItem parent, IServiceLocator serviceLocator) 
            : base(parent, serviceLocator)
        {
            this.MainBranch = mainBranch;
            Items.Add(CreateMainExplorerBarItem(mainBranch));

            LoadFeatureBranches();

            SubscribeToEvent<NewFeatureBranchEventData>(NewFeatureBranchEventHandler);
        }

        protected TMainBranch MainBranch { get; private set; }

        protected abstract IExplorerBarItem CreateMainExplorerBarItem(TMainBranch mainBranch);
        protected abstract IExplorerBarItem CreateFaturesBranchesExplorerBarItem(TMainBranch mainBranch);

        private void NewFeatureBranchEventHandler(NewFeatureBranchEventData obj)
        {
            if (MainBranch.Equals(obj.Owner) && !_featureBranchesItem.Any())
            {
                LoadFeatureBranches();
                _featureBranchesItem.Do(item => item.EnsureExpanded());
            }
        }

        Optional<IExplorerBarItem> _featureBranchesItem = Optional<IExplorerBarItem>.None();

        private void LoadFeatureBranches()
        {
            if (MainBranch.GetFeatureBranches().Any())
            {
                _featureBranchesItem = Optional<IExplorerBarItem>.Some(CreateFaturesBranchesExplorerBarItem(MainBranch));
                _featureBranchesItem.Do(item => this.Items.Add(item));
            }

        }
    }
}
