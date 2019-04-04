using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using NUnit.Framework;

namespace LayoutTool.Models
{
    [TestFixture]
    public class PathDescriptorTests
    {
        [TestCase("http://localhost", "test/index.html", "http://localhost/test/index.html")]
        [TestCase("http://localhost/", "/test/index.html", "http://localhost/test/index.html")]
        [TestCase("http://localhost/", "test/index.html", "http://localhost/test/index.html")]
        [TestCase("http://localhost", "/test/index.html", "http://localhost/test/index.html")]
        [TestCase("http://localhost/test/index.html", "", "http://localhost/test/index.html")]
        [TestCase("http://localhost/test/index.html", null, "http://localhost/test/index.html")]
        [TestCase("http://localhost/NDLVersionHead/VersionY", "VersionY/brand/brand_0/brand.xml", "http://localhost/NDLVersionHead/VersionY/VersionY/brand/brand_0/brand.xml")]
        public void OperatorPlus_RetrunsCorrectResults(string path1, string path2, string expectedResult)
        {
            PathDescriptor p1 = new PathDescriptor(path1);
            PathDescriptor p2 = new PathDescriptor(path2);

            var result = p1 + p2;

            Assert.AreEqual(expectedResult, result.ToString());
        }


    


        [Test]
        public void Equals_IfTheyAreDifferentOnlyByTraillingSlash_ShouldReturnTrue()
        {
            var p1 = new PathDescriptor("http://ndl-cdn.safe-iplay.com/casino/7.2-0-60c/");
            var p2 = new PathDescriptor("http://ndl-cdn.safe-iplay.com/casino/7.2-0-60c");

            Assert.IsTrue(p1.Equals(p2));

        }

        [Test]
        public void ToUrl_ShouldNotAlterArguments()
        {
            var p1 = new PathDescriptor("http://localhost/casino");
            var p2 = new PathDescriptor($"bin/application/ViewActual.html?&navigationplanid=-1&languageid=en&brandid=0&skinid=0&ABTestingOverridePath=../../setups/ab-testing/fileMapping3.xml");
            var result = p1 + p2;

            Assert.AreEqual("http://localhost/casino/bin/application/ViewActual.html?&navigationplanid=-1&languageid=en&brandid=0&skinid=0&ABTestingOverridePath=../../setups/ab-testing/fileMapping3.xml",
                            result.ToHttpUrlFormat());
        }

        [Test]
        public void Equals_ComparingWithNull_ShouldReturnFalse()
        {
            var p = new PathDescriptor("http://ndl-cdn.safe-iplay.com/casino/7.2-0-60c/");
            

            Assert.IsFalse(p.Equals(null));

        }


        [TestCase("")]
        [TestCase("  ")]
        [TestCase(null)]
        public void IsEmpty_IfEmptyOrNullIsProvided_ShouldReturnTrue(string path)
        {
            var p = new PathDescriptor(path);

            Assert.IsTrue(p.IsEmpty());
        }

        [Test]
        public void EqualsOperator_IfBothAreNull_ShouldReturnTrue()
        {
            PathDescriptor p1 = null;
            PathDescriptor p2 = null;


            Assert.IsTrue(p1 == p2);

        }

        [Test]
        public void ToUrlFormat_DoesntAlterTheReceivedUrlReceivedInConstructor()
        {
            var p = new PathDescriptor("http://localhost");
            Assert.AreEqual("http://localhost", p.ToHttpUrlFormat());

        }

        [Test]
        public void EndsWith_ThePathToCompareWithIsLongerThatThisOne_RetursnTrue()
        {
            var p1 = new PathDescriptor("localhost/test");

            Assert.IsFalse(p1.EndsWith(new PathDescriptor("http://localhost/test")));
            
        }

        [Test]
        public void EndsWith_ThePathEndsWithTheProvidedOne_RetursnTrue()
        {
            var p1 = new PathDescriptor("http://localhost/test");
            Assert.IsTrue(p1.EndsWith(new PathDescriptor("localhost/test")));
        }
        
        [Test]
        public void EndsWith_ThePathDoesntEndsWithTheProvidedOne_RetursFalse()
        {
            var p1 = new PathDescriptor("http://localhost/test/a/b");

            Assert.IsFalse(p1.EndsWith(new PathDescriptor("localhost/x/a/b")));

        }
    }
}
