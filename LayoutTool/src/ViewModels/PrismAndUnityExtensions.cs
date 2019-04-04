using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using Microsoft.Practices.Unity;
using Prism.Regions;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public static class PrismAndUnityExtensions
    {
        public static TViewModel ExtractViewModel<TViewModel>(this NavigationContext navigationContext)
            where TViewModel : IViewModel
        {
            
            if (navigationContext == null)
                throw new ArgumentNullException(nameof(navigationContext));

            return (TViewModel)navigationContext.Parameters[typeof(TViewModel).FullName];
        }

        public static void NavigateToViewModel<TViewModel>(this IRegionManager regionManager, string regionName, TViewModel viewModel)
         where TViewModel : IViewModel
        {
            if(regionManager == null)
                throw new ArgumentNullException(nameof(regionManager));

            if (string.IsNullOrEmpty(regionName))
                throw new ArgumentNullException(nameof(regionName));

            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            var navParams = new NavigationParameters();
            navParams.Add(viewModel.GetType().FullName, viewModel);
            regionManager.RequestNavigate(regionName, viewModel.GetType().FullName, navParams);

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


        public static void NavigateToMainContent<TViewModel>(this IRegionManager regionManager, TViewModel viewModel)
            where TViewModel : IViewModel
        {
            NavigateToViewModel(regionManager, RegionNames.MainContent, viewModel);
        }
    }
}
