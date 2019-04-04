using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Service
{
    public interface  ISynchronizerStopperFactory
    {
        ISynchronizerStopper Create();
    }

    public interface ISynchronizerStopper : IShuttingDownAware
    {
        bool IsStopped { get; set; }
    }


    public class SynchronizerStopper : ISynchronizerStopper
    {
        public bool IsStopped { get; set; } = false;

        public void ShutDown()
        {
            IsStopped = true;
        }
    }

    public class SynchronizerStopperFactory : ISynchronizerStopperFactory
    {
        public ISynchronizerStopper Create()
        {
            return new SynchronizerStopper();
        }
    }
}
