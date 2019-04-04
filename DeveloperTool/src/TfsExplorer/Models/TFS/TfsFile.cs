using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.TFS
{
    public class TfsFile : ISourceControlFile
    {
        public TfsFile(IServerPath serverPath, ILocalPath localPath, ITfsGateway tfsGateway)
        {
            ServerPath = serverPath;
            LocalPath = localPath;
            _tfsGateway = tfsGateway;
        }

        
        ITfsGateway _tfsGateway;

        private IServerPath ServerPath { get; set; }
        private ILocalPath LocalPath { get; set; }

        public string Name
        {
            get { return ServerPath.GetName(); }
        }

        public void Branch(IServerPath targetFolder)
        {
            _tfsGateway.Branch(this.ServerPath, targetFolder.Subpath(this.Name), false);
        }

        public IChangeSet GetLatestChangeSet()
        {
            return new TfsChangeSet(_tfsGateway.GetLatestChangeSet(ServerPath));
        }

        public void UpdateContent(byte[] content)
        {
            _tfsGateway.UpdateFileContent(this.ServerPath, content);
        }

        public byte[] GetContent()
        {
            return _tfsGateway.GetFileContent(this.ServerPath);
        }

        
        public ILocalPath GetLocalPath()
        {
            return LocalPath;
        }
    }
}
