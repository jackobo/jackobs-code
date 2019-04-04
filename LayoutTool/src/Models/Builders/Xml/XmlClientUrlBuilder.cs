using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.Models.Builders.Xml
{
    public class XmlClientUrlBuilder : ClientUrlBuilder
    {
        public XmlClientUrlBuilder(PathDescriptor cdnUrl, SkinCode skinCode, string version)
            : base(cdnUrl, skinCode, version)
        {

        }

        public override PathDescriptor BuildLaunchUrl(ABTestCase abTest, EnvironmentConnection environmentConnection)
        {
            string abTestingOverridePathUrlParameter = abTest?.GetOverrideParameterPathUrlParameter();


#warning I might need to do something with languageid parameter

            //var antiCacheKey = new Random().Next();
            //antiCacheKey ={ antiCacheKey}&

            return BuildBaseUrl(abTest) +
                   new PathDescriptor($"bin/application/ViewActual.html?navigationplanid=-1&languageid=en&brandid={SkinCode.BrandId}&skinid={SkinCode.SkinId}{abTestingOverridePathUrlParameter}");

            
            

        }

    }
}
