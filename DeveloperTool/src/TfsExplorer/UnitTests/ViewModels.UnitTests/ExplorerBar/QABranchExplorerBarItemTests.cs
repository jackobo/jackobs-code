using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.TfsExplorer.ViewModels.Helpers;
using System.Collections.ObjectModel;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.UnitTesting.Helpers;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    [TestFixture]
    public class QABranchExplorerBarItemTests
    {

        IQaBranch _qaBranch;
        IExplorerBarItemsRepository _explorerBarItemsRepository;
        IExplorerBarItemsRepositoryFactory _explorerBarItemsRepositoryFactory;
        IServiceLocator _serviceLocator;
        IExplorerBarItem _parent;
        IPubSubMediator _pubSubMediator;

        [SetUp]
        public void Setup()
        {
            InitDependencies();
        }

        private void InitDependencies()
        {
            _qaBranch = Substitute.For<IQaBranch>();
            _parent = Substitute.For<IExplorerBarItem>();
            _explorerBarItemsRepositoryFactory = Substitute.For<IExplorerBarItemsRepositoryFactory>();
            _explorerBarItemsRepository = Substitute.For<IExplorerBarItemsRepository>();
            _explorerBarItemsRepositoryFactory.GetRepository(Arg.Any<IExplorerBarItem>()).Returns(_explorerBarItemsRepository);

            _pubSubMediator = new Spark.Wpf.Common.TestHelpers.MockPubSubMediator();

            _serviceLocator = ServiceLocatorBuilder.ServiceLocator()
                                                   .WithService(_explorerBarItemsRepositoryFactory)
                                                   .WithService(_pubSubMediator)
                                                   .Build();
        }


        private QABranchExplorerBarItem CreateQABranchItem()
        {
            return new QABranchExplorerBarItem(_qaBranch, _parent, _serviceLocator);
        }
        
   
        [Test]
        public void Constructor_ShouldAddAn_QaMainExplorerBarItem()
        {
            var qaMainExplorerBarItem = ExplorerBarItemBuilder.ExplorerBarItem().Build();
            
            _explorerBarItemsRepository.CreateMainQaBranchItem(_qaBranch).Returns(qaMainExplorerBarItem);

            var qaBranchItem = CreateQABranchItem();
            
            Assert.IsTrue(qaBranchItem.Items.Contains(qaMainExplorerBarItem));
        }

        [Test]
        public void Constructor_IfThereAreAnyFeatureBranches_AddsTheFeaturesItem()
        {
            var f1 = Substitute.For<IFeatureBranch>();
            var f2 = Substitute.For<IFeatureBranch>();
            _qaBranch.GetFeatureBranches().Returns(new IFeatureBranch[] { f1, f2 });

            var featuresBranchesExplorerBarItem = ExplorerBarItemBuilder.ExplorerBarItem().Build();
            _explorerBarItemsRepository.CreateQAFeaturesBranchesItem(_qaBranch).Returns(featuresBranchesExplorerBarItem);
            var qaBranchItem = CreateQABranchItem();

            Assert.IsTrue(qaBranchItem.Items.Contains(featuresBranchesExplorerBarItem));
        }


        [Test]
        public void Constructor_IfThereAreNoFeatureBranches_DontAddTheFeaturesItem()
        {
            _qaBranch.GetFeatureBranches().Returns(new IFeatureBranch[0]);

            var featuresBranchesExplorerBarItem = ExplorerBarItemBuilder.ExplorerBarItem().Build();
            _explorerBarItemsRepository.CreateQAFeaturesBranchesItem(_qaBranch)
                                    .Returns(featuresBranchesExplorerBarItem);
            var qaBranchItem = CreateQABranchItem();

            Assert.IsFalse(qaBranchItem.Items.Contains(featuresBranchesExplorerBarItem));
        }


        [Test]
        public void FeatureBranches_WhenNewFeatureBranchNotificationIsReceived_LoadTheFeatureBranchesIfIsNotAlreadyLoaded()
        {
            var newFeature = Substitute.For<IFeatureBranch>();
            _qaBranch.GetFeatureBranches().Returns(new IFeatureBranch[0], 
                                                   new IFeatureBranch[] { newFeature });

            var featuresBranchesExplorerBarItem = ExplorerBarItemBuilder.ExplorerBarItem().Build();
            _explorerBarItemsRepository.CreateQAFeaturesBranchesItem(_qaBranch)
                                    .Returns(featuresBranchesExplorerBarItem);

            var qaBranchItem = CreateQABranchItem();
            

            _pubSubMediator.Publish(new NewFeatureBranchEventData(newFeature, _qaBranch));

            Assert.IsTrue(qaBranchItem.Items.Contains(featuresBranchesExplorerBarItem));
        }

        [Test]
        public void FeatureBranches_WhenNewFeatureBranchNotificationIsReceivedForADifferentQABranch_DontLoadTheFeatureBranchesIfIsNotAlreadyLoaded()
        {
            var newFeature = Substitute.For<IFeatureBranch>();
            _qaBranch.GetFeatureBranches().Returns(new IFeatureBranch[0],
                                                   new IFeatureBranch[] { newFeature });

            var featuresBranchesExplorerBarItem = ExplorerBarItemBuilder.ExplorerBarItem().Build();
            _explorerBarItemsRepository.CreateQAFeaturesBranchesItem(_qaBranch)
                                       .Returns(featuresBranchesExplorerBarItem);

            var qaBranchItem = CreateQABranchItem();


            _pubSubMediator.Publish(new NewFeatureBranchEventData(newFeature, Substitute.For<IMainBranch>()));

            Assert.IsFalse(qaBranchItem.Items.Contains(featuresBranchesExplorerBarItem));
        }


    }
}
