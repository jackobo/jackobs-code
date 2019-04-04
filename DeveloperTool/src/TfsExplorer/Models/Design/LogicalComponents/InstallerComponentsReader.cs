using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.DeveloperToolService;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.Design
{
    public class InstallerComponentsReader : IComponentsReader
    {
        public InstallerComponentsReader(Guid installerId, ILogicalComponentFactory componentsFactory)
        {
            _installerId = installerId;
            _componentsFactory = componentsFactory;
        }

        Guid _installerId;
        ILogicalComponentFactory _componentsFactory;
        public IEnumerable<ILogicalComponent> ReadComponents(ComponentsFolder componentsFolder)
        {
            var installerContent = ReadInstallerContent();

            var components = new List<ILogicalComponent>();
            foreach (var coreComponent in installerContent.CoreComponents)
            {
                if (coreComponent.Name == Folders.PackagesFolder.WellKnownName)
                {
                    components.Add(_componentsFactory.CreatePackagesComponent(componentsFolder.Packages,
                                                                              new MemoryBasedComponentUniqueID(coreComponent.ComponentUniqueId)));
                }
                else
                {
                    components.Add(_componentsFactory.CreateCoreComponent(componentsFolder.Core.CoreComponent(coreComponent.Name),
                                                           new MemoryBasedComponentUniqueID(coreComponent.ComponentUniqueId)));
                }
            }

            foreach(var gameEngine in installerContent.GameEngines)
            {
                components.Add(_componentsFactory.CreateGameEngineComponent(componentsFolder.EnginesAndGames.GameEngine(gameEngine.Name).Engine,
                                                            new MemoryBasedComponentUniqueID(gameEngine.ComponentUniqueId)));
                
            }

            

            var gameLimits = installerContent.GamesLimits.ToDictionary(item => new GameKey(new GameEngineName(item.EngineName),  item.Name));
            
            foreach(var gameMath in installerContent.GamesMaths)
            {
                var gameKey = new GameKey(new GameEngineName(gameMath.EngineName), gameMath.Name);
                var mathUniqueId = new MemoryBasedComponentUniqueID(gameMath.ComponentUniqueId);
                var limitsUniqueId = new MemoryBasedComponentUniqueID();
                if (gameLimits.ContainsKey(gameKey))
                {
                    limitsUniqueId = new MemoryBasedComponentUniqueID(gameLimits[gameKey].ComponentUniqueId);
                }


                components.Add(_componentsFactory.CreateGameComponent(componentsFolder.EnginesAndGames
                                                                                       .GameEngine(gameMath.EngineName)
                                                                                       .Games
                                                                                       .Game(gameMath.Name),
                                                                      mathUniqueId,
                                                                      limitsUniqueId));
                                
            }
            

            return components;

        }

        private GetInstallerContentResponse ReadInstallerContent()
        {
            using (var proxy = DeveloperToolServiceProxyFactory.CreateProxy())
            {
                return proxy.GetInstallerContent(new GetInstallerContentRequest() { InstallerId = _installerId});
            }
        }
    }
}
