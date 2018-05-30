using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Modularity;
using Microsoft.Practices.Unity;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.ViewModels.ExplorerBar;
using Spark.Wpf.Common.ViewModels;
using Spark.TfsExplorer.ViewModels.Workspace;
using Spark.Wpf.Common;

namespace Spark.TfsExplorer.ViewModels
{
    [Module(ModuleName = WellKnownModules.ViewModels)]
    [ModuleDependency(WellKnownModules.Models)]
    public class ViewModelsModule : ExplorerStyleViewModelsModule
    {
        public ViewModelsModule(IUnityContainer container)
            : base(container)
        {
        }

        protected override void Initialize(IUnityContainer container)
        {
            RegisterExplorerBarItemsFactory(container);
            base.Initialize(container);
        }

        protected override IExplorerBar ResolveExplorerBar(IUnityContainer container)
        {
            return container.Resolve<TfsExplorerBar>();
        }
        
      
        void RegisterExplorerBarItemsFactory(IUnityContainer container)
        {
            container.RegisterType<IExplorerBarItemsRepositoryFactory, ExplorerBarItemsRepositoryFactory>();
        }

    }
}
