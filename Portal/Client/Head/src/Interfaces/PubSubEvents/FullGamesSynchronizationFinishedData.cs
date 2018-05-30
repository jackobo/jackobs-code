using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Client.Interfaces.PubSubEvents
{
    public class FullGamesSynchronizationFinishedEventData
    {
        public FullGamesSynchronizationFinishedEventData(DateTime synchronizationTime, string initiatedBy)
        {
            this.SynchronizationTime = synchronizationTime;
            this.InitiatedBy = initiatedBy;
        }

        
        public DateTime SynchronizationTime { get; private set; }

        public string InitiatedBy { get; private set; }
    }
}
