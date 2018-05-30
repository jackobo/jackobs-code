using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.ViewModels
{
    public class ComponentsPublishersClassification
    {
        public ComponentsPublishersClassification(IEnumerable<ICoreComponentPublisherViewModel> coreComponentsPublishers,
                                                  IEnumerable<IGameEnginePublisherViewModel> gameEnginesPublishers)
        {
            this.CoreComponentsPublishers = coreComponentsPublishers;
            this.GameEnginesPublishers = gameEnginesPublishers;
            
        }

        public IEnumerable<ICoreComponentPublisherViewModel> CoreComponentsPublishers { get; private set; }
        public IEnumerable<IGameEnginePublisherViewModel> GameEnginesPublishers { get; private set; }
    }

    public class ComponentsPublishersClassificationBuilder : Interfaces.IComponentPublisherVisitor
    {

        public ComponentsPublishersClassificationBuilder(IEnumerable<IComponentPublisher> componentsPublishers)
        {
            foreach(var cp in componentsPublishers)
            {
                cp.AcceptCommandVisitor(() => this);
            }
        }



        private List<ICoreComponentPublisher> _coreComponentsPublishers = new List<ICoreComponentPublisher>();
        private List<IGameEnginePublisher> _gameEnginesPublishers = new List<IGameEnginePublisher>();
        private List<IGameMathPublisher> _gamesMathsPublishers = new List<IGameMathPublisher>();
        private List<IGameLimitsPublisher> _gamesLimitsPublishers = new List<IGameLimitsPublisher>();

        public void Visit(ICoreComponentPublisher coreComponentPublisher)
        {
            _coreComponentsPublishers.Add(coreComponentPublisher);
        }

        public void Visit(IGameEnginePublisher gameEnginePublisher)
        {
            _gameEnginesPublishers.Add(gameEnginePublisher);
        }

        public void Visit(IGameMathPublisher gameMathPublisher)
        {
            _gamesMathsPublishers.Add(gameMathPublisher);
        }

        public void Visit(IGameLimitsPublisher gameLimistPublisher)
        {
            _gamesLimitsPublishers.Add(gameLimistPublisher);
        }

        public ComponentsPublishersClassification Build()
        {
            return new ComponentsPublishersClassification(GetCoreComponents(), GetGamesEngines());
        }

        private IEnumerable<IGameEnginePublisherViewModel> GetGamesEngines()
        {
            var gamesMathsByEngineAndGame = _gamesMathsPublishers.ToDictionary(item => new GameKey(item.EngineName, item.GameName),
                                                                               item => (IGameMathPublisherViewModel)new GameMathPublisherViewModel(item));
            var gamesLimitsByEngineAndGame = _gamesLimitsPublishers.ToDictionary(item => new GameKey(item.EngineName, item.GameName),
                                                                                 item => (IGameLimitsPublisherViewModel)new GameLimitsPublisherViewModel(item));

            var gamesByEngine = _gamesMathsPublishers.Select(item => new GameKey(item.EngineName, item.GameName))
                                  .Union(_gamesLimitsPublishers.Select(item => new GameKey(item.EngineName, item.GameName)))
                                  .Distinct()
                                  .GroupBy(item => item.EngineName)
                                  .ToDictionary(group => group.Key,
                                                group => group.Select(item => new GamePublisherViewModel(item.GameName,
                                                                                                         gamesMathsByEngineAndGame.TryGetAValue(new GameKey(item.EngineName, item.GameName)),
                                                                                                         gamesLimitsByEngineAndGame.TryGetAValue(new GameKey(item.EngineName, item.GameName)))));



            var result = new List<IGameEnginePublisherViewModel>();
            result.AddRange(_gameEnginesPublishers.Select(ge => new GameEnginePublisherViewModel(ge,
                                                                    gamesByEngine.ContainsKey(ge.EngineName)
                                                                            ? gamesByEngine[ge.EngineName]
                                                                            : new GamePublisherViewModel[0])));

            //missing game engines infered from games
            result.AddRange(gamesByEngine.Where(gbye => !_gameEnginesPublishers.Select(ge => ge.EngineName).Contains(gbye.Key))
                                                            .Select(gbye => new GameEnginePublisherPlaceholder(gbye.Key, gbye.Value)));
            return result;
        }

        private IEnumerable<ICoreComponentPublisherViewModel> GetCoreComponents()
        {
            return _coreComponentsPublishers.Select(ccp => new CoreComponentPublisherViewModel(ccp)).ToList();
        }

        
    }
}
