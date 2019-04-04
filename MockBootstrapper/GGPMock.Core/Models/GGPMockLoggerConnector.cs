using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GGPMockBootstrapper.Models
{
    public class GGPMockLoggerConnector : GGPMockLoggerService.IGGPMockLoggerServiceCallback
    {

        static GGPMockLoggerConnector()
        {
            Singleton = new GGPMockLoggerConnector();
        }

        public static GGPMockLoggerConnector Singleton { get; private set; }

        public GGPMockLoggerConnector()
        {
            Thread t1 = new Thread(new ThreadStart(() => Connect()));
            t1.IsBackground = true;
            t1.Start();


            Thread t2 = new Thread(new ThreadStart(() => LoggerLoop()));
            t2.IsBackground = true;
            t2.Start(); 
        }


        WcfClientProxySafeRelease<GGPMockLoggerService.GGPMockLoggerServiceClient, GGPMockLoggerService.IGGPMockLoggerService> _proxySafeRelease;
        private void Connect()
        {
            while (true)
            {
                try
                {
                    if (_proxySafeRelease == null || _proxySafeRelease.Client.State == System.ServiceModel.CommunicationState.Faulted)
                    {
                        var oldSafeRelease = _proxySafeRelease;
                        _proxySafeRelease = CreateProxy();
                        _proxySafeRelease.Client.Subscribe();

                        if (oldSafeRelease != null)
                            oldSafeRelease.Dispose();
                    }

                }
                catch 
                {
                }

                Thread.Sleep(TimeSpan.FromSeconds(2));
            }

        }


        private WcfClientProxySafeRelease<GGPMockLoggerService.GGPMockLoggerServiceClient, GGPMockLoggerService.IGGPMockLoggerService> CreateProxy()
        {
            return new WcfClientProxySafeRelease<GGPMockLoggerService.GGPMockLoggerServiceClient, GGPMockLoggerService.IGGPMockLoggerService>(
                    new GGPMockLoggerService.GGPMockLoggerServiceClient(new System.ServiceModel.InstanceContext(this), "NetNamedPipeBinding_IGGPMockLoggerService")
                    );
        }
        
        public void Log(GGPMockLoggerService.GGPMockLoggerMessage message)
        {
            LogQueue.Enqueue(message);
            LoggerLoopAutoResetEvent.Set();

        }

        AutoResetEvent LoggerLoopAutoResetEvent = new AutoResetEvent(true);

        private List<ILoggerAppender> _loggerAppenders = new List<ILoggerAppender>();

        public void RegisterLoggerAppender(ILoggerAppender appender)
        {
            _loggerAppenders.Add(appender);
        }

        System.Collections.Concurrent.ConcurrentQueue<GGPMockLoggerService.GGPMockLoggerMessage> LogQueue = new System.Collections.Concurrent.ConcurrentQueue<GGPMockLoggerService.GGPMockLoggerMessage>();
        private void LoggerLoop()
        {
            while (true)
            {

                while (LogQueue.Count > 0)
                {
                    GGPMockLoggerService.GGPMockLoggerMessage message;

                    if (LogQueue.TryDequeue(out message))
                    {
                        foreach (var appender in _loggerAppenders)
                        {
                            try
                            {
                                appender.Append(message);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                        }
                    }
                    
                }

                LoggerLoopAutoResetEvent.WaitOne();

            }
        }
    }

  
}

namespace GGPMockBootstrapper.GGPMockLoggerService
{
    public partial class GGPMockLoggerMessage
    {
        public bool IsCriticalError
        {
            get { return this.Priority > 3; }
        }

        public bool IsWarn
        {
            get { return this.Priority == 4; }
        }
    }
}
