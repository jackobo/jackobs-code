using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.Build
{
    public class ComponentBuilderFactory : IComponentBuilderFactory
    {
        public ComponentBuilderFactory(IGGPSolutionParser ggpSolutionParser)
        {
            _ggpSolutionParser = ggpSolutionParser;
        }

        IGGPSolutionParser _ggpSolutionParser;

        public IComponentBuilder GetCoreComponentBuilder(string coreComponentName, VersionNumber proposedVersion)
        {
            var contentProvider = _ggpSolutionParser.GetCoreComponentContentProvider(coreComponentName);

            return new CoreComponentBuilder(contentProvider, proposedVersion);
        }

        public IComponentBuilder GetGameEngineBuilder(GameEngineName engineName, VersionNumber proposedVersion)
        {
            var contentProvider = _ggpSolutionParser.GetGameEngineContentProvider(engineName);
            return new GameEngineBuilder(contentProvider, proposedVersion);
        }

        public IComponentBuilder GetGameMathBuilder(string gameName, GameEngineName engineName, VersionNumber proposedVersion)
        {
            var engine = _ggpSolutionParser.GetGameEngineContentProvider(engineName);

            var game = engine.GetGame(gameName);

            return new GameMathBuilder(game.Name, engineName, game.MathContent, proposedVersion);
        }

        public IComponentBuilder GetGameLimitsBuilder(string gameName, GameEngineName engineName, VersionNumber proposedVersion)
        {
            var engine = _ggpSolutionParser.GetGameEngineContentProvider(engineName);

            var game = engine.GetGame(gameName);

            return new GameLimitsBuilder(game.Name, engineName, game.LimitsContent, proposedVersion);
        }
    }

}
