using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GGPGameServer.ApprovalSystem.Common.Logger;

namespace GamesPortal.Service.Synchronizers
{
    public interface ISynchronizer
    {
        void Run();
    }

    public abstract class SynchronizerBase
    {
        public SynchronizerBase(IGamesPortalInternalServices services)
        {
            Services = services;
            _autoResetEvent = Services.ThreadingServices.CreateAutoResetEvent(false);
            Logger = services.LoggerFactory.CreateLogger(typeof(ArtifactoryToGameLanguageSynchronizer));
        }

        IAutoresetEvent _autoResetEvent;

        protected IGamesPortalInternalServices Services { get; private set; }
        protected ILogNotifier Logger { get; private set; }


        public void Start()
        {
            Services.ThreadingServices.StartNewTask(RunLoop);
        }

        public void Run()
        {
            _autoResetEvent.Set();
        }


        private void RunLoop()
        {
            while (!Services.ApplicationLifetimeManager.IsShuttingDown)
            {
                try
                {
                    DoWork();
                }
                catch(Exception ex)
                {
                    Logger.Exception(ex);
                }
                finally
                {
                    _autoResetEvent.WaitOne();
                }
            }
        }


        protected abstract void DoWork();
        
    }
}
