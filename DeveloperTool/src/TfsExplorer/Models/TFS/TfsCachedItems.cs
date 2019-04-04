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
    internal abstract class TfsCachedItems
    {
        Dictionary<string, Item> _cachedItems = new Dictionary<string, Item>(StringComparer.OrdinalIgnoreCase);
        public TfsCachedItems(string rootFolder, 
                                TimeSpan refreshInterval,
                                VersionControlServer versionControlServer,  
                                IThreadingServices threadingServices)
        {
            _rootFolder = rootFolder;
            _refreshInterval = refreshInterval;
            this.VersionControlServer = versionControlServer;

            RebuildCache();

            _autoResetEvent = threadingServices.CreateAutoResetEvent(false);
            threadingServices.StartNewTask(RebuildCacheLoop);
        }

        string _rootFolder;
        TimeSpan _refreshInterval;
        protected VersionControlServer VersionControlServer { get; private set; }
        IAutoresetEvent _autoResetEvent;
        protected abstract Item[] ReadTfsItems(string rootPath);
        protected abstract ItemType GetItemType();

        private void RebuildCacheLoop()
        {
            while (true)
            {
                _autoResetEvent.WaitOne(_refreshInterval);

                RebuildCache();
            }
        }



        private void RebuildCache()
        {
            try
            {
                _cachedItems = ReadTfsItems(_rootFolder)
                               .ToDictionary(item => item.ServerItem, StringComparer.OrdinalIgnoreCase);
            }
            catch(Exception ex)
            {
#warning Logging here
            }
        }

      
        public void Refresh()
        {
            _autoResetEvent.Set();
        }

        public virtual Optional<Item> FindItem(string serverPath)
        {
            if (_cachedItems.ContainsKey(serverPath))
                return Optional<Item>.Some(_cachedItems[serverPath]);
            else
                return Optional<Item>.None();
        }

        public IEnumerable<Item> GetSubItems(string serverPath)
        {
            return _cachedItems.Where(item => item.Key.StartsWith(serverPath)
                                              && item.Key != serverPath
                                              && item.Key.Substring(serverPath.Length).Count(chr => chr == '/') == 1)
                               .Select(item => item.Value)
                               .ToList();
        }

        public void AddToCache(string serverPath)
        {
            var item = VersionControlServer.GetItems(serverPath,
                                         VersionSpec.Latest,
                                         RecursionType.None,
                                         DeletedState.NonDeleted,
                                         GetItemType())
                                         .Items
                                         .FirstOrDefault();
            
            if (item == null)
                throw new ArgumentException($"{serverPath} doesn't exists!");

            if (item.ItemType != GetItemType())
                throw new ArgumentException($"Item should be of type {GetItemType()} and provided item {serverPath} is of type {item.ItemType}");
            
            TryAddToCache(item);
        }

        private void TryAddToCache(Item item)
        {
            if (!_cachedItems.ContainsKey(item.ServerItem))
            {
                try
                {
                    _cachedItems.Add(item.ServerItem, item);
                }
                catch (ArgumentException) //in case the item is already in the cache
                {

                }
            }
        }
    }
}
