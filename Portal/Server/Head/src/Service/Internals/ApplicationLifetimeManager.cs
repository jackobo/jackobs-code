using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Logging;

namespace GamesPortal.Service
{
    public interface IApplicationLifetimeManager
    {
        //bool IsShuttingDown { get; }
        void RegisterShuttingDownAware(IShuttingDownAware subscriber);
        void UnregisterShuttingDownAware(IShuttingDownAware subscriber);
    }


    public interface IShuttingDownAware
    {
        void ShutDown();
    }

    public class ApplicationLifetimeManager : IApplicationLifetimeManager
    {
        public bool IsShuttingDown { get; private set; } = false;


        SynchronizedCollection<IShuttingDownAware> Subscribers { get; set; } = new SynchronizedCollection<IShuttingDownAware>();
        
        
        public ApplicationLifetimeManager(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger(this.GetType());

        }

        ILogger Logger { get; set; }

        public void RegisterShuttingDownAware(IShuttingDownAware subscriber)
        {
            if (subscriber == null)
                throw new ArgumentNullException(nameof(subscriber));

            if (Subscribers.Contains(subscriber))
                throw new ArgumentException($"ShuttingDownAware subscriber already registered {subscriber.ToString()}");

            Subscribers.Add(subscriber);
        }

        public void ShuttingDown()
        {
            IsShuttingDown = true;

            foreach (var s in Subscribers)
            {
                try
                {
                    s.ShutDown();
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }

        }

        public void UnregisterShuttingDownAware(IShuttingDownAware subscriber)
        {
            Subscribers.Remove(subscriber);
        }
    }
}
