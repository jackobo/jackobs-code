using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Spark.Infra.Types;

namespace Spark.Wpf.Common
{
    public abstract class ExplorerStyleAppBoostrapper<TApp> : AppBootstrapper<TApp, Views.ExplorStyleMainWindow>
        where TApp : Application, new()
    {

        public ExplorerStyleAppBoostrapper(Optional<Icon> appIcon) : base(appIcon)
        {
        }
    }
}
