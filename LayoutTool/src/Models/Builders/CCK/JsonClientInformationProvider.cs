using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spark.Infra.Logging;
using static LayoutTool.Models.Builders.CCK.DynamicsUtils;

namespace LayoutTool.Models.Builders.CCK
{
    public class JsonClientInformationProvider : IClientInformationProvider
    {
        public JsonClientInformationProvider(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            _preloaderSetupParser = new PreloaderSetupParser();
        }


        IPreloaderSetupParser _preloaderSetupParser;
        IServiceLocator _serviceLocator;
     
        public bool CanHandle(BrandEntity brand, SkinEntity skin)
        {
            using (var webClient = _serviceLocator.GetInstance<IWebClientFactory>().CreateWebClient())
            {
                return CCKUtils.IsCCKClient(brand, skin, webClient);
            }
        }



        public ISkinDefinitionBuilder GetSkinDefinitionBuilder(BrandEntity brand, SkinEntity skin, ABTestCaseDescriptor abTestDescriptor)
        {

            var clientBuilder = GetClientUrlBuilder(brand, skin);
            IPreloaderSetup preloaderSetup = GetPreloaderSetup(brand, skin);

            var files = CreateFiles(preloaderSetup.GetFileDescriptors(brand, skin),
                                    clientBuilder.BuildBaseUrl(abTestDescriptor?.Test),
                                    new SkinCode(brand.Id, skin.Id),
                                    abTestDescriptor?.Test);
            
            return new SkinDefinitionBuilder(clientBuilder.GetVersion(abTestDescriptor?.Test), CreateParser(files), CreateConverer(files));
        }

        private IPreloaderSetup GetPreloaderSetup(BrandEntity brand, SkinEntity skin)
        {
            using (var webClient = _serviceLocator.GetInstance<IWebClientFactory>().CreateWebClient())
            {
                return _preloaderSetupParser.Parse(CCKUtils.ReadPreloaderSetup(brand, skin, webClient));
            }
        }

        private ISkinDefinitionConverter CreateConverer(IEnumerable<ClientConfigurationFile<IJsonConfigurationFileDescriptor>> files)
        {
            var converter = new JsonSkinDefinitionConverter();
            foreach (var file in files)
            {
                file.Descriptor.ApplyToConverter(converter, file);
            }

            return converter;
        }

        private ISkinDefinitionParser CreateParser(IEnumerable<ClientConfigurationFile<IJsonConfigurationFileDescriptor>> files)
        {
            var parser = new JsonSkinDefinitionParser(_serviceLocator.GetInstance<ILoggerFactory>());
            foreach(var file in files)
            {
                try
                {
                    file.Descriptor.ApplyToParser(parser, file);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"Failed to parse file {file.Location}", ex);
                }
            }

            return parser;
        }

        private List<ClientConfigurationFile<IJsonConfigurationFileDescriptor>> CreateFiles(
                        IEnumerable<IJsonConfigurationFileDescriptor> fileDescriptors, 
                        PathDescriptor rootFolder,
                        SkinCode skinCode, 
                        ABTestCase abTest)
        {
            var files = new List<ClientConfigurationFile<IJsonConfigurationFileDescriptor>>();
            foreach (var fileDescriptor in fileDescriptors)
            {
                var fullPath = BuildFullPath(fileDescriptor, rootFolder, skinCode, abTest);
                var json = TextFileReader.ReadAllText(fullPath);

                files.Add(new ClientConfigurationFile<IJsonConfigurationFileDescriptor>(fullPath, json, fileDescriptor));
            }

            return files;
        }

        private PathDescriptor BuildFullPath(IConfigurationFileDescriptor fileDescriptor, PathDescriptor rootFolder, SkinCode skinCode, ABTestCase abTest)
        {
            PathDescriptor relativeFilePath = null;
            if (abTest != null)
            {
                relativeFilePath = abTest.GetOverrideFileOrNull(fileDescriptor, skinCode);
            }

            if (relativeFilePath == null)
                relativeFilePath = fileDescriptor.GetRelativePath(skinCode);


            return rootFolder + relativeFilePath;

        }

        
        ITextFileReader TextFileReader
        {
            get
            {
                return _serviceLocator.GetInstance<ITextFileReader>();
            }
        }
        

        public ABTestCaseSet[] GetABTestCases(BrandEntity brand, SkinEntity skin)
        {
            

            var preloaderSetup = GetPreloaderSetup(brand, skin);

            var skinCode = new SkinCode(brand.Id, skin.Id);
            var fileDescriptors = preloaderSetup.GetFileDescriptors(brand, skin);

            var result = new List<ABTestCaseSet>();
            foreach(var testCase in FindABTestCases(brand, skin, preloaderSetup))
            {
                if(testCase.AppliesTo(skinCode, fileDescriptors))
                {
                    result.Add(testCase);
                }
            }

            return result.ToArray();
        }


        private ABTestCaseSet[] FindABTestCases(BrandEntity brand, SkinEntity skin, IPreloaderSetup preloaderSetup)
        {
            var abTestConfigurationReader = CreateJsonABTestingConfigurationReader(brand, skin, preloaderSetup);

            var abTestConfiguration = abTestConfigurationReader.Read(brand.CDNUrl);

            if (abTestConfiguration == null)
                return new ABTestCaseSet[0];

            return abTestConfiguration.GetApplicableTestCases(new SkinCode(brand.Id, skin.Id));


        }

        private JsonABTestingConfigurationReader CreateJsonABTestingConfigurationReader(BrandEntity brand, SkinEntity skin, IPreloaderSetup preloaderSetup)
        {
            return new JsonABTestingConfigurationReader(_serviceLocator.GetInstance<IWebClientFactory>(),
                                                       preloaderSetup.GetNavigationPlanPath(brand, skin));
        }
        

        public EnvironmentConnection[] GetEnvironmentsConnections(BrandEntity brand, SkinEntity skin)
        {
            return GetPreloaderSetup(brand, skin).GetEnvironmentConnections(brand, skin);
        }

     

        public IFiddlerOverrideProvider GetSocketsSetupOverrideProvider(BrandEntity brand, SkinEntity skin, EnvironmentConnection environmentConnection)
        {
            if (environmentConnection == null)
                throw new ArgumentNullException(nameof(environmentConnection));
            var environmentConfigurationFilePath = GetClientUrlBuilder(brand, skin).BuildBaseUrl(null) + environmentConnection.ConfigurationFilePath;
            return new JsonSocketsSetupOverrideProvider(environmentConfigurationFilePath,
                                                        _serviceLocator.GetInstance<IMainProxyAdapter>().Port);
        }

        public IClientUrlBuilder GetClientUrlBuilder(BrandEntity brand, SkinEntity skin)
        {
            return new JsonClientUrlBuilderFactory(_serviceLocator.GetInstance<IWebClientFactory>(),
                                                   GetPreloaderSetup(brand, skin))
                                                   .GetClientUrlBuilder(brand, skin);
        }
    }
}
