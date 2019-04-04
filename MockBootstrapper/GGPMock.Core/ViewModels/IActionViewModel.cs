using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;

namespace GGPMockBootstrapper.ViewModels
{
    public interface IActionViewModel : INotifyPropertyChanged
    {
        string Caption { get; }
        ImageSource ImageSource { get; }
        ICommand Command { get; }
    }
}
