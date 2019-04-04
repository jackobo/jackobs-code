using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Models.Builders.CCK;
using LayoutTool.Models.Builders.Xml;
using Newtonsoft.Json.Linq;
using static LayoutTool.Models.Builders.CCK.DynamicsUtils;

namespace LayoutTool.Models.Builders
{
    public abstract class ClientUrlBuilderFactory
    {
        public ClientUrlBuilderFactory(IWebClientFactory webClientFactory)
        {
            WebClientFactory = webClientFactory;
        }

        protected abstract string GetVersion(BrandEntity brand, SkinEntity skin, IWebClient webClient);

        IWebClientFactory WebClientFactory { get; set; }

        public virtual IClientUrlBuilder GetClientUrlBuilder(BrandEntity brand, SkinEntity skin)
        {
            using (var webClient = WebClientFactory.CreateWebClient())
            {
                return CreateUrlBuilder(brand, skin, GetVersion(brand, skin, webClient));
            }
        }


        protected abstract IClientUrlBuilder CreateUrlBuilder(BrandEntity brand, SkinEntity skinCode, string version);


        protected string TryReadDefaultFolderFromVersionsXml(BrandEntity brand, SkinEntity skin, PathDescriptor cdnUrlWithTrailingSlash, IWebClient webClient)
        {
            string versionsXml = null;

            try
            {
                versionsXml = webClient.DownloadString(cdnUrlWithTrailingSlash + new PathDescriptor("application/versions.xml"));
            }
            catch (HttpNotFoundException)
            {
                return null;
            }


            return ExtractVersion(brand, skin, ParseVersionsXml(versionsXml)).Version;
        }



        protected string TryReadeDefaultFolderFromClientSettingsJs(IWebClient webClient, PathDescriptor cdnUrl)
        {
            string clientSettingsJs = null;

            try
            {
                clientSettingsJs = webClient.DownloadString(cdnUrl + new PathDescriptor("application/client_settings.js"));
            }
            catch (HttpNotFoundException)
            {
                return null;
            }

            var jsLine = clientSettingsJs.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                          .FirstOrDefault(line => line.Contains("client_build_version "));

            if (jsLine == null)
                return null;

            return jsLine.Replace("var ", "")
                         .Replace("client_build_version ", "")
                         .Replace("=", "")
                         .Replace("'", "")
                         .Replace("\"", "")
                         .Replace(";", "")
                         .Trim();

        }



        private VersionsFileContent ParseVersionsXml(string xml)
        {
            var xmlDoc = XmlUtils.Parse(xml);

            var result = new VersionsFileContent();
            result.DefaultFolder = xmlDoc.Root.GetAttributeValue("defaultFolder");

            foreach (var clientXmlElement in xmlDoc.Root.Elements("client"))
            {
                result.Clients.Add(new VersionsFileContent.ClientElement(clientXmlElement.GetAttributeValue("brand"),
                                                              clientXmlElement.GetAttributeValue("skin"),
                                                              clientXmlElement.GetAttributeValue("lang"),
                                                              clientXmlElement.GetAttributeValue("version"),
                                                              null));
            }

            return result;
        }

        public static ClientVersionInfo ExtractVersion(BrandEntity brand, SkinEntity skin, VersionsFileContent versions)
        {
            var specificVersion = versions.Clients.FirstOrDefault(c => c.Brand == brand.Id.ToString()
                                                 && c.Skin == skin.Id.ToString()
                                                 && c.Language == "eng");

            if (specificVersion == null)
            {
                specificVersion = versions.Clients.FirstOrDefault(c => c.Brand == brand.Id.ToString()
                                                 && c.Skin == skin.Id.ToString()
                                                 && c.Language == "*");
            }

            if (specificVersion == null)
            {
                specificVersion = versions.Clients.FirstOrDefault(c => c.Brand == brand.Id.ToString()
                                                 && c.Skin == "*"
                                                 && c.Language == "*");
            }

            if (specificVersion != null)
            {
                return new ClientVersionInfo(specificVersion.Version, specificVersion.PreloaderSetupPath);
            }
            else
            {
                return new ClientVersionInfo(versions.DefaultFolder, versions.PreloaderSetupPath);
            }
        }

        public string TryReadDefaultFolderFromVersionsJson(BrandEntity brand, SkinEntity skin, PathDescriptor cdnUrlWithTrailingSlash, IWebClient webClient)
        {
            string versionsJson = null;

            try
            {
                versionsJson = CCKUtils.ReadVersionsJsonFile(cdnUrlWithTrailingSlash, webClient);
            }
            catch (HttpNotFoundException)
            {
                return null;
            }

            return ExtractVersion(brand, skin, ParseVersionsJson(versionsJson)).Version;

        }

        public static VersionsFileContent ParseVersionsJson(string versionsJson)
        {
            dynamic versions = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(versionsJson);

            var result = new VersionsFileContent();

            if (((JObject)versions).Property(JsonNames._default ) != null)
            {
                var defaultVersion = ((JObject)versions).Property(JsonNames._default).Value as JObject;
                result.DefaultFolder = defaultVersion.Property(JsonNames.version).Value.ToString();
                result.PreloaderSetupPath = defaultVersion.Property(JsonNames.setup).Value.ToString();
            }
            else
            {
                result.DefaultFolder = ConvertDynamicValue<string>(versions.defaultFolder);
            }
            

            if (versions.clients == null)
                return result;

            foreach (dynamic clientVersion in versions.clients)
            {
                string language = clientVersion.lang?.ToString();
                if (language == null)
                    language = clientVersion.language?.ToString();

                result.Clients.Add(new VersionsFileContent.ClientElement(ConvertDynamicValue<string>(clientVersion.brand),
                                                              ConvertDynamicValue<string>(clientVersion.skin),
                                                              language,
                                                              ConvertDynamicValue<string>(clientVersion.version),
                                                              ConvertDynamicValue<string>(clientVersion.setup)));
            }

            return result;
        }

    }
}
