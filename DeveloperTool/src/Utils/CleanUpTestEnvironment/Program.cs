using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CleanUpTestEnvironment
{
    class Program
    {
        static void Main(string[] args)
        {
            new Cleaner().Clean();
        }
        
      
    }
}
