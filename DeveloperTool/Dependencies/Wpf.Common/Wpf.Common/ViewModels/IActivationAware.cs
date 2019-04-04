using System;
using System.ComponentModel;

namespace Spark.Wpf.Common.ViewModels
{
    public interface IActivationAware : INotifyPropertyChanged
    {
        bool IsActive { get; }
        void Activate();
        void Deactivate();
    }
}