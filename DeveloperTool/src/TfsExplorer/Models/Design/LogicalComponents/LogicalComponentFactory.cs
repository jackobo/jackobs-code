using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;
using Spark.TfsExplorer.Models.Publish;

namespace Spark.TfsExplorer.Models.Design
{
    public class LogicalComponentFactory : ILogicalComponentFactory
    {
        public LogicalComponentFactory(IServiceLocator serviceLocator,
                                       IComponentsVersionsProvider componentsVersionsProvider)
        {
            _serviceLocator = serviceLocator;
            _componentsVersionsProvider = Optional<IComponentsVersionsProvider>.Some(componentsVersionsProvider);
        }

        public LogicalComponentFactory(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        IServiceLocator _serviceLocator;
        Optional<IComponentsVersionsProvider> _componentsVersionsProvider = Optional<IComponentsVersionsProvider>.None();

        public ICoreComponent CreateCoreComponent(Folders.CoreComponentFolder coreComponentFolder)
        {
            return new CoreComponent(coreComponentFolder, _componentsVersionsProvider, _serviceLocator);
        }

        public ICoreComponent CreateCoreComponent(CoreComponentFolder coreComponentFolder, IComponentUniqueId uniqueId)
        {
            return new CoreComponent(coreComponentFolder, uniqueId, _componentsVersionsProvider, _serviceLocator);
        }

        public IGameComponent CreateGameComponent(Folders.GameFolder gameFolder)
        {
            return new GameComponent(gameFolder, _componentsVersionsProvider, _serviceLocator);
        }

        public IGameComponent CreateGameComponent(Folders.GameFolder gameFolder, IComponentUniqueId mathUniqueId, IComponentUniqueId limitsUniqueId)
        {
            return new GameComponent(gameFolder, mathUniqueId, limitsUniqueId, _componentsVersionsProvider, _serviceLocator);
        }


        public IGameEngineComponent CreateGameEngineComponent(Folders.EngineFolder engineFolder)
        {
            return new GameEngineComponent(engineFolder, _componentsVersionsProvider, _serviceLocator);
        }

        public IGameEngineComponent CreateGameEngineComponent(EngineFolder engineFolder, IComponentUniqueId uniqueId)
        {
            return new GameEngineComponent(engineFolder, uniqueId, _componentsVersionsProvider, _serviceLocator);
        }

        public ICoreComponent CreatePackagesComponent(PackagesFolder packagesFolder)
        {
            return new PackagesComponent(packagesFolder, _componentsVersionsProvider, _serviceLocator);
        }

        public ICoreComponent CreatePackagesComponent(PackagesFolder packagesFolder, IComponentUniqueId uniqueId)
        {
            return new PackagesComponent(packagesFolder, uniqueId, _componentsVersionsProvider, _serviceLocator);
        }

        public ICoreComponent CreateNonDeployableComponent(NonDeployableFolder nonDeployableFolder)
        {
            return new NonDeployableComponents(nonDeployableFolder, _serviceLocator);
        }
    }
}
