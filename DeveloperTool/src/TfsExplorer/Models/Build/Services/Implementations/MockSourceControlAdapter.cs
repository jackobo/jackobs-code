using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;
using Spark.TfsExplorer.Models.TFS;

namespace Spark.TfsExplorer.Models.Build
{
    public class MockSourceControlAdapter : ISourceControlAdapter
    {
        private static readonly Random _rnd = new Random();
        public void CheckInPendingChanges(string comment)
        {
            _changeSetId = _rnd.Next();
        }
        
        
        public void CheckoutForEdit(ILocalPath filePath)
        {
            File.SetAttributes(filePath.AsString(), FileAttributes.Normal);
        }

        int? _changeSetId;
        public int GetChangeSetId()
        {
            if (_changeSetId == null)
                throw new InvalidOperationException("CheckInPendingChanges was not called so there is not change set ID");

            return _changeSetId.Value;
        }

        public void PendAdd(ILocalPath filePath)
        {
            
        }

        public void GetLatest(string localPath)
        {
            
        }
        
        public void GetLatest(IFolderHolder folder)
        {
            
        }

        public IServerPath CreateServerPath(string serverPath)
        {
            return new ServerPath(serverPath);
        }

        public ILocalPath GetLocalPathFromServerPath(IServerPath serverPath)
        {
            return new LocalPath(serverPath.AsString().Replace("$/", "C:\\")
                                           .Replace("/", "\\"));
        }

        public void TryGetLatestVersion(ILocalPath filePath)
        {
            
        }

        public bool HasPendingChanges(ILocalPath targetFile)
        {
            return false;
        }
    }
}
