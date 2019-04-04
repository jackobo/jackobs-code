using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public class CreateComponentUniqueIdTxtAction : IBuildAction
    {
        private IServerPath _serverPath;
        private string _componentUniqueId;
        
        public CreateComponentUniqueIdTxtAction(IServerPath serverPath, string componentUniqueId)
        {
            _serverPath = serverPath;
            _componentUniqueId = componentUniqueId;
        }

        public void Execute(IBuildContext buildContext)
        {
            buildContext.Logger.Info($"Writting ComponentUniqueID {_componentUniqueId} in to file {_serverPath.AsString()}");
            var localPath = buildContext.SourceControlAdapter.GetLocalPathFromServerPath(_serverPath);
            buildContext.FileSystemAdapter.WriteAllText(localPath, _componentUniqueId);
            buildContext.SourceControlAdapter.PendAdd(localPath);
        }
    }
}
