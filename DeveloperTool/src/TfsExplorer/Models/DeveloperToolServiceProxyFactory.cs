using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models
{
    
    public static class DeveloperToolServiceProxyFactory
    {
        public static DeveloperToolService.GGPDeveloperToolServiceClient CreateProxy()
        {
            return new DeveloperToolService.GGPDeveloperToolServiceClient();
        }
    }
}
