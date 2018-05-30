using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Build
{
    public interface IGameContentProvider
    {
        string Name { get; }
        IGameMathContentProvider MathContent { get; }
        IGameLimitsContentProvider LimitsContent { get; }
    }

    public interface IGameMathContentProvider : IComponentContentProvider
    {
        IEnumerable<IGameOutputFile> MathFiles { get; }
        IComponentUniqueIdBuilder EngineUniqueIdBuilder { get; }
    }

    public interface IGameLimitsContentProvider : IComponentContentProvider
    {
        IEnumerable<IGameOutputFile> LimitsFiles { get; }
        IComponentUniqueIdBuilder EngineUniqueIdBuilder { get; }
    }
}
