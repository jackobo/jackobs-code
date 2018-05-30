using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.Interfaces.UI
{
   

    public enum BackgroundOperationStatus
    {
        NotStarted,
        InProgress,
        Done,
        Failed
    }

    public interface IBackgroundOperation : INotifyPropertyChanged
    {
        BackgroundOperationStatus Status { get; }
        string OperationDescription { get; }
        string ErrorDetailes { get; }
        decimal Percentage { get; }

    }
    public interface IBackgroundOperationsRegion
    {
        void RegisterOperation(IBackgroundOperation operation);
        void UnregisterOperation(IBackgroundOperation operation);
    }
}
