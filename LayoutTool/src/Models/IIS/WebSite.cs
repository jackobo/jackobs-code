using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using Microsoft.Web.Administration;

namespace LayoutTool.Models.IIS
{
    internal interface IIisWebSite
    {
        string BuildAppUrl(string appPath);
    }

    internal class WebSite : Interfaces.IWebSite, IIisWebSite
    {
        private Site _iisSite;
        internal WebSite(Site site)
        {
            _iisSite = site;
        }

        public string Name
        {
            get
            {
                return _iisSite.Name;
            }
        }

        public IWebApp[] GetApps()
        {
            
            
            return _iisSite.Applications.Select(app => new WebApp(app, this)).ToArray();
        }

        public override string ToString()
        {
            return this.Name;
        }

        public string BuildAppUrl(string appPath)
        {
            var port = GetWebsiteHttpPort();

            if(port == 80)
                return $"http://localhost{appPath}";
            else
                return $"http://localhost:{GetWebsiteHttpPort()}{appPath}";
        }
        
        private int? GetWebsiteHttpPort()
        {
            var httpBinding = _iisSite.Bindings.Where(b => b.Protocol == "http").FirstOrDefault();

            if (httpBinding != null && httpBinding.EndPoint != null)
            {
                return httpBinding.EndPoint.Port;
            }

            return null;
        }

    }
}
