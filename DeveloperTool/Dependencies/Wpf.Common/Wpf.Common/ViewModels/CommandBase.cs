using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Spark.Infra.Types;

namespace Spark.Wpf.Common.ViewModels
{
    public class CommandBase : ViewModelBase, ICommand
    {

        public CommandBase(Action<object> action)
            : this(action, null)
        {
        }

        Optional<INotifyPropertyChanged> _componentToMonitorPropertiesChanged = Optional<INotifyPropertyChanged>.None();
        public CommandBase(Action<object> action, Func<object, bool> canExecuteMethod, INotifyPropertyChanged monitorPropertiesChanged = null)
        {
            _action = action;
            _canExecuteMethod = canExecuteMethod;
            
            if (monitorPropertiesChanged != null)
                _componentToMonitorPropertiesChanged = Optional<INotifyPropertyChanged>.Some(monitorPropertiesChanged);
            
            _componentToMonitorPropertiesChanged.Do(c => c.PropertyChanged += monitorPropertiesChanged_PropertyChanged);
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
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Execute(object parameter)
        {
            _action(parameter);
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                _componentToMonitorPropertiesChanged.Do(c => c.PropertyChanged -= monitorPropertiesChanged_PropertyChanged);
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
