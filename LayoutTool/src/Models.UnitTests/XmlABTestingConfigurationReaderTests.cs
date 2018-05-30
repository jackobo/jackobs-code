using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Models.Builders.Xml;
using NSubstitute;
using NUnit.Framework;

namespace LayoutTool.Models.Builders.Xml
{
    [TestFixture]
    public class XmlABTestingConfigurationReaderTests
    {
        [Test]
        public void Read_IfNoABTestConfigurationFile_ReturnEmptyArray()
        {
            var webClient = Substitute.For<IWebClient>();
            webClient.DownloadString(Arg.Any<PathDescriptor>()).Returns(x => { throw new HttpNotFoundException(404, "Not found", "http://someurl"); });
            var reader = CreateReader(webClient);

            var testCases = reader.Read(new PathDescriptor("http://localhost"));

            Assert.AreEqual(0, testCases.TestCaseSets.Count);

        }

        [TestCase("http://ndl-cdn.888.com/casino")]
        [TestCase("http://ndl-cdn.888.com/casino/")]
        public void Read_UsesTheCorrectUrlToDownloadTheAbTestingConfigurationFile(string cdnUrl)
        {
            
            var webClient = Substitute.For<IWebClient>();
            
            var reader = CreateReader(webClient);

            reader.Read(new PathDescriptor(cdnUrl));

            webClient.Received().DownloadString(new PathDescriptor("http://ndl-cdn.888.com/casino/application/ABTesting_Configuration.xml"));
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void Read_IfEmptyConfigFileContentIsReturned_ReturnEmptyArray(string content)
        {

            var webClient = Substitute.For<IWebClient>();
            webClient.DownloadString(Arg.Any<PathDescriptor>()).Returns(content);

            var reader = CreateReader(webClient);

            var configuration = reader.Read(new PathDescriptor("http://localhost/casino"));

            Assert.AreEqual(0, configuration.TestCaseSets.Count);
        }

        [Test]
        public void Read_ConfigurationFileWithNoTestCase_ReturnEmptyArray()
        {

            var webClient = Substitute.For<IWebClient>();
            webClient.DownloadString(Arg.Any<PathDescriptor>()).Returns("<test ID=\"2\" Name=\"Test name as string\" Description=\"Long description of the test\"></test>");

            var reader = CreateReader(webClient);

            var configuration = reader.Read(new  PathDescriptor("http://localhost/casino"));

            Assert.AreEqual(0, configuration.TestCaseSets.Count);
        }

        [Test]
        public void Read_ConfigurationFileWithTestCasesDefined_ReturnsTheCorrentNumberOfItems()
        {
            var webClient = Substitute.For<IWebClient>();
            webClient.DownloadString(Arg.Any<PathDescriptor>()).Returns(
                @"<test ID='2' Name='Test name as string' Description='Long description of the test'>
                    <test_cases brand='0' skin='4' lang='en'>
                        <test_case ID='1' IsDefault='false' Method='1' Name='Acquisition' UsePercentage='98.00' Description='Acquisition login test case' ClientPath='setups/ab-testing/fileMapping1.xml'/>
                        <test_case ID='2' IsDefault='false' Method='1' Name='Anonymous' UsePercentage='1.0' Description='Anonymous login test case' ClientPath='setups/ab-testing/fileMapping1.xml'/>
                    </test_cases>

                    <test_cases brand='7' skin='10' lang='es'>
                        <test_case ID='4' IsDefault='false' Method='1' Name='Anonymous' UsePercentage='1.0' Description='Anonymous login test case' ClientPath='setups/ab-testing/fileMapping1.xml'/>
                    </test_cases>

                </test>");

            var reader = CreateReader(webClient);

            var configuration = reader.Read( new PathDescriptor("http://localhost/casino"));

            Assert.AreEqual(2, configuration.TestCaseSets.Count);

            var testCases = configuration.TestCaseSets[0];
            Assert.AreEqual(2, testCases.Count);

            testCases = configuration.TestCaseSets[1];
            Assert.AreEqual(1, testCases.Count);
        }

        [Test]
        public void Read_ConfigurationFileWithTestCasesDefined_TestCasePropertiesAreSetCorrectly()
        {
            var webClient = Substitute.For<IWebClient>();
            webClient.DownloadString(Arg.Any<PathDescriptor>()).Returns(
                @"<test ID='2' Name='Test name as string' Description='Long description of the test'>
                    <test_cases brand='0' skin='4' lang='en'>
                        <test_case ID='1' IsDefault='true' Method='1' Name='Acquisition' UsePercentage='93.55' Description='Acquisition login test case' ClientPath='setups/ab-testing/fileMapping1.xml'/>
                        <test_case ID='2' IsDefault='false' Method='2' Name='Test' UsePercentage='6.45' Description='test case' ClientPath='a/b/test.xml'/>
                    </test_cases>

                </test>");

            var reader = CreateReader(webClient);

            var configuration = reader.Read(new PathDescriptor("http://localhost/casino"));


            Assert.AreEqual("2", configuration.Id);
            Assert.AreEqual("Test name as string", configuration.Name);
            Assert.AreEqual("Long description of the test", configuration.Description);

            var testCases = configuration.TestCaseSets[0];
            Assert.AreEqual("0", testCases.BrandId);
            Assert.AreEqual("4", testCases.SkinId);
            Assert.AreEqual("en", testCases.Language);
            
            var testCase = testCases[0];
            Assert.AreEqual("1", testCase.Id);
            Assert.AreEqual(true, testCase.IsDefault);
            Assert.AreEqual(ABTestMethod.Override, testCase.Method);
            Assert.AreEqual("Acquisition", testCase.Name);
            Assert.AreEqual(93.55m, testCase.UsePercentage);
            Assert.AreEqual("Acquisition login test case", testCase.Description);
            AssertEquals("setups/ab-testing/fileMapping1.xml", testCase.ClientPath);

            testCase = testCases[1];
            Assert.AreEqual("2", testCase.Id);
            Assert.AreEqual(false, testCase.IsDefault);
            Assert.AreEqual(ABTestMethod.FullClient, testCase.Method);
            Assert.AreEqual("Test", testCase.Name);
            Assert.AreEqual(6.45m, testCase.UsePercentage);
            Assert.AreEqual("test case", testCase.Description);
            AssertEquals("a/b/test.xml", testCase.ClientPath);
        }



        [Test]
        public void Read_ConfigurationFileWithTestCasesDefined_FilesOverridesAreCorrectlyRead()
        {
            var brand = new BrandEntity(1, "888Casino", "http://localhost/casino");
            var skin = new SkinEntity(4, "Non UK");
            
            var webClient = Substitute.For<IWebClient>();
            webClient.DownloadStringReturns("http://localhost/casino/application/ABTesting_Configuration.xml",
                @"<test ID='2' Name='Test name as string' Description='Long description of the test'>
                    <test_cases brand='0' skin='4' lang='en'>
                        <test_case ID='1' IsDefault='true' Method='1' Name='Acquisition' UsePercentage='93.55' Description='Acquisition login test case' ClientPath='../../setups/ab-testing/fileMapping1.xml'/>
                        <test_case ID='2' IsDefault='false' Method='1' Name='Test' UsePercentage='6.45' Description='test case' ClientPath='../../a/b/test.xml'/>
                    </test_cases>

                </test>");

            webClient.DownloadStringReturns("http://localhost/casino/versionX/setups/ab-testing/fileMapping1.xml",
              @"<overrides>
                   <overrides originalFile='../../bin/application/version.xml' overrideFile='../../setups/ab-testing/FLASH_MLNG/version.xml'/>
                </overrides>");


            webClient.DownloadStringReturns("http://localhost/casino/versionX/a/b/test.xml",
             @"<overrides>
                   <overrides originalFile='../../navigation/plan/4/navigation_plan_ndl.xmm' overrideFile='../../setups/ab-testing/HTML5_MLNG/navigation_plan_ndl.xmm'/>
                   <overrides originalFile='../../navigation/plan/games_properties.xmm' overrideFile='../../setups/ab-testing/HTML5_MLNG/games_properties.xmm'/>
                   <overrides type='configuration' originalFile='../../brand/brand_%BRAND%/skin_%SKIN%/skin.xml' overrideFile='../../setups/ab-testing/testCase1/skin.xml'/> 
               </overrides>");


            var skinCode = new SkinCode(0, 4);

            var reader = CreateReader(webClient, CreateClientUrlBuilder(brand, skin));

            var configuration = reader.Read(brand.CDNUrl);

            var testCases = configuration.TestCaseSets[0];

            var test = testCases[0];
            Assert.AreEqual(1, test.FilesOverride.Count);
            var file = test.FilesOverride[0];
            AssertEquals("../../bin/application/version.xml", file.GetOriginalFile(skinCode));
            AssertEquals("../../setups/ab-testing/FLASH_MLNG/version.xml", file.OverrideFile);

            test = testCases[1];
            Assert.AreEqual(3, test.FilesOverride.Count);
            file = test.FilesOverride[0];
            AssertEquals("../../navigation/plan/4/navigation_plan_ndl.xmm", file.GetOriginalFile(skinCode));
            AssertEquals("../../setups/ab-testing/HTML5_MLNG/navigation_plan_ndl.xmm", file.OverrideFile);
            file = test.FilesOverride[1];
            AssertEquals("../../navigation/plan/games_properties.xmm", file.GetOriginalFile(skinCode));
            AssertEquals("../../setups/ab-testing/HTML5_MLNG/games_properties.xmm", file.OverrideFile);

            file = test.FilesOverride[2];
            AssertEquals("../../brand/brand_0/skin_4/skin.xml", file.GetOriginalFile(skinCode));
            AssertEquals("../../setups/ab-testing/testCase1/skin.xml", file.OverrideFile);

        }


        private void AssertEquals(string expected, PathDescriptor actual)
        {
            Assert.AreEqual(new PathDescriptor(expected), actual);
        }

        private static IClientUrlBuilder CreateClientUrlBuilder(BrandEntity brand, SkinEntity skin)
        {
            var clientUrlBuilderFactory = Substitute.For<IClientUrlBuilder>();
            return new XmlClientUrlBuilder(brand.CDNUrl, new SkinCode(brand.Id, skin.Id), "versionX");
            
        }


        private XmlABTestingConfigurationReader CreateReader(IWebClient webClient)
        {
            var clientUrlBuilder = new XmlClientUrlBuilder(new PathDescriptor("http://localhost/casino"), new SkinCode(0, 4), "versionX");
            
            return CreateReader(webClient, clientUrlBuilder);
        }


        private XmlABTestingConfigurationReader CreateReader(IWebClient webClient, IClientUrlBuilder clientUrlBuilder)
        {
            return new XmlABTestingConfigurationReader(WebClientFactorySubstitute(webClient), clientUrlBuilder);
        }



        IWebClientFactory WebClientFactorySubstitute(IWebClient webClient)
        {
            var factory = Substitute.For<IWebClientFactory>();

            factory.CreateWebClient().Returns(webClient);

            return factory;
        }

    }


    
}
