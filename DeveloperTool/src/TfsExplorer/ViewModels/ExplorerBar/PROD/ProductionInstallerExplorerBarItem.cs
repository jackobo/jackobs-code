using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.TfsExplorer.ViewModels.Workspace;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    public class ProductionInstallerExplorerBarItem : NavigationAwareItem<Workspace.ProductionInstallerWorkspaceItem>
    {
        public ProductionInstallerExplorerBarItem(IProductionInstaller installer, IExplorerBarItem parent, IServiceLocator serviceLocator) 
            : base(parent, serviceLocator)
        {
            _installer = installer;
            SubscribeToEvent<InstallerBranchedEventData>(InstallerBranchedHandler);
        }

        private void InstallerBranchedHandler(InstallerBranchedEventData eventData)
        {
            if (_installer.Equals(eventData.Installer))
            {
                OnPropertyChanged(nameof(IsBranched));
            }
        }

        IProductionInstaller _installer;

        public override string Caption
        {
            get
            {
                return _installer.Version.ToString();
            }
        }

        public bool IsBranched
        {
            get { return _installer.IsBranched(); }
        }

        protected override ProductionInstallerWorkspaceItem CreateWorkspaceItemViewModel()
        {
            return new ProductionInstallerWorkspaceItem(_installer, this.ServiceLocator);
        }
    }
}
