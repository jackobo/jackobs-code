using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Spark.Infra.Types;
using Spark.Infra.Windows;

namespace Spark.TfsExplorer.Models.TFS
{
    public class TfsCache
    {
        public TfsCache(TfsTeamProjectCollection tfs, string rootFolder, IThreadingServices threadingServices)
        {
            
            this.Tfs = tfs;
            this.VersionControlServer = Tfs.GetService<VersionControlServer>();
            _folders = new TfsFolderCache(rootFolder, TimeSpan.FromSeconds(5), VersionControlServer, threadingServices);
            _files = new TfsFilesCache(rootFolder, TimeSpan.FromSeconds(30), VersionControlServer, threadingServices);
            
        }

        TfsFolderCache _folders;
        TfsFilesCache _files;

        /*
        //https://msdn.microsoft.com/en-us/library/bb130324(v=vs.90).aspx
        //https://social.msdn.microsoft.com/Forums/vstudio/en-US/ce716355-b193-499d-9e4b-271b3302ede5/how-to-consume-events-from-tfs?forum=tfsgeneral
        IEventService EventService { get;set;}
        private void RegisterEvents()
        {
        
            this.EventService = this.Tfs.GetService<IEventService>();
            string filter = "";
            var deliveryPreference = new DeliveryPreference();
            deliveryPreference.Address = "http://localhost";
            deliveryPreference.Schedule = DeliverySchedule.Immediate;
            deliveryPreference.Type = DeliveryType.Soap;

            this.EventService.SubscribeEvent("CheckinEvent", filter, deliveryPreference);
        }
        */

        private VersionControlServer VersionControlServer { get; set; }
        private TfsTeamProjectCollection Tfs { get; set; }
        public void Refresh()
        {
            _folders.Refresh();
            _files.Refresh();
        }

        public IEnumerable<Item> GetSubfolders(string serverPath)
        {
            return _folders.GetSubItems(serverPath);
        }
        
        public Optional<Item> FindFolder(string serverPath)
        {
            return _folders.FindItem(serverPath);
        }

        public Optional<Item> FindFile(string serverPath)
        {
            return _files.FindItem(serverPath);
        }

        public void AddFolderToCache(string serverPath)
        {
            _folders.AddToCache(serverPath);
        }

        public void AddFileToCache(string serverPath)
        {
            _files.AddToCache(serverPath);
        }
    }
}
