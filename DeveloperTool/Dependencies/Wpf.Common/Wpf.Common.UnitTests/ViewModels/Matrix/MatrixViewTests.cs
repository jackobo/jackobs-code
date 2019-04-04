using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Spark.Wpf.Common.ViewModels.Matrix
{
    [TestFixture]
    public class MatrixViewTests
    {

        IEnumerable<MatrixMockItem> _sampleItems;
        
        [SetUp]
        public void Setup()
        {
            _sampleItems = new MatrixMockItem[]
            {
                new MatrixMockItem("A", "X", "C2", 1),
                new MatrixMockItem("A", "X", "C1", 1),
                new MatrixMockItem("A", "Y", "C1", 1),
                new MatrixMockItem("A", "Y", "C2", 1),
                new MatrixMockItem("B", "X", "C1", 1),
                new MatrixMockItem("B", "X", "C2", 1),
                new MatrixMockItem("B", "Y", "C1", 1),
            };

            
        }

        [Test]
        public void Load_OneRowHeader_ReturnsTheCorectNumberOfItems()
        {
            
            var matrix = MatrixBuilder<MatrixMockItem>
                            .Matrix()
                            .AddRowHeader("Header1", item => item.RowHeader1)
                            .NoMoreRowHeaders()
                            .NoColumnHeaders()
                            .Build();
            
            matrix.Load(_sampleItems);

            Assert.AreEqual(2, matrix.Count);
        }


        [Test]
        public void Load_BeforeLoadingNewItems_ClearTheOldOnes()
        {

            var matrix = MatrixBuilder<MatrixMockItem>
                            .Matrix()
                            .AddRowHeader("Header1", item => item.RowHeader1)
                            .NoMoreRowHeaders()
                            .NoColumnHeaders()
                            .Build();
            
            matrix.Load(_sampleItems);
            matrix.Load(_sampleItems);

            Assert.AreEqual(2, matrix.Count);
        }

        [Test]
        public void Load_TwoOneRowHeaders_ReturnsTheCorectNumberOfItems()
        {

            var matrix = MatrixBuilder<MatrixMockItem>
                            .Matrix()
                            .AddRowHeader("Header1", item => item.RowHeader1)
                            .AddRowHeader("Header2", item => item.RowHeader2)
                            .NoMoreRowHeaders()
                            .NoColumnHeaders()
                            .Build();
            
            matrix.Load(_sampleItems);

            Assert.AreEqual(4, matrix.Count);
        }


        [Test]
        public void Load_OneRowHeader_EachMatrixRowMustContainsTheCorrectNumberOfAggregatedItems()
        {

            var matrix = MatrixBuilder<MatrixMockItem>
                            .Matrix()
                            .AddRowHeader("Header1", item => item.RowHeader1)
                            .NoMoreRowHeaders()
                            .NoColumnHeaders()
                            .Build();
            
            matrix.Load(_sampleItems);

            Assert.IsTrue(matrix.Any(row => row.GetAggregatedItems().Count() == 3));
            Assert.IsTrue(matrix.Any(row => row.GetAggregatedItems().Count() == 4));
        }


        [Test]
        public void Load_IfNoColumnHeaderProvided_MatrixItemPropertiesMustBeEqualWithTheNumberOfAddedRowHeaders()
        {

            var matrix = MatrixBuilder<MatrixMockItem>
                            .Matrix()
                            .AddRowHeader("Header1", item => item.RowHeader1)
                            .AddRowHeader("Header2", item => item.RowHeader2)
                            .NoMoreRowHeaders()
                            .NoColumnHeaders()
                            .Build();

            
            matrix.Load(_sampleItems);

            var matrixRow = matrix.First();
            Assert.AreEqual(2, TypeDescriptor.GetProperties(matrixRow).Count);
        }

        [Test]
        public void Load_ColumnHeaderIsProvided_EachDistinctValueShouldBeAddedAsAProperty()
        {
            var matrix = MatrixBuilder<MatrixMockItem>
                            .Matrix()
                            .AddRowHeader("Header1", item => item.RowHeader1)
                            .AddRowHeader("Header2", item => item.RowHeader2)
                            .NoMoreRowHeaders()
                            .AddColumnHeader(item => item.ColumnHeader)
                            .AggregateBy(items => items.Sum(i => i.Value))
                            .Build();
            
            matrix.Load(_sampleItems);

            var matrixRow = matrix.First();
            Assert.AreEqual(4, TypeDescriptor.GetProperties(matrixRow).Count);
        }

        [TestCase("C1", 2)]
        [TestCase("C2", 5)]
        public void Load_CorrectlyAggregatesTheRowValues(string propertyName, int expectedValue)
        {
            var sampleItems = new MatrixMockItem[]
            {
                    new MatrixMockItem("A", "X", "C1", 1),
                    new MatrixMockItem("A", "X", "C2", 2),
                    new MatrixMockItem("A", "Y", "C1", 1),
                    new MatrixMockItem("A", "Y", "C2", 3),
            };

            var matrix = MatrixBuilder<MatrixMockItem>
                            .Matrix()
                            .AddRowHeader("Header1", item => item.RowHeader1)
                            .NoMoreRowHeaders()
                            .AddColumnHeader(item => item.ColumnHeader)
                            .AggregateBy(items => items.Sum(i => i.Value))
                            .Build();
            
            matrix.Load(sampleItems);

            var matrixRow = matrix[0];

            var property = TypeDescriptor.GetProperties(matrixRow)[propertyName];
            Assert.AreEqual(expectedValue, property.GetValue(matrixRow));
            
        }
        
        [Test]
        public void Load_IfAnyOfTheColumnHeaderValuesIsNull_ShouldAddAColumnNamedNULL()
        {
            var sampleItems = new MatrixMockItem[]
            {
                    new MatrixMockItem("A", "X", "C1", 1),
                    new MatrixMockItem("A", "X", null, 2),
                    
            };

            var matrix = MatrixBuilder<MatrixMockItem>
                           .Matrix()
                           .AddRowHeader("Header1", item => item.RowHeader1)
                           .NoMoreRowHeaders()
                           .AddColumnHeader(item => item.ColumnHeader)
                           .AggregateBy(items => items.Sum(i => i.Value))
                           .Build();

            
            matrix.Load(sampleItems);

            var matrixRow = matrix[0];

            var property = TypeDescriptor.GetProperties(matrixRow)["NULL"];

            Assert.IsNotNull(property);
            

        }

        [Test]
        public void Load_CorrectlyAggregatesForNulls()
        {
            var sampleItems = new MatrixMockItem[]
            {
                    new MatrixMockItem("A", "X", "C1", 1),
                    new MatrixMockItem("A", "X", null, 2),
                    new MatrixMockItem("A", "Y", "C1", 1),
                    new MatrixMockItem("A", "Y", null, 3),
            };

            var matrix = MatrixBuilder<MatrixMockItem>
                          .Matrix()
                          .AddRowHeader("Header1", item => item.RowHeader1)
                          .NoMoreRowHeaders()
                          .AddColumnHeader(item => item.ColumnHeader)
                          .AggregateBy(items => items.Sum(i => i.Value))
                          .Build();

            
            matrix.Load(sampleItems);

            var matrixRow = matrix[0];

            var property = TypeDescriptor.GetProperties(matrixRow)["NULL"];
            Assert.AreEqual(5, property.GetValue(matrixRow));

        }

        [TestCase("A", "C2")]
        [TestCase("B", "C1")]
        public void Load_IfNoneOfTheAggregatedItemsOfARowHaveTheColumnHeader_Return0(string rowHeaderValue, string columnHeader)
        {
            var sampleItems = new MatrixMockItem[]
            {
                    new MatrixMockItem("A", "X", "C1", 1),
                    new MatrixMockItem("A", "X", "C1", 2),
                    new MatrixMockItem("B", "Y", "C2", 1),
                    new MatrixMockItem("B", "Y", "C2", 3),
            };

            var matrix = MatrixBuilder<MatrixMockItem>
                            .Matrix()
                            .AddRowHeader("Header1", item => item.RowHeader1)
                            .NoMoreRowHeaders()
                            .AddColumnHeader(item => item.ColumnHeader)
                            .AggregateBy(items => items.Sum(i => i.Value))
                            .Build();

            matrix.Load(sampleItems);

            
            var matrixRow = matrix.FirstOrDefault(row =>
            {
                var property = TypeDescriptor.GetProperties(row)["Header1"];
                return property.GetValue(row).Equals(rowHeaderValue);
            });

            Assert.AreEqual(0, TypeDescriptor.GetProperties(matrixRow)[columnHeader].GetValue(matrixRow));

        }

    
        [TestCase(0, "Header1")]
        [TestCase(1, "C1")]
        [TestCase(2, "C2")]
        [TestCase(3, "Header2")]
        public void Load_RowHeadersAddedAfterColumnHeaders_CheckCorrectHeadersPositions(int propertyIndex, string expectedPropertyName)
        {
            var matrix = MatrixBuilder<MatrixMockItem>
                            .Matrix()
                            .AddRowHeader("Header1", item => item.RowHeader1)
                            .AddRowHeader("Header2", item => item.RowHeader2, true)
                            .NoMoreRowHeaders()
                            .AddColumnHeader(item => item.ColumnHeader)
                            .AggregateBy(items => items.Sum(i => i.Value))
                            .Build();

            matrix.Load(_sampleItems);

            var matrixRow = matrix.First();

            var property = TypeDescriptor.GetProperties(matrixRow)[propertyIndex];
            Assert.AreEqual(expectedPropertyName, property.Name);
            
        }
    }
    
}
