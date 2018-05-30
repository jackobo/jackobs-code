using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.ViewModels
{
    public class ContextCommand : Command, IContextCommand
    {
        public ContextCommand(string caption, Action action) : base(action)
        {
            Caption = caption;
        }

        public ContextCommand(string caption, Action action, Func<bool> canExecuteMethod, INotifyPropertyChanged monitorPropertiesChanged = null) 
            : base(action, canExecuteMethod, monitorPropertiesChanged)
        {
            Caption = caption;
        }


        public string Caption { get; private set; }


        public override string ToString()
        {
            return Caption;

        }

    }


    public abstract class CustomContextCommand : ViewModelBase, IContextCommand
    {
        public CustomContextCommand()
        {

        }
        public abstract string Caption { get; }
        public abstract void Execute(object parameter);

        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public virtual void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        
    }

    public class ContextCommand<T> : Command<T>, IContextCommand
    {
        public ContextCommand(string caption, Action<T> executeMethod) : base(executeMethod)
        {
            Caption = caption;
        }

        public ContextCommand(string caption, Action<T> executeMethod, Func<T, bool> canExecuteMethod, INotifyPropertyChanged monitorPropertiesChanged = null) : base(executeMethod, canExecuteMethod, monitorPropertiesChanged)
        {
            Caption = caption;
        }

        public string Caption { get; private set; }

        public override string ToString()
        {
            return Caption;

        }
    }
}
