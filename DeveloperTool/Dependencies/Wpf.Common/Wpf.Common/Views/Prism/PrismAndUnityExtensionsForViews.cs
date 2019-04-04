using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Unity;
using Spark.Wpf.Common.ViewModels;

namespace Spark.Wpf.Common.Views
{
    public static class PrismAndUnityExtensionsForViews
    {
        public static void RegisterViewWithViewModel<TView, TViewModel>(this IUnityContainer container)
                        where TView : FrameworkElement
                        where TViewModel : IViewModel
        {
            container.RegisterType<object, TView>(typeof(TViewModel).FullName);
        }

        public static void RegisterViewWithViewModel(this IUnityContainer container, Type viewType, Type viewModelType)
        {
            container.RegisterType(typeof(object), viewType, viewModelType.FullName);
        }

    }
}
