using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;

namespace Spark.TfsExplorer.Models.TFS
{
    public static class TfsCollectionFactory
    {
        public static TfsTeamProjectCollection Create()
        {
            return new TfsTeamProjectCollection(new Uri("http://tfs2012:8080/tfs/DefaultCollection_2010"));
        }
    }
}
