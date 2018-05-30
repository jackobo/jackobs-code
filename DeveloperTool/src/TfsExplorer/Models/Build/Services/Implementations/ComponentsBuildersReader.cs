using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.Build
{
    public class ComponentsBuildersReader : IComponentsBuildersReader
    {
        public IEnumerable<IComponentBuilder> GetBuilders(IPublishPayload publishPayload, ComponentsFolder componentsFolder)
        {
            var componentsBuilders = new List<IComponentBuilder>();

            var factory = GetBuildersFactory(componentsFolder);

            AppendCoreComponentsBuilders(publishPayload, componentsBuilders, factory);

            AppendGameEnginesBuilders(publishPayload, componentsBuilders, factory);

            return componentsBuilders;

        }

        private static void AppendGameEnginesBuilders(IPublishPayload publishPayload, List<IComponentBuilder> componentsBuilders, IComponentBuilderFactory factory)
        {
            foreach (var gameEngine in publishPayload.GameEngines)
            {
                gameEngine.Version.Do(version =>
                {
                    componentsBuilders.Add(factory.GetGameEngineBuilder(gameEngine.Name, version));
                });

                AppendGamesBuilders(componentsBuilders, factory, gameEngine);
            }
        }

        private static void AppendGamesBuilders(List<IComponentBuilder> componentsBuilders, IComponentBuilderFactory factory, IGameEnginePublishPayload gameEngine)
        {
            foreach (var game in gameEngine.Games)
            {
                game.MathVersion.Do(version =>
                {
                    componentsBuilders.Add(factory.GetGameMathBuilder(game.Name, gameEngine.Name, version));
                });

                game.LimitsVersion.Do(version =>
                {
                    componentsBuilders.Add(factory.GetGameLimitsBuilder(game.Name, gameEngine.Name, version));
                });
            }
        }

        private static void AppendCoreComponentsBuilders(IPublishPayload publishPayload, List<IComponentBuilder> componentsBuilders, IComponentBuilderFactory factory)
        {
            foreach (var coreComponent in publishPayload.CoreComponents)
            {
                componentsBuilders.Add(factory.GetCoreComponentBuilder(coreComponent.Name, coreComponent.Version));
            }
        }

        private static IComponentBuilderFactory GetBuildersFactory(ComponentsFolder componentsFolder)
        {
            IBuildCustomizationProvider buildCustomizationProvider = new VoidCustomizationProvider();
          
            if(componentsFolder.BuildCustomizationXml.Exists())
            {
                buildCustomizationProvider = new BuildCustomizationProvider(componentsFolder.BuildCustomizationXml.ToSourceControlFile());
            }

            return new ComponentBuilderFactory(new GGPSolutionParser(componentsFolder, buildCustomizationProvider));
        }
    }
}
