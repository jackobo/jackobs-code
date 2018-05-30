using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.VersionControl.Client;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.TFS
{
    public class AutomaticWorkspaceSelector : IWorkspaceSelector
    {
        public AutomaticWorkspaceSelector(string serverPath)
        {
            _serverPath = serverPath;
        }

        string _serverPath;

        string _selectedWorkspaceName = null;
        public string SelectedWorkspaceName
        {
            get
            {
                if (string.IsNullOrEmpty(_selectedWorkspaceName))
                {
                    _selectedWorkspaceName = FindWorkspace();
                }

                if (string.IsNullOrEmpty(_selectedWorkspaceName))
                {
                    throw new ApplicationException($"Can't detect a workspace for folder {TfsGateway.ROOT_FOLDER}");
                }

                return _selectedWorkspaceName;
            }
        }

        private string FindWorkspace()
        {
            using (var tfsCollection = TfsCollectionFactory.Create())
            {
                tfsCollection.EnsureAuthenticated();

                var server = tfsCollection.GetService<VersionControlServer>();


                var workspaces = server.QueryWorkspaces(null, server.AuthorizedUser, Environment.MachineName);

                foreach (var w in workspaces)
                {
                    foreach (var f in w.Folders)
                    {
                        if (_serverPath.StartsWith(f.ServerItem, StringComparison.OrdinalIgnoreCase))
                            return w.Name;
                    }

                }
            }

            return null;
        }
    }
}
