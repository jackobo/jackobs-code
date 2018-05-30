using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace LayoutTool.Models.XmlFilesDescriptors
{
    [TestFixture]
    public class BrandXmlFileDescriptorTests
    {
        [Test]
        public void FileName_ReturnsTheCorrectValue()
        {
            var fileDescriptor = CreateFileDescriptor();
            Assert.AreEqual("brand.xml", fileDescriptor.FileName);
        }

        [Test]
        public void ComputeFullPathFromNavigationFilePath_ShouldReturnTheCorrectPath()
        {
            var fileDescriptor = CreateFileDescriptor();
            var fullPath = fileDescriptor.ComputeLocalFullPathFromNavigationFilePath(@"C:\ndl\build\versionX\navigation\plan\4\navigation_plan_ndl.xmm");
            Assert.AreEqual(@"C:\ndl\build\versionX\brand\brand_0\brand.xml", fullPath);
        }


        [Test]
        public void ApplyXmlToParser_ShouldSetThe_GamesPropertiesXml_Property()
        {
            var fileDescriptor = CreateFileDescriptor();
            var parser = Substitute.For<IXmlSkinDefinitionParser>();
            fileDescriptor.ApplyXmlToParser(parser, "<xml></xml>");
            parser.Received().BrandXml = "<xml></xml>";

        }


        [Test]
        public void ComputeUrl_ShouldReturnTheCorrectUrlAccordingWithTheBrandAndSkin()
        {
            var fileDescriptor = CreateFileDescriptor();

            var skinCode = new SkinCode(1, 2);
            
            var url = fileDescriptor.ComputeUrl("http://ndl-cdn.safe-iplay.com/casino/6.4-0-270c/", skinCode);
            
            Assert.AreEqual("http://ndl-cdn.safe-iplay.com/casino/6.4-0-270c/brand/brand_1/brand.xml", url);

        }

        private static BrandXmlFileDescriptor CreateFileDescriptor()
        {
            return new BrandXmlFileDescriptor();
        }
    }
}
