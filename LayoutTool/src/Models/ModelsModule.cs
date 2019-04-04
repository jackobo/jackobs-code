using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using Microsoft.Practices.Unity;
using Prism.Modularity;

using Spark.Infra.Windows;
using Spark.Wpf.Common;

namespace LayoutTool.Models
{
    [Module(ModuleName = WellKnownModules.Models)]
    public class ModelsModule : IModule
    {
        public ModelsModule(IUnityContainer container)
        {
            this.Container = container;
        }

        IUnityContainer Container { get; set; }
        public void Initialize()
        {
            this.Container.RegisterType<IWcfServiceFactory, WcfServiceFactory>();
            this.Container.RegisterInstance<IFiddlerServices>(this.Container.Resolve<FiddlerServices>());
            this.Container.RegisterType<IWebClientFactory, WebClientFactory>();
            this.Container.RegisterType<ITextFileReader, TextFileReader>();
            this.Container.RegisterInstance<IBrandInformationProvider>(this.Container.Resolve<BrandInformationProvider>());
            this.Container.RegisterType<IClientInformationProviderFactory, Builders.ClientInformationProviderFactory>();
            this.Container.RegisterType<ISkinDefinitionSerializer, Builders.SkinDefinitionSerializer>();
            this.Container.RegisterType<IGamesInformationProvider, GamesInformationProvider>();
            this.Container.RegisterType<ICountryInformationProvider, CountryInformationProvider>();
            this.Container.RegisterType<ICurrencyInformationProvider, CurrencyInformationProvider>();
            this.Container.RegisterType<ISkinPublisher, SkinPublisher>();
            this.Container.RegisterType<IJackpotInfoProvider, JackpotInfoProvider>();

            if (this.Container.Resolve<IOperatingSystemServices>().InternetInformationServicesInterop.IsInstalled())
            {
                this.Container.RegisterInstance<IWebServerManager>(this.Container.Resolve<IIS.IISServerManager>());
            }
        }
        
    }
}
