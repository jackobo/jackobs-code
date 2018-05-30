using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.TFS
{
    public interface ITfsGateway
    {
        IEnumerable<IServerPath> GetSubfolders(IServerPath folder);
        Optional<IServerPath> TryGetSubFolder(IServerPath parentServerPath, string subfolderName);
        IServerPath CreateSubfolder(IServerPath parentServerPath, string subfolderName);
        void Branch(IServerPath sourceServerPath, IServerPath targetServerPath, bool createBranchObject);
        void Branch(IServerPath sourceServerPath, IServerPath targetServerPath, bool createBranchObject, int changeSet);
        IServerPath GetRootServerPath();
        bool HasPendingChanges(ILocalPath filePath);
        IEnumerable<MergeCandidate> GetMergeCanditates(IServerPath sourceBranchServerPath, IServerPath targetBranchServerPath);
        IEnumerable<ISourceControlFolder> GetRoots();
        GetStatus Merge(IServerPath sourceBranchServerPath, IServerPath targetBranchServerPath);
        ICheckinTransaction CreateCheckInTransaction();
        int CheckinPendingChanges(string[] files, string comment, string policyOverrideMessage = "");
        Changeset GetLatestChangeSet(IServerPath serverPath);
        GetStatus MergeChangeSet(IServerPath sourceBranchServerPath, int changesetId, IServerPath targetBranchServerPath);
        void TryGetLatest(ILocalPath filePath);
        Optional<IServerPath> TryGetFile(IServerPath parentFolderServerPath, string fileName);
        ILocalPath GetLocalPathForServerPath(IServerPath serverPath);
        IServerPath GetServerPathFromLocalPath(ILocalPath localPath);
        byte[] GetFileContent(IServerPath fileServerPath);
        void GetLatest(IServerPath serverPath);
        void PendAdd(ILocalPath localFilePath);
        void CheckoutForEdit(ILocalPath filePath);
        IEnumerable<Changeset> QueryHistory(IServerPath serverPath, int sinceThisChangeSet);
        void CreateFile(IServerPath folderServerPath, string fileName, byte[] content);
        IServerPath CreateServerPath(string serverPath);
        void UpdateFileContent(IServerPath fileServerPath, byte[] content);
        IEnumerable<WorkItem> GetRelatedWorkItems(IEnumerable<Changeset> changeSets);
        IRootFolder GetRootFolder(RootBranchVersion rootVersion);
        IHistoryCache CreateHistoryCache(IServerPath serverPath, IChangeSet changeSet);
        IRootFolder CreateRootFolder(RootBranchVersion newRootVersion);
    }
}
