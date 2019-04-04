using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyGamesToOutputDir
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("************************** SYNTAX ***************************");
                Console.WriteLine();
                Console.WriteLine("CopyGamesToOutputDir.exe ggpSolutionFolder outputGamesFolder");
                Console.WriteLine();
                Console.WriteLine("*************************************************************");
                return;
            }

            new CopyGamesToOutputDir(args[0], args[1]).Run();
        }

        
    }
}
