using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static LayoutTool.Models.Builders.CCK.DynamicsUtils;

namespace LayoutTool.Models.Builders.CCK
{
    public class JsonABTestingConfigurationReader 
    {
        public JsonABTestingConfigurationReader(IWebClientFactory webClientFactory, string defaultNavigationPlanPath)
        {
            _webClientFactory = webClientFactory;
            _defaultNavigationPlanPath = defaultNavigationPlanPath;
        }

        IWebClientFactory _webClientFactory;
        string _defaultNavigationPlanPath;

        public ABTestConfiguration Read(PathDescriptor cdnUrl)
        {
            using (var webClient = _webClientFactory.CreateWebClient())
            {
                try
                {
                    var json = webClient.DownloadString(cdnUrl + new PathDescriptor("application/ABTestConfig.json"));

                    if (string.IsNullOrEmpty(json?.Trim()))
                        return new ABTestConfiguration();

                    return ParseABTestingConfiguration(json);
                }
                catch (HttpNotFoundException)
                {
                    return new ABTestConfiguration();
                }
            }
        }

        private ABTestConfiguration ParseABTestingConfiguration(string abTestConfigContent)
        {
            var languageJson = JsonConvert.DeserializeObject<dynamic>(abTestConfigContent);

            var config = new ABTestConfiguration();

            if (languageJson.abTests == null)
                return config;
            
            foreach (var abTestElement in languageJson.abTests)
            {
                config.Id = ConvertDynamicValue<string>(abTestElement.ID);
                config.Name = ConvertDynamicValue<string>(abTestElement.Name);
                config.Description = ConvertDynamicValue<string>(abTestElement.Description);

                if (abTestElement.testCasesSets == null)
                    continue;

                foreach (var testCaseSetElement in abTestElement.testCasesSets)
                {
                    var testCaseSet = new ABTestCaseSet(ConvertDynamicValue<string>(testCaseSetElement.id),
                                                     ConvertDynamicValue<string>(testCaseSetElement.brand),
                                                     ConvertDynamicValue<string>(testCaseSetElement.skin),
                                                     ConvertDynamicValue<string>(testCaseSetElement.lang));

                    if (testCaseSetElement.testCases == null)
                        continue;

                    foreach(var testCaseElement in testCaseSetElement.testCases)
                    {
                        var testCase = new ABTestCase(ConvertDynamicValue<string>(testCaseElement.ID),
                                                       ConvertDynamicValue<bool>(testCaseElement.isDefault),
                                                       ConvertDynamicValue<ABTestMethod>(testCaseElement.Method),
                                                       ConvertDynamicValue<string>(testCaseElement.Name),
                                                       ConvertDynamicValue<decimal>(testCaseElement.UsePercentage),
                                                       ConvertDynamicValue<string>(testCaseElement.Description),
                                                       new PathDescriptor(ConvertDynamicValue<string>(testCaseElement.ClientFolder)));
                        

                        if(null != ((JObject)testCaseElement)?.Property("configurationPaths"))
                        {
                            

                            if(null != ((JObject)testCaseElement.configurationPaths)?.Property("navigationPlanPath"))
                            {
                                testCase.FilesOverride.Add(new ABTestFileOverride(_defaultNavigationPlanPath, ConvertDynamicValue<string>(testCaseElement.configurationPaths.navigationPlanPath)));
                            }
                        }

                        testCaseSet.Add(testCase);

                    }

                    config.TestCaseSets.Add(testCaseSet);
                }
            }


            return config;

        }
    }
}
