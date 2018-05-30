using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using Microsoft.Web.Administration;

namespace LayoutTool.Models.IIS
{
    internal class IISVirtualDirectory : IVirtualDirectory
    {
        
        public IISVirtualDirectory(string name, PathDescriptor httpAddress, string physicalPath)
        {
            Name = name;
            HttpAddress = httpAddress;
            PhysicalPath = physicalPath;
        }

        public string Name
        {
            get; private set;
        }

        

        public PathDescriptor HttpAddress { get; private set; }
        
        public string PhysicalPath
        {
            get; private set;
        }

     

        public override string ToString()
        {
            return Name;
        }
    }
}
