using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.VersionControl.Client;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.TFS
{
    public sealed class TfsFolder : ISourceControlFolder
    {
        ITfsGateway _tfsGateway;
        public TfsFolder(IServerPath serverPath, ILocalPath localPath, ITfsGateway  tfsGateway)
        {
            this.ServerPath = serverPath;
            this.LocalPath = localPath;
            _tfsGateway = tfsGateway;
        }
        
        public string Name
        {
            get { return this.ServerPath.GetName(); }
        }


        private IServerPath ServerPath { get; set; }
        private ILocalPath LocalPath { get; set; }

        public void Branch(IServerPath destination)
        {
            _tfsGateway.Branch(this.ServerPath, destination, true);
        }

        public ISourceControlFolder CreateSubfolder(string name)
        {
            var subFolder = TryGetSubfolder(name);
            if (subFolder.Any())
                return subFolder.First();

            return NewTfsFolderInstance(_tfsGateway.CreateSubfolder(this.ServerPath, name),
                                        LocalPath.Subpath(name));
        }

        internal MergeResult MergeChangeSet(int changesetId, IServerPath targetFolder)
        {
            var status = _tfsGateway.MergeChangeSet(this.ServerPath, changesetId, targetFolder);
            return new MergeResult(status.NumConflicts);
        }

     
        
        public IEnumerable<IMergeableChangeSet> GetMergeChangeSets(IServerPath targetFolder)
        {
            var changeSets = new List<IMergeableChangeSet>();
            foreach (var mergeCandidate in _tfsGateway.GetMergeCanditates(this.ServerPath, targetFolder))
            {
                changeSets.Add(new TfsMergeableChangeSet(mergeCandidate.Changeset, this, targetFolder));
            }

            return changeSets;
        }


        public MergeResult Merge(IServerPath targetFolder)
        {
            var status = _tfsGateway.Merge(this.ServerPath, targetFolder);
            return new MergeResult(status.NumConflicts);
            
        }

        public ISourceControlFolder GetSubfolder(string name)
        {
            var subFolder = TryGetSubfolder(name);
            if (!subFolder.Any())
                throw new ArgumentException($"There is no folder with name {name} inside {ServerPath}");


            return subFolder.First();
        }

        public IEnumerable<ISourceControlFolder> GetSubfolders()
        {
            return  _tfsGateway.GetSubfolders(this.ServerPath)
                               .Select(serverPath => NewTfsFolderInstance(serverPath, 
                                                                          this.LocalPath.Subpath(serverPath.GetName())))
                               .ToList();
        }
        
        
        public Optional<ISourceControlFolder> TryGetSubfolder(string name)
        {
            var result = Optional<ISourceControlFolder>.None();
            _tfsGateway.TryGetSubFolder(this.ServerPath, name)
                       .Do(subfolderFullPath => result = Optional<ISourceControlFolder>.Some(NewTfsFolderInstance(subfolderFullPath, LocalPath.Subpath(name))));
            
            return result;
        }

        private TfsFolder NewTfsFolderInstance(IServerPath serverPath, ILocalPath localPath)
        {
            return new TfsFolder(serverPath, localPath, _tfsGateway);
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as TfsFolder;
            if (theOther == null)
                return false;

            return ServerPath.Equals(theOther.ServerPath);
        }

        public override int GetHashCode()
        {
            return ServerPath.GetHashCode();
        }

        public Optional<ISourceControlFile> TryGetFile(string fileName)
        {
            var file = _tfsGateway.TryGetFile(this.ServerPath, fileName);

            if (file.Any())
                return Optional<ISourceControlFile>.Some(NewTfsFile(file.First(), LocalPath.Subpath(fileName)));
            else
                return Optional<ISourceControlFile>.None();
        }

        private TfsFile NewTfsFile(IServerPath serverPath, ILocalPath localPath)
        {
            return new TfsFile(serverPath, localPath, _tfsGateway);
        }

        public IEnumerable<IChangeSet> QueryHistory(IChangeSet sinceThisChangeSet)
        {
            return _tfsGateway.QueryHistory(this.ServerPath, sinceThisChangeSet.Id)
                              .Select(cs => new TfsChangeSet(cs))
                              .ToArray();
        }

        public IChangeSet GetLatestChangeSet()
        {
            return new TfsChangeSet(_tfsGateway.GetLatestChangeSet(ServerPath));
        }

        public ISourceControlFile CreateFile(string fileName, byte[] content)
        {
            _tfsGateway.CreateFile(this.ServerPath, fileName, content);
            return this.TryGetFile(fileName).First();
        }

        

        public ILocalPath GetLocalPath()
        {
            return LocalPath;
        }

        public void GetLatest()
        {
            _tfsGateway.GetLatest(this.ServerPath);
        }
    }
}
