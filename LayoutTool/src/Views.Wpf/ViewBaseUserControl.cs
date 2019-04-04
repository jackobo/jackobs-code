using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Prism.Regions;
using Spark.Wpf.Common.ViewModels;
using LayoutTool.ViewModels;

namespace LayoutTool.Views.Wpf
{

    public class ViewBaseUserControl<TViewModel> : UserControl, INavigationAware
        where TViewModel : class, IViewModel
    {
        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
           
            this.DataContext = navigationContext.ExtractViewModel<TViewModel>();
        }
    }
}
