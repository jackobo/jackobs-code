using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using NSubstitute;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;
using Spark.UnitTesting.Helpers;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    [TestFixture]
    public class TfsExplorerBarTests
    {
        IServiceLocator _serviceLocator;
        IComponentsRepository _repositoryServices;
        IExplorerBarItemsRepository _explorerBarItemsRepository;
        IExplorerBarItemsRepositoryFactory _explorerBarItemsRepositoryFactory;

        TfsExplorerBar _explorerBar;
        
        [SetUp]
        public void Setup()
        {
            Init();
        }

        private void Init(params IRootBranch[] rootBranches)
        {
            InitServices(rootBranches);
            InitExplorerBar();
        }

        private void InitExplorerBar()
        {
            _explorerBar = new TfsExplorerBar(_serviceLocator);
        }

        private void InitServices(params IRootBranch[] rootBranches)
        {
            _repositoryServices = Substitute.For<IComponentsRepository>();
            _repositoryServices.GetRootBranches().Returns(rootBranches);
            _explorerBarItemsRepositoryFactory = Substitute.For<IExplorerBarItemsRepositoryFactory>();
            _explorerBarItemsRepository = Substitute.For<IExplorerBarItemsRepository>();
            _explorerBarItemsRepositoryFactory.GetRepository(Arg.Any<IExplorerBarItem>()).Returns(_explorerBarItemsRepository);


            _serviceLocator = ServiceLocatorBuilder
                                     .ServiceLocator()
                                     .WithService(_repositoryServices)
                                     .WithService(_explorerBarItemsRepositoryFactory)
                                     .Build();


        }

        private IRootBranch CreateLogicalBranch(int majorVersion = 3)
        {
            return Helpers.LogicalBranchBuilder.LogicalBranch()
                                                .WithName(majorVersion)
                                                .Build();
        }

        private IExplorerBarItem CreateExplorerBarItem(string caption = "")
        {
            return Helpers.ExplorerBarItemBuilder.ExplorerBarItem()
                                                  .WithCaption(caption)
                                                  .Build();
        }

        [Test]
        public void Constructor_ShouldLoadTheLogicalBranches()
        {
            Init(CreateLogicalBranch());
            Assert.AreEqual(1, _explorerBar.Items.Count);
        }


        [Test]
        public void Constructor_IfThereAreNoLogicalBranches_DontLoadAnyItems()
        {
            Assert.AreEqual(0, _explorerBar.Items.Count);
        }


        [Test]
        public void Constructor_ShouldCallExplorerBarItemsFactory_ToCreateLogicalBranchExplorerBarItem()
        {
            var logicalBranch = CreateLogicalBranch();
            Init(logicalBranch);
            _explorerBarItemsRepository.Received().CreateRootBranchItem(logicalBranch, _explorerBar);
        }

       
        [Test]
        public void Constructor_ShouldLoadLogicalBranches_InDescendingOrder()
        {
            var b1 = CreateLogicalBranch(3);
            var b2 = CreateLogicalBranch(4);
            var b3 = CreateLogicalBranch(5);

            var item1 = CreateExplorerBarItem("3.x");
            var item2 = CreateExplorerBarItem("4.x");
            var item3 = CreateExplorerBarItem("5.x");

            InitServices(b1, b2, b3);

            _explorerBarItemsRepository.CreateRootBranchItem(b1, Arg.Any<IExplorerBar>()).Returns(item1);
            _explorerBarItemsRepository.CreateRootBranchItem(b2, Arg.Any<IExplorerBar>()).Returns(item2);
            _explorerBarItemsRepository.CreateRootBranchItem(b3, Arg.Any<IExplorerBar>()).Returns(item3);

            InitExplorerBar();

            Assert.AreEqual(new RootBranchVersion(5).ToString() , _explorerBar.Items[0].Caption);
            Assert.AreEqual(new RootBranchVersion(4).ToString(), _explorerBar.Items[1].Caption);
            Assert.AreEqual(new RootBranchVersion(3).ToString(), _explorerBar.Items[2].Caption);
        }


    }
}
