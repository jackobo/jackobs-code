using Autofac.Integration.WebApi;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace InvestTracker.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            var container = IoCContainer.Configure();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            //var httpConfiguration = WebApiConfig.CreateConfiguration(new AutofacWebApiDependencyResolver(container));
            
            appBuilder.UseAutofacMiddleware(container);
            appBuilder.UseAutofacWebApi(GlobalConfiguration.Configuration);
            appBuilder.UseWebApi(GlobalConfiguration.Configuration);
        }
    }
}