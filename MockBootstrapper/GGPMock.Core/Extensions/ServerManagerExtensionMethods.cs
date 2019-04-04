using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.Administration;

namespace GGPMockBootstrapper
{
    public static class ServerManagerExtensionMethods
    {
        public static Application FindApplication(this ServerManager iisManager, string applicationName)
        {
            string appPath = "/" + applicationName;

            foreach (var site in iisManager.Sites)
            {
                foreach (var app in site.Applications)
                {
                    if (0 == string.Compare(appPath, app.Path, true))
                    {
                        return app;
                    }
                }
            }

            return null;

        }


        public static Site FindWebsiteContainingApp(this ServerManager iisManager, string applicationName)
        {
            string appPath = "/" + applicationName;

            foreach (var site in iisManager.Sites)
            {
                foreach (var app in site.Applications)
                {
                    if (0 == string.Compare(appPath, app.Path, true))
                    {
                        return site;
                    }
                }
            }

            return null;
        }


        public static string GetRootHomeDirectory(this ServerManager iisManager, string appName)
        {
            return iisManager.FindWebsiteContainingApp(appName).GetWebSiteHomeDirectory();
        }


        public static string GetWebSiteHomeDirectory(this Site webSite)
        {
            if (webSite == null)
                return null;

            return Environment.ExpandEnvironmentVariables(webSite.Applications["/"].VirtualDirectories["/"].PhysicalPath);
        }

        public static int GetApplicationHttpPort(string applicationName)
        {
            using (var siteManager = new ServerManager())
            {
                return siteManager.GetApplicationHttpPort(applicationName);
            }
        }

        public static int GetApplicationHttpPort(this ServerManager iisManager, string applicationName)
        {
            using (var serverManager = new ServerManager())
            {
                var webSite = serverManager.FindWebsiteContainingApp(applicationName);

                if (webSite != null)
                {

                    var httpBinding = webSite.Bindings.Where(b => b.Protocol == "http").FirstOrDefault();

                    if (httpBinding != null && httpBinding.EndPoint != null)
                    {
                        return httpBinding.EndPoint.Port;
                    }

                }
            }
            return 80;
        }

        public static string GetPhysicalPath(this Application app)
        {
            if (app == null)
                return null;

            return app.VirtualDirectories["/"].PhysicalPath;
        }
    }
}
