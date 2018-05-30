using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GGPMockBootstrapper.ViewModels
{
    public class Command : ICommand
    {
        Action _action;
        public Command(Action action)
        {
            _action = action;

        }

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public virtual void Execute(object parameter)
        {
            _action();
        }




    }
}
