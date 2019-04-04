using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Spark.Wpf.Common.Views;
using Spark.Wpf.Common.ViewModels;
using Spark.TfsExplorer.ViewModels.ExplorerBar;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common;
using Spark.TfsExplorer.Views.ExplorerBar;

namespace Spark.TfsExplorer.Views
{
    [Module(ModuleName = WellKnownModules.Views)]
    [ModuleDependency(WellKnownModules.ViewModels)]
    public class ViewsModule : ExplorerStyleViewsModule
    {
        public ViewsModule(IUnityContainer container)
            : base(container)
        {
        }

        protected override Type GetExplorerBarViewType()
        {
            return typeof(ExplorerBarView);
        }
    }
}
