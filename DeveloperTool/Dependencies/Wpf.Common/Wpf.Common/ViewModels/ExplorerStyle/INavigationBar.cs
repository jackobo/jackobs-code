using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.ViewModels
{
    public interface INavigationBar : IViewModel
    {
        void Navigate(IExplorerBarItem explorerBarItem);
        IContextCommand[] NavigationList { get; }
    }

    
}
