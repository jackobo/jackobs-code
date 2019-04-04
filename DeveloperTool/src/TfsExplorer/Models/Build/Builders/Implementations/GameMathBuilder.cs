using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public class GameMathBuilder : IComponentBuilder
    {
        private IGameMathContentProvider _mathContent;
        private string _gameName;
        GameEngineName _engineName;
        private VersionNumber _version;
        public GameMathBuilder(string name, GameEngineName engineName, IGameMathContentProvider mathContent, VersionNumber version)
        {
            _gameName = name;
            _engineName = engineName;
            _mathContent = mathContent;
            _version = version;
        }

        public void AppendContent(IBuildContext builderContext)
        {
            builderContext.DeploymentContentBuilder.AppendGameMath(
                new Build.GGPDeploymentContent.GameMathDeployContent(
                _mathContent.ComponentUniqueIdBuilder.Id.Value,
                _gameName,
                _version,
                _engineName,
                _mathContent.EngineUniqueIdBuilder.Id.Value,
                _mathContent.GetProjectPath(),
                _mathContent.OutputFiles.Select(f => f.GetDeployableFileDefinition()),
                GetComponentDescriptionTxtContent(),
                ResolveDistributionServerPath(builderContext.BuildConfiguration.DistributionServerPath)));
        }

        public IEnumerable<IBuildAction> GetPreCompileActions()
        {
            return _mathContent.MathFiles.Select(f => new GameFileVersionUpdateAction(f.SourceFile, _version)).ToList();
        }

        public IEnumerable<IBuildAction> GetDeployActions()
        {
            return _mathContent.OutputFiles.Select(f => new DeployFileAction(this, f, _version));
        }

        public IEnumerable<IBuildAction> GetPostCompileActions()
        {
            var actions = new List<IBuildAction>();
            actions.AddRange(_mathContent.OutputFiles.Select(f => new CheckOutputFileExistAction(f)));
            actions.AddRange(_mathContent.ComponentUniqueIdBuilder.GetBuildAction());

            return actions;

        }

        public string GetComponentDescriptionTxtContent()
        {
            return GenericComponentDescriptionBuilder.BuildGameDescriptionProperties(
                                                     _mathContent.ComponentUniqueIdBuilder.Id, 
                                                     _gameName, 
                                                     _version,
                                                     _mathContent.EngineUniqueIdBuilder.Id, 
                                                     "Math",
                                                     _mathContent.GetProjectPath())
                    .ToString();

        }




        public ILocalPath ResolveDistributionLocalPath(ILocalPath basePath)
        {
            return basePath.Subpath(Folders.EnginesAndGamesFolder.WellKnownName)
                          .Subpath(_engineName.ToString())
                          .Subpath(Folders.GamesFolder.WellKnownName)
                          .Subpath(_gameName)
                          .Subpath(Folders.GameMathFolder.WellKnownName)
                          .Subpath(_version.ToString());
        }

        public IServerPath ResolveDistributionServerPath(IServerPath basePath)
        {
            return basePath.Subpath(Folders.EnginesAndGamesFolder.WellKnownName)
                         .Subpath(_engineName.ToString())
                         .Subpath(Folders.GamesFolder.WellKnownName)
                         .Subpath(_gameName)
                         .Subpath(Folders.GameMathFolder.WellKnownName)
                         .Subpath(_version.ToString());
        }
    }
}
