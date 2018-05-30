using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LayoutTool.ViewModels
{
    [TestFixture]
    public class FilterableCollectionViewModelTests
    {


        List<TestItem> _theList;
        FilterableCollectionViewModel<TestItem> collectionViewModel;
        [SetUp]
        public void Setup()
        {
            InitTheList();
            collectionViewModel = new FilterableCollectionViewModel<TestItem>(_theList);
        }

        private void InitTheList()
        {
            _theList = new List<TestItem>();
            _theList.Add(new TestItem("Florin", 40));
            _theList.Add(new TestItem("Lucia", 37));
            _theList.Add(new TestItem("Florinache", 41));
            _theList.Add(new TestItem("Mihai", 40));
        }

        [Test]
        public void Constructor_InitializeInternalCollectionCorrectly()
        {
            Assert.AreEqual(_theList.Count, collectionViewModel.Count);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("      ")]
        public void Filter_WhenFilterIsRemoved_ShouldShowTheFullList(string filter)
        {
            collectionViewModel.Filter = "xxx";
            Assert.IsTrue(collectionViewModel.IsEmpty);
            collectionViewModel.Filter = filter;
            Assert.AreEqual(_theList.Count, collectionViewModel.Count);
        }


        [TestCase("xxx", 0)]
        [TestCase("Flo", 2)]
        [TestCase("Lucia", 1)]
        [TestCase("4", 2)]
        [TestCase("40", 1)]
        [TestCase("Flo 40", 1)]
        [TestCase("Florin 40", 1)]
        [TestCase("Flo 4", 2)]
        public void Filter_ReturnsTheCorrectNumberOfItems(string filter, int expectedItemsCount)
        {
            collectionViewModel.Filter = "xxx";
            Assert.IsTrue(collectionViewModel.IsEmpty);

        }

        [Test]
        public void GetOriginalItems_WhenFiltered_ShouldReturnAllItems()
        {
            collectionViewModel.Filter = "Lucia";
            Assert.AreEqual(4, collectionViewModel.GetOriginalItems().Length);

        }


        [Test]
        public void Filter_WhenAPropertyIsExcluded_ShouldNotFilterOnIt()
        {
            collectionViewModel.Filter = "xxx";
            
            Assert.AreEqual(_theList.Count, collectionViewModel.GetOriginalItems().Length);

        }

        private class TestItem
        {
            public TestItem(string prop1, int prop2)
            {
                Prop1 = prop1;
                Prop2 = prop2;
            }
            public string Prop1 { get; set; }
            public int Prop2 { get; set; }
        }
    }

    
}
