using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public class CoreComponentBuilder : IComponentBuilder
    {
        public CoreComponentBuilder(ICoreComponentContentProvider contentProvider, VersionNumber version)
        {
            _contentProvider = contentProvider;
            _version = version;
            
        }
        
        ICoreComponentContentProvider _contentProvider;
        VersionNumber _version;
        
        public void AppendContent(IBuildContext buildContext)
        {
            
            buildContext.DeploymentContentBuilder.AppendCoreComponent(
                new GGPDeploymentContent.CoreComponentDeployContent(
                _contentProvider.ComponentUniqueIdBuilder.Id.Value,
                _contentProvider.Name,
                _version,
                _contentProvider.GetProjectPath(),
                _contentProvider.GetCustomizationMetaData(),
                GetDeployableFilesDefinitions(),
                GetComponentDescriptionTxtContent(),
                ResolveDistributionServerPath(buildContext.BuildConfiguration.DistributionServerPath)));            
        }

        public string GetComponentDescriptionTxtContent()
        {
            
            return GenericComponentDescriptionBuilder.BuildComponentDescriptionProperties(
                                                                GetUniqueComponentID(),
                                                                _version,
                                                                _contentProvider.OutputFiles,
                                                                _contentProvider.GetCustomizationMetaData().FirstOrDefault()?.FriendlyName ?? _contentProvider.Name,
                                                                _contentProvider.GetProjectPath())
                                                     .ToString();
            
            
        }

        private IEnumerable<DeployableFileDefinition> GetDeployableFilesDefinitions()
        {
            return _contentProvider.OutputFiles.Select(f => f.GetDeployableFileDefinition());
        }

      
        StringKeyValue GetUniqueComponentID()
        {
            string componentUniqueId = _contentProvider.ComponentUniqueIdBuilder.Id.Value;

            var componentType = _contentProvider.GetCustomizationMetaData().FirstOrDefault()?.ComponentType;
            if (componentType == null)
                return new StringKeyValue(AntPropertyNames.CoreComponentUniqueID, componentUniqueId);

            switch (componentType.Value)
            {
                case 10:
                    return new StringKeyValue(AntPropertyNames.GGPGameServerUniqueID, componentUniqueId);
                case 20:
                    return new StringKeyValue(AntPropertyNames.GameCommonInterfaceUniqueID, componentUniqueId);
                case 30:
                    return new StringKeyValue(AntPropertyNames.GamesCommonUniqueID, componentUniqueId);
                case 39:
                    return new StringKeyValue(AntPropertyNames.GGPConfigurationEditorUniqueID, componentUniqueId);
                default:
                    throw new ApplicationException($"Can't handle specific component type {componentType.Value}");

            }
        }

        public IEnumerable<IBuildAction> GetPreCompileActions()
        {
            return _contentProvider.AssemblyInfoFiles.Select(asmInfo => new AssemblyInfoVersionUpdateAction(asmInfo, _version)).ToList();
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

        public ILocalPath ResolveDistributionLocalPath(ILocalPath basePath)
        {
            return basePath.Subpath(Folders.CoreFolder.WellKnownName)
                          .Subpath(_contentProvider.Name)
                          .Subpath(_version.ToString());
        }

        private IServerPath ResolveDistributionServerPath(IServerPath basePath)
        {
            return basePath.Subpath(Folders.CoreFolder.WellKnownName)
                         .Subpath(_contentProvider.Name)
                         .Subpath(_version.ToString());
        }
    }
}
