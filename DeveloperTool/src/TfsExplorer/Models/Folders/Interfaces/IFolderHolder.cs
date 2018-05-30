using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Folders
{
    public interface IFolderHolder
    {
        ISourceControlFolder ToSourceControlFolder();
        Optional<ISourceControlFolder> TryGetSubfolder(string subfolderName);
        string Name { get; }
        bool Exists();
        ISourceControlFolder Create();
        IServerPath GetServerPath();

        IRootFolder Root { get; }
    }

   
}
