using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace GGPMockBootstrapper.Models
{
    public static class ApplicationFolders
    {
        public static string AppData
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "GGPMockBootstrapper"); }
        }
    }
}
