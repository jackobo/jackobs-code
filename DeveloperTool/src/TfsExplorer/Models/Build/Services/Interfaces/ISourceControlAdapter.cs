using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.Build
{
    public interface ISourceControlAdapter
    {
        void TryGetLatestVersion(ILocalPath filePath);
        void CheckoutForEdit(ILocalPath filePath);
        void PendAdd(ILocalPath filePath);
        void CheckInPendingChanges(string comment);
        int GetChangeSetId();
        IServerPath CreateServerPath(string serverPath);
        ILocalPath GetLocalPathFromServerPath(IServerPath serverPath);
        
        void GetLatest(IFolderHolder folder);
        bool HasPendingChanges(ILocalPath targetFile);
    }
}
