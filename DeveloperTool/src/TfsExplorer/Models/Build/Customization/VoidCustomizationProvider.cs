using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.Models.Build
{
    public class VoidCustomizationProvider : IBuildCustomizationProvider
    {
        
        public Optional<CoreComponentCustomizationMetaData> GetCoreComponentCustomizationMetaData(string componentName)
        {
            return Optional<CoreComponentCustomizationMetaData>.None();
        }

        public IEnumerable<BuildOutputFileDefinition> GetCoreComponentCustomizedOutputFiles(string componentName)
        {
            return new BuildOutputFileDefinition[0];
        }

        public IEnumerable<BuildOutputFileDefinition> GetGameEngineCustomizedOutputFiles(string name)
        {
            return new BuildOutputFileDefinition[0];
        }
    }
}
