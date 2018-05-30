using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            GGPMockProgram.Run(args);
        }
    }
}
