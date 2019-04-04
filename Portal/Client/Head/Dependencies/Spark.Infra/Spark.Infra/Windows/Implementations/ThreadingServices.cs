using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Spark.Infra.Logging;

namespace Spark.Infra.Windows
{
    public class ThreadingServices : IThreadingServices
    {
        ILogger _logger;

        public ThreadingServices(ILoggerFactory loggerFactoryManager)
        {
            _logger = loggerFactoryManager.CreateLogger(this.GetType());
        }

        public IAutoresetEvent CreateAutoResetEvent(bool initialStateSignaled = true)
        {
            return new AutoResetEventWrapper(initialStateSignaled);
        }

        public Task StartNewTask(Action action)
        {
            return StartNewTask(action, (ex) => { });
        }

        public Task StartNewTask(Action action, Action<Exception> onException)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (onException == null)
                throw new ArgumentNullException(nameof(onException));

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    _logger.Exception("Failed to execute parallel task", ex);
                    onException(ex);
                }
            });
        }
    }

    internal class AutoResetEventWrapper : IAutoresetEvent
    {
        AutoResetEvent _autoResetEvent;
        public AutoResetEventWrapper(bool initialStateSignaled)
        {
            _autoResetEvent = new AutoResetEvent(initialStateSignaled);
        }
        public void Set()
        {
            _autoResetEvent.Set();
        }

        public void WaitOne()
        {
            _autoResetEvent.WaitOne();
        }

        public void WaitOne(TimeSpan timeout)
        {
            _autoResetEvent.WaitOne(timeout);
        }
    }
}
