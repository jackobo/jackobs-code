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
    public class SkinCodeTests
    {

        [Test]
        public void Constructor_ReceivesTheSkinCodeSetsTheCodeProperty()
        {
            var skinCode = new SkinCode(4);
            Assert.AreEqual(4, skinCode.Code);
                
        }

        [TestCase(4, 0, 4)]
        [TestCase(10, 0, 10)]
        [TestCase(100, 1, 0)]
        [TestCase(104, 1, 4)]
        [TestCase(720888, 72, 888)]
        [TestCase(70070, 70, 70)]
        public void Constructor_ReceivesTheSkinCode(int code, int expectedBrandId, int expectedSkinId)
        {

            var skinCode = new SkinCode(code);
            
            Assert.AreEqual(expectedBrandId, skinCode.BrandId);
            Assert.AreEqual(expectedSkinId, skinCode.SkinId);
        }

        [TestCase(4, 0, 4)]
        [TestCase(10, 0, 10)]
        [TestCase(100, 1, 0)]
        [TestCase(104, 1, 4)]
        [TestCase(720888, 72, 888)]
        public void Constructor_ReceivesBrandAndSkin(int expectedCode, int brandId, int skinId)
        {
            var skinCode = new SkinCode(brandId, skinId);

            Assert.AreEqual(expectedCode, skinCode.Code);
        }

        [Test]
        public void Constructor_ReceivesBrandAndSkin_SetsTheBrandAndSkinProperties()
        {
            var skinCode = new SkinCode(1, 5);

            Assert.AreEqual(1, skinCode.BrandId);
            Assert.AreEqual(5, skinCode.SkinId);
        }
    }
}
