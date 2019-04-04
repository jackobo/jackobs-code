using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels
{
    public class ComponentsHierarchyBuilder : LogicalComponentVisitor
    {
        
        List<ICoreComponent> _coreComponents = new List<ICoreComponent>();
        List<IGameEngineComponent> _gameEngines = new List<IGameEngineComponent>();
        List<IGameComponent> _games = new List<IGameComponent>();

        public ComponentsHierarchyBuilder(IEnumerable<ILogicalComponent> components)
        {
            foreach(var c in components)
            {
                c.AcceptCommandVisitor(() => this);
            }
        }

        public ComponentsHierarchy Build()
        {
            return new ComponentsHierarchy(GetCoreComponents(), GetGameEngines());

        }

        private IEnumerable<CoreComponentViewModel> GetCoreComponents()
        {
            return _coreComponents.Select(cc => new CoreComponentViewModel(cc)).ToList();
        }

        private IEnumerable<IGameEngineViewModel> GetGameEngines()
        {
            var gamesByEngine = _games.GroupBy(g => g.EngineName)
                                      .ToDictionary(group => group.Key, 
                                                    group => group.Select(game => new GameViewModel(game)).ToArray());


            var result = new List<IGameEngineViewModel>();
            result.AddRange(_gameEngines.Select(ge => new GameEngineViewModel(ge,
                                                                    gamesByEngine.ContainsKey(ge.EngineName)
                                                                            ? gamesByEngine[ge.EngineName]
                                                                            : new GameViewModel[0])));

            //missing game engines infered from games
            result.AddRange(gamesByEngine.Where(gbye => !_gameEngines.Select(ge => ge.EngineName).Contains(gbye.Key))
                                                            .Select(gbye => new GameEnginePlaceholderViewModel(gbye.Key, gbye.Value)));


            return result;


        }

        public override void Visit(ICoreComponent coreComponent)
        {
            _coreComponents.Add(coreComponent);
        }

        public override void Visit(IGameComponent game)
        {
            _games.Add(game);
        }

        public override void Visit(IGameEngineComponent gameEngine)
        {
            _gameEngines.Add(gameEngine);
        }  
    }

    
}
