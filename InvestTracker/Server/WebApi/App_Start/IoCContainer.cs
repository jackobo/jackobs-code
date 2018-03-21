using Autofac;
using Autofac.Integration.WebApi;
using InvestTracker.DataAccessLayer;
using InvestTracker.DataAccessLayer.Contracts;
using InvestTracker.DomainLogic;
using InvestTracker.DomainLogic.Contracts;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace InvestTracker.WebApi
{
    public static class IoCContainer
    {
        public static IContainer Configure()
        {
            var containerBuilder = new ContainerBuilder();

            RegisterServices(containerBuilder);
            RegisterApiControllers(containerBuilder);

            return containerBuilder.Build();

        }

        private static void RegisterApiControllers(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterApiControllers(Assembly.GetExecutingAssembly()).InstancePerRequest();
        }

        private static void RegisterServices(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<InvestmentFundsRepository>()
                            .As<IInvestmentFundsRepository>();
            containerBuilder.RegisterType<InvestementFundsServices>()
                            .As<IInvestementFundsServices>();
        }
    }
}