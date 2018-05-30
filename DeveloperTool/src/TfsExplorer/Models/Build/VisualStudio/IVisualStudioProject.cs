using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.Models.Build
{
    public interface IVisualStudioProject
    {
        IEnumerable<string> OutputFilesNames { get; }
        IEnumerable<string> ReferencedAssemblies { get; }

        Optional<string> AssemblyInfoFile { get; }
    }
}
