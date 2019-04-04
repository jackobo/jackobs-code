using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.PubSubEvents;
using GamesPortal.Client.Interfaces.Services;
using GamesPortal.Client.Models.GamesPortalHubContracts;
using GamesPortal.Client.Models.Services;
using Spark.Infra.Types;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Prism.Logging;
using Spark.Infra.Logging;
using System.IO;

namespace GamesPortal.Client.Models
{
    public static class HubProxyExtensions
    {
        public static void On<TContract, TData>(this IHubProxy hub, Action<TData> handler, Expression<Func<TContract, Action<TData>>> contractMethodExpression)
        {
            var methodName = SparkReflector.GetMethodName<TContract, TData>(contractMethodExpression);
            hub.On<TData>(methodName, handler);
        }
    }

    public class SignalRHandler
    {
        public SignalRHandler(IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
            this.Logger = serviceLocator.GetInstance<ILoggerFactory>().CreateLogger(this.GetType());

            StartNewSignalRConnection();
        }

       
        IServiceLocator ServiceLocator { get; set; }

        private string Host
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["GamesPortalHubUrl"];
            }
        }

        private IHubProxy Proxy { get; set; }
        private HubConnection Connection { get; set; }
        
        private void StartNewSignalRConnection()
        {
            try
            {

                if (Connection != null)
                {
                    Connection.StateChanged -= Connection_StateChanged;
                    Connection.Dispose();
                }

                Connection = new HubConnection(Host);
                /*
                var writer = new StreamWriter(@"C:\temp\SignalRLog.txt");
                writer.AutoFlush = true;
                Connection.TraceLevel = TraceLevels.All;
                Connection.TraceWriter = writer;
                */
                Connection.StateChanged += Connection_StateChanged;

                Proxy = Connection.CreateHubProxy("GamesPortalHub");
                //Proxy.On<GameSynchronizationFinishedData>("GameSynchronizationFinished", 
                Proxy.On<IGamesPortalHubContext, GameSynchronizationFinishedData>(GameSynchronizationFinishedHandler, c => c.GameSynchronizationFinished);
                Proxy.On<IGamesPortalHubContext, FullGamesSynchronizationFinishedData>(FullGamesSynchronizationFinishedHandler, c => c.FullGamesSynchronizationFinished);
                
                  
                Connection.Start();

            }
            catch (Exception ex)
            {
                LogException(ex);
            }

        }

        
        void Connection_StateChanged(StateChange obj)
        {
            if (obj.NewState == ConnectionState.Disconnected)
            {
                StartNewSignalRConnection();
            }
        }

       

        private void LogException(Exception ex)
        {
            Logger.Exception(ex.ToString(), ex);
        }

        private ILogger Logger { get; set; }

        private IGamesRepositorySynchronizer GamesRepository
        {
            get { return this.ServiceLocator.GetInstance<IGamesRepositorySynchronizer>(); }
        }

        private void PublishEvent<TData>(TData payload)
        {
            try
            {
                var eventAgregator = this.ServiceLocator.GetInstance<IEventAggregator>();
                if (eventAgregator != null)
                {
                    var eventToPublish = eventAgregator.GetEvent<PubSubEvent<TData>>();

                    if (eventToPublish != null)
                    {

                        eventToPublish.Publish(payload);
                    }

                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void GameSynchronizationFinishedHandler(GameSynchronizationFinishedData data)
        {
            Game newGame = null;

            if (data.ChangeType == RecordChangeType.Deleted)
            {
                GamesRepository.GameRemoved(data.GameId);
            }
            else
            {
                newGame = GamesRepository.RefreshGame(data.GameId);
            }
            

            PublishEvent(new GameSynchronizationFinishedEventData(data.GameId, newGame, ConvertChangeType(data.ChangeType), data.IsExternal));
        }

      

        private void FullGamesSynchronizationFinishedHandler(FullGamesSynchronizationFinishedData data)
        {
            PublishEvent(new FullGamesSynchronizationFinishedEventData(data.SynchronizationTime, data.SynchronizedBy));
        }


        private Interfaces.ChangeType ConvertChangeType(RecordChangeType recordChangeType)
        {
            switch (recordChangeType)
            {
                case RecordChangeType.Added:
                    return Interfaces.ChangeType.Added;
                case RecordChangeType.Changed:
                    return Interfaces.ChangeType.Changed;
                case RecordChangeType.Deleted:
                    return Interfaces.ChangeType.Removed;
                default:
                    throw new ArgumentException(string.Format("Unknown recordChangeType {0}", recordChangeType));
            }
        }

      
        
    }

   

    
}
