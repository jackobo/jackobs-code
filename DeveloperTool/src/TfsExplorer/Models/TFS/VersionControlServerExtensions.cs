using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace Spark.TfsExplorer.Models.TFS
{
    public static class VersionControlServerExtensions
    {
        public static IEnumerable<Changeset> QueryHistoryEx(this VersionControlServer server, string serverPath, int sinceThisChangeset, bool includeChanges = true)
        {
            var p = new QueryHistoryParameters(serverPath, RecursionType.Full);
            p.VersionStart = new ChangesetVersionSpec(sinceThisChangeset);
            p.IncludeChanges = includeChanges;
            p.SlotMode = false;
            p.IncludeDownloadInfo = false;
            p.SortAscending = false;
            return server.QueryHistory(p);
        }

        public static Changeset QueryLatestChangeset(this VersionControlServer server, string serverPath, bool includeChanges = true)
        {
            var p = new QueryHistoryParameters(serverPath, RecursionType.Full);
            p.MaxResults = 1;
            p.IncludeChanges = includeChanges;
            p.SlotMode = false;
            p.IncludeDownloadInfo = false;
            p.SortAscending = false;
            return server.QueryHistory(p).First();
        }
    }
}
