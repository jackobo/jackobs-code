using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Spark.Infra.Windows;

namespace GamesPortal.Service.Helpers
{
    public static class ThreadingServicesHelper
    {
        public static IAutoresetEvent MockAutoResetEvent(this IThreadingServices threadingServices)
        {
            var autoResetEvent = Substitute.For<IAutoresetEvent>();
            threadingServices.CreateAutoResetEvent(Arg.Any<bool>()).Returns(autoResetEvent);
            return autoResetEvent;
        }
    }
}
