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
    public class ProductionBranchExplorerBarItem : NavigationAwareItem<Workspace.ProductionBranchWorkspaceItem>
    {
        public ProductionBranchExplorerBarItem(IProductionBranch productionBranch, IExplorerBarItem parent, IServiceLocator serviceLocator) 
            : base(parent, serviceLocator)
        {
            _productionBranch = productionBranch;

            foreach(var env in _productionBranch.GetEnvironments())
            {
                this.Items.Add(ItemsFactory.CreateProductionEnvironment(env));
            }
        }

        IProductionBranch _productionBranch;

        public override string Caption
        {
            get
            {
                return "PROD";
            }
        }

        protected override ProductionBranchWorkspaceItem CreateWorkspaceItemViewModel()
        {
            return new ProductionBranchWorkspaceItem(this.ServiceLocator);
        }
    }
}
