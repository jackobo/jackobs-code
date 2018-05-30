using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using GamesPortal.Service.Helpers;
using Spark.Infra.Logging;

namespace GamesPortal.Service
{
    [TestFixture]
    public class ApplicationLifetimeManagerTests
    {

        [SetUp]
        public void Setup()
        {
            _internalServices = GamesPortalInternalServicesHelper.Create();
            _logger = Substitute.For<ILogger>();
            _internalServices.LoggerFactory.CreateLogger(typeof(ApplicationLifetimeManager)).Returns(_logger);
            
            _manager = new ApplicationLifetimeManager(_internalServices.LoggerFactory);

        }



        ApplicationLifetimeManager _manager;
        IGamesPortalInternalServices _internalServices;
        ILogger _logger;

        [Test]
        public void RegisterShuttingDownAware_IfNullSubscriberIsProvided_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _manager.RegisterShuttingDownAware(null));
        }

        [Test]
        public void RegisterShuttingDownAware_IfTheSameSubscriberIsRegisteredTwice_ThrowArgumentException()
        {
            var subscriber = Substitute.For<IShuttingDownAware>();
            _manager.RegisterShuttingDownAware(subscriber);

            Assert.Throws<ArgumentException>(() => _manager.RegisterShuttingDownAware(subscriber));
        }

        [Test]
        public void ShuttingDown_ShuldCallAllSubscribers_ShutDown()
        {
            

            var subscriber1 = Substitute.For<IShuttingDownAware>();
            var subscriber2 = Substitute.For<IShuttingDownAware>();

            _manager.RegisterShuttingDownAware(subscriber1);
            _manager.RegisterShuttingDownAware(subscriber2);

            _manager.ShuttingDown();

            subscriber1.Received().ShutDown();
            subscriber2.Received().ShutDown();
        }
        
        [Test]
        public void ShuttingDown_IfAnySubscriberThrowsException_LogIt()
        {
            var subscriber = Substitute.For<IShuttingDownAware>();

            var exception = new ApplicationException("Test");

            subscriber.When(s => s.ShutDown())
                      .Do(s => { throw exception; });

            _manager.RegisterShuttingDownAware(subscriber);
            
            _manager.ShuttingDown();

            _logger.Received().Exception(exception);
        }

        [Test]
        public void ShuttingDown_IfAnySubscriberThrowsException_ShouldStillCallTheOthers()
        {
            var subscriber1 = Substitute.For<IShuttingDownAware>();
            var subscriber2 = Substitute.For<IShuttingDownAware>();

            subscriber1.When(s => s.ShutDown())
                      .Do(s => { throw new ApplicationException("Test"); });

            _manager.RegisterShuttingDownAware(subscriber1);
            _manager.RegisterShuttingDownAware(subscriber2);

            _manager.ShuttingDown();

            subscriber2.Received().ShutDown();
        }

        [Test]
        public void ShuttingDown_ShouldSetTheIsShuttingDownOnTrue()
        {
            _manager.ShuttingDown();

            Assert.AreEqual(true, _manager.IsShuttingDown);
        }

        [Test]
        public void UnregisterShuttingDownAware_ShouldRemoveTheSubscriberFromTheList()
        {
            var subscriber1 = Substitute.For<IShuttingDownAware>();
            var subscriber2 = Substitute.For<IShuttingDownAware>();
            
            _manager.RegisterShuttingDownAware(subscriber1);
            _manager.RegisterShuttingDownAware(subscriber2);

            _manager.UnregisterShuttingDownAware(subscriber1);

            _manager.ShuttingDown();

            subscriber1.DidNotReceive().ShutDown();
            subscriber2.Received().ShutDown();
        }
    }
}
