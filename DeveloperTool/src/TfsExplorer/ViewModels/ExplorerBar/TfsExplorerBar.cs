using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    public class TfsExplorerBar : StandardExplorerBar
    {
        public TfsExplorerBar(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            LoadLogicalBranches();
            SubscribeToEvent<Interfaces.Events.NewRootBranchEventData>(NewRootBranchEventHandler);
        }

        private void NewRootBranchEventHandler(NewRootBranchEventData eventData)
        {
            var newItem = ItemsFactory.CreateRootBranchItem(eventData.RootBranch, this);
            newItem.IsExpanded = true;
            this.Items.Insert(0, newItem);
        }

        IExplorerBarItemsRepository ItemsFactory
        {
            get
            {
                return ServiceLocator.GetInstance<IExplorerBarItemsRepositoryFactory>().GetRepository(null);
            }
        }

        private void LoadLogicalBranches()
        {
            var itemsFactory = ItemsFactory;
            foreach (var rootBranch in ServiceLocator.GetInstance<IComponentsRepository>()
                                                        .GetRootBranches().
                                                        OrderByDescending(b => b.Version))
            {
                var item = itemsFactory.CreateRootBranchItem(rootBranch, this);
                this.Items.Add(item);
            }

            if (this.Items.Count > 0)
                this.Items[0].IsExpanded = true;
        }

      

    
    }
}
