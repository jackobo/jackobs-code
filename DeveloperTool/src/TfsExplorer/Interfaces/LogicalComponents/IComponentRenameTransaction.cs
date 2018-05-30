using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Interfaces
{
    public interface IComponentRenameTransaction
    {
        IServerPath OldServerPath { get; }
        IServerPath NewServerPath { get; }
        void Commit();
    }
}
