using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.VersionControl.Client;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.TFS
{
    public class GenericWorkspaceSelector : IWorkspaceSelector
    {
        public GenericWorkspaceSelector(string workspaceName)
        {
            this.SelectedWorkspaceName = workspaceName;
        }
        public string SelectedWorkspaceName { get; private set; }
    }
    
}
