using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GamesPortal.Service.Synchronizers
{
    public abstract class TimerBasedSynchronizer : Synchronizer
    {
        Timer _timer;
        TimeSpan _delayTime;
        TimeSpan _repeatTime;
        public TimerBasedSynchronizer(IGamesPortalInternalServices services, TimeSpan delayTime, TimeSpan repeatTime)
            : base(services)
        {
            _delayTime = delayTime;
            _repeatTime = repeatTime;
        }


        public override void Start()
        {
            base.Start();
            _timer = new Timer(Synchronize, null, _delayTime, _repeatTime);
        }

        private void Synchronize(object state)
        {
            Run();
        }
        
    }
}
