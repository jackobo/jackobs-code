using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Interfaces
{
    public abstract class LogicalComponentVisitor : ILogicalComponentVisitor
    {
        

        public virtual void Visit(IGameEngineComponent gameEngine){}

        public virtual void Visit(IGameComponent game){}

        public virtual void Visit(ICoreComponent coreComponent){}

        public virtual void Visit(IServerPath serverPath) { }
    }
}
