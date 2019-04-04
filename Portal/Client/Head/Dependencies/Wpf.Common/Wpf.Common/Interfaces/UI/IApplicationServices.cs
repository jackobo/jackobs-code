using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.Interfaces.UI
{
    public interface IApplicationServices
    {
        void ExecuteOnUIThread(Action action);
        void Exit();

        event EventHandler ShuttingDown;

        string GetApplicationUniqueName();

        string GetUserAppDataFolder();

        void RegisterApplicationExitHandler(IApplicationExitHandler handler);
        void UnregisterApplicationExitHandler(IApplicationExitHandler handler);

        Task StartNewParallelTask(Action action);

        IUserInterfaceServices UserInterfaceServices { get; }

    }

    public interface IApplicationExitHandler
    {
        bool CanExit();
    }
}
