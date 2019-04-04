using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public interface IComponentBuilderFactory
    {
        IComponentBuilder GetCoreComponentBuilder(string coreComponentName, VersionNumber proposedVersion);
        IComponentBuilder GetGameEngineBuilder(GameEngineName engineName, VersionNumber proposedVersion);
        IComponentBuilder GetGameMathBuilder(string gameName, GameEngineName engineName, VersionNumber proposedVersion);
        IComponentBuilder GetGameLimitsBuilder(string gameName, GameEngineName engineName, VersionNumber proposedVersion);
    }
}
