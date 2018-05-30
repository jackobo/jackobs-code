using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Spark.Infra.Data.LinqToSql.RecordsSynchronization
{
    [TestFixture]
    public class ReflectionBasedRecordsSynchronizerPropertiesProviderTests
    {

        ReflectionBasedPropertiesProvider<MockRecord> _provider;

        [SetUp]
        public void Setup()
        {
            _provider = new ReflectionBasedPropertiesProvider<MockRecord>();
        }

        [TestCase("Column1")]
        [TestCase("Column2")]
        public void GetProperties_IfPropertyHasColumnAttribute_ReturnIt(string propertyName)
        {
            var property = _provider.GetProperties().FirstOrDefault(p => p.Name == propertyName);

            Assert.IsNotNull(property, $"Property {propertyName} not found");
        }


        [Test]
        public void GetProperties_IfPropertyDoesntHaveColumnAttribute_DontReturnIt()
        {
           
            Assert.IsFalse(_provider.GetProperties().Any(p => p.Name == nameof(MockRecord.PropertyWithoutAttribute)));
        }


        [Test]
        public void GetProperties_IfPropertyIsPrimaryKey_DontReturnIt()
        {

            Assert.IsFalse(_provider.GetProperties().Any(p => p.Name == nameof(MockRecord.PrimaryKey)));
        }


        [Test]
        public void GetProperties_IfPropertyIsPrivate_DontReturnIt()
        {
            Assert.IsFalse(_provider.GetProperties().Any(p => p.Name == "PrivateProperty"));
        }

        [Test]
        public void GetProperties_IfExcludedPropertiesAreProvided_DontReturnThem()
        {
            var excludedProperty = nameof(MockRecord.Column2);
            _provider = new ReflectionBasedPropertiesProvider<MockRecord>(excludedProperty);
            Assert.IsFalse(_provider.GetProperties().Any(p => p.Name == excludedProperty));
        }

        private class MockRecord
        {
            [ColumnAttribute(Storage = "_PrimaryKey", DbType = "UniqueIdentifier NOT NULL", IsPrimaryKey = true)]
            public Guid PrimaryKey { get; set; }
            

            [ColumnAttribute(Storage = "_Column1", DbType = "NVarChar(250) NOT NULL")]
            
            public string Column1 { get; set; }

            [ColumnAttribute(Storage = "_Column2", DbType = "NVarChar(250) NOT NULL")]

            public string Column2 { get; set; }

            private string PrivateColumn { get; set; }

            public int PropertyWithoutAttribute { get; set; }
        }
    }
}
