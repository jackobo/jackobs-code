using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prism.Regions;
using Spark.Wpf.Common.ViewModels;

namespace Spark.Wpf.Common.Views
{
    public abstract class ExplorerStyleViewsModule : StandardViewsModule
    {
        public ExplorerStyleViewsModule(IUnityContainer container) : base(container)
        {
        }

        protected override void Initialize(IUnityContainer container)
        {
            base.Initialize(container);
            RegisterNavigationBar(container);
            ActivateNavigationBar(container);
            RegisterSideBarView(container);
            RegisterWorkspaceView(container);
            RegisterExplorerBarView(container);
            ActivateExplorerBar(container);
            SelectFirstItemInTheExplorerBar(container);
            container.Resolve<IRegionManager>().RegisterViewWithRegion(WellKnowRegionNames.NotificationArea, 
                                                                       typeof(BackgroundOperationsRegionView));
        }

        protected virtual void RegisterWorkspaceView(IUnityContainer container)
        {
            container.Resolve<IRegionManager>().RegisterViewWithRegion(WellKnowRegionNames.WorkspaceContainer, typeof(WorkspaceView));
        }


        protected virtual void RegisterSideBarView(IUnityContainer container)
        {
            container.Resolve<IRegionManager>().RegisterViewWithRegion(WellKnowRegionNames.SidebarContainer, typeof(SideBarView));
        }

        private void RegisterNavigationBar(IUnityContainer container)
        {
            container.RegisterViewWithViewModel(typeof(NavigationBarView), typeof(INavigationBar));
        }

        protected virtual void ActivateNavigationBar(IUnityContainer container)
        {
            container.Resolve<IRegionManager>().NavigateToViewModel(WellKnowRegionNames.NavigationBar,
                                                                    container.Resolve<INavigationBar>());
        }

        protected virtual void RegisterExplorerBarView(IUnityContainer container)
        {
            container.RegisterViewWithViewModel(GetExplorerBarViewType(), typeof(IExplorerBar));
        }

        protected virtual void ActivateExplorerBar(IUnityContainer container)
        {
            container.Resolve<IRegionManager>().NavigateToViewModel(WellKnowRegionNames.ExplorerBar,
                                                                    container.Resolve<IExplorerBar>());
        }

        protected virtual void SelectFirstItemInTheExplorerBar(IUnityContainer container)
        {
            container.Resolve<IExplorerBar>().SelectFirstItem();
        }

        protected abstract Type GetExplorerBarViewType();
    }
}
