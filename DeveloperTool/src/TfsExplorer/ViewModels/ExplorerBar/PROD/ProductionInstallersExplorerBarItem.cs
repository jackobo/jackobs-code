using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.TfsExplorer.ViewModels.Workspace;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    public class ProductionInstallersExplorerBarItem : NavigationAwareItem<Workspace.ProductionInstallersWorkspaceItem>
    {
        public ProductionInstallersExplorerBarItem(IProductionEnvironment environment, IExplorerBarItem parent, IServiceLocator serviceLocator) 
            : base(parent, serviceLocator)
        {
            _environment = environment;

            if (!UseLazyLoading)
            {
                foreach (var item in GetLazyLoadedItems())
                {
                    this.Items.Add(item);
                }
            }
        }

        IProductionEnvironment _environment;

        private void InstallerAddedHandler(NewProductionInstallerEventData eventData)
        {
            this.Items.Insert(0, ItemsFactory.CreateProductionInstallerItem(eventData.Installer));   
        }

        protected override bool UseLazyLoading
        {
            get
            {
                return true;
            }
        }

        protected override IEnumerable<IExplorerBarItem> GetLazyLoadedItems()
        {
            
            var items = _environment.GetInstallers().OrderByDescending(i => i.Version)
                                                    .Select(i => ItemsFactory.CreateProductionInstallerItem(i))
                                                    .ToList();

            SubscribeToEvent<NewProductionInstallerEventData>(InstallerAddedHandler, eventData => _environment.Equals(eventData.Environment));

            return items;
        }

        public override string Caption
        {
            get
            {
                return "Installers";
            }
        }

        protected override ProductionInstallersWorkspaceItem CreateWorkspaceItemViewModel()
        {
            return new ProductionInstallersWorkspaceItem(_environment, this.ServiceLocator);
        }
    }
}
