using System;
using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;

namespace Spark.Infra.Types
{
    [TestFixture]
    public class VersionNumberTests
    {
        [Test]
        public void ToSortableStringTest()
        {
            var version = new VersionNumber(1, 3, 0, 15);
            Assert.AreEqual("0001.0003.0000.0015", version.ToSortableString());
        }

        [Test]
        public void ParseTest()
        {
            var version = VersionNumber.Parse("1.3.2.4");
            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(3, version.Minor);
            Assert.AreEqual(2, version.Revision);
            Assert.AreEqual(4, version.Build);
        }

        [Test]
        public void EqualsTest()
        {
            VersionNumber version = new VersionNumber(1, 2, 3, 4);
            Assert.IsTrue(version.Equals(new VersionNumber(1, 2, 3, 4)));
        }


        [Test]
        public void NotEqualsTest()
        {
            VersionNumber version = new VersionNumber(1, 2, 3, 4);
            Assert.IsFalse(version.Equals(new VersionNumber(2, 2, 3, 4)));
        }

        [Test]
        public void NotEqualsWithNullTest()
        {
            VersionNumber version = new VersionNumber(1, 2, 3, 4);
            Assert.IsFalse(version.Equals(null));
        }

        [Test]
        public void EqualOperatorTest_BothNull()
        {
            VersionNumber v1 = null;
            VersionNumber v2 = null;

            Assert.IsTrue(v1 == v2);
        }


        [Test]
        public void EqualOperatorTest_OneNullTheOtherOneNot()
        {
            VersionNumber v1 = new VersionNumber(1,0,0,1);
            VersionNumber v2 = null;

            Assert.IsFalse(v1 == v2);

            v1 = null;
            v2 = new VersionNumber(1, 0, 0, 1);

            Assert.IsFalse(v1 == v2);
        }


        [Test]
        public void LesThanOperatorTest()
        {
            VersionNumber v1 = null;
            VersionNumber v2 = null;
            Assert.IsFalse(v1 < v2);

            v1 = null;
            v2 = new VersionNumber(1,0,0,0);
            Assert.IsTrue(v1 < v2);

            v1 = new VersionNumber(1, 0, 0, 0);
            v2 = null;
            Assert.IsFalse(v1 < v2);


            v1 = new VersionNumber(1, 0, 2, 0);
            v2 = new VersionNumber(1, 0, 2, 1);
            Assert.IsTrue(v1 < v2);

            v1 = new VersionNumber(1, 0, 2, 1);
            v2 = new VersionNumber(1, 0, 2, 0);
            Assert.IsFalse(v1 < v2);

            v1 = new VersionNumber(1, 0, 2, 1);
            v2 = new VersionNumber(1, 0, 2, 1);
            Assert.IsFalse(v1 < v2);


        }



        [Test]
        public void LesThanOrEqualOperatorTest()
        {
            VersionNumber v1 = new VersionNumber(1, 0, 0, 0);
            VersionNumber v2 = new VersionNumber(1, 0, 0, 0);

            Assert.IsTrue(v1 <= v2);

            v1 = new VersionNumber(1, 0, 0, 0);
            v2 = new VersionNumber(1, 0, 1, 0);

            Assert.IsTrue(v1 <= v2);
        }

        [Test]
        public void GreatherThanOperatorTest()
        {
            VersionNumber v1 = null;
            VersionNumber v2 = null;
            Assert.IsFalse(v1 > v2);

            v1 = null;
            v2 = new VersionNumber(1, 0, 0, 0);
            Assert.IsFalse(v1 > v2);

            v1 = new VersionNumber(1, 0, 0, 0);
            v2 = null;
            Assert.IsTrue(v1 > v2);


            v1 = new VersionNumber(1, 0, 2, 0);
            v2 = new VersionNumber(1, 0, 2, 1);
            Assert.IsFalse(v1 > v2);

            v1 = new VersionNumber(1, 0, 2, 1);
            v2 = new VersionNumber(1, 0, 2, 0);
            Assert.IsTrue(v1 > v2);

            v1 = new VersionNumber(1, 0, 2, 1);
            v2 = new VersionNumber(1, 0, 2, 1);
            Assert.IsFalse(v1 > v2);


        }

        [Test]
        public void GreatherThanOrEqualOperatorTest()
        {
            VersionNumber v1 = new VersionNumber(1, 0, 0, 0);
            VersionNumber v2 = new VersionNumber(1, 0, 0, 0);

            Assert.IsTrue(v1 >= v2);

            v1 = new VersionNumber(1, 0, 1, 0);
            v2 = new VersionNumber(1, 0, 0, 0);

            Assert.IsTrue(v1 >= v2);
        }

        [Test]
        public void ParseNegativeVersionNumber()
        {
            var version = VersionNumber.Parse("-99.1.0.7");
            Assert.AreEqual("_0099.0001.0000.0007", version.ToSortableString());
        }

        [Test]
        public void ParseNegativeVersionNumberInSortableFormat()
        {

            var version = VersionNumber.Parse("_0099.0001.0000.0007");
            Assert.AreEqual(new VersionNumber(-99, 1, 0, 7), version);
        }

        [Test]
        public void IncrementTest()
        {
            var version = new VersionNumber(1, 2, 3, 4);
            version++;
            Assert.AreEqual(version, new VersionNumber(1,2,3,5));
        }


        [Test]
        public void AddTest()
        {
            var version = new VersionNumber(1, 2, 3, 4);
            Assert.AreEqual(version + 3, new VersionNumber(1, 2, 3, 7));
        }

        [Test]
        public void LessThanWithNegativeVersion()
        {
            var v1 = new VersionNumber(-99, 0, 0, 1);
            var v2 = new VersionNumber(1, 0, 0, 5);

            Assert.IsTrue(v1 < v2);
        }

        [Test]
        public void FromLongTest1()
        {
            var v = new VersionNumber(1000100010001);
            Assert.AreEqual("1.1.1.1", v.ToString());
        }

        [Test]
        public void FromLongTest2()
        {
            var v = new VersionNumber(10001000100010);
            Assert.AreEqual("10.10.10.10", v.ToString());
        }

        [Test]
        public void FromLongTest3()
        {
            var v = new VersionNumber(1000000000000);
            Assert.AreEqual("1.0.0.0", v.ToString());
        }

        [Test]
        public void FromLongTest4()
        {
            var v = new VersionNumber(9999999999999999);
            Assert.AreEqual("9999.9999.9999.9999", v.ToString());
        }

        [Test]
        public void FromLongTest5()
        {
            var v = new VersionNumber(1000200030004);
            Assert.AreEqual("1.2.3.4", v.ToString());
        }

       
        [Test]
        public void FromLongTest6()
        {
            Assert.Throws<ArgumentException>(() => new VersionNumber(-1000200030004));
        }


        [Test]
        public void FromLongTest7()
        {
            var v = new VersionNumber(0);
            Assert.AreEqual("0.0.0.0", v.ToString());
        }

        [Test]
        public void FromLongTest8()
        {
            var v = new VersionNumber(99002003000400);
            Assert.AreEqual("99.20.300.400", v.ToString());
        }
    }
}
