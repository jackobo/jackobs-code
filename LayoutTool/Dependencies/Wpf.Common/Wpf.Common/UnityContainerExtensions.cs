using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Spark.Wpf.Common
{
    public static class UnityContainerExtensions
    {
        public static void DiscoverAndRegisterAll<TInterface>(this IUnityContainer container,  params Assembly[] assemblies)
        {
            container.DiscoverAndRegisterAll<TInterface>(null, assemblies);
        }

        public static void DiscoverAndRegisterAll<TInterface>(this IUnityContainer container, Func<Type, bool> extraFilter, params Assembly[] assemblies)
        {
            foreach (var type in DiscoverTypes<TInterface>(extraFilter, assemblies))
            {
                container.RegisterType(typeof(TInterface), type, type.FullName);
            }
        }


        public static IEnumerable<TInterface> DiscoverAndResolveAll<TInterface>(this IServiceLocator serviceLocator, params Assembly[] assemblies)
        {
            return serviceLocator.DiscoverAndResolveAll<TInterface>(null, assemblies);
        }

        public static IEnumerable<TInterface> DiscoverAndResolveAll<TInterface>(this IServiceLocator serviceLocator, Func<Type, bool> extraFilter, params Assembly[] assemblies)
        {
            
            var result = new List<TInterface>();

            foreach (var type in DiscoverTypes<TInterface>(extraFilter, assemblies))
            {
                result.Add((TInterface)serviceLocator.GetInstance(type));
            }

            return result;
        }

        private static IEnumerable<Type> DiscoverTypes<TInterface>(Func<Type, bool> extraFilter, Assembly[] assemblies)
        {
            var types = new List<Type>();

            foreach (var assembly in assemblies)
            {

                var query = assembly.GetTypes().Where(t => !t.IsAbstract && typeof(TInterface).IsAssignableFrom(t));
                if (extraFilter != null)
                {
                    query = query.Where(extraFilter);
                }

                types.AddRange(query.ToArray());
            }

            return types;
        }

    }
}
