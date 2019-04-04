using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.Interfaces
{
    public interface ILogicalComponent
    {
        string Name { get; }
        void AcceptCommandVisitor(Func<ILogicalComponentVisitor> visitorFactory);
        T AcceptQueryVisitor<T>(Func<ILogicalComponentVisitor<T>> visitorFactory);

        Optional<T> As<T>();
        bool SameAs(ILogicalComponent component);

        bool AllowRename { get; }
        bool AllowDelete { get; }

        IComponentRenameTransaction CreateRenameTransaction(string newName);

        IComponentDeleteTransaction CreateDeleteTransaction();

    }
}
