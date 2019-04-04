using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Design
{
    public class ProductionBranch : IProductionBranch
    {
        public ProductionBranch(Folders.ProdFolder productionFolder, IRootBranch owner, IServiceLocator serviceLocator)
        {
            _productionFolder = productionFolder;
            _serviceLocator = serviceLocator;
            _owner = owner;
            
        }

        Folders.ProdFolder _productionFolder;
        IRootBranch _owner;
        IServiceLocator _serviceLocator;

        IEnumerable<IProductionEnvironment> _environments;
        public IEnumerable<IProductionEnvironment> GetEnvironments()
        {
            if (_environments == null)
            {
                using (var proxy = DeveloperToolServiceProxyFactory.CreateProxy())
                {
                    _environments = proxy.GetProductionEnvironments()
                                   .Environments
                                   .Select(e => new ProductionEnvironment(e.Id,
                                                                          e.Name, 
                                                                          _productionFolder.Environment(e.Name),
                                                                          _owner,
                                                                          _serviceLocator))
                                   .ToList();
                }
            }

            return _environments;
        }

        
    }
    
}
