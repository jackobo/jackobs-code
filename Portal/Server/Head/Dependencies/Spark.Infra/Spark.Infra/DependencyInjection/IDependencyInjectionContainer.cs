using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.DependencyInjection
{
    public interface IDependencyInjectionContainer
    {
        void RegisterType<TFrom, TTo>() where TTo : TFrom;
        void RegisterInstance<T>(T instance);
    }
}
