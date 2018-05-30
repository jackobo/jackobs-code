using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using LayoutTool.Interfaces;

namespace LayoutTool.Models.Builders.Xml
{
    public class XmlABTestingConfigurationReader 
    {
        public XmlABTestingConfigurationReader(IWebClientFactory webClientFactory, IClientUrlBuilder clientUrlBuilder)
        {
            _webClientFactory = webClientFactory;
            _clientUrlBuilder = clientUrlBuilder;
        }


        
        IWebClientFactory _webClientFactory;
        IClientUrlBuilder _clientUrlBuilder;

        public ABTestConfiguration Read(PathDescriptor cdnUrl)
        {

            using (var webClient = _webClientFactory.CreateWebClient())
            {
                try
                {
                    var xml = webClient.DownloadString(cdnUrl + new PathDescriptor("application/ABTesting_Configuration.xml"));

                    if (string.IsNullOrEmpty(xml?.Trim()))
                        return new ABTestConfiguration();

                    return ParseABTestingConfiguration(xml, _clientUrlBuilder, webClient);
                }
                catch (HttpException)
                {
                    return new ABTestConfiguration();
                }
            }
        }

        private ABTestConfiguration ParseABTestingConfiguration(string xml, IClientUrlBuilder clientUrlInfo, IWebClient webClient)
        {
            var xmlDoc = XmlUtils.Parse(xml);
                        
            var root = xmlDoc.Root;

            var abTestConfiguration = new ABTestConfiguration(root.GetAttributeValue("ID"),
                                                              root.GetAttributeValue("Name"),
                                                              root.GetAttributeValue("Description"));

            
            foreach (var testCasesElement in root.Elements("test_cases"))
            {
                var testCasesCollection = new ABTestCaseSet(abTestConfiguration.Id, 
                                                         testCasesElement.GetAttributeValue("brand"),
                                                         testCasesElement.GetAttributeValue("skin"),
                                                         testCasesElement.GetAttributeValue("lang"));

                foreach(var tcElement in testCasesElement.Elements("test_case"))
                {
                    var abTest = new ABTestCase(tcElement.GetAttributeValue("ID"),
                                                           tcElement.GetAttributeValue<bool>("IsDefault"),
                                                           tcElement.GetAttributeValue<ABTestMethod>("Method"),
                                                           tcElement.GetAttributeValue("Name"),
                                                           tcElement.GetAttributeValue<decimal>("UsePercentage"),
                                                           tcElement.GetAttributeValue("Description"),
                                                           new PathDescriptor(tcElement.GetAttributeValue("ClientPath")));


                    if (abTest.Method == ABTestMethod.Override)
                    {
                        abTest.FilesOverride = ReadFilesOverride(webClient, clientUrlInfo, abTest.ClientPath);
                    }

                    testCasesCollection.Add(abTest);
                }

                if(testCasesCollection.Count > 0)
                {
                    abTestConfiguration.TestCaseSets.Add(testCasesCollection);
                }

            }

            return abTestConfiguration;
        }

        private ABTestFileOverrideCollection ReadFilesOverride(IWebClient webClient, IClientUrlBuilder clientUrlInfo, PathDescriptor overrideFileRelativeUrl)
        {
            
            var baseUrl = clientUrlInfo.BuildBaseUrl(null);
          
            var overrideFileUrl = baseUrl + overrideFileRelativeUrl;

            var overrideFileContent = webClient.DownloadString(overrideFileUrl);

            var xmlDocument = XmlUtils.Parse(overrideFileContent);

            var result = new ABTestFileOverrideCollection();
            foreach (var overrideElement in xmlDocument.Root.Elements("overrides"))
            {
                result.Add(new ABTestFileOverride(overrideElement.GetAttributeValue("originalFile"),
                                                  overrideElement.GetAttributeValue("overrideFile")));
            }

            return result;
            
        }


     
    }
}
