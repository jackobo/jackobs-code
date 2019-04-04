using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.Interfaces
{
    public interface IPublishPayload
    {
        bool GenerateCustomizedInstaller { get; }
        IEnumerable<ICoreComponentPublishPayload> CoreComponents { get; }
        IEnumerable<IGameEnginePublishPayload> GameEngines { get; }
    }

    public interface IPublishPayloadBuilder
    {
        bool GenerateCustomizedInstaller { get; set; }
        void AddCoreComponent(string coreComponentName, VersionNumber version);
        void AddGameEngine(GameEngineName engineName, VersionNumber version);
        void AddGameMath(string gameName, GameEngineName engineName, VersionNumber mathVersion);
        void AddGameLimits(string gameName, GameEngineName engineName, VersionNumber limitsVersion);

        IPublishPayload Build();
    }

    public interface ICoreComponentPublishPayload
    {
        string Name { get; }
        VersionNumber Version { get; }
    }

    public interface IGameEnginePublishPayload
    {
        GameEngineName Name { get; }
        Optional<VersionNumber> Version { get; }
        IEnumerable<IGamePublishPayload> Games { get; }
    }

    public interface IGamePublishPayload
    {
        string Name { get;}
        Optional<VersionNumber> MathVersion { get; }
        Optional<VersionNumber> LimitsVersion { get; }

    }
}
