using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public interface ICompilableComponentContentProvider : IComponentContentProvider
    {
        IEnumerable<string> ReferencedAssemblies { get; }
        IEnumerable<ILocalPath> AssemblyInfoFiles { get; }
    }
}
