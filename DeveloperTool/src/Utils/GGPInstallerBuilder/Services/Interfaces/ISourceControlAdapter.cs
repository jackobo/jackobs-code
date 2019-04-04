using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Logging;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace GGPInstallerBuilder
{
    public interface ISourceControlAdapter
    {
        IServerPath CreateServerPath(string serverPath);
        string ReadTextFile(IServerPath serverPath);
        void DownloadFolderContent(IServerPath serverPath, ILocalPath localPath, ILogger logger);
        void GetLatest(IServerPath serverPath);
        ILocalPath GetLocalPathFromServerPath(IServerPath serverPath);
    }
}
