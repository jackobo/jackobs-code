using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public interface IDeploymentContentBuilder
    {
        void AppendCoreComponent(GGPDeploymentContent.CoreComponentDeployContent coreComponent);
        void AppendGameEngine(GGPDeploymentContent.GameEngineDeployContent gameEngine);
        void AppendGameLimits(GGPDeploymentContent.GameLimitsDeployContent gameLimits);
        void AppendGameMath(GGPDeploymentContent.GameMathDeployContent gameMath);

        GGPDeploymentContent Build();
    }
}
