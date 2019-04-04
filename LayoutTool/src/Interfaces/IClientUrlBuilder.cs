using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Interfaces
{
    
    public interface IClientUrlBuilder
    {
        PathDescriptor BuildBaseUrl(ABTestCase abTest);
        PathDescriptor BuildLaunchUrl(ABTestCase abTest, EnvironmentConnection environmentConnection);
        string GetVersion(ABTestCase test);
    }

    
}
