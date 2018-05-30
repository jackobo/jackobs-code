using System;

namespace Spark.Wpf.Common.ViewModels
{
    public interface IActivationAware
    {
        event EventHandler Activated;
        void Activate();
    }
}