using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    internal static class GenericComponentDescriptionBuilder
    {
        public static StringKeyValueCollection BuildComponentDescriptionProperties(StringKeyValue componentUniqueID, VersionNumber version, IEnumerable<IOutputFile> files, string friendlyName, IServerPath projectPath)
        {
            var properties = new StringKeyValueCollection();
            properties.Add(new StringKeyValue(AntPropertyNames.major, version.Major.ToString()));
            properties.Add(new StringKeyValue(AntPropertyNames.minor, version.Minor.ToString()));
            properties.Add(new StringKeyValue(AntPropertyNames.revision, version.Revision.ToString()));
            properties.Add(new StringKeyValue(AntPropertyNames.build, version.Build.ToString()));
            properties.Add(componentUniqueID);

            properties.Add(new StringKeyValue(AntPropertyNames.ProjDir, projectPath.AsString()));

            properties.Add(new StringKeyValue(AntPropertyNames.FriendlyName, friendlyName));

            properties.Add(new StringKeyValue(AntPropertyNames.TestingDeployFiles, string.Join(", ", files.Select(f => f.GetDeployableFileDefinition()).Where(f => f.Environment == DeployEnvironment.All || f.Environment == DeployEnvironment.QA).Select(f => f.RelativePath))));

            properties.Add(new StringKeyValue(AntPropertyNames.ProductionDeployFiles, string.Join(", ", files.Select(f => f.GetDeployableFileDefinition()).Where(f => f.Environment == DeployEnvironment.All || f.Environment == DeployEnvironment.Production).Select(f => f.RelativePath))));

            properties.Add(new StringKeyValue(AntPropertyNames.PublishDate, DateTime.Now.ToString()));

            return properties;
        }

        public static StringKeyValueCollection BuildGameDescriptionProperties(IComponentUniqueId componentUniqueId, string gameName, VersionNumber version, IComponentUniqueId engineUniqueId, string gamePart, IServerPath projectPath)
        {
            var properties = new StringKeyValueCollection();
            properties.Add(new StringKeyValue(AntPropertyNames.GamePartUniqueID, componentUniqueId.Value));
            properties.Add(new StringKeyValue(AntPropertyNames.GameUniqueName, gameName));
            properties.Add(new StringKeyValue(AntPropertyNames.GameVersion, version.ToString()));
            properties.Add(new StringKeyValue(AntPropertyNames.ParentGameEngineUniqueID, engineUniqueId.Value));
            properties.Add(new StringKeyValue(AntPropertyNames.GamePart, gamePart));
            properties.Add(new StringKeyValue(AntPropertyNames.ProjDir, projectPath.AsString()));
            properties.Add(new StringKeyValue(AntPropertyNames.PublishDate, DateTime.Now.ToString()));
            return properties;
        }
    }
}
