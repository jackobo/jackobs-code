using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LayoutTool.Interfaces;

namespace LayoutTool.Models.Builders.Xml
{
    
    public class XmlClientUrlBuilderFactory : ClientUrlBuilderFactory
    {
        public XmlClientUrlBuilderFactory(IWebClientFactory webClientFactory)
            : base(webClientFactory)
        {
            
        }


        protected override IClientUrlBuilder CreateUrlBuilder(BrandEntity brand, SkinEntity skin, string version)
        {
            return new XmlClientUrlBuilder(brand.CDNUrl,
                                         new SkinCode(brand.Id, skin.Id),
                                         version);
        }

        protected override string GetVersion(BrandEntity brand, SkinEntity skin, IWebClient webClient)
        {

            var version = TryReadDefaultFolderFromVersionsJson(brand, skin, brand.CDNUrl, webClient);

            if (!string.IsNullOrEmpty(version))
            {
                return version;
            }

            version = TryReadDefaultFolderFromVersionsXml(brand, skin, brand.CDNUrl, webClient);

            if (!string.IsNullOrEmpty(version))
            {
                return version;

            }

            return TryReadeDefaultFolderFromClientSettingsJs(webClient, brand.CDNUrl);

        }
        
    }
}
