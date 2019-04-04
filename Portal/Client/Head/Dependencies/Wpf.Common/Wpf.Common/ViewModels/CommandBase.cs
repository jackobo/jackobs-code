using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Spark.Wpf.Common.ViewModels
{
    public class CommandBase : ICommand
    {

        public CommandBase(Action<object> action)
            : this(action, null)
        {
        }


        public CommandBase(Action<object> action, Func<object, bool> canExecuteMethod, INotifyPropertyChanged monitorPropertiesChanged = null)
        {
            _action = action;
            _canExecuteMethod = canExecuteMethod;

            if (monitorPropertiesChanged != null)
                monitorPropertiesChanged.PropertyChanged += monitorPropertiesChanged_PropertyChanged;
        }

        void monitorPropertiesChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnCanExecuteChanged();
        }


        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        Action<object> _action;
        Func<object, bool> _canExecuteMethod;

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            if (_canExecuteMethod == null)
                return true;
            else
                return _canExecuteMethod(parameter);

        }

        public event EventHandler CanExecuteChanged;
        protected virtual void OnCanExecuteChanged()
        {
            var ev = CanExecuteChanged;
            if (ev != null)
                ev(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }

        #endregion
    }
}
