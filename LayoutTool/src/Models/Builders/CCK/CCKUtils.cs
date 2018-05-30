using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using Spark.Infra.Types;

namespace LayoutTool.Models.Builders.CCK
{
    public static class CCKUtils
    {
        public static string ReadPreloaderSetup(BrandEntity brand, SkinEntity skin, IWebClient webClient)
        {
            
            PathDescriptor cdnUrl = brand.CDNUrl;

            var versionInfo = ExtractVersionInfo(brand, skin, ParseVersionsJson(cdnUrl, webClient));

            PathDescriptor preloaderSetupUrl = null;
            if (string.IsNullOrEmpty(versionInfo.PreloaderSetupPath))
                preloaderSetupUrl = cdnUrl + new PathDescriptor("application/preloader_setup.json");
            else
                preloaderSetupUrl = cdnUrl + new PathDescriptor("application/" + new PathDescriptor(versionInfo.PreloaderSetupPath));
            return webClient.DownloadString(preloaderSetupUrl);
        }


        public static string ReadVersionsJsonFile(PathDescriptor cndUrl, IWebClient webClient)
        {
            var versionsUrl = cndUrl + new PathDescriptor("application/versions.json");
            return webClient.DownloadString(versionsUrl);
        }

        public static bool IsCCKClient(BrandEntity brand, SkinEntity skin, IWebClient webClient)
        {
            try
            {
                PathDescriptor cdnUrl = brand.CDNUrl;
                var versionInfo = ExtractVersionInfo(brand, skin, ParseVersionsJson(cdnUrl, webClient));
                
                if (versionInfo == null)
                    return false;

                var majorDefaultClientVersion = NdlVersionParser.ExtractMajorVersion(versionInfo.Version);

                if (string.IsNullOrEmpty(majorDefaultClientVersion))
                    return false;

                return VersionNumber.Parse(majorDefaultClientVersion) >= new VersionNumber("7.4");

            }
            catch (HttpException)
            {
                return false;
            }

        }

        private static VersionsFileContent ParseVersionsJson(PathDescriptor cdnUrl, IWebClient webClient)
        {
            var content = ReadVersionsJsonFile(cdnUrl, webClient);
            return ClientUrlBuilderFactory.ParseVersionsJson(content);
            
        }


        private static ClientVersionInfo ExtractVersionInfo(BrandEntity brand, SkinEntity skin, VersionsFileContent versions)
        {
            return ClientUrlBuilderFactory.ExtractVersion(brand, skin, versions);
        }




    }
}
