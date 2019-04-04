using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Models.Builders.Xml;
using NSubstitute;
using NUnit.Framework;

namespace LayoutTool.Models
{
    [TestFixture]
    public class XmlClientUrlBuilderTests
    {

        [Test]
        public void GetBaseUrl_WebClientDownloadString_ReceivesCorrectParameters()
        {
            var webClient = Substitute.For<IWebClient>();
            webClient.DownloadString(new PathDescriptor("http://ndl-cdn.safe-iplay.com/casino/application/versions.xml")).Returns("<setup defaultFolder='6.2-0-375'/>");
            var clientUrlBuilderFactory = new XmlClientUrlBuilderFactory(CreateWebClientFactorySubstitute(webClient));

            var brand = new BrandEntity(0, "888Casino", "http://ndl-cdn.safe-iplay.com/casino");
            var skin = new SkinEntity(4, "Non UK");
            var urlInfo = clientUrlBuilderFactory.GetClientUrlBuilder(brand, skin);

            webClient.Received().DownloadString(new PathDescriptor("http://ndl-cdn.safe-iplay.com/casino/application/versions.xml"));


        }

        [Test]
        public void GetBaseUrl_IfNoBrandSkinSpecialFolder_UseDefaultFolder()
        {
            var webClient = Substitute.For<IWebClient>();
            webClient.DownloadStringReturns("http://ndl-cdn.safe-iplay.com/casino/application/versions.xml",
                                            "<setup defaultFolder='6.2-0-375'/>");
            var clientUrlBuilderFactory = new XmlClientUrlBuilderFactory(CreateWebClientFactorySubstitute(webClient));

            var brand = new BrandEntity(0, "888Casino", "http://ndl-cdn.safe-iplay.com/casino");
            var skin = new SkinEntity(4, "Non UK");
            var url = clientUrlBuilderFactory.GetClientUrlBuilder(brand, skin).BuildBaseUrl(null);

            AssertEquals("http://ndl-cdn.safe-iplay.com/casino/6.2-0-375/", url);
        }


        

        [Test]
        public void GetBaseUrl_AFolderIsSpecifidedForTheProvidedBrandAndSkinAndEnglishLanguage_UseProvidedFolder()
        {
            var webClient = Substitute.For<IWebClient>();
            webClient.DownloadStringReturns("http://ndl-cdn.safe-iplay.com/casino/application/versions.xml",
                     @"<setup defaultFolder='6.2-0-375'>
                                    <client brand='0' skin='4' lang='eng' version='7.2-0-60c'/>
                                </setup>");

            var clientUrlBuilderFactory = new XmlClientUrlBuilderFactory(CreateWebClientFactorySubstitute(webClient));

            var brand = new BrandEntity(0, "888Casino", "http://ndl-cdn.safe-iplay.com/casino");
            var skin = new SkinEntity(4, "Non UK");
            var url = clientUrlBuilderFactory.GetClientUrlBuilder(brand, skin).BuildBaseUrl(null);

            AssertEquals("http://ndl-cdn.safe-iplay.com/casino/7.2-0-60c/", url);
        }

        [Test]
        public void GetBaseUrl_AFolderIsSpecifidedForTheProvidedBrandAndSkinAndAnyLanguage()
        {
            var webClient = Substitute.For<IWebClient>();
            webClient.DownloadStringReturns("http://ndl-cdn.safe-iplay.com/casino/application/versions.xml",
                     @"<setup defaultFolder='6.2-0-375'>
                                    <client brand='0' skin='4' lang='*' version='7.2-0-60c'/>
                                </setup>");

            var clientUrlBuilderFactory = new XmlClientUrlBuilderFactory(CreateWebClientFactorySubstitute(webClient));

            var brand = new BrandEntity(0, "888Casino", "http://ndl-cdn.safe-iplay.com/casino");
            var skin = new SkinEntity(4, "Non UK");
            var url = clientUrlBuilderFactory.GetClientUrlBuilder(brand, skin).BuildBaseUrl(null);

            AssertEquals("http://ndl-cdn.safe-iplay.com/casino/7.2-0-60c/", url);
        }

        [Test]
        public void GetBaseUrl_AFolderIsSpecifidedForTheProvidedBrandAnySkinAndAnyLanguage()
        {
            var webClient = Substitute.For<IWebClient>();
            webClient.DownloadStringReturns("http://ndl-cdn.safe-iplay.com/casino/application/versions.xml",
                     @"<setup defaultFolder='6.2-0-375'>
                                    <client brand='0' skin='*' lang='*' version='7.2-0-60c'/>
                                </setup>");

            var clientUrlBuilderFactory = new XmlClientUrlBuilderFactory(CreateWebClientFactorySubstitute(webClient));

            var brand = new BrandEntity(0, "888Casino", "http://ndl-cdn.safe-iplay.com/casino");
            var skin = new SkinEntity(4, "Non UK");
            var url = clientUrlBuilderFactory.GetClientUrlBuilder(brand, skin).BuildBaseUrl(null);

            AssertEquals("http://ndl-cdn.safe-iplay.com/casino/7.2-0-60c/", url);
        }

        [Test]
        public void GetBaseUrl_WhenWebClientThrowsHttpNotFound_ReturnTheBrandCDNUrl()
        {
            var webClient = Substitute.For<IWebClient>();
            webClient.DownloadString(new PathDescriptor("http://ndl-cdn.safe-iplay.com/casino/7.2-0-60c/application/versions.xml"))
                                     .Returns(x => { throw new HttpNotFoundException(404, "Not found", "http://ndl-cdn.safe-iplay.com/casino/7.2-0-60c/application/versions.xml"); });
                     

            var clientUrlBuilderFactory = new XmlClientUrlBuilderFactory(CreateWebClientFactorySubstitute(webClient));

            var brand = new BrandEntity(0, "888Casino", "http://ndl-cdn.safe-iplay.com/casino/7.2-0-60c");
            var skin = new SkinEntity(4, "Non UK");
            var url = clientUrlBuilderFactory.GetClientUrlBuilder(brand, skin).BuildBaseUrl(null);

            AssertEquals(brand.CDNUrl, url);
        }

        //<setup defaultFolder='6.2-0-375'>

        private IWebClientFactory CreateWebClientFactorySubstitute(IWebClient webClient)
        {
            var factory = Substitute.For<IWebClientFactory>();
            factory.CreateWebClient().Returns(webClient);
            return factory;
        }

        private void AssertEquals(PathDescriptor expected, PathDescriptor actual)
        {
            Assert.AreEqual(expected, actual);
        }
        private void AssertEquals(string expected, PathDescriptor actual)
        {
            Assert.AreEqual(new PathDescriptor(expected), actual);
        }
    }
}
