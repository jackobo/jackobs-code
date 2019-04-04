using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common;

namespace LayoutTool.Models.Builders.Xml
{

    public class XmlClientInformationProvider : IClientInformationProvider
    {
        public XmlClientInformationProvider(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }


        IServiceLocator _serviceLocator;
        ITextFileReader TextFileReader
        {
            get
            {
                return _serviceLocator.GetInstance<ITextFileReader>();
            }
        }
        

        private IEnumerable<IXmlConfigurationFileDescriptor> DiscoverFilesDescriptors()
        {

            return _serviceLocator.DiscoverAndResolveAll<IXmlConfigurationFileDescriptor>(this.GetType().Assembly);

        }

        public bool CanHandle(BrandEntity brand, SkinEntity skin)
        {
            return !IsCCKClient(brand, skin); 
        }

        public ISkinDefinitionBuilder GetSkinDefinitionBuilder(BrandEntity brand, SkinEntity skin, ABTestCaseDescriptor abTest)
        {
            var clientUrlBuilder = GetClientUrlBuilder(brand, skin);
            var baseUrl = clientUrlBuilder.BuildBaseUrl(abTest?.Test);
            
            var files = CreateFiles(new SkinCode(brand.Id, skin.Id), baseUrl, abTest?.Test);
            return new SkinDefinitionBuilder(clientUrlBuilder.GetVersion(abTest?.Test), CreateParser(files), CreateConverter(files));
        }

        private bool IsCCKClient(BrandEntity brand, SkinEntity skin)
        {
            using (var webClient = _serviceLocator.GetInstance<IWebClientFactory>().CreateWebClient())
            {
                return CCK.CCKUtils.IsCCKClient(brand, skin, webClient);
            }
        }
        

        private string GetParentFolder(string path, int levels)
        {
            for (int i = 1; i <= levels; i++)
            {
                path = Path.GetDirectoryName(path);
            }

            return path;
        }


        private static SkinCode GetSkinCodeFromNavigationPlanPath(string navigationPlanFilePath)
        {
            var code = new DirectoryInfo(Path.GetDirectoryName(navigationPlanFilePath)).Name;

            return new SkinCode(int.Parse(code));
        }



        private List<ClientConfigurationFile<IXmlConfigurationFileDescriptor>> CreateFiles(SkinCode skinCode, PathDescriptor rootFolder, ABTestCase abTest)
        {
            var files = new List<ClientConfigurationFile<IXmlConfigurationFileDescriptor>>();

            
            foreach (var fileDescriptor in DiscoverFilesDescriptors())
            {   
                var fullPath = BuildFullPath(fileDescriptor, rootFolder, skinCode, abTest);
                
                var xml = TextFileReader.ReadAllText(fullPath);

                files.Add(new ClientConfigurationFile<IXmlConfigurationFileDescriptor>(fullPath, xml, fileDescriptor));
            }

            return files;
        }

        private PathDescriptor BuildFullPath(IConfigurationFileDescriptor fileDescriptor, PathDescriptor rootFolder, SkinCode skinCode, ABTestCase abTest)
        {
            PathDescriptor relativeFilePath = null;
            if(abTest != null)
            {
                relativeFilePath = abTest.GetOverrideFileOrNull(fileDescriptor, skinCode);
            }
            
            if (relativeFilePath == null)
                relativeFilePath = fileDescriptor.GetRelativePath(skinCode);


            return rootFolder + relativeFilePath;

        }

        private IXmlSkinDefinitionParser CreateParser(IEnumerable<ClientConfigurationFile<IXmlConfigurationFileDescriptor>> files)
        {
            var parser = _serviceLocator.TryResolve<XmlSkinDefinitionParser>();
            foreach (var file in files)
            {
                try
                {
                    file.Descriptor.ApplyToParser(parser, file.Content);
                }
                catch(Exception ex)
                {
                    throw new ApplicationException($"Failed to parse file {file.Location}", ex);
                }
            }

            return parser;
        }


        private IXmlSkinDefinitionConverter CreateConverter(IEnumerable<ClientConfigurationFile<IXmlConfigurationFileDescriptor>> files)
        {
            var converter = _serviceLocator.TryResolve<XmlSkinDefinitionConverter>();
            foreach (var file in files)
            {
                file.Descriptor.ApplyToConverter(converter, file);
            }

            return converter;
        }

        public ABTestCaseSet[] GetABTestCases(BrandEntity brand, SkinEntity skin)
        {

            var skinCode = new SkinCode(brand.Id, skin.Id);
            var fileDescriptors = DiscoverFilesDescriptors();
            
            return FindABTestCases(brand, skin).Where(tc => tc.AppliesTo(skinCode, fileDescriptors)).ToArray();
        }

        IWebClient CreateWebClient()
        {
            return _serviceLocator.GetInstance<IWebClientFactory>().CreateWebClient();
        }

        

        private ABTestCaseSet[] FindABTestCases(BrandEntity brand, SkinEntity skin)
        {
            var abTestConfigurationReader = CreateABTestingConfigurationReader(brand, skin);

            var abTestConfiguration = abTestConfigurationReader.Read(brand.CDNUrl);

            if (abTestConfiguration == null)
                return new ABTestCaseSet[0];

            return abTestConfiguration.GetApplicableTestCases(new SkinCode(brand.Id, skin.Id));
                

        }

        private XmlABTestingConfigurationReader CreateABTestingConfigurationReader(BrandEntity brand, SkinEntity skin)
        {
            
            return new XmlABTestingConfigurationReader(_serviceLocator.GetInstance<IWebClientFactory>(),
                                                       _serviceLocator.TryResolve<XmlClientUrlBuilderFactory>().GetClientUrlBuilder(brand, skin));
            
        }

        
        public EnvironmentConnection[] GetEnvironmentsConnections(BrandEntity brand, SkinEntity skin)
        {
            return new EnvironmentConnection[0];
        }

        public IFiddlerOverrideProvider GetSocketsSetupOverrideProvider(BrandEntity brand, SkinEntity skin, EnvironmentConnection environmentConnection)
        {
            var clientUrlBuilder = GetClientUrlBuilder(brand, skin);

            return new XmlSocketsSetupOverrideProvider(clientUrlBuilder.BuildBaseUrl(null) + new PathDescriptor("setups/socket_setup.xml"),
                                                      _serviceLocator.GetInstance<IMainProxyAdapter>().Port);

        }

        public IClientUrlBuilder GetClientUrlBuilder(BrandEntity brand, SkinEntity skin)
        {
            return _serviceLocator.TryResolve<XmlClientUrlBuilderFactory>().GetClientUrlBuilder(brand, skin);
        }
    }
}
