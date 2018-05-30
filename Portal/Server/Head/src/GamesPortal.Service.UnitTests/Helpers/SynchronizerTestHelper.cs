using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Logging;
using NSubstitute;
using NUnit.Framework;

namespace GamesPortal.Service.Helpers
{
    public abstract class SynchronizerTestHelper<TSynchronizer>
        where TSynchronizer : Synchronizers.ISynchronizer
    {
        protected IGamesPortalInternalServices _internalServices;
        protected ISynchronizerStopper _synchronizerStopper;
        protected ILogger _logger;
        protected TSynchronizer _synchronizer;

        [SetUp]
        public virtual void Setup()
        {
            Setup(doWorkCount: 1);
        }

        protected virtual void Setup(int doWorkCount)
        {
            SetupInternalServices();
            SetupSynchronizerStopper(doWorkCount);
            _synchronizer = CreateSynchronizer();
        }

        protected virtual void SetupLogger()
        {
            _logger = Substitute.For<ILogger>();
            _internalServices.LoggerFactory.CreateLogger(Arg.Any<Type>()).Returns(_logger);
        }


        protected abstract TSynchronizer CreateSynchronizer();

        protected virtual void SetupInternalServices()
        {
            _internalServices = GamesPortalInternalServicesHelper.Create();
            

            _internalServices.ThreadingServices.StartNewTask(Arg.Do<Action>(action => action()))
                                             .Returns(new Task(() => { }));

            SetupLogger();
        }

        protected virtual void SetupSynchronizerStopper(int doWorkCount)
        {
            _synchronizerStopper = Substitute.For<ISynchronizerStopper>();
            _synchronizerStopper.When(s => s.ShutDown())
                    .Do(s => _synchronizerStopper.IsStopped.Returns(true));

            if (doWorkCount == 0)
            {
                _synchronizerStopper.IsStopped.Returns(true);
            }
            else
            {
                var isStoppedReturns = new bool[doWorkCount];
                isStoppedReturns[isStoppedReturns.Length - 1] = true;
                _synchronizerStopper.IsStopped.Returns(false, isStoppedReturns);
            }

            _internalServices.SynchronizerStopperFactory.Create().Returns(_synchronizerStopper);
        }
    }
}
