using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using Microsoft.Web.Administration;

namespace LayoutTool.Models.IIS
{
    internal class WebApp : Interfaces.IWebApp
    {
        private Application _iisApp;
        private IIisWebSite _iisWebSite;

        

        internal WebApp(Application app,  IIisWebSite iisWebSite)
        {
            _iisApp = app;
            _iisWebSite = iisWebSite;    
        }
        
        public IVirtualDirectory[] GetVirtualDirectories()
        {
            return _iisApp.VirtualDirectories.Select(vd => new IISVirtualDirectory(ExtractVirtualDirectoryName(vd), 
                                                                                   new PathDescriptor(_iisWebSite.BuildAppUrl(_iisApp.Path + vd.Path)),
                                                                                   vd.PhysicalPath)).ToArray();
        }

        private string ExtractVirtualDirectoryName(VirtualDirectory vd)
        {
            if (vd.Path == "/")
                return _iisApp.Path.Substring(1);
            else
                return vd.Path.Substring(1);
        }
    }
}
