using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using NSubstitute;

namespace Spark.UnitTesting.Helpers
{
    public class ServiceLocatorBuilder
    {

        IServiceLocator _serviceLocator;
        private ServiceLocatorBuilder()
        {
            _serviceLocator = Substitute.For<IServiceLocator>();
        }


        public static ServiceLocatorBuilder ServiceLocator()
        {
            return new ServiceLocatorBuilder();
        }

        public ServiceLocatorBuilder WithService<T>(T instance) where T : class
        {
            _serviceLocator.GetInstance<T>().Returns(instance);
            return this;
        }

        public IServiceLocator Build()
        {
            return _serviceLocator;
        }
    }
}
