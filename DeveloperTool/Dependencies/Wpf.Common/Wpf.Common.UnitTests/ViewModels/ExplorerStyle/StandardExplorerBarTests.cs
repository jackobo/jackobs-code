using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using Microsoft.Practices.ServiceLocation;
using System.Collections.ObjectModel;

namespace Spark.Wpf.Common.ViewModels
{
    [TestFixture]
    public class StandardExplorerBarTests
    {

        IServiceLocator _serviceLocator;

        [SetUp]
        public void Setup()
        {
            _serviceLocator = Substitute.For<IServiceLocator>();
        }
        
        private StandardExplorerBar CreateStandardExplorerBar(params IExplorerBarItem[] items)
        {
            var explorerBar = new StandardExplorerBar(_serviceLocator);

            explorerBar.Items.AddRange(items);

            return explorerBar;
        }

        private IExplorerBarItem CreateExplorerBarItem()
        {
            var item = Substitute.For<IExplorerBarItem>();
            item.Items.Returns(new ObservableCollection<IExplorerBarItem>());
            return item;
        }

        private ObservableCollection<IExplorerBarItem> CreateObservableCollection(params IExplorerBarItem[] items)
        {
            var collection = new ObservableCollection<IExplorerBarItem>();
            collection.AddRange(items);
            return collection;
        }

        [Test]
        public void SelectFirstItem_IfThereIsOneItem_ShouldSetTheIsSelectedPropertyForThatItem()
        {
            var item = CreateExplorerBarItem();
            
            var explorerBar = CreateStandardExplorerBar(item);
            
            explorerBar.SelectFirstItem();
            Assert.IsTrue(item.IsSelected);
        }

        [Test]
        public void SelectFirstItem_IfThereIsNoItem_ShouldDoNothing()
        {
            var explorerBar = CreateStandardExplorerBar();

            Assert.DoesNotThrow(() => explorerBar.SelectFirstItem());
            
        }


        [Test]
        public void SelectFirstItem_IfThereAreTwoItems_ShouldSetTheIsSelectedPropertyForTheFirstItemInTheList()
        {
            var item1 = CreateExplorerBarItem();
            var item2 = CreateExplorerBarItem();
            var explorerBar = CreateStandardExplorerBar(item1, item2);
            
            explorerBar.SelectFirstItem();

            Assert.IsTrue(item1.IsSelected);
        }


        [Test]
        public void CheckAllCommand_ShouldSetIsCheckedInAllItems()
        {
            var item1 = CreateExplorerBarItem();
            var item2 = CreateExplorerBarItem();
            var explorerBar = CreateStandardExplorerBar(item1, item2);

            explorerBar.CheckAllCommand.Execute(null);

            Assert.IsTrue(explorerBar.Items.All(item => item.IsChecked == true));
        }

        [Test]
        public void UncheckAllCommand_ShouldSetIsCheckedInAllItems()
        {
            var item1 = CreateExplorerBarItem();
            item1.IsChecked = true;
            var item2 = CreateExplorerBarItem();
            item2.IsChecked = true;

            var explorerBar = CreateStandardExplorerBar(item1, item2);
            
            explorerBar.UncheckAllCommand.Execute(null);

            Assert.IsTrue(explorerBar.Items.All(item => item.IsChecked == false));

        }

        [Test]
        public void ExplandAll_ShouldCallExpandAllForAllRootItems()
        {
            var item1 = CreateExplorerBarItem();
            var item2 = CreateExplorerBarItem();
            
            var explorerBar = CreateStandardExplorerBar(item1, item2);

            explorerBar.ExpandAll();

            item1.Received().ExpandAll();
            item2.Received().ExpandAll();

        }
        
        [Test]
        public void CollapseAll_ShouldCallCollapseAllForAllRootItems()
        {
            var item1 = CreateExplorerBarItem();
            var item2 = CreateExplorerBarItem();

            var explorerBar = CreateStandardExplorerBar(item1, item2);

            explorerBar.CollapseAll();

            item1.Received().CollapseAll();
            item2.Received().CollapseAll();
        }


        [Test]
        public void GetCheckedItems_IfThereIsOneItemAndIsCheckedShouldBeReturned()
        {
            var item = CreateExplorerBarItem();
            var explorerBar = CreateStandardExplorerBar(item);

            item.IsChecked = true;

            Assert.AreEqual(1, explorerBar.GetCheckedItemsAsExplorerBar().Items.Count);
        }

        [Test]
        public void GetCheckedItems_IfThereIsNoItem_ShouldReturnEmptyList()
        {
            var explorerBar = CreateStandardExplorerBar();
            
            Assert.IsFalse(explorerBar.GetCheckedItemsAsExplorerBar().Items.Any());
        }

        [Test]
        public void GetCheckedItems_IfThereIsOneItemAndIsNotChecked_ShouldReturnEmptyList()
        {
            var item = CreateExplorerBarItem();
            var explorerBar = CreateStandardExplorerBar(item);
            item.IsChecked = false;
            Assert.IsFalse(explorerBar.GetCheckedItemsAsExplorerBar().Items.Any());
        }

        [Test]
        public void GetCheckedItems_IfTheItemHasCheckedChild_ShouldBeReturnedInTheSelection()
        {
            var rootItem1 = CreateExplorerBarItem();
            rootItem1.IsChecked = null;
            var childItem1 = CreateExplorerBarItem();
            childItem1.IsChecked = false;
            var childItem2 = CreateExplorerBarItem();
            childItem2.IsChecked = true;
            rootItem1.Items.Returns(CreateObservableCollection(childItem1, childItem2));
            
            var rootItem2 = CreateExplorerBarItem();
            rootItem2.IsChecked = false;

            var checkedItems = CreateStandardExplorerBar(rootItem1, rootItem2).GetCheckedItemsAsExplorerBar();
            
            Assert.AreEqual(1, checkedItems.Items.Count);
            Assert.AreEqual(1, checkedItems.Items.First().Items.Count);
        }

     
    }
}
