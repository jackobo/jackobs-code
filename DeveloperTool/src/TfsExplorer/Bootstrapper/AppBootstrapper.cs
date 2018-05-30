using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Spark.Infra.Types;
using Spark.Wpf.Common;

namespace Spark.TfsExplorer
{
    public class AppBootstrapper : ExplorerStyleAppBoostrapper<App>
    {
        public AppBootstrapper(string environmentName, Optional<Icon> appIcon)
            : base(appIcon)
        {
            _environmentName = environmentName;
            
        }
        string _environmentName;

        public static void Start(string environmentName, string[] args)
        {
            Start(environmentName, args, Optional<Icon>.None());
        }

        public static void Start(string environmentName, string[] args, Optional<Icon> appIcon)
        {
            AppRunner.RunAsAdmin(args, new AppBootstrapper(environmentName, appIcon), true);
        }
        

        public override string GetApplicationUniqueName()
        {
            return "GGPDeveloperTool";
        }

        protected override string GetApplicationFriendlyName()
        {
            return $"GGP Developer Tool - {_environmentName}";
        }
    }
}
