using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using GamesPortal.Client.Views.Wpf;
using Spark.Infra.Logging;

using Prism.Modularity;
using Prism.Unity;
using Spark.Wpf.Common;

namespace GamesPortal.Client
{
    public class AppBootstrapper : AppBootstrapper<App, MainWindow>
    {
        public AppBootstrapper(string environmentName, System.Drawing.Icon appIcon = null)
        {
            _environmentName = environmentName;
            _appIcon = appIcon;
        }
        string _environmentName;

        public static void Start(string environmentName, string[] args, System.Drawing.Icon appIcon = null)
        {
            AppRunner.RunAsAdmin(args, new AppBootstrapper(environmentName, appIcon), false);
            
        }

        System.Drawing.Icon _appIcon;


        protected override DependencyObject CreateShell()
        {
            MainWindow mainWindow = (MainWindow)base.CreateShell();


            if (_appIcon != null)
            {
                mainWindow.Icon = Imaging.CreateBitmapSourceFromHIcon(_appIcon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }

            return mainWindow;
        }

        public override string GetApplicationUniqueName()
        {
            return "GamesPortalClient";
        }

        protected override string GetApplicationFriendlyName()
        {
            return $"Games Portal Client - {_environmentName}";
        }
    }

    
}
