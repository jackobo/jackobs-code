using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Interfaces
{
    public interface ILogicalComponentVisitor
    {
        void Visit(ICoreComponent coreComponent);
        void Visit(IGameEngineComponent gameEngine);
        void Visit(IGameComponent game);
        void Visit(IServerPath location);
    }

    public interface ILogicalComponentVisitor<T> : ILogicalComponentVisitor
    {
        T ProduceResult();
    }
}
