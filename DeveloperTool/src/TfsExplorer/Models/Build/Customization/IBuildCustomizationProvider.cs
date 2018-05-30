using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public interface IBuildCustomizationProvider
    {
        IEnumerable<BuildOutputFileDefinition> GetCoreComponentCustomizedOutputFiles(string componentName);
        Optional<CoreComponentCustomizationMetaData> GetCoreComponentCustomizationMetaData(string componentName);
        IEnumerable<BuildOutputFileDefinition> GetGameEngineCustomizedOutputFiles(string name);
        
    }

    public class BuildOutputFileDefinition
    {
        public BuildOutputFileDefinition(string buildOutputRelativePath, string distributionRelativePath, DeployEnvironment deployEnvironment)
        {
            this.BuildOutputRelativePath = buildOutputRelativePath;
            this.DistributionRelativePath = distributionRelativePath;
            this.DeployEnvironment = deployEnvironment;
        }

        
        public string BuildOutputRelativePath { get; private set; }
        public string DistributionRelativePath { get; private set; }
        public DeployEnvironment DeployEnvironment { get; private set; }
    }

}
