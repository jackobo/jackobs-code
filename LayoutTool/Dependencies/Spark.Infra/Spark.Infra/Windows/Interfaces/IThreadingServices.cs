using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spark.Infra.Windows
{
    public interface IThreadingServices
    {
        IAutoresetEvent CreateAutoResetEvent(bool initialStateSignaled = true);
        Task StartNewTask(Action action);
        Task StartNewTask(Action action, Action<Exception> onException);
    }

    public interface IAutoresetEvent
    {
        void Set();
        void WaitOne();
        void WaitOne(TimeSpan timeout);
    }
}
