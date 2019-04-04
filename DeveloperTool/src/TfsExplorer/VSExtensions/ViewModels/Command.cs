using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Spark.Wpf.Common.ViewModels
{
    
    public class Command : CommandBase
    {
        public Command(Action action)
            : base(param => action())
        {
        }


        public Command(Action action, Func<bool> canExecuteMethod, INotifyPropertyChanged monitorPropertiesChanged = null)
            : base(param => action(), param => canExecuteMethod(), monitorPropertiesChanged)
        {

         
        }

    }

    public class Command<T> : CommandBase
    {
        public Command(Action<T> executeMethod)
            : base(param => executeMethod((T)param))
        {

        }
        public Command(Action<T> executeMethod, Func<T, bool> canExecuteMethod, INotifyPropertyChanged monitorPropertiesChanged = null)
            : base(param => executeMethod((T)param), param => canExecuteMethod((T)param), monitorPropertiesChanged)
        {
        }
    }
}
