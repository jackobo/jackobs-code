using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public class GameEngineBuilder : IComponentBuilder
    {
        private IGameEngineContentProvider _contentProvider;
        private VersionNumber _version;
        

        public GameEngineBuilder(IGameEngineContentProvider contentProvider, VersionNumber version)
        {
            _contentProvider = contentProvider;
            _version = version;
        }

        public void AppendContent(IBuildContext buildContext)
        {
            buildContext.DeploymentContentBuilder.AppendGameEngine(
                new GGPDeploymentContent.GameEngineDeployContent(_contentProvider.ComponentUniqueIdBuilder.Id.Value,
                                                                 _contentProvider.Name,
                                                                 _version,
                                                                 _contentProvider.GetProjectPath(),
                                                                 GetDeployableFilesDefinitions(),
                                                                 GetComponentDescriptionTxtContent(),
                                                                 ResolveDistributionServerPath(buildContext.BuildConfiguration.DistributionServerPath)));
        }

        public IEnumerable<IBuildAction> GetPreCompileActions()
        {
            return _contentProvider.AssemblyInfoFiles.Select(aif => new AssemblyInfoVersionUpdateAction(aif, _version)).ToList();
        }

        public IEnumerable<IBuildAction> GetDeployActions()
        {
            return _contentProvider.OutputFiles.Select(file => new DeployFileAction(this, file, _version)).ToList();
        }
        

        public IEnumerable<IBuildAction> GetPostCompileActions()
        {
            var actions = new List<IBuildAction>();
            actions.AddRange(_contentProvider.OutputFiles.Select(file => new CheckOutputFileExistAction(file)));
            actions.AddRange(_contentProvider.ComponentUniqueIdBuilder.GetBuildAction());

            return actions;
        }

        private IEnumerable<DeployableFileDefinition> GetDeployableFilesDefinitions()
        {
            return _contentProvider.OutputFiles.Select(f => f.GetDeployableFileDefinition());
        }

        public string GetComponentDescriptionTxtContent()
        {

            return GenericComponentDescriptionBuilder.BuildComponentDescriptionProperties(
                        GetUniqueComponentID(),
                        _version, 
                        _contentProvider.OutputFiles, 
                        _contentProvider.Name,
                        _contentProvider.GetProjectPath()).ToString();
                
        }
        
        StringKeyValue GetUniqueComponentID()
        {
            return new StringKeyValue(AntPropertyNames.GameEngineUniqueID, _contentProvider.ComponentUniqueIdBuilder.Id.Value);
        }

        public ILocalPath ResolveDistributionLocalPath(ILocalPath basePath)
        {
            return basePath.Subpath(Folders.EnginesAndGamesFolder.WellKnownName)
                           .Subpath(_contentProvider.Name)
                           .Subpath(Folders.EngineFolder.WellKnownName)
                           .Subpath(_version.ToString());
        }

        public IServerPath ResolveDistributionServerPath(IServerPath basePath)
        {
            return basePath.Subpath(Folders.EnginesAndGamesFolder.WellKnownName)
                           .Subpath(_contentProvider.Name)
                           .Subpath(Folders.EngineFolder.WellKnownName)
                           .Subpath(_version.ToString());
        }
    }
}
