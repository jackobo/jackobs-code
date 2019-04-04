using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Build
{
    public interface IGameEngineContentProvider : ICompilableComponentContentProvider
    {
        IGameContentProvider GetGame(string gameName);
        IEnumerable<IComponentUniqueIdBuilder> GetUniqueIdBuilders();
    }
}
