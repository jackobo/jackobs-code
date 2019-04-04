using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Workspace
{
    public class ProductionEnvironmentWorkspaceItem : WorkspaceItemBase
    {
        public ProductionEnvironmentWorkspaceItem(IProductionEnvironment environment, IServiceLocator serviceLocator) 
            : base(serviceLocator)
        {
            _environment = environment;
        }

        IProductionEnvironment _environment;
        public override string Title
        {
            get
            {
                return _environment.Name;
            }
        }
    }
}
