using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.VersionControl.Client;
using Spark.Infra.Windows;

namespace Spark.TfsExplorer.Models.TFS
{
    internal class TfsFolderCache : TfsCachedItems
    {
        public TfsFolderCache(string rootFolder, TimeSpan refreshInterval, VersionControlServer versionControlServer, IThreadingServices threadingServices) 
            : base(rootFolder, refreshInterval, versionControlServer, threadingServices)
        {
        }

        protected override ItemType GetItemType()
        {
            return ItemType.Folder;
        }

        protected override Item[] ReadTfsItems(string rootPath)
        {
            return VersionControlServer.GetItems(rootPath,
                                       VersionSpec.Latest,
                                       RecursionType.Full,
                                       DeletedState.NonDeleted,
                                       GetItemType())
                                       .Items;
        }
    }
}
