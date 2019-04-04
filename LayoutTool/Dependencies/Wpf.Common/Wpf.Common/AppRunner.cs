using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using Prism.Unity;
using Spark.Infra.Types;
using Spark.Wpf.Common.Interfaces;

namespace Spark.Wpf.Common
{
    public class AppRunner : ISingleInstanceApp
    {

        private AppRunner(UnityBootstrapper unityBootstrapper)
        {
            _unityBootstrapper = unityBootstrapper;
        }

        UnityBootstrapper _unityBootstrapper;
        
        private static class CLIArgs
        {
            public static readonly string version = "version";
            public static readonly string identity = "identity";
            public static readonly string runAsAdmin = "runAsAdmin";
        }


        public static void RunAsAdmin<TBootstrapper>(string[] args, TBootstrapper bootstrapper, bool runAsSingleInstanceApplication = true)
            where TBootstrapper : UnityBootstrapper, IAppBootstrapper
        {
            if (runAsSingleInstanceApplication)
            {
                if (!SingleInstance<AppRunner>.InitializeAsFirstInstance(bootstrapper.GetApplicationUniqueName()))
                {
                    return;
                }
            }

            Current = new AppRunner(bootstrapper);
            Current.RunAsAdmin(ParseCommandLineArgs(args));
        }
        
        public static void Run<TBootstrapper>(string[] args, TBootstrapper bootstrapper, bool runAsSingleInstanceApplication = true)
            where TBootstrapper : UnityBootstrapper, IAppBootstrapper
        {

            if (runAsSingleInstanceApplication)
            {
                if (!SingleInstance<AppRunner>.InitializeAsFirstInstance(bootstrapper.GetApplicationUniqueName()))
                {
                    return;
                }
            }

            Run(args, bootstrapper);
        }


        
        public static void Run(string[] args, UnityBootstrapper bootstrapper)
        {
            Current = new AppRunner(bootstrapper);
            Current.Run(ParseCommandLineArgs(args));
        }

        //needed for the Single Instance Application behavior
        private static AppRunner Current { get; set; }

       

        private void RunAsAdmin(StringKeyValueCollection args)
        {
            if (args.Contains(CLIArgs.runAsAdmin))
            {
                System.Threading.Thread.Sleep(100);
                Run(args);
                return;
            }

            if (IsCurrentUserAnAdmin())
            {
                Run(args);
            }
            else
            {
                var processStartInfo = new ProcessStartInfo(Assembly.GetEntryAssembly().CodeBase);
                processStartInfo.Arguments = CreateRunAsAdminCommandLineArguments();

                processStartInfo.UseShellExecute = true;
                processStartInfo.Verb = "runas";
                Process.Start(processStartInfo);
            }

        }

        private static string CreateRunAsAdminCommandLineArguments()
        {
            var commandLineArguments = new StringKeyValueCollection();

            commandLineArguments.Add(new StringKeyValue(CLIArgs.runAsAdmin, "true"));

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                var activationContext = AppDomain.CurrentDomain.ActivationContext;
                commandLineArguments.Add(new StringKeyValue(CLIArgs.version, ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()));
                commandLineArguments.Add(new StringKeyValue(CLIArgs.identity, activationContext.Identity.FullName));

            }

            return string.Join(" ", commandLineArguments.Select(a => $"\"{a.Name}={a.Value}\""));
        }

        private void Run(StringKeyValueCollection commandLineArgs)
        {
            

            if (commandLineArgs.Contains(CLIArgs.identity))
            {
                ManualClickOnceInit(commandLineArgs[CLIArgs.identity].Value);
            }

            _unityBootstrapper.Run(true);
        }

        private static StringKeyValueCollection ParseCommandLineArgs(string[] args)
        {
            if (args == null || args.Length == 0)
                return new StringKeyValueCollection();

            return StringKeyValueCollection.Parse(string.Join(Environment.NewLine, args));
        }


        private static bool IsCurrentUserAnAdmin()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static void ManualClickOnceInit(string fullApplicationName)
        {
            try
            {
                var field = typeof(ApplicationDeployment).GetField("_currentDeployment", BindingFlags.NonPublic | BindingFlags.Static);
                var ctor = typeof(ApplicationDeployment).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(string) }, new ParameterModifier[0]);

                field.SetValue(null, ctor.Invoke(new string[] { fullApplicationName }));
            }
            catch
            {

            }
        }

        bool ISingleInstanceApp.SignalExternalCommandLineArgs(IList<string> args)
        {
            if (System.Windows.Application.Current != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (System.Windows.Application.Current.MainWindow == null)
                        return;

                    var Window = System.Windows.Application.Current.MainWindow;

                    if (!Window.IsVisible)
                    {
                        Window.Show();
                    }

                    if (Window.WindowState == System.Windows.WindowState.Minimized)
                    {
                        Window.WindowState = System.Windows.WindowState.Maximized;
                    }

                    Window.Activate();
                    Window.Topmost = true;  // important
                    Window.Topmost = false; // important
                    Window.Focus();         // important
                }));

            }

            return true;
        }
    }
}
