using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.ViewModels.Helpers;
using Spark.Wpf.Common.ViewModels;
using Spark.Infra.Types;
using Prism.Events;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.TestHelpers;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.UnitTesting.Helpers;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    [TestFixture]
    public class LogicalBranchExplorerBarItemTests 
    {
        IRootBranch _logicalBranch;
        IExplorerBarItemsRepository _explorerBarItemsRepository;
        IExplorerBarItemsRepositoryFactory _explorerBarItemsRepositoryFactory;
        IServiceLocator _serviceLocator;
        readonly int _logicalBranchMajorVersion = 3;
        IPubSubMediator _pubSubMediator;

        [SetUp]
        public void Setup()
        {
            InitDependencies();
        }

        private void InitDependencies()
        {
            _logicalBranch = LogicalBranchBuilder.LogicalBranch()
                                                   .WithName(_logicalBranchMajorVersion)
                                                   .Build();
            _logicalBranch.GetDevBranch().Returns(NoDevBranch());

            

            _pubSubMediator = new MockPubSubMediator();

            _explorerBarItemsRepositoryFactory = Substitute.For<IExplorerBarItemsRepositoryFactory>();
            _explorerBarItemsRepository = Substitute.For<IExplorerBarItemsRepository>();
            _explorerBarItemsRepositoryFactory.GetRepository(Arg.Any<IExplorerBarItem>()).Returns(_explorerBarItemsRepository);

            _serviceLocator = ServiceLocatorBuilder.ServiceLocator()
                                                   .WithService(_explorerBarItemsRepositoryFactory)
                                                   .WithService(_pubSubMediator)
                                                   .Build();

            
            
        }

        private RootBranchExplorerBarItem CreateLogicalBranchItem()
        {
            return new RootBranchExplorerBarItem(_logicalBranch, Substitute.For<IExplorerBar>(), _serviceLocator);
        }

        private Optional<IDevBranch> NoDevBranch()
        {
            return Optional<IDevBranch>.None();
        }


        private Optional<IDevBranch> SomeDevBranch()
        {
            IDevBranch devBranch = Substitute.For<IDevBranch>();
            return Optional<IDevBranch>.Some(devBranch);
        }

        [Test]
        public void Constructor_ShouldSetTheCaptionAccordingWithTheLogicalBranchName()
        {
            
            var item = CreateLogicalBranchItem();

            Assert.AreEqual(new RootBranchVersion(_logicalBranchMajorVersion).ToString(), item.Caption);
        }

        [Test]
        public void Constructor_ShouldUseTheExplorerBarItemsFactoryToCreateA_QaBranchExplorerBarItem()
        {
            var qaExplorerBarItem = ExplorerBarItemBuilder.ExplorerBarItem().Build();
            var qaBranch = Substitute.For<IQaBranch>();
            _logicalBranch.GetQaBranch().Returns(qaBranch);
            _explorerBarItemsRepository.CreateQABranchItem(qaBranch).Returns(qaExplorerBarItem);
            
            var logicalBranchItem = CreateLogicalBranchItem();

            _explorerBarItemsRepository.Received().CreateQABranchItem(qaBranch);
        }
        
        [Test]
        public void Constructor_ShouldAddThe_QaBranchExplorerBarItem_ToTheItemsList()
        {
            var qaExplorerBarItem = ExplorerBarItemBuilder.ExplorerBarItem().Build();

            var qaBranch = Substitute.For<IQaBranch>();
            _logicalBranch.GetQaBranch().Returns(qaBranch);
            _explorerBarItemsRepository.CreateQABranchItem(qaBranch).Returns(qaExplorerBarItem);

            var logicalBranchItem = CreateLogicalBranchItem();

            Assert.IsTrue(logicalBranchItem.Items.Contains(qaExplorerBarItem));

        }

        [Test]
        public void Constructor_IfThereIsADevBranch_ItShouldUseTheFactoryToCreateA_DevBranchExplorerBarItem()
        {
            
            _logicalBranch.GetDevBranch().Returns(SomeDevBranch());

            var devExplorerBarItem = ExplorerBarItemBuilder.ExplorerBarItem().Build();
            _explorerBarItemsRepository.CreateDevBranchItem(Arg.Any<IDevBranch>()).Returns(devExplorerBarItem);

            var logicalBranchItem = CreateLogicalBranchItem();

            Assert.IsTrue(logicalBranchItem.Items.Contains(devExplorerBarItem));

        }


        [Test]
        public void Constructor_IfThereIsNoDevBranch_ItShouldNotAddA_DevBranchExplorerBarItem()
        {
            _logicalBranch.GetDevBranch().Returns(NoDevBranch());

            var devExplorerBarItem = ExplorerBarItemBuilder.ExplorerBarItem().Build();
            _explorerBarItemsRepository.CreateDevBranchItem(Arg.Any<IDevBranch>()).Returns(devExplorerBarItem);

            var logicalBranchItem = CreateLogicalBranchItem();

            Assert.IsFalse(logicalBranchItem.Items.Contains(devExplorerBarItem));

        }

        [Test]
        public void Actions_IfThereIsNoDevBranch_ShouldHaveOneActionForCreatingTheDevBranch()
        {
            var devBranch = Substitute.For<IDevBranch>();
            _logicalBranch.CanCreateDevBranch().Returns(true);
            _logicalBranch.GetDevBranch().Returns(NoDevBranch());
            var logicalBranchItem = CreateLogicalBranchItem();

            Assert.IsTrue(logicalBranchItem.Actions.Any(a => a.GetType().Equals(typeof(Actions.CreateDevelopmentBranchAction))));
        }


        [Test]
        public void Actions_IfThereIsADevBranch_ShouldNotHaveAnActionForCreatingFeatureBranch()
        {
            _logicalBranch.GetDevBranch().Returns(SomeDevBranch());
            var logicalBranchItem = CreateLogicalBranchItem();

            Assert.IsFalse(logicalBranchItem.Actions.Any(a => a.Caption == "Create DEVELOPMENT branch"));
        }

        [Test]
        public void CreateDevBranchNotificationReceived_AddsTheNewDevBranchItem()
        {
            var logicalBranchItem = CreateLogicalBranchItem();

            var devBranch = Substitute.For<IDevBranch>();
            _logicalBranch.GetDevBranch().Returns(Optional<IDevBranch>.Some(devBranch));

            var devExplorerBarItem = ExplorerBarItemBuilder.ExplorerBarItem().Build();
            _explorerBarItemsRepository.CreateDevBranchItem(devBranch).Returns(devExplorerBarItem);

            _pubSubMediator.Publish(new CreateDevBranchFinishEventData(_logicalBranch));
            
            Assert.IsTrue(logicalBranchItem.Items.Contains(devExplorerBarItem));

        }

        [Test]
        public void CreateDevBranchNotificationReceived_RaiseThePropertyChangedForActionsProperty()
        {
            var logicalBranchItem = CreateLogicalBranchItem();

            string actualPropertyName = null;
            logicalBranchItem.PropertyChanged += (sndr, args) => actualPropertyName = args.PropertyName;

            var devBranch = Substitute.For<IDevBranch>();
            _logicalBranch.GetDevBranch().Returns(Optional<IDevBranch>.Some(devBranch));

            var devExplorerBarItem = ExplorerBarItemBuilder.ExplorerBarItem().Build();
            _explorerBarItemsRepository.CreateDevBranchItem(devBranch).Returns(devExplorerBarItem);

            _pubSubMediator.Publish(new CreateDevBranchFinishEventData(_logicalBranch));

            Assert.AreEqual(nameof(RootBranchExplorerBarItem.Actions), actualPropertyName);
        }

        [Test]
        public void CreateDevBranchNotificationReceived_IfDifferentLogicalBranch_DontAddTheNewDevBranchItem()
        {
            var logicalBranchItem = CreateLogicalBranchItem();

            var devBranch = Substitute.For<IDevBranch>();
            _logicalBranch.GetDevBranch().Returns(Optional<IDevBranch>.Some(devBranch));

            var devExplorerBarItem = ExplorerBarItemBuilder.ExplorerBarItem().Build();
            _explorerBarItemsRepository.CreateDevBranchItem(devBranch).Returns(devExplorerBarItem);

            var otherLogicalBranch = Substitute.For<IRootBranch>();
            _pubSubMediator.Publish(new CreateDevBranchFinishEventData(otherLogicalBranch));

            Assert.IsFalse(logicalBranchItem.Items.Contains(devExplorerBarItem));
        }

        [Test]
        public void CreateDevBranchNotificationReceived_ExplandTheNewAddedDevBranchItem()
        {
            var logicalBranchItem = CreateLogicalBranchItem();
            logicalBranchItem.IsSelected = true;
            var devBranch = Substitute.For<IDevBranch>();
            _logicalBranch.GetDevBranch().Returns(Optional<IDevBranch>.Some(devBranch));

            var devExplorerBarItem = ExplorerBarItemBuilder.ExplorerBarItem().Build();
            _explorerBarItemsRepository.CreateDevBranchItem(devBranch).Returns(devExplorerBarItem);

            _pubSubMediator.Publish(new CreateDevBranchFinishEventData(_logicalBranch));
            
            devExplorerBarItem.Received().EnsureExpanded();
        }


        [TestCase(true)]
        [TestCase(false)]
        public void CreateDevBranchNotificationReceived_NewlyAddedDevBranchItemShouldBeSelectedAccordingWithTheSelectionOfTheLogicalBranchItem(bool expectedSelection)
        {
            var logicalBranchItem = CreateLogicalBranchItem();
            logicalBranchItem.IsSelected = expectedSelection;

            var devBranch = Substitute.For<IDevBranch>();
            _logicalBranch.GetDevBranch().Returns(Optional<IDevBranch>.Some(devBranch));

            var devExplorerBarItem = ExplorerBarItemBuilder.ExplorerBarItem().Build();
            _explorerBarItemsRepository.CreateDevBranchItem(devBranch).Returns(devExplorerBarItem);

            _pubSubMediator.Publish(new CreateDevBranchFinishEventData(_logicalBranch));

            Assert.AreEqual(expectedSelection, devExplorerBarItem.IsSelected);
        }
    }
}
