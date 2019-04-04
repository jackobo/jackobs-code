using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Design
{
    public class ComponentsReader : IComponentsReader
    {
        public ComponentsReader(ILogicalComponentFactory componentsFactory)
        {
            _componentsFactory = componentsFactory;
        }


        ILogicalComponentFactory _componentsFactory;

        public IEnumerable<ILogicalComponent> ReadComponents(Folders.ComponentsFolder componentsFolder)
        {
            var components = new List<ILogicalComponent>();

            components.AddRange(GetCoreComponents(componentsFolder));
            components.AddRange(GetGameEngines(componentsFolder));

            return components;
        }

        private IEnumerable<ILogicalComponent> GetCoreComponents(Folders.ComponentsFolder componentsFolder)
        {
            var result = componentsFolder.Core.AllCoreComponents
                                        .Select(folder => _componentsFactory.CreateCoreComponent(folder))
                                        .ToList();

            if (componentsFolder.Packages.Exists())
            {
                result.Add(_componentsFactory.CreatePackagesComponent(componentsFolder.Packages));
            }

            if(componentsFolder.NonDeployable.Exists())
            {
                result.Add(_componentsFactory.CreateNonDeployableComponent(componentsFolder.NonDeployable));
            }

            return result;
        }

        private IEnumerable<ILogicalComponent> GetGameEngines(Folders.ComponentsFolder componentsFolder)
        {
            var enginesAndGames = new List<ILogicalComponent>();

            foreach (var gameEngineFolder in componentsFolder.EnginesAndGames.AllGameEngines)
            {
                enginesAndGames.Add(_componentsFactory.CreateGameEngineComponent(gameEngineFolder.Engine));
                enginesAndGames.AddRange(ReadGames(gameEngineFolder));
            }

            return enginesAndGames;
        }

        private IEnumerable<ILogicalComponent> ReadGames(Folders.GameEngineFolder gameEngineFolder)
        {
            var games = new List<IGameComponent>();

            foreach (var gameFolder in gameEngineFolder.Games.AllGames)
            {
                games.Add(_componentsFactory.CreateGameComponent(gameFolder));
            }

            return games;
        }

    }
}
