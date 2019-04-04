using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Spark.Infra.Types;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.TFS
{
    public class TfsGateway : ITfsGateway
    {
        public TfsGateway(IFileSystemManager fileSystemManager, IWorkspaceSelector workspaceSelector, IThreadingServices threadingServices)
        {
            _fileSystemManager = fileSystemManager;
            _workspaceSelector = workspaceSelector;
            this.Tfs = TfsCollectionFactory.Create();
            _tfsCache = new TfsCache(Tfs, ROOT_FOLDER, threadingServices);
            _historyCacheManager = new HistoryCacheManager(this.VersionControlServer);

        }

        public static string ROOT_FOLDER
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["GGPBranchesFolder"];
            }
        }

        IFileSystemManager _fileSystemManager;
        IWorkspaceSelector _workspaceSelector;

        VersionControlServer VersionControlServer
        {
            get
            {
                return Tfs.GetService<VersionControlServer>();
            }
        }


        TfsCache _tfsCache;

        TfsTeamProjectCollection Tfs { get; set; }
        
        public IEnumerable<IServerPath> GetSubfolders(IServerPath serverPath)
        {
            return _tfsCache.GetSubfolders(serverPath.AsString())
                        .Select(item => CreateServerPathDescriptor(item.ServerItem))
                        .ToList();
        }

        IServerPath CreateServerPathDescriptor(string path)
        {
            return new ServerPath(path);
        }

        ILocalPath CreateLocalPath(string path)
        {
            return new LocalPath(path);
        }

        public void CheckoutForEdit(ILocalPath localFilePath)
        {
            GetWorkspace().PendEdit(localFilePath.AsString());
        }

        public IRootFolder CreateRootFolder(RootBranchVersion newRootVersion)
        {
            var root = CreateServerPathDescriptor(ROOT_FOLDER);
            var serverPath = CreateSubfolder(root, newRootVersion.ToString());
            var localPath = GetLocalPathForServerPath(serverPath);
            return new RootBranchFolder(root,
                                      new TfsFolder(serverPath, localPath, this));


        }

        public IRootFolder GetRootFolder(RootBranchVersion rootVersion)
        {

            var root = CreateServerPathDescriptor(ROOT_FOLDER);
            var serverPath = TryGetSubFolder(root, rootVersion.ToString()).First();
            var localPath = GetLocalPathForServerPath(serverPath);

            return new RootBranchFolder(root,
                                        new TfsFolder(serverPath, localPath, this));
        }

        
        public IServerPath CreateSubfolder(IServerPath parentServerPath, string subfolderName)
        {

            var workspace = GetWorkspace();
            var localParentFolder = workspace.GetLocalItemForServerItem(parentServerPath.AsString());
            var subfolderLocalPath = Path.Combine(localParentFolder, subfolderName);
            var subfolderServerPath = parentServerPath.Subpath(subfolderName);
            _fileSystemManager.CreateFolder(subfolderLocalPath);

            workspace.Map(subfolderServerPath.AsString(), subfolderLocalPath);
            workspace.PendAdd(subfolderLocalPath);
            

            workspace.CheckInWithPolicyOverride($"Create folder {subfolderName}",
                                                new string[] { subfolderLocalPath },
                                                "This folder was created automatically by the GGP Developer Tool.");

            _tfsCache.AddFolderToCache(subfolderServerPath.AsString());
            return subfolderServerPath;
        }


        private ITfsWorkspace GetWorkspace()
        {
            
            var w = VersionControlServer.QueryWorkspaces(_workspaceSelector.SelectedWorkspaceName, 
                                                        VersionControlServer.AuthorizedUser, 
                                                        Environment.MachineName)
                                        .FirstOrDefault();

            if (w == null)
                throw new InvalidOperationException($"There is no TFS workspace named {_workspaceSelector.SelectedWorkspaceName}");

            return new TfsWorkspaceWrapper(w);
        }

        public Optional<IServerPath> TryGetSubFolder(IServerPath parentServerPath, string subfolderName)
        {
            var result = Optional<IServerPath>.None();

            var subfolderServerPath = parentServerPath.Subpath(subfolderName);

            _tfsCache.FindFolder(subfolderServerPath.AsString())
                         .Do(item => result = Optional<IServerPath>.Some(subfolderServerPath));

            return result;
        }

        public Optional<IServerPath> TryGetFile(IServerPath parentFolderServerPath, string fileName)
        {
            var fileServerPath = parentFolderServerPath.Subpath(fileName);

            Optional<Item> tfsItem = _tfsCache.FindFile(fileServerPath.AsString());

            if (tfsItem.Any())
                return Optional<IServerPath>.Some(fileServerPath);
            else
                return Optional<IServerPath>.None();
        }

        
        private void Branch(IServerPath sourceServerPath, IServerPath targetServerPath, bool createBranchObject, VersionSpec versionSpec)
        {
            var server = this.VersionControlServer;

            server.CreateBranch(sourceServerPath.AsString(),
                                targetServerPath.AsString(),
                                versionSpec,
                                null,
                                "Branch from " + sourceServerPath.AsString(),
                                null,
                                null,
                                new Mapping[0]
                                );


            if (createBranchObject)
            {
                var itemIdentifier = new ItemIdentifier(targetServerPath.AsString());
                if (!server.QueryBranchObjects(itemIdentifier, RecursionType.None).Any())
                    server.CreateBranchObject(new BranchProperties(itemIdentifier));
            }

            _tfsCache.Refresh();
        }


        public void Branch(IServerPath sourceServerPath, IServerPath targetServerPath, bool createBranchObject)
        {
            Branch(sourceServerPath, targetServerPath, createBranchObject, VersionSpec.Latest);
        }

        public void Branch(IServerPath sourceServerPath, IServerPath targetServerPath, bool createBranchObject, int changeSet)
        {
            Branch(sourceServerPath, targetServerPath, createBranchObject, new ChangesetVersionSpec(changeSet));
        }


        public IEnumerable<MergeCandidate> GetMergeCanditates(IServerPath sourceBranchServerPath, IServerPath targetBranchServerPath)
        {
            return this.VersionControlServer.GetMergeCandidates(sourceBranchServerPath.AsString(), targetBranchServerPath.AsString(), RecursionType.Full);
        }

        public GetStatus Merge(IServerPath sourceBranchServerPath, IServerPath targetBranchServerPath)
        {
            
            var result = GetWorkspace().Merge(sourceBranchServerPath.AsString(), 
                                              targetBranchServerPath.AsString(), 
                                              null, 
                                              null, 
                                              LockLevel.None, 
                                              RecursionType.Full, 
                                              MergeOptionsEx.Conservative);

            _tfsCache.Refresh();

            return result;


        }

        public GetStatus MergeChangeSet(IServerPath sourceBranchServerPath, int changesetId, IServerPath targetBranchServerPath)
        {
            var changeSetVersionSpec = new ChangesetVersionSpec(changesetId);
            var result = GetWorkspace().Merge(sourceBranchServerPath.AsString(), 
                                              targetBranchServerPath.AsString(),
                                              changeSetVersionSpec,
                                              changeSetVersionSpec, 
                                              LockLevel.None, 
                                              RecursionType.Full, 
                                              MergeOptionsEx.Conservative);

            _tfsCache.Refresh();
            return result;
        }

        
        public Changeset GetLatestChangeSet(IServerPath serverPath)
        {
            return this.VersionControlServer.QueryLatestChangeset(serverPath.AsString(), true);
        }
        
        HistoryCacheManager _historyCacheManager;
        public IHistoryCache CreateHistoryCache(IServerPath serverPath, IChangeSet changeSet)
        {
            return _historyCacheManager.CreateCache(serverPath, changeSet.Id);
        }
       
        public IEnumerable<Changeset> QueryHistory(IServerPath serverPath, int sinceThisChangeSet)
        {
            return _historyCacheManager.QueryHistory(serverPath, sinceThisChangeSet);
            
        }

        public void CreateFile(IServerPath folderServerPath, string fileName, byte[] content)
        {
            var workspace = GetWorkspace();
            var fileServerPath = folderServerPath.Subpath(fileName);
            var fileLocalPath = workspace.GetLocalItemForServerItem(fileServerPath.AsString());
            _fileSystemManager.WriteFileContent(fileLocalPath, content);
            
            workspace.PendAdd(fileLocalPath);

            workspace.CheckInWithPolicyOverride($"Add file {fileName}",
                                                new string[] { fileLocalPath },
                                                "This file was automatically created by the GGP Developer Tool.");

            _tfsCache.AddFileToCache(fileServerPath.AsString());
        }

        public void UpdateFileContent(IServerPath fileServerPath, byte[] content)
        {
            var workspace = GetWorkspace();
            workspace.Get(new GetRequest(fileServerPath.AsString(), RecursionType.OneLevel, VersionSpec.Latest), GetOptions.GetAll);
            var fileLocalPath = workspace.GetLocalItemForServerItem(fileServerPath.AsString());
            workspace.PendEdit(fileLocalPath);

            _fileSystemManager.WriteFileContent(fileLocalPath, content);

            workspace.CheckInWithPolicyOverride($"Update file {fileServerPath}",
                                                new string[] { fileLocalPath},
                                                "This file was updated automatically by the GGP Developer Tool.");

        }

        public byte[] GetFileContent(IServerPath fileServerPath)
        {

            using (var stream = VersionControlServer.GetItem(fileServerPath.AsString()).DownloadFile())
            using (var memStream = new MemoryStream())
            {
                do
                {
                    var b = stream.ReadByte();
                    if(b >= 0)
                    {
                        memStream.WriteByte((byte)b);
                    }
                    else
                    {
                        return memStream.ToArray();
                    }

                } while (true);

            }
        }

        public IEnumerable<WorkItem> GetRelatedWorkItems(IEnumerable<Changeset> changeSets)
        {
            var workItems = new List<WorkItem>();
            var workItemStore = Tfs.GetService<WorkItemStore>();

            var artifactsIds = GetRelatedArtifactsIds(changeSets);
            foreach (var workItemId in artifactsIds)
            {
                workItems.Add(workItemStore.GetWorkItem(workItemId));
            }

            return workItems;
        }


        private int[] GetRelatedArtifactsIds(IEnumerable<Changeset> rawChangesets)
        {
            var workItemsIds = new List<int>();

            foreach (Artifact a in GetRelatedArtifacts(rawChangesets))
            {
                workItemsIds.Add(int.Parse(a.ExtendedAttributes.First(attribute => attribute.Name == "System.Id").Value));
            }

            return workItemsIds.Distinct().ToArray();
        }

        private Artifact[] GetRelatedArtifacts(IEnumerable<Changeset> rawChangesets)
        {
            var vcs = Tfs.GetService<VersionControlServer>();

            var linkingService = Tfs.GetService<ILinking>();

            return linkingService.GetReferencingArtifacts(rawChangesets.Select(cs => cs.ArtifactUri.AbsoluteUri).ToArray());
        }

   

        public void GetLatest(IServerPath serverPath)
        {
            GetWorkspace().GetFullLatest(serverPath.AsString());
        }

        public int CheckinPendingChanges(string[] files, string comment, string policyOverrideMessage = "")
        {
            return GetWorkspace().CheckInWithPolicyOverride(comment, files, policyOverrideMessage);
        }

        public void PendAdd(ILocalPath filePath)
        {
            GetWorkspace().PendAdd(filePath.AsString());
        }


        public void TryGetLatest(ILocalPath localPath)
        {
            var workspace = GetWorkspace();
            if (!workspace.IsLocalPathMapped(localPath.AsString()))
                return;

            var serverPath = workspace.GetServerItemForLocalItem(localPath.AsString());

            var tfsItem = workspace.VersionControlServer.GetItems(serverPath, RecursionType.None).Items.FirstOrDefault();
            if(tfsItem != null)
            {
                workspace.GetFullLatest(tfsItem.ServerItem);
            }

        }
        
        public ILocalPath GetLocalPathForServerPath(IServerPath serverPath)
        {
            return CreateLocalPath(GetWorkspace().GetLocalItemForServerItem(serverPath.AsString()));
        }

        public IServerPath GetServerPathFromLocalPath(ILocalPath localPath)
        {
            return CreateServerPathDescriptor(GetWorkspace().GetServerItemForLocalItem(localPath.AsString()));            
        }
        
     

        public IServerPath GetRootServerPath()
        {
            return new ServerPath(ROOT_FOLDER);
        }

        public IEnumerable<ISourceControlFolder> GetRoots()
        {
            return this.GetSubfolders(GetRootServerPath())
                  .Select(serverPath => new TfsFolder(serverPath, GetLocalPathForServerPath(serverPath), this))
                  .ToList();
        }
        
        public IServerPath CreateServerPath(string serverPath)
        {
            return new ServerPath(serverPath);
        }

        public bool HasPendingChanges(ILocalPath filePath)
        {
            return GetWorkspace().GetPendingChanges(filePath.AsString()).Any();
        }

        public ICheckinTransaction CreateCheckInTransaction()
        {
            return new CheckinTransaction(GetWorkspace(), () => _tfsCache.Refresh());
        }

      
    }
}
