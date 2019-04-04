using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;

namespace GGPMockBootstrapper
{
    public class GGPMockProgram
    {
        public static void Run(string[] args)
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                
                RunAsAdmin(args);
            }
            else
            {
                RunApp(args);
            }
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

        private static void RunApp(string[] args)
        {
            string version = null;

            if (args.Length > 0)
            {
                version = args[0];
                if(args.Length >= 2)
                    ManualClickOnceInit(args[1]);
            }
            App app = new App(version);

            app.StartWithSplash<App>(args, () => new MainWindow());
        }

        private static void RunAsAdmin(string[] args)
        {
            if (args.Length > 0)
            {
                RunApp(args);
                return;
            }

            if (IsCurrentUserAnAdmin())
            {
                RunApp(new string[0]);
            }
            else
            {
                var processStartInfo = new ProcessStartInfo(Assembly.GetEntryAssembly().CodeBase);
                var activationContext = AppDomain.CurrentDomain.ActivationContext;
                
                processStartInfo.Arguments =  "v" + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
                                                + " "
                                                + "\"" + activationContext.Identity.FullName + "\"";

                processStartInfo.UseShellExecute = true;
                processStartInfo.Verb = "runas";
                Process.Start(processStartInfo);
            }
            
        }

        private static bool IsWindows8OrAbove()
        {
            return System.Environment.OSVersion.Version.Major > 6
                    || (System.Environment.OSVersion.Version.Major == 6 && System.Environment.OSVersion.Version.Minor >= 2);
        }


        private static bool IsCurrentUserAnAdmin()
        {
            
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }


    
}
