using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace LayoutTool.Models.Builders.Xml.FilesDescriptors
{
    [TestFixture]
    public class NavigationPlanXmlFileDescriptorTests
    {
        [Test]
        public void FileName_ShouldReturnTheCorrectFileName()
        {
            var fileDescriptor = CreateFileDescriptor();
            Assert.AreEqual("navigation_plan_ndl.xmm", fileDescriptor.FileName);
        }

       
        [Test]
        public void ComputeFullPathFromNavigationFilePath_ReturnsTheCorrectPath()
        {
            var fileDescriptor = CreateFileDescriptor();
            var fullPath = fileDescriptor.ComputeLocalFullPathFromNavigationFilePath(@"C:\ndl\build\versionX\navigation\plan\4\navigation_plan_ndl.xmm");
            Assert.AreEqual(@"C:\ndl\build\versionX\navigation\plan\4\navigation_plan_ndl.xmm", fullPath);
        }

        [Test]
        public void ApplyXmlToParser_ShouldSetTheRightPropertyInTheParser()
        {
            var fileDescriptor = CreateFileDescriptor();
            var parser = Substitute.For<IXmlSkinDefinitionParser>();
            fileDescriptor.ApplyXmlToParser(parser, "<xml></xml>");
            parser.Received().NavigationPlanXml = "<xml></xml>";
        }
        
        [Test]
        public void ComputeUrl_ShouldReturnTheCorrectUrlAccordingWithTheBrandAndSkin()
        {
            var fileDescriptor = CreateFileDescriptor();
            var skinCode = new SkinCode(1, 2);
            var url = fileDescriptor.ComputeUrl("http://ndl-cdn.safe-iplay.com/casino/6.4-0-270c/", skinCode);

            Assert.AreEqual("http://ndl-cdn.safe-iplay.com/casino/6.4-0-270c/navigation/plan/102/navigation_plan_ndl.xmm", url);

        }

        private static NavigationPlanXmlFileDescriptor CreateFileDescriptor()
        {
            return new NavigationPlanXmlFileDescriptor();
        }

    }
}
