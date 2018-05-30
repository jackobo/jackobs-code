using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public class GameLimitsBuilder : IComponentBuilder
    {
        private IGameLimitsContentProvider _limitsContent;
        GameEngineName _engineName;
        private string _gameName;
        private VersionNumber _version;
        

        public GameLimitsBuilder(string gameName, GameEngineName engineName, IGameLimitsContentProvider limitsContent, VersionNumber version)
        {
            _gameName = gameName;
            _engineName = engineName;
            _limitsContent = limitsContent;
            _version = version;
        }

        public void AppendContent(IBuildContext buildContext)
        {
            buildContext.DeploymentContentBuilder.AppendGameLimits(
                new GGPDeploymentContent.GameLimitsDeployContent(
                _limitsContent.ComponentUniqueIdBuilder.Id.Value,
                _gameName,
                _version,
                _engineName,
                _limitsContent.EngineUniqueIdBuilder.Id.Value,
                _limitsContent.GetProjectPath(),
                _limitsContent.OutputFiles.Select(f => f.GetDeployableFileDefinition()),
                GetComponentDescriptionTxtContent(),
                ResolveDistributionServerPath(buildContext.BuildConfiguration.DistributionServerPath)
                ));
        }

        public IEnumerable<IBuildAction> GetPreCompileActions()
        {
            return _limitsContent.LimitsFiles.Select(f => new GameFileVersionUpdateAction(f.SourceFile, _version)).ToList();
        }

        public IEnumerable<IBuildAction> GetDeployActions()
        {
            return _limitsContent.OutputFiles.Select(f => new DeployFileAction(this, f, _version));
        }

        public IEnumerable<IBuildAction> GetPostCompileActions()
        {
            var actions = new List<IBuildAction>();
            actions.AddRange(_limitsContent.OutputFiles.Select(f => new CheckOutputFileExistAction(f)));
            actions.AddRange(_limitsContent.ComponentUniqueIdBuilder.GetBuildAction());
            
            return actions;

        }

        public string GetComponentDescriptionTxtContent()
        {
            return GenericComponentDescriptionBuilder.BuildGameDescriptionProperties(_limitsContent.ComponentUniqueIdBuilder.Id, 
                                                                                     _gameName, 
                                                                                     _version, 
                                                                                     _limitsContent.EngineUniqueIdBuilder.Id, 
                                                                                     "Limits", 
                                                                                     _limitsContent.GetProjectPath())
                    .ToString();
        }

        public ILocalPath ResolveDistributionLocalPath(ILocalPath basePath)
        {
            return basePath.Subpath(Folders.EnginesAndGamesFolder.WellKnownName)
                          .Subpath(_engineName.ToString())
                          .Subpath(Folders.GamesFolder.WellKnownName)
                          .Subpath(_gameName)
                          .Subpath(Folders.GameLimitsFolder.WellKnownName)
                          .Subpath(_version.ToString());
        }

        private IServerPath ResolveDistributionServerPath(IServerPath basePath)
        {
            return basePath.Subpath(Folders.EnginesAndGamesFolder.WellKnownName)
                        .Subpath(_engineName.ToString())
                        .Subpath(Folders.GamesFolder.WellKnownName)
                        .Subpath(_gameName)
                        .Subpath(Folders.GameLimitsFolder.WellKnownName)
                        .Subpath(_version.ToString());
        }
    }
}
