using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Interfaces
{
    public interface IClientInformationProvider
    {
        bool CanHandle(BrandEntity brand, SkinEntity skin);

        IClientUrlBuilder GetClientUrlBuilder(BrandEntity brand, SkinEntity skin);

        ISkinDefinitionBuilder GetSkinDefinitionBuilder(BrandEntity brand, SkinEntity skin, ABTestCaseDescriptor abTest);

        ABTestCaseSet[] GetABTestCases(BrandEntity brand, SkinEntity skin);

        EnvironmentConnection[] GetEnvironmentsConnections(BrandEntity brand, SkinEntity skin);
        IFiddlerOverrideProvider GetSocketsSetupOverrideProvider(BrandEntity brand, SkinEntity skin, EnvironmentConnection environmentConnection);
    }

  
}
