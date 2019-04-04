using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;

namespace Spark.TfsExplorer.Models.TFS
{
    public class TfsWorkspaceWrapper : ITfsWorkspace
    {
       
        public TfsWorkspaceWrapper(Workspace workspace)
        {
            _workspace = workspace;
        }

        Workspace _workspace;
        Workspace Workspace
        {
            get
            {
                UpdateWorkspaceInfoCache();
                return _workspace;
            }
        }

        private void UpdateWorkspaceInfoCache()
        {
            Workstation.Current.EnsureUpdateWorkspaceInfoCache(_workspace.VersionControlServer, _workspace.VersionControlServer.AuthorizedUser);
        }

        public VersionControlServer VersionControlServer
        {
            get
            {
                return Workspace.VersionControlServer;
            }
        }

        public int CheckInWithPolicyOverride(string comment, string[] files, string policyOverrideMessage = "")
        {
            return Workspace.ExecuteReturn(w => w.CheckInWithPolicyOverride(comment,
                                                                             w.GetPendingChanges(files),
                                                                             policyOverrideMessage));
        }

        public void GetFullLatest(string serverPath)
        {
            Workspace.ExecuteVoid(w => w.Get(new GetRequest(serverPath,
                                                           RecursionType.Full,
                                                           VersionSpec.Latest),
                                              GetOptions.GetAll));
        }

        public PendingChange[] GetPendingChanges(string localPath)
        {
            
            return Workspace.GetPendingChanges(localPath);
        }
        public string GetLocalItemForServerItem(string serverPath)
        {
            return Workspace.GetLocalItemForServerItem(serverPath);
        }

        public string GetServerItemForLocalItem(string localPath)
        {
            return Workspace.GetServerItemForLocalItem(localPath);
        }

        public void Map(string serverPath, string localPath)
        {
            Workspace.ExecuteVoid(w => w.Map(serverPath, localPath));
        }

        public void PendAdd(string path)
        {
            MapFolder(Path.GetDirectoryName(path));
            Workspace.ExecuteVoid(wks => wks.PendAdd(path));
        }

        private void MapFolder(string folderLocalPath)
        {

            var folderServerPath = Workspace.GetServerItemForLocalItem(folderLocalPath);

            if (Workspace.VersionControlServer.GetItems(folderServerPath, RecursionType.None)
                                          .Items.Any())
            {
                Workspace.Get(new GetRequest(folderServerPath, RecursionType.None, VersionSpec.Latest), GetOptions.GetAll);
            }
            else
            {
                if (!Directory.Exists(folderLocalPath))
                {
                    Directory.CreateDirectory(folderLocalPath);
                }
                Workspace.ExecuteVoid(wks => wks.Map(folderServerPath, folderLocalPath));
            }
        }

       

        public void PendEdit(string localPath)
        {
            Workspace.PendEdit(localPath);
        }

        public bool IsLocalPathMapped(string localPath)
        {
            return Workspace.IsLocalPathMapped(localPath);
        }

        public void Get(GetRequest request, GetOptions options)
        {
            Workspace.Get(request, options);
        }

        public void PendDelete(string localPath, RecursionType recursionType)
        {
            Workspace.PendDelete(localPath, recursionType);
        }

        public void PendRename(string oldName, string newName)
        {
            Workspace.PendRename(oldName, newName);
        }

        public void UndoPendingChanges(string[] files)
        {
            var pendingChanges = Workspace.GetPendingChanges(files)
                                        .Select(p => p.FileName)
                                        .ToArray();

            if (pendingChanges.Any())
                Workspace.Undo(pendingChanges);
        }

        public GetStatus Merge(string sourcePath, string targetPath, VersionSpec versionFrom, VersionSpec versionTo, LockLevel lockLevel, RecursionType recursion, MergeOptionsEx mergeOptions)
        {
            return Workspace.Merge(sourcePath, targetPath, versionFrom, versionTo, lockLevel, recursion, mergeOptions);
        }
    }
}
