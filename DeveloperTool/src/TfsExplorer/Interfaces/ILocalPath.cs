using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Interfaces
{
    public interface ILocalPath
    {
        string AsString();
        ILocalPath Subpath(string name);
        string GetName();
    }
}
