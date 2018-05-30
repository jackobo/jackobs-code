using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.ViewModels
{
    public interface IWorkspaceItem : IViewModel, IActivationAware, IDisposable
    {
        string Title { get; }
        IContextCommand[] Actions { get; }
    }
}
