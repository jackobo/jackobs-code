using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    [XmlRoot("buildCustomization")]
    public class BuildCustomizationConfiguration
    {
        [XmlArray(ElementName = "coreComponents")]
        [XmlArrayItem(ElementName = "coreComponent")]
        public List<CoreComponentCustomization> CoreComponents { get; set; } = new List<CoreComponentCustomization>();

        [XmlArray(ElementName = "gameEngines")]
        [XmlArrayItem(ElementName = "gameEngine")]
        public List<GameEngineCustomization> GameEngines { get; set; } = new List<GameEngineCustomization>();

        public class CoreComponentCustomization
        {
            [XmlAttribute("name")]
            public string Name { get; set; }

            [XmlElement("componentType")]
            public int? ComponentType { get; set; }

            
            [XmlAttribute("friendlyName")]
            public string FriendlyName { get; set; }

            [XmlArray(ElementName = "customizedFiles")]
            [XmlArrayItem(ElementName = "file")]
            public List<FileCustomization> CustomizedFiles { get; set; } = new List<FileCustomization>();
            
        }

        public class GameEngineCustomization
        {
            [XmlAttribute("name")]
            public string Name { get; set; }

            [XmlArray(ElementName = "customizedFiles")]
            [XmlArrayItem(ElementName = "file")]
            public List<FileCustomization> CustomizedFiles { get; set; } = new List<FileCustomization>();
        }

        public class FileCustomization
        {
            [XmlAttribute("buildOutputRelativePath")]
            public string BuildOutputRelativePath { get; set; }

            [XmlAttribute("distributionRelativePath")]
            public string DistributionRelativePath { get; set; }

            [XmlAttribute("environment")]
            public DeployEnvironment DeployEnvironment { get; set; }
        }
    }
}
