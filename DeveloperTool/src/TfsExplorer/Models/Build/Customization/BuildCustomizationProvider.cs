using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public class BuildCustomizationProvider : IBuildCustomizationProvider
    {
        public BuildCustomizationProvider(ISourceControlFile buildCustomizationXml)
        {
            _configuration = ReadCustomization(buildCustomizationXml);
        }

        BuildCustomizationConfiguration _configuration;

        private BuildCustomizationConfiguration ReadCustomization(ISourceControlFile buildCustomizationXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(BuildCustomizationConfiguration));
            using (var memStream = new MemoryStream(buildCustomizationXml.GetContent()))
            using (var xmlReader = XmlReader.Create(memStream))
            {
                return (BuildCustomizationConfiguration)xmlSerializer.Deserialize(xmlReader);
            }
        }

        Optional<BuildCustomizationConfiguration.CoreComponentCustomization> FindCoreComponentCustomization(string componentName)
        {
            var coreComponent = _configuration.CoreComponents.FirstOrDefault(c => c.Name == componentName);
            if (coreComponent == null)
                return Optional<BuildCustomizationConfiguration.CoreComponentCustomization>.None();
            else
                return Optional<BuildCustomizationConfiguration.CoreComponentCustomization>.Some(coreComponent);
        }

        Optional<BuildCustomizationConfiguration.GameEngineCustomization> FindGameEngineCustomization(string gameEngineName)
        {
            var gameEngine = _configuration.GameEngines.FirstOrDefault(c => c.Name == gameEngineName);
            if (gameEngine == null)
                return Optional<BuildCustomizationConfiguration.GameEngineCustomization>.None();
            else
                return Optional<BuildCustomizationConfiguration.GameEngineCustomization>.Some(gameEngine);
        }

        public IEnumerable<BuildOutputFileDefinition> GetCoreComponentCustomizedOutputFiles(string componentName)
        {
            var result = new List<BuildOutputFileDefinition>();

            FindCoreComponentCustomization(componentName)
                .Do(c =>
                {
                    result = c.CustomizedFiles.Select(f => new BuildOutputFileDefinition(f.BuildOutputRelativePath, f.DistributionRelativePath, f.DeployEnvironment))
                                                 .ToList();
                });


            return result;

        }

        public IEnumerable<BuildOutputFileDefinition> GetGameEngineCustomizedOutputFiles(string engineName)
        {
            var result = new List<BuildOutputFileDefinition>();

            FindGameEngineCustomization(engineName)
                .Do(e =>
                {
                    result = e.CustomizedFiles.Select(f => new BuildOutputFileDefinition(f.BuildOutputRelativePath, f.DistributionRelativePath, f.DeployEnvironment))
                                             .ToList();
                });

            return result;
        }

        public Optional<CoreComponentCustomizationMetaData> GetCoreComponentCustomizationMetaData(string componentName)
        {
            var result = Optional<CoreComponentCustomizationMetaData>.None();

            FindCoreComponentCustomization(componentName)
                .Do(e =>
                {
                    result = Optional<CoreComponentCustomizationMetaData>.Some(new CoreComponentCustomizationMetaData(e.ComponentType, e.FriendlyName));
                });

            return result;
        }
    }
}
