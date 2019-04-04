using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;

namespace LayoutTool.Models
{
    public class BrandInformationProvider : IBrandInformationProvider
    {
        public BrandInformationProvider(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            LoadProductionClientsInformation();
            LoadQAClientsInformation();
        }

        IServiceLocator _serviceLocator;

        JsonEntities.NDL ProductionClients { get; set; }
        JsonEntities.NDL QAClients { get; set; }

        IWebClientFactory WebClientFactory
        {
            get { return _serviceLocator.GetInstance<IWebClientFactory>(); }
        }

        private void LoadProductionClientsInformation()
        {
            using (var webClient = CreateWebClient())
            {
                string response = webClient.DownloadString(new PathDescriptor("http://192.118.67.182:85/productionBrandsNDL/versions.json"));

                this.ProductionClients = DeserializeNDLEntities(response);
            }
        }

        private static JsonEntities.NDL DeserializeNDLEntities(string response)
        {
            var result = JsonConvert.DeserializeObject<JsonEntities.NDL>(response);

            foreach(var brand in result.brands)
            {
                foreach(var skin in brand.skins)
                {
                    if (skin.languages == null)
                        skin.languages = result.languages;
                }
            }

            return result;
        }

        private void LoadQAClientsInformation()
        {
            using (var webClient = CreateWebClient())
            {
                string response = webClient.DownloadString(new PathDescriptor("https://mirage-ndl.888sport.com/ndl/versions.json"));

                
                this.QAClients = DeserializeNDLEntities(response);
            }
        }

        private IWebClient CreateWebClient()
        {
            return this.WebClientFactory.CreateWebClient();
        }

        public BrandEntity[] GetProductionBrands()
        {
            return this.ProductionClients.brands.Select(b => new BrandEntity(int.Parse(b.id), 
                                                                            b.value, 
                                                                            GetProductionCdnUrl(b.id), 
                                                                            GetProductionSkins(b.id)))
                                                .ToArray();
        }

        public QAEnvironmentEntity[] GetQAEnvironments()
        {
            var environmnets = new List<QAEnvironmentEntity>();
            foreach (var env in QAClients.environments)
            {
                var environmentEntity = new QAEnvironmentEntity(env.id, env.value, env.path);
                var envBaseUrl = env.path;
                if (!envBaseUrl.EndsWith("/"))
                    envBaseUrl += "/";

                foreach(var ver in env.versions)
                {
                    var versionJson = QAClients.versions.FirstOrDefault(v => v.id == ver.id);

                    if (versionJson == null)
                        continue;


                    var versionEntity = new ClientVersionEntity(versionJson.id /*, versionJson.value*/);

                    foreach(var bg in versionJson.brandsGroups)
                    {
                        foreach (var brandJson in QAClients.brands.Where(b => b.group == bg.id))
                        {
                            versionEntity.Brands.Add(new BrandEntity(int.Parse(brandJson.id),
                                                                     brandJson.value,
                                                                     envBaseUrl + versionEntity.Path,
                                                                     brandJson.skins.Select(s => CreateSkinEntity(s)).ToArray()));
                        }

                    }

                    environmentEntity.ClientVersions.Add(versionEntity);
                }

                environmnets.Add(environmentEntity); 
            }

            return environmnets.ToArray();
        }

        private static SkinEntity CreateSkinEntity(JsonEntities.Skin skin)
        {
            return new SkinEntity(int.Parse(skin.id), 
                                  skin.value, 
                                  skin.languages.Select(lang => new LanguageEntity(lang.id, lang.value))
                                                .ToArray());
        }

        SkinEntity[] GetProductionSkins(string brandId)
        {


            var brand = this.ProductionClients.brands.FirstOrDefault(b => b.id == brandId.ToString());

            if(brand == null)
            {
                return new SkinEntity[0];
            }

            return brand.skins.Select(s => CreateSkinEntity(s))
                              .ToArray();
            
        }

        private PathDescriptor GetProductionCdnUrl(string brandId)
        {
            var brand = this.ProductionClients.brands.FirstOrDefault(b => b.id == brandId);

            if (brand == null)
                return null;

            var group = this.ProductionClients.GetGroupForBrand(brand);

            return CDNMappings.GetCdnUrl(group.domain);
        }

        private ITextFileReader TextFileReader
        {
            get { return _serviceLocator.GetInstance<ITextFileReader>(); }
        }
    }
}
