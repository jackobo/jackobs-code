using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.VersionControl.Client;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.TFS
{
    public interface ICheckinTransaction : IDisposable
    {
        void PendAdd(ILocalPath path);
        void PendEdit(ILocalPath path);
        void PendDelete(ILocalPath path);
        void PendRename(IServerPath oldPath, IServerPath newPath);

        void CheckIn(string comments, string policyOverrideMessage = "");
    }
    public class CheckinTransaction : ICheckinTransaction
    {
        public CheckinTransaction(ITfsWorkspace workspace, Action onCheckAfterIn)
        {
            _workspace = workspace;
            _onAfterCheckIn = onCheckAfterIn;
        }

        ITfsWorkspace _workspace;
        Action _onAfterCheckIn;



        List<string> _pendingChanges = new List<string>();
        public void PendAdd(ILocalPath path)
        {
            _workspace.PendAdd(path.AsString());
            _pendingChanges.Add(path.AsString());
        }

        public void PendEdit(ILocalPath path)
        {
            _workspace.PendEdit(path.AsString());
            _pendingChanges.Add(path.AsString());
        }

        public void PendDelete(ILocalPath path)
        {
            var serverPath = _workspace.GetServerItemForLocalItem(path.AsString());
            _workspace.Get(new GetRequest(serverPath, RecursionType.Full, VersionSpec.Latest), GetOptions.None);
            _workspace.PendDelete(path.AsString(), RecursionType.Full);
            _pendingChanges.Add(path.AsString());
        }

        public void PendRename(IServerPath oldPath, IServerPath newPath)
        {
            _workspace.Get(new GetRequest(oldPath.AsString(), RecursionType.Full, VersionSpec.Latest), GetOptions.None);
            _workspace.PendRename(oldPath.AsString(), newPath.AsString());
            _pendingChanges.Add(newPath.AsString());
        }

        public void CheckIn(string comments, string policyOverrideMessage = "")
        {
            _workspace.CheckInWithPolicyOverride(comments, _pendingChanges.ToArray(), policyOverrideMessage);
            _onAfterCheckIn();
        }

        public void Dispose()
        {
            _workspace.UndoPendingChanges(_pendingChanges.ToArray());
        }

       
    }
}
