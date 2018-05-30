using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.ViewModels
{
    public interface ISupportDynamicLayout<T> : INotifyPropertyChanged
    {
        PlayerStatusTypeViewModel PlayerStatus { get; }
        T Clone(PlayerStatusTypeViewModel newPlayerStatus);
    }
}
