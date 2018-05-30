using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using GamesPortal.Service.Synchronizers;
using GamesPortal.Service.Helpers;
using GGPGameServer.ApprovalSystem.Common.Logger;

namespace GamesPortal.Service
{
    [TestFixture]
    public class SynchronizerBaseTests
    {

        IGamesPortalInternalServices _internalServices;
        IAutoresetEvent _autoResetEvent;
        MockSynchronizer _mockSynchronizer;

        [SetUp]
        public void Setup()
        {
            SetupInternalServices(1);
            SetupMockSynchronizer();
        }

        private void SetupMockSynchronizer()
        {
            _mockSynchronizer = new MockSynchronizer(_internalServices);
        }

        private void SetupInternalServices(int appLifeTimeCycles)
        {
            _internalServices = GamesPortalInternalServicesHelper.Create();
            
            SetupAppLifetime(appLifeTimeCycles);

            _autoResetEvent = _internalServices.ThreadingServices.MockAutoResetEvent();
            _internalServices.ThreadingServices.StartNewTask(Arg.Do<Action>(action => action()))
                                             .Returns(new Task(() => { }));
        }

        private void SetupAppLifetime(int appLifeTimeCycles)
        {
            if (appLifeTimeCycles == 0)
            {
                _internalServices.ApplicationLifetimeManager.IsShuttingDown.Returns(true);
            }
            else
            {
                var isShuttintDownReturns = new bool[appLifeTimeCycles];
                isShuttintDownReturns[isShuttintDownReturns.Length - 1] = true;
                _internalServices.ApplicationLifetimeManager.IsShuttingDown.Returns(false, isShuttintDownReturns);
            }
        }

        private class MockSynchronizer : SynchronizerBase
        {
            public MockSynchronizer(IGamesPortalInternalServices services)
                : base(services)
            {

            }

            protected override void DoWork()
            {
                DoWorkCallCount++;
                if (DoWorkAction != null)
                    DoWorkAction();

            }

            public int DoWorkCallCount = 0;

            public Action DoWorkAction { get; internal set; }
        }



        [Test]
        public void Constructor_CreateAnAutoResetEventWithInitialStateNOTSignaled()
        {
            _internalServices.ThreadingServices.Received().CreateAutoResetEvent(false);
        }

        [Test]
        public void Start_ShouldInitializeNewTask()
        {
            _mockSynchronizer.Start();
            _internalServices.ThreadingServices.Received().StartNewTask(Arg.Any<Action>());
        }

        [Test]
        public void Run_ShouldCallAutoResetEvent_Set()
        {
            _mockSynchronizer.Run();
            _autoResetEvent.Received().Set();
        }

        [Test]
        public void Start_ShouldCallAutoResetEvent_WaitOne()
        {
            _mockSynchronizer.Start();
            _autoResetEvent.Received().WaitOne();
        }

        [Test]
        public void Start_WhileServiceIsRunning_CallAutoResetEvent_WaitOne()
        {
            SetupInternalServices(3);
            SetupMockSynchronizer();
            _mockSynchronizer.Start();
            _autoResetEvent.Received(3).WaitOne();
        }

        [Test]
        public void Start_MustCallDoWorkAtLeastOnce()
        {
            _mockSynchronizer.Start();
            Assert.AreEqual(1, _mockSynchronizer.DoWorkCallCount);
        }

        [Test]
        public void Start_IfAnExceptionIsCought_TheLoopShouldContinue()
        {
            SetupInternalServices(2);
            SetupMockSynchronizer();

            _mockSynchronizer.DoWorkAction = () => { throw new ApplicationException("Test"); };
            _mockSynchronizer.Start();

            _autoResetEvent.Received(2).WaitOne();
        }

        [Test]
        public void Start_IfAnExceptionIsCought_LogTheEntireException()
        {
            SetupInternalServices(2);
            var logger = Substitute.For<ILogNotifier>();
            _internalServices.LoggerFactory.CreateLogger(Arg.Any<Type>()).Returns(logger);


            SetupMockSynchronizer();

            var exception = new ApplicationException("Test");

            _mockSynchronizer.DoWorkAction = () => { throw exception; };

            _mockSynchronizer.Start();

            logger.Received().Exception(exception);
        }
    }
}
