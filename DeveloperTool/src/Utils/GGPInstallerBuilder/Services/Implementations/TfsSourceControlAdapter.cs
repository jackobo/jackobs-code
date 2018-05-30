using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Spark.Infra.Logging;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models;
using Spark.TfsExplorer.Models.Folders;
using Spark.TfsExplorer.Models.TFS;

namespace GGPInstallerBuilder
{
    public class TfsSourceControlAdapter : ISourceControlAdapter
    {
        public TfsSourceControlAdapter(IWorkspaceSelector workspaceSelector)
        {
            _workspaceSelector = workspaceSelector;

            this.Tfs = TfsCollectionFactory.Create(); 
        }

        IWorkspaceSelector _workspaceSelector;
        TfsTeamProjectCollection Tfs { get; set; }

        public IServerPath CreateServerPath(string serverPath)
        {
            return new ServerPath(serverPath);
        }

        public void DownloadFolderContent(IServerPath serverPath, ILocalPath localPath, ILogger logger)
        {
            var items = this.VersionControlServer.GetItems(serverPath.AsString(), VersionSpec.Latest, RecursionType.OneLevel);
            
            foreach (var item in items.Items)
            {
                if (0 == string.Compare(item.ServerItem, serverPath.AsString(), true))
                    continue;

                var itemServerPath = new ServerPath(item.ServerItem);
                var itemLocalPath = localPath.Subpath(itemServerPath.GetName());

                if(item.ItemType == ItemType.File)
                {
                    logger.Info($"Download file {item.ServerItem} to {itemLocalPath.AsString()}");
                    item.DownloadFile(itemLocalPath.AsString());
                }
                else
                {
                    DownloadFolderContent(itemServerPath, itemLocalPath, logger);
                }
            }
        }

        public void GetLatest(IServerPath serverPath)
        {
            var request = new GetRequest(serverPath.AsString(), RecursionType.Full, VersionSpec.Latest);
            GetWorkspace().Get(request, GetOptions.GetAll | GetOptions.Overwrite);
        }

        public string ReadTextFile(IServerPath serverPath)
        {
            var item = this.VersionControlServer.GetItem(serverPath.AsString());
            if (item.ItemType != ItemType.File)
                throw new ArgumentException($"The path {serverPath.AsString()} is not a file path");

            using (var stream = item.DownloadFile())
            using(var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }

        private ITfsWorkspace GetWorkspace()
        {


            var w = VersionControlServer.QueryWorkspaces(_workspaceSelector.SelectedWorkspaceName,
                                                        Environment.UserName,
                                                        Environment.MachineName)
                                        .FirstOrDefault();

            if (w == null)
                throw new InvalidOperationException($"There is no TFS workspace named {_workspaceSelector.SelectedWorkspaceName}");

            return new TfsWorkspaceWrapper(w);
        }

        public ILocalPath GetLocalPathFromServerPath(IServerPath serverPath)
        {
            return new LocalPath(GetWorkspace().GetLocalItemForServerItem(serverPath.AsString()));
        }

        VersionControlServer VersionControlServer
        {
            get
            {
                return Tfs.GetService<VersionControlServer>();
            }
        }
    }
}
