using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.VersionControl.Client;
using Spark.Infra.Types;
using Spark.Infra.Windows;

namespace Spark.TfsExplorer.Models.TFS
{
    internal class TfsFilesCache : TfsCachedItems
    {
        public TfsFilesCache(string rootFolder, TimeSpan refreshInterval, VersionControlServer versionControlServer, IThreadingServices threadingServices) : base(rootFolder, refreshInterval, versionControlServer, threadingServices)
        {
        }

        protected override ItemType GetItemType()
        {
            return ItemType.File;
        }

        protected override Item[] ReadTfsItems(string rootPath)
        {
            
            if (!rootPath.EndsWith("/"))
                rootPath += "/";
            
            var itemsSpecs = Folders.WellKnownFileName.All.Select(f => new ItemSpec($"{rootPath}*{f.Name}", RecursionType.Full))
                                         .ToArray();

            var result = VersionControlServer.GetItems(itemsSpecs,
                                         VersionSpec.Latest,
                                         DeletedState.NonDeleted,
                                         GetItemType())
                                         .SelectMany(itemsSet => itemsSet.Items)
                                         .ToArray();

            
            //eliminating duplicates (LatestPublish.xml is found twice: first when searching *LatestPublish.xml second when searching *Publish.xml
            return result.GroupBy(item => item.ServerItem)
                          .Select(g => g.First())
                          .ToArray();

        }

        public override Optional<Item> FindItem(string serverPath)
        {
            var item = base.FindItem(serverPath);

            if (item.Any())
                return item;

            var tfsItem = VersionControlServer.GetItems(serverPath,
                                        VersionSpec.Latest,
                                        RecursionType.None,
                                        DeletedState.NonDeleted,
                                        GetItemType())
                                        .Items.FirstOrDefault();

            if (tfsItem == null)
                return Optional<Item>.None();

            return Optional<Item>.Some(tfsItem);
        }
    }
}
