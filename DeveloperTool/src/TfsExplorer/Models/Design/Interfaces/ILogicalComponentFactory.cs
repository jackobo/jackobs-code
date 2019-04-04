using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.Design
{
    public interface ILogicalComponentFactory
    {
        ICoreComponent CreateCoreComponent(CoreComponentFolder componentFolder);
        ICoreComponent CreateCoreComponent(CoreComponentFolder componentFolder, IComponentUniqueId uniqueId);
        IGameEngineComponent CreateGameEngineComponent(Folders.EngineFolder engineFolder);
        IGameEngineComponent CreateGameEngineComponent(Folders.EngineFolder engineFolder, IComponentUniqueId uniqueId);
        IGameComponent CreateGameComponent(GameFolder gameFolder);
        IGameComponent CreateGameComponent(GameFolder gameFolder, IComponentUniqueId mathUniqueId, IComponentUniqueId limitsUniqueId);
        ICoreComponent CreatePackagesComponent(PackagesFolder packagesFolder);
        ICoreComponent CreatePackagesComponent(PackagesFolder packagesFolder, IComponentUniqueId uniqueId);
        ICoreComponent CreateNonDeployableComponent(NonDeployableFolder nonDeployableFolder);
    }
}
