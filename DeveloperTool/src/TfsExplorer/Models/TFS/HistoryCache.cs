using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.VersionControl.Client;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.TFS
{
    public interface IHistoryCache : IDisposable
    {
    }

    internal class HistoryCache : IHistoryCache
    {
        int _changeSetId;
        private IEnumerable<Changeset> _changeSets;
        HistoryCacheManager _historyCacheManager;
        string _serverPath;


        public HistoryCache(int changeSetId, 
                            IServerPath serverPath,
                            IEnumerable<Changeset> changeSets,
                            HistoryCacheManager historyCacheManager)
        {
            _changeSetId = changeSetId;
            _serverPath = serverPath.AsString();
            _changeSets = changeSets;
            _historyCacheManager = historyCacheManager;
        }

        
        public void Dispose()
        {
            _historyCacheManager.Remove(_changeSetId);
        }

        internal bool HasCacheFor(IServerPath serverPath)
        {
            return serverPath.AsString().StartsWith(_serverPath, StringComparison.OrdinalIgnoreCase);
        }

        internal IEnumerable<Changeset> GetCachedChangeSets(IServerPath serverPath)
        {
            var path = serverPath.AsString();
            return _changeSets.Where(cs => cs.Changes.Any(change => change.Item.ServerItem.StartsWith(path, StringComparison.OrdinalIgnoreCase)))
                              .ToList();
        }
    }

    internal class HistoryCacheManager
    {
        ConcurrentDictionary<int, HistoryCache> _historyCache = new ConcurrentDictionary<int, HistoryCache>();
        VersionControlServer _versionControlServer;
        public HistoryCacheManager(VersionControlServer versionControlServer)
        {
            _versionControlServer = versionControlServer;
        }

        public IHistoryCache CreateCache(IServerPath serverPath, int sinceThisChangeSet)
        {
            var historyCache = new HistoryCache(sinceThisChangeSet, 
                                                serverPath, 
                                                _versionControlServer.QueryHistoryEx(serverPath.AsString(), 
                                                                                     sinceThisChangeSet,
                                                                                     true)
                                                                      .ToList(),
                                                this);

            _historyCache.TryAdd(sinceThisChangeSet, historyCache);
            return historyCache;
        }

        internal void Remove(int changeSetId)
        {
            HistoryCache hc;
            _historyCache.TryRemove(changeSetId, out hc);
        }

        internal IEnumerable<Changeset> QueryHistory(IServerPath serverPath, int sinceThisChangeSet)
        {
            
            HistoryCache cache;
            if(_historyCache.TryGetValue(sinceThisChangeSet, out cache))
            {
                if (cache.HasCacheFor(serverPath))
                {
                    return cache.GetCachedChangeSets(serverPath);
                }
            }
            
            return _versionControlServer.QueryHistoryEx(serverPath.AsString(), sinceThisChangeSet)
                                        .Where(cs => cs.Changes.Any(change => change.Item.ServerItem.StartsWith(serverPath.AsString())))
                                        .ToList();


        }
    }
}
