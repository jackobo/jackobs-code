using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace GamesPortal.Service.SignalR
{
    [ServiceContract]
    public interface IGamesPortalHubContext
    {
        [OperationContract]
        void GameSynchronizationFinished(GameSynchronizationFinishedData data);

        [OperationContract]
        void FullGamesSynchronizationFinished(FullGamesSynchronizationFinishedData data);
    }

    [DataContract]
    public class GameSynchronizationFinishedData
    {
        public GameSynchronizationFinishedData()
        {

        }

        public GameSynchronizationFinishedData(Guid gameId, RecordChangeType changeType, bool isExternal)
        {
            this.GameId = gameId;
            this.ChangeType = changeType;
            this.IsExternal = isExternal;
        }

        [DataMember]
        public Guid GameId { get; set; }

        [DataMember]
        public RecordChangeType ChangeType { get; set; }

        [DataMember]
        public bool IsExternal { get; set; }
    }

    [DataContract]
    public class FullGamesSynchronizationFinishedData
    {
        public FullGamesSynchronizationFinishedData()
        {

        }

        public FullGamesSynchronizationFinishedData(DateTime synchronizationTime, string synchronizedBy)
        {
            this.SynchronizationTime = synchronizationTime;
            this.SynchronizedBy = synchronizedBy;
        }

        [DataMember]
        public DateTime SynchronizationTime { get; set; }
        [DataMember]
        public string SynchronizedBy { get; set; }
    }

    public class GamesPortalHubContext : IGamesPortalHubContext
    {
        
        public void GameSynchronizationFinished(GameSynchronizationFinishedData data)
        {
            Task.Factory.StartNew(() =>
            {
                HubContext.Clients.All.GameSynchronizationFinished(data);
            });
        }


        public void FullGamesSynchronizationFinished(FullGamesSynchronizationFinishedData data)
        {
            Task.Factory.StartNew(() =>
            {
                HubContext.Clients.All.FullGamesSynchronizationFinished(data);
            });
        }

        private IHubContext HubContext
        {
            get
            {
                return GlobalHost.ConnectionManager.GetHubContext<GamesPortalHub>();
            }
        }



    }

}
