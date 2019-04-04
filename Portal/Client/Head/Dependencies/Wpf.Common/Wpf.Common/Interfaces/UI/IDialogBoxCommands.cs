using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Spark.Wpf.Common.Interfaces.UI
{
    public interface IDialogBoxCommands
    {
        object DialogViewModel { get; }
        ICommand OkCommand { get; }
        ICommand CancelCommand { get; }
    }
}
