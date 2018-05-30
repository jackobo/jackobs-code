using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.ViewModels.Workspace;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    public class ProductionEnvironmentExplorerBarItem : NavigationAwareItem<Workspace.ProductionEnvironmentWorkspaceItem>
    {
        public ProductionEnvironmentExplorerBarItem(IProductionEnvironment environment, IExplorerBarItem parent, IServiceLocator serviceLocator) 
            : base(parent, serviceLocator)
        {
            _environment = environment;
            this.Items.Add(ItemsFactory.CreateProductionInstallersItem(environment));
            
        }
        
        
        private IProductionEnvironment _environment;
        
        public override string Caption
        {
            get
            {
                return _environment.Name;
            }
        }

        protected override ProductionEnvironmentWorkspaceItem CreateWorkspaceItemViewModel()
        {
            return new ProductionEnvironmentWorkspaceItem(_environment, ServiceLocator);
        }
    }
}
