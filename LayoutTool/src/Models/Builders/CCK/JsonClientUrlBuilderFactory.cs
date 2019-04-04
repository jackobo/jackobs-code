using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using static LayoutTool.Models.Builders.CCK.DynamicsUtils;

namespace LayoutTool.Models.Builders.CCK
{

    public interface IJsonClientUrlBuilderFactory
    {
        IClientUrlBuilder GetClientUrlBuilder(BrandEntity brand, SkinEntity skin);
    }
    public class JsonClientUrlBuilderFactory : ClientUrlBuilderFactory, IJsonClientUrlBuilderFactory
    {
        public JsonClientUrlBuilderFactory(IWebClientFactory webClientFactory, IPreloaderSetup preloaderSetup)
            : base(webClientFactory)
        {
            _preloaderSetup = preloaderSetup;
        }

        IPreloaderSetup _preloaderSetup;

        protected override IClientUrlBuilder CreateUrlBuilder(BrandEntity brand, SkinEntity skinCode, string version)
        {

            return new JsonClientUrlBuilder(brand.CDNUrl, new SkinCode(brand.Id, skinCode.Id), version, _preloaderSetup);
        }

        protected override string GetVersion(BrandEntity brand, SkinEntity skin, IWebClient webClient)
        {
            return TryReadDefaultFolderFromVersionsJson(brand, skin, brand.CDNUrl, webClient);
        }



       
    }
}
