using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.Wpf.Common.ViewModels
{
    public abstract class ExplorerStyleViewModelsModule : IModule
    {
        public ExplorerStyleViewModelsModule(IUnityContainer container)
        {
            this.Container = container;
        }

        IUnityContainer Container { get; set; }

        
        void IModule.Initialize()
        {
            Initialize(this.Container);
        }

        protected virtual void Initialize(IUnityContainer container)
        {
            Container.RegisterInstance(ResolveNavigationBar(container));
            Container.RegisterInstance(ResolveExplorerBar(container));
            Container.RegisterInstance(ResolveSidebar(container));
            Container.RegisterInstance(ResolveWorkspace(container));

            var backgroundOperations = container.Resolve<BackgroundOperationsRegion>();
            container.RegisterInstance(backgroundOperations);
            container.RegisterInstance<IBackgroundOperationsRegion>(backgroundOperations);
            
        }

        protected abstract IExplorerBar ResolveExplorerBar(IUnityContainer container);

        protected virtual IWorkspace ResolveWorkspace(IUnityContainer container)
        {
            return container.Resolve<Workspace>();
        }

        protected virtual INavigationBar ResolveNavigationBar(IUnityContainer container)
        {
            return container.Resolve<NavigationBar>();
        }

        protected virtual ISidebar ResolveSidebar(IUnityContainer container)
        {
            return container.Resolve<Sidebar>();
        }

    }
}
