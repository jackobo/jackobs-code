using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Spark.Wpf.Common.ViewModels
{
    public interface IContextCommand : ICommand
    {
        string Caption { get; }
    }
}
