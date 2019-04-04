using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    public class ExplorerBarItemsRepositoryFactory : IExplorerBarItemsRepositoryFactory
    {
        public ExplorerBarItemsRepositoryFactory(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        IServiceLocator _serviceLocator;
        public IExplorerBarItemsRepository GetRepository(IExplorerBarItem parentItem)
        {
            return new ExplorerBarItemsRepository(_serviceLocator, parentItem);
        }
    }
}
