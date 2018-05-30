using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;
using Spark.TfsExplorer.Models.TFS;

namespace Spark.TfsExplorer.Models.Build
{
    public class TfsSourceControlAdapter : ISourceControlAdapter
    {
        public TfsSourceControlAdapter(TFS.ITfsGateway tfsGateway)
        {
            _tfsGateway = tfsGateway;
        }

        

        TFS.ITfsGateway _tfsGateway;
        
        public void CheckInPendingChanges(string comment)
        {
            _changeSetId = _tfsGateway.CheckinPendingChanges(_pendingFiles.ToArray(), comment, "This is an automatic check-in executed by the GGP build process");
        }

        
        
        List<string> _pendingFiles = new List<string>();

        public void CheckoutForEdit(ILocalPath filePath)
        {
            if (!_tfsGateway.HasPendingChanges(filePath))
            {
                _tfsGateway.CheckoutForEdit(filePath);
            }

            _pendingFiles.Add(filePath.AsString());
        }

        public void PendAdd(ILocalPath filePath)
        {
            if (!_tfsGateway.HasPendingChanges(filePath))
            {
                _tfsGateway.PendAdd(filePath);
            }
            _pendingFiles.Add(filePath.AsString());
        }

        public void TryGetLatestVersion(ILocalPath filePath)
        {
            _tfsGateway.TryGetLatest(filePath);
        }

        int? _changeSetId;
        public int GetChangeSetId()
        {
            if (_changeSetId == null)
                throw new InvalidOperationException($"No change set available. You must first call {nameof(CheckInPendingChanges)}");

            return _changeSetId.Value; 
        }

      

        public void GetLatest(IFolderHolder folder)
        {
            folder.ToSourceControlFolder().GetLatest();
        }
        
        public IServerPath CreateServerPath(string serverPath)
        {
            return _tfsGateway.CreateServerPath(serverPath);
            
        }

        public ILocalPath GetLocalPathFromServerPath(IServerPath serverPath)
        {
            return _tfsGateway.GetLocalPathForServerPath(serverPath);
        }

        public bool HasPendingChanges(ILocalPath targetFile)
        {
            return _tfsGateway.HasPendingChanges(targetFile);
        }
    }
}
