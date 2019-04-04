using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.TfsExplorer.ViewModels.Workspace;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    public class RootBranchExplorerBarItem : NavigationAwareItem<RootBranchWorkspaceItem>
    {
        public RootBranchExplorerBarItem(IRootBranch rootBranch, IExplorerBar explorerBar, IServiceLocator serviceLocator)
            : base(explorerBar, serviceLocator)
        {
            _rooBranch = rootBranch;
            AddDevBranchItem();
            AddQaBranchItem();
            AddProductionBranchItem();
            SubscribeToEvent<CreateDevBranchFinishEventData>(CreateDevBranchEventHandler);
        }

        private void AddProductionBranchItem()
        {
            this.Items.Add(ItemsFactory.CreateProductionBranchItem(_rooBranch.GetProductionBranch()));
        }

        private void AddQaBranchItem()
        {
            this.Items.Add(ItemsFactory.CreateQABranchItem(_rooBranch.GetQaBranch()));
        }

        private Optional<IExplorerBarItem> AddDevBranchItem()
        {
            var result = Optional<IExplorerBarItem>.None();

            _rooBranch.GetDevBranch().Do(devBranch =>
            {
                var devBranchExplorerBarItem = ItemsFactory.CreateDevBranchItem(devBranch);
                Items.Insert(0, devBranchExplorerBarItem);
                result = Optional<IExplorerBarItem>.Some(devBranchExplorerBarItem);
            });


            return result;

            
        }

        private void CreateDevBranchEventHandler(CreateDevBranchFinishEventData eventPayload)
        {
            if (_rooBranch.Equals(eventPayload.LogicalBranch))
            {
                AddDevBranchItem().Do(devBranchItem =>
                {
                    devBranchItem.EnsureExpanded();

                    if (this.IsSelected)
                    {
                        devBranchItem.IsSelected = true;
                    }

                });

                OnPropertyChanged(nameof(Actions));
            }
        }

        IRootBranch _rooBranch;

        public override string Caption
        {
            get
            {
                return _rooBranch.Version.ToString();
            }
        }

        protected override RootBranchWorkspaceItem CreateWorkspaceItemViewModel()
        {
            return new RootBranchWorkspaceItem(_rooBranch, ServiceLocator);
        }

        
        public override IContextCommand[] Actions
        {
            get
            {
                var actions = new List<IContextCommand>();

                if (_rooBranch.CanCreateDevBranch())
                {
                    actions.Add(new Actions.CreateDevelopmentBranchAction(_rooBranch, ServiceLocator));
                }

                if (_rooBranch.CanBranch)
                {
                    actions.Add(new Actions.CreateRootBranchAction(_rooBranch, this.ServiceLocator));
                }

              
                
                return actions.ToArray();
            }
        }
        
    }
}
