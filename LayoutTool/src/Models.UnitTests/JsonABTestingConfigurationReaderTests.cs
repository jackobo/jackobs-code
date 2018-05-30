using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Models.Builders.CCK;
using NSubstitute;
using NUnit.Framework;

namespace LayoutTool.Models
{
    [TestFixture]
    public class JsonABTestingConfigurationReaderTests
    {
        [SetUp]
        public void Setup()
        {
            var defaultNavigationPlanPath = "Configuration/navigation_plans/theme_4/brand_0/skin_4/navigation_plan.json";
            var webClient = Substitute.For<IWebClient>();
            webClient.DownloadStringReturns("http://localhost/casino/application/ABTestConfig.json", Properties.Resources.ABTestConfig);
            var webClientFactory = Substitute.For<IWebClientFactory>();
            webClientFactory.CreateWebClient().Returns(webClient);
            _jsonAbTestReader = new JsonABTestingConfigurationReader(webClientFactory, defaultNavigationPlanPath);
        }


        JsonABTestingConfigurationReader _jsonAbTestReader;


        private readonly PathDescriptor _cdnUrl = new PathDescriptor("http://localhost/casino");

        [Test]
        public void Read_ReturnsTheCorrectNumberOfAbTestCases()
        {
            var abTestConfiguration = _jsonAbTestReader.Read(_cdnUrl);

            Assert.AreEqual(3, abTestConfiguration.TestCaseSets.Count);
        }


        [Test]
        public void Read_SetsTheCorrectProperiesForTheConfiguration()
        {
            var abTestConfiguration = _jsonAbTestReader.Read(_cdnUrl);
            
            Assert.AreEqual("9", abTestConfiguration.Id);
            Assert.AreEqual("Test name as string", abTestConfiguration.Name);
            Assert.AreEqual("Long description of the test", abTestConfiguration.Description);
            
        }

        [TestCase(0, "1", "4", "en")]
        [TestCase(1, "1", "5", "en")]
        [TestCase(2, "2", "5", "it")]
        public void Read_LoadsAllTheTestsCaseSetsWithTheCorrectProperties(int testCaseSetIndex, string expectedBrand, string expectedSkin, string expectedLanguage)
        {
            var abTestConfiguration = _jsonAbTestReader.Read(_cdnUrl);

            var testCaseSet = abTestConfiguration.TestCaseSets[testCaseSetIndex];

            Assert.AreEqual(expectedBrand, testCaseSet.BrandId);
            Assert.AreEqual(expectedSkin, testCaseSet.SkinId);
            Assert.AreEqual(expectedLanguage, testCaseSet.Language);
        }

        [TestCase(0, 2)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        public void Read_LoadsTheCorrectNumberOfTestCases(int testCaseSetIndex, int expectedNumberOfTestCases)
        {
            var abTestConfiguration = _jsonAbTestReader.Read(_cdnUrl);

            var testCaseSet = abTestConfiguration.TestCaseSets[testCaseSetIndex];

            Assert.AreEqual(expectedNumberOfTestCases, testCaseSet.Count);
        }


        [Test]
        public void Read_CorrectlyFillTestCaseProperties()
        {
            var abTestConfiguration = _jsonAbTestReader.Read(_cdnUrl);

            var testCaseSet = abTestConfiguration.TestCaseSets[2];
            var testCase = testCaseSet[0];


            Assert.AreEqual("1", testCase.Id);
            Assert.AreEqual(true, testCase.IsDefault);
            Assert.AreEqual(ABTestMethod.Override, testCase.Method);
            Assert.AreEqual("FLASH", testCase.Name);
            Assert.AreEqual(99m, testCase.UsePercentage);
            Assert.AreEqual("MLNG is an FLASH game", testCase.Description);
            Assert.AreEqual(new PathDescriptor(""), testCase.ClientPath);
        }

        [Test]
        public void Read_CorrectlyLoadsTheOverridePaths()
        {
            var abTestConfiguration = _jsonAbTestReader.Read(_cdnUrl);

            var testCaseSet = abTestConfiguration.TestCaseSets[0];
            var testCase = testCaseSet[1];

            Assert.AreEqual(1, testCase.FilesOverride.Count);
            Assert.AreEqual(new PathDescriptor("Configuration/navigation_plans/theme_4/brand_0/skin_4/navigation_plan.json"),
                                               testCase.FilesOverride[0].OverrideFile);
            
        }

        [Test]
        public void Read_IfNoOverrideIsProvidedForTheNavigationPlan_DoNotAddAnyOverrideFile()
        {
            var abTestConfiguration = _jsonAbTestReader.Read(_cdnUrl);

            var testCaseSet = abTestConfiguration.TestCaseSets[2];
            var testCase = testCaseSet[0];

            Assert.AreEqual(0, testCase.FilesOverride.Count);

        }

    }
}
