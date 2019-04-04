using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.Models
{
    public abstract class ClientUrlBuilder : IClientUrlBuilder
    {
        public ClientUrlBuilder(PathDescriptor cdnUrl, SkinCode skinCode, string version)
        {
            CdnUrl = cdnUrl;
            SkinCode = skinCode;
            Version = version;
        }


        protected PathDescriptor CdnUrl { get; private set; }
        protected SkinCode SkinCode { get; private set; }
        protected string Version { get; private set; }

        public PathDescriptor BuildBaseUrl(ABTestCase abTest)
        {
            if (abTest == null || abTest.Method != ABTestMethod.FullClient)
                return GetDefaultBaseUrl();


            if (abTest.ClientPath.IsEmpty())
                return GetDefaultBaseUrl();
            else
                return CdnUrl + abTest.ClientPath;

        }

        public string GetVersion(ABTestCase test)
        {
#warning in case of A/B test the version folder might be overriden
            return Version;
        }

        public abstract PathDescriptor BuildLaunchUrl(ABTestCase abTest, EnvironmentConnection environmentConnection);

        
        private PathDescriptor GetDefaultBaseUrl()
        {
            return CdnUrl + new PathDescriptor(Version);
        }

       
    }
}
