using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.ViewModels
{
    public interface IExplorerBarItem : INotifyPropertyChanged
    {
        string Caption { get; }
        bool IsSelected { get; set; }
        bool IsExpanded { get; set; }
    }
}
