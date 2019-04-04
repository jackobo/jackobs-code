using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using LayoutTool.Interfaces;
using Spark.Infra.Windows;

namespace LayoutTool.Models
{
    public class WebClientFactory : IWebClientFactory
    {
        public WebClientFactory(IWcfServiceFactory wcfServiceFactory, IOperatingSystemServices environmentServices)
        {
            _wcfServiceFactory = wcfServiceFactory;
            _environmentServices = environmentServices;
        }

        IWcfServiceFactory _wcfServiceFactory;
        IOperatingSystemServices _environmentServices;


        public IWebClient CreateWebClient()
        {
            return new WebClientWrapper(_wcfServiceFactory, _environmentServices);
        }
    }
}
