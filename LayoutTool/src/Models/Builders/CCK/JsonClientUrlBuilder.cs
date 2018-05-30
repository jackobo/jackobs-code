using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.Models.Builders.CCK
{
    public class JsonClientUrlBuilder : ClientUrlBuilder
    {
        public JsonClientUrlBuilder(PathDescriptor cdnUrl, SkinCode skinCode, string version, IPreloaderSetup preloaderSetup)
            : base(cdnUrl, skinCode, version)
        {
            _preloaderSetup = preloaderSetup;
        }

        IPreloaderSetup _preloaderSetup;

        public override PathDescriptor BuildLaunchUrl(ABTestCase abTest, EnvironmentConnection environmentConnection)
        {
            
            var launchConfiguration = _preloaderSetup.GetLaunchConfigurationBuilder(SkinCode);

            var configData = launchConfiguration.Build(abTest, environmentConnection);

            return BuildBaseUrl(abTest) +
                       new PathDescriptor($"bin/application/ViewActual.html?entrancemode=1&navigationplanid=-1&languageid=en&brandid={SkinCode.BrandId}&skinid={SkinCode.SkinId}&isdebugmode=1&isSelectedDebugHost=0&GRSState=LATEST&GRSAdditionalState=LATEST&GRSGameTechnology=DEFAULT&CommunicationType=socketProxy&Environment={environmentConnection.Name}&preloaderSetupPath=preloader_setup.json&configData={configData}");


        }

    }
}
