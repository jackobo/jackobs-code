using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Interfaces
{
    public interface IWebServerManager
    {
        IWebSite[] GetWebSites(); 
    }


    public interface IWebSite
    {
        string Name { get; }

        
        IWebApp[] GetApps();
    }

    public interface IWebApp
    {
        IVirtualDirectory[] GetVirtualDirectories();
    }

    public interface IVirtualDirectory
    {
        string Name { get; }
        string PhysicalPath { get; }
        PathDescriptor HttpAddress { get; }
    }
}
