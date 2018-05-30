using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            AppBootstrapper.Start("DEV", args, Properties.Resources.LayoutTool);
        }
    }
}
