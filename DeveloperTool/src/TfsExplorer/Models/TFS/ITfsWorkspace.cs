using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.TFS
{
    public interface ITfsWorkspace
    {
        VersionControlServer VersionControlServer { get; }
        string GetLocalItemForServerItem(string serverPath);
        string GetServerItemForLocalItem(string localPath);
        void Map(string serverPath, string localPath);
        void PendAdd(string path);
        int CheckInWithPolicyOverride(string comment, string[] files, string policyOverrideMessage = "");
        void GetFullLatest(string serverPath);
        void Get(GetRequest getRequest, GetOptions options);
        PendingChange[] GetPendingChanges(string localPath);
        void PendEdit(string localPath);
        bool IsLocalPathMapped(string localPath);
        void PendDelete(string localPath, RecursionType recursionType);
        void PendRename(string oldPath, string newPath);
        void UndoPendingChanges(string[] files);
        GetStatus Merge(string sourcePath, string targetPath, VersionSpec versionFrom, VersionSpec versionTo, LockLevel lockLevel, RecursionType recursion, MergeOptionsEx mergeOptions);
    }
}
