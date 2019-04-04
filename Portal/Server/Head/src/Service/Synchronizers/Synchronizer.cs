using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Spark.Infra.Logging;
using Spark.Infra.Windows;

namespace GamesPortal.Service.Synchronizers
{
    public interface ISynchronizer
    {
        bool IsStarted { get; }
        void Start();
        void Run();

        void Stop();
    }

    

    public abstract class Synchronizer : ISynchronizer
    {
        public Synchronizer(IGamesPortalInternalServices services)
        {
            Services = services;
            _stopper = services.SynchronizerStopperFactory.Create();
            _autoResetEvent = Services.ThreadingServices.CreateAutoResetEvent(false);
            Logger = services.LoggerFactory.CreateLogger(this.GetType());
        }

        IAutoresetEvent _autoResetEvent;
        ISynchronizerStopper _stopper;

        protected IGamesPortalInternalServices Services { get; private set; }
        protected ILogger Logger { get; private set; }

        protected abstract void DoWork();

        public virtual void Start()
        {
            if (IsStarted)
                throw new InvalidOperationException($"Synchronnizer '{this.GetType().FullName}' is already running!");
                
            Services.ThreadingServices.StartNewTask(RunLoop);
            Services.ApplicationLifetimeManager.RegisterShuttingDownAware(_stopper);
        }

        public virtual void Run()
        {
            if(!IsStarted)
            {
                throw new InvalidOperationException($"Synchronnizer '{this.GetType().FullName}' is not running! You must call {nameof(Start)} method before calling {nameof(Run)} method.");
            }
            _autoResetEvent.Set();
        }

        
        public virtual void Stop()
        {
            _stopper.ShutDown();
            Services.ApplicationLifetimeManager.UnregisterShuttingDownAware(_stopper);
            _autoResetEvent.Set();
        }

        public bool IsStarted { get; private set; } = false;
       

        public bool IsWorkInProgress { get; private set; } = false;

        
        private void RunLoop()
        {
            IsStarted = true;
            try
            {
                while (ShouldContinue())
                {
                    IsWorkInProgress = true;
                    try
                    {
                        StartWorking();
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex);
                    }
                    finally
                    {
                        IsWorkInProgress = false;
                        WaitOne(_autoResetEvent);
                    }
                }
            }
            finally
            {
                IsStarted = false;
            }
        }

        private void StartWorking()
        {
            var timer = StartTimer();
            OnBeginWork();

            DoWork();

            timer.Stop();
            OnFinishWork(timer.Elapsed);
        }

        private Stopwatch StartTimer()
        {
            var timer = new Stopwatch();
            timer.Start();
            return timer;
        }


        protected virtual void OnBeginWork()
        {

        }

        protected virtual void OnFinishWork(TimeSpan elapsedTime)
        {

        }
        
        protected virtual void WaitOne(IAutoresetEvent autoresetEvent)
        {
            autoresetEvent.WaitOne();
        }

        protected virtual bool ShouldContinue()
        {
            return !_stopper.IsStopped;
        }
        
        
    }
}
