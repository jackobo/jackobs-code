using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using NUnit.Framework;

namespace GamesPortal.Service
{
    [TestFixture]
    public class ArtifactoryPropertyTests
    {
        [TestCase("NDL.State", "State")]
        [TestCase("Brand.State.B1", "B1")]
        public void Name_ReturnCorrectValue(string key, string expectedValue)
        {
            var prop = new ArtifactoryProperty(key, "value");
            Assert.AreEqual(expectedValue, prop.Name);
        }

        [TestCase("NDL.State", "NDL")]
        [TestCase("Brand.State.B1", "Brand.State")]
        public void SetName_ReturnCorrectValue(string key, string expectedValue)
        {
            var prop = new ArtifactoryProperty(key, "value");
            Assert.AreEqual(expectedValue, prop.SetName);
        }

        [Test]
        public void ConcatValues_JoinAllValuesWithASemiColon_AndReturTheString()
        {
            var prop = new ArtifactoryProperty("NDL.State", "InProgress", "Approved", "Production");
            Assert.AreEqual("InProgress;Approved;Production", prop.ConcatValues());
        }
    }
}
