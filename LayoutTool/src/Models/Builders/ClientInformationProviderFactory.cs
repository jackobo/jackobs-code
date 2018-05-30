using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common;

namespace LayoutTool.Models.Builders
{
    internal class ClientInformationProviderFactory : IClientInformationProviderFactory
    {
        public ClientInformationProviderFactory(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            _providers = DiscoverProviders();
        }

        IServiceLocator _serviceLocator;
        IEnumerable<IClientInformationProvider> _providers;

        private IEnumerable<IClientInformationProvider> DiscoverProviders()
        {
            return _serviceLocator.DiscoverAndResolveAll<IClientInformationProvider>(type => type != this.GetType(),
                                                                                                             this.GetType().Assembly);
        }
        

        public IClientInformationProvider GetProvider(BrandEntity brand, SkinEntity skin)
        {
            foreach(var provider in _providers)
            {
                if (provider.CanHandle(brand, skin))
                    return provider;
            }

            throw new InvalidOperationException($"Can't find a client for '{brand.ToString()}' and skin '{skin.ToString()}'; CDN={brand.CDNUrl}");
        }
    }
}
