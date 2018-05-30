using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public interface IBranchFolder : IFolderHolder
    {
        ComponentsFolder Components { get; }
        BuildToolsFolder BuildTools { get; }
    }
}
