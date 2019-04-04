using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    internal class ExplorerBarItemsRepository : IExplorerBarItemsRepository
    {
        
        public ExplorerBarItemsRepository(IServiceLocator serviceLocator, IExplorerBarItem parent)
        {
            _serviceLocator = serviceLocator;
            _parent = parent;
        }
        
        IServiceLocator _serviceLocator;
        IExplorerBarItem _parent;

        public IExplorerBarItem CreateRootBranchItem(IRootBranch rootBranch, IExplorerBar explorerBar)
        {
            return new RootBranchExplorerBarItem(rootBranch, explorerBar, _serviceLocator);
        }

        public IExplorerBarItem CreateQABranchItem(IQaBranch qaBranch)
        {
            return new QABranchExplorerBarItem(qaBranch, _parent, _serviceLocator);
        }

        public IExplorerBarItem CreateDevBranchItem(IDevBranch devBranch)
        {
            return new DevBranchExplorerBarItem(devBranch, _parent, _serviceLocator);
        }

        public IExplorerBarItem CreateMainQaBranchItem(IQaBranch qaBranch)
        {
            return new QaMainBranchExplorerBarItem(qaBranch, _parent,  _serviceLocator);
        }

        public IExplorerBarItem CreateQAFeaturesBranchesItem(IQaBranch qaBranch)
        {
            return new QaFeaturesBranchesExplorerBarItem(qaBranch, _parent, _serviceLocator);
        }

        public IExplorerBarItem CreateQAFeatureBranchItem(IFeatureBranch featureBranch)
        {
            return new QAFeatureBranchExplorerBarItem(featureBranch, _parent, _serviceLocator);
        }

        public IExplorerBarItem CreateDevMainBranchItem(IDevBranch devBranch)
        {
            return new DevMainBranchExplorerBarItem(devBranch, _parent, _serviceLocator);
        }

        public IExplorerBarItem CreateDevFeaturesBranchesItem(IDevBranch devBranch)
        {
            return new DevFeaturesBranchesExplorerBarItem(devBranch, _parent, _serviceLocator);
        }

        public IExplorerBarItem CreateDevFeatureBrancheItem(IFeatureBranch featureBranch)
        {
            return new DevFeatureBranchExplorerBarItem(featureBranch, _parent, _serviceLocator);
        }

        public IExplorerBarItem CreateQAInstallersItem(IQaBranch qaBranch)
        {
            return new QAInstallersExplorerBarItem(qaBranch, _parent, _serviceLocator);
        }

        public IExplorerBarItem CreateProductionBranchItem(IProductionBranch productionBranch)
        {
            return new ProductionBranchExplorerBarItem(productionBranch, _parent, _serviceLocator);
        }

        public IExplorerBarItem CreateProductionEnvironment(IProductionEnvironment environment)
        {
            return new ProductionEnvironmentExplorerBarItem(environment, _parent, _serviceLocator);
        }

        public IExplorerBarItem CreateProductionInstallersItem(IProductionEnvironment environment)
        {
            return new ProductionInstallersExplorerBarItem(environment, _parent, _serviceLocator);
        }

        public IExplorerBarItem CreateProductionInstallerItem(IProductionInstaller installer)
        {
            return new ProductionInstallerExplorerBarItem(installer, _parent, _serviceLocator);
        }

        public IExplorerBarItem CreateQAInstallerItem(IQAInstaller installer)
        {
            return new QAInstallerExplorerBarItem(installer, _parent, _serviceLocator);
        }
    }
}
