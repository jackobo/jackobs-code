using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Spark.Infra.Types;

namespace Spark.Infra.Types
{
    [TestFixture]
    public class VersionNumberTests
    {
        [TestCase("1.2.3.4")]
        [TestCase("1.2.3")]
        [TestCase("1.2")]
        [TestCase("1")]
        public void IsValid_WhenTheVersionIsValid_ShouldReturnTrue(string version)
        {
            Assert.AreEqual(true, VersionNumber.IsValid(version));
        }

        [TestCase("1.2.3.4-dev")]
        [TestCase("")]
        [TestCase(null)]
        public void IsValid_WhenTheVersionIsNotValid_ShouldReturnFalse(string version)
        {
            Assert.AreEqual(false, VersionNumber.IsValid(version));
        }
    }
}
