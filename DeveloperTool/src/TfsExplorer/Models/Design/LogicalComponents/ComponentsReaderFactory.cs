using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.TfsExplorer.Models.Folders;
using Spark.TfsExplorer.Models.Publish;
using Spark.Wpf.Common.Interfaces.UI;


namespace Spark.TfsExplorer.Models.Design
{
    public class ComponentsReaderFactory : IComponentsReaderFactory
    {
        public ComponentsReaderFactory(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        IServiceLocator _serviceLocator;

        public IComponentsReader DevMainBranchComponentsReader()
        {
            return new ComponentsReader(new LogicalComponentFactory(_serviceLocator));
        }


        public IComponentsReader FeatureBranchComponentsReader()
        {
            return DevMainBranchComponentsReader();
        }

        public IComponentsReader QAMainBranchComponentsReader(RootBranchVersion branchName)
        {
            return new ComponentsReader(new LogicalComponentFactory(_serviceLocator, new RegularVersionsProvider(branchName)));
        }

        
        public IComponentsReader InstallerComponentsReader(Guid installerId)
        {
            return new InstallerComponentsReader(installerId, new LogicalComponentFactory(_serviceLocator, new HotFixVersionsProvider(installerId)));
        }
        
    }

   
}

