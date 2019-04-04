using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using GamesPortal.Service.Synchronizers;
using GamesPortal.Service.Helpers;
using Spark.Infra.Logging;
using Spark.Infra.Windows;

namespace GamesPortal.Service
{
    [TestFixture]
    public class SynchronizerTests : SynchronizerTestHelper<SynchronizerTests.MockSynchronizer>
    {
        
        IAutoresetEvent _autoResetEvent;
                
        protected override void SetupInternalServices()
        {
            base.SetupInternalServices();
            _autoResetEvent = _internalServices.ThreadingServices.MockAutoResetEvent();
        }


        protected override MockSynchronizer CreateSynchronizer()
        {
            return new MockSynchronizer(_internalServices);
        }
        

        public class MockSynchronizer : Synchronizer
        {
            public MockSynchronizer(IGamesPortalInternalServices services)
                : base(services)
            {

            }

            protected override void DoWork()
            {
                DoWorkCallCount++;
                DoWorkAction?.Invoke();

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
        public void Constructor_IsStartedShouldBeFalse()
        {
            Assert.AreEqual(false, _synchronizer.IsStarted);
        }

     
        [Test]
        public void Constructor_ShouldCallInternalServicesToCreateA_SynchronizerStopper()
        {
            _internalServices.SynchronizerStopperFactory.Received().Create();
        }

        [Test]
        public void Start_ShouldInitializeNewTask()
        {
            _synchronizer.Start();
            _internalServices.ThreadingServices.Received().StartNewTask(Arg.Any<Action>());
        }

        [Test]
        public void Start_ShouldSet_IsRunning_OnTrue()
        {
            bool? isStarted = null;
            _synchronizer.DoWorkAction = () =>
            {
                isStarted = _synchronizer.IsStarted;
            };

            _synchronizer.Start();

            Assert.AreEqual(true, isStarted.Value);
        }

        [Test]
        public void RunLoop_ShouldSet_IsStarted_OnFalseAtTheEnd()
        {
            _synchronizer.Start();

            Assert.AreEqual(false, _synchronizer.IsStarted);
        }

        [Test]
        public void Start_IfAlreadyStarted_ThrowInvalidOperationException()
        {
            InvalidOperationException expectedException = null;
            _logger.Exception(Arg.Do<Exception>(arg => expectedException = (InvalidOperationException)arg));

            //this is a trick to start the synchronizer when is already started
            _synchronizer.DoWorkAction = () =>
            {
                _synchronizer.Start();
            };

            _synchronizer.Start();

            Assert.IsNotNull(expectedException);
        }

        [Test]
        public void Start_ShouldRegisterItsStopperWithTheApplicationLifeTimeManager()
        {
            _synchronizer.Start();
            _internalServices.ApplicationLifetimeManager.Received().RegisterShuttingDownAware(_synchronizerStopper);
        }

        [Test]
        public void Run_ShouldCallAutoResetEvent_Set()
        {
            _synchronizer.DoWorkAction = () => _synchronizer.Run();
            _synchronizer.Start();
            _autoResetEvent.Received().Set();
        }

        [Test]
        public void Start_ShouldCallAutoResetEvent_WaitOne()
        {
            _synchronizer.Start();
            _autoResetEvent.Received().WaitOne();
        }

        [Test]
        public void Start_WhileServiceIsRunning_CallAutoResetEvent_WaitOne()
        {
            Setup(doWorkCount: 3);
            _synchronizer.Start();
            _autoResetEvent.Received(3).WaitOne();
        }

        [Test]
        public void Start_MustCallDoWorkAtLeastOnce()
        {
            _synchronizer.Start();
            Assert.AreEqual(1, _synchronizer.DoWorkCallCount);
        }

        [Test]
        public void Start_IfAnExceptionIsCought_TheLoopShouldContinue()
        {
            Setup(doWorkCount: 2);

            _synchronizer.DoWorkAction = () => { throw new ApplicationException("Test"); };
            _synchronizer.Start();

            _autoResetEvent.Received(2).WaitOne();
        }

        [Test]
        public void Start_IfAnExceptionIsCought_LogTheEntireException()
        {
            var exception = new ApplicationException("Test");

            _synchronizer.DoWorkAction = () => { throw exception; };

            _synchronizer.Start();

            _logger.Received().Exception(exception);
        }

        

        [Test]
        public void Stop_ShouldSetTheAutoResetEvent()
        {
            _synchronizer.Stop();

            _autoResetEvent.Received().Set();
        }

        [Test]
        public void Stop_ShouldSetTheIsStartedOnFalse()
        {
            _synchronizer.Start();
            _synchronizer.Stop();

            Assert.AreEqual(false, _synchronizer.IsStarted);
        }
        

        [Test]
        public void Stop_OnceCalledTheLoopShouldEnd()
        {

            Setup(doWorkCount: 3);
            
            _synchronizer.DoWorkAction = () => _synchronizer.Stop();
            _synchronizer.Start();
            
            Assert.AreEqual(1, _synchronizer.DoWorkCallCount);
        }

        [Test]
        public void Stop_ShouldUnregisterItsStopperFromTheApplicationLifeTimeManager()
        {
            _synchronizer.Start();
            _synchronizer.Stop();
            _internalServices.ApplicationLifetimeManager.Received().UnregisterShuttingDownAware(_synchronizerStopper);
        }
        
        [Test]
        public void Stop_WhenCalled_ShouldForwardTheCallToTheStopper()
        {
            _synchronizer.Start();
            _synchronizer.Stop();

            _synchronizerStopper.Received().ShutDown();
        }

        [Test]
        public void Run_IfNotStarted_ThrowInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() =>  _synchronizer.Run());
        }
    }
}
