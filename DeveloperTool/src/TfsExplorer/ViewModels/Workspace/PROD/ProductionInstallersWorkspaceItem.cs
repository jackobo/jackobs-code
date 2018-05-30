using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Workspace
{
    public class ProductionInstallersWorkspaceItem : WorkspaceItemBase
    {
        public ProductionInstallersWorkspaceItem(IProductionEnvironment environment, IServiceLocator serviceLocator) 
            : base(serviceLocator)
        {
            _environment = environment;
        }

        IProductionEnvironment _environment;

        public override string Title
        {
            get
            {
                return $"{_environment.Name} Installers";
            }
        }
    }
}
