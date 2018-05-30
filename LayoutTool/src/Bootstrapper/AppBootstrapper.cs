using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Spark.Wpf.Common;

namespace LayoutTool
{
    public class AppBootstrapper : AppBootstrapper<App, MainWindow>
    {
        public AppBootstrapper(string environmentName, System.Drawing.Icon appIcon = null)
        {
            _environmentName = environmentName;
            _appIcon = appIcon;
        }

        string _environmentName;
        public override string GetApplicationUniqueName()
        {
            return "NdlLayoutTool";
        }


        System.Drawing.Icon _appIcon;

        protected override DependencyObject CreateShell()
        {
            MainWindow mainWindow = (MainWindow)base.CreateShell();


            if(_appIcon != null)
            {
                mainWindow.Icon = Imaging.CreateBitmapSourceFromHIcon(_appIcon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }

            return mainWindow;
        }


        protected override string GetApplicationFriendlyName()
        {
            return $"NDL Layout Tool - {_environmentName}";
        }

        public static void Start(string environmentName, string[] args, System.Drawing.Icon appIcon = null)
        {
            AppRunner.RunAsAdmin(args, new AppBootstrapper(environmentName, appIcon));
        }


        private static void SetBrowserFeatureControlKey(string feature, string appName, uint value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(
                String.Concat(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\", feature),
                RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                key.SetValue(appName, (UInt32)value, RegistryValueKind.DWord);
            }
        }

        public static void SetBrowserEmulationMode()
        {
            var fileName = System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);

            if (String.Compare(fileName, "devenv.exe", true) == 0 || String.Compare(fileName, "XDesProc.exe", true) == 0)
                return;

            UInt32 mode = 10001; //IE10
            SetBrowserFeatureControlKey("FEATURE_BROWSER_EMULATION", fileName, mode);
        }

    }
}
