using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Regions;

namespace Spark.Wpf.Common.ViewModels
{
    public static class PrismAndUnityExtensionsForViewModels
    {

        public static TViewModel ExtractViewModel<TViewModel>(this NavigationContext navigationContext)
            where TViewModel : IViewModel
        {

            if (navigationContext == null)
                throw new ArgumentNullException(nameof(navigationContext));

            return (TViewModel)navigationContext.Parameters[typeof(TViewModel).FullName];
        }

        public static void NavigateToViewModel<TViewModel>(this IRegionManager regionManager, 
                                                           string regionName, 
                                                           TViewModel viewModel) where TViewModel : IViewModel
        {
            if (regionManager == null)
                throw new ArgumentNullException(nameof(regionManager));

            if (string.IsNullOrEmpty(regionName))
                throw new ArgumentNullException(nameof(regionName));

            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            var navParams = new NavigationParameters();
            navParams.Add(typeof(TViewModel).FullName, viewModel);
            regionManager.RequestNavigate(regionName, typeof(TViewModel).FullName, navParams);

            Activate(viewModel);

        }

        private static void Activate<TViewModel>(TViewModel viewModel) where TViewModel : IViewModel
        {
            var selectionAware = viewModel as IActivationAware;
            if (selectionAware != null)
            {
                selectionAware.Activate();
            }
        }
    }
}
