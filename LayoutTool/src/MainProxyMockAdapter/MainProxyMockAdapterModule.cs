using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.MainProxyDataControlService;
using Microsoft.Practices.Unity;
using Prism.Logging;
using Prism.Modularity;
using Spark.Wpf.Common.Interfaces.UI;

namespace LayoutTool.MainProxy
{
    [Module(ModuleName = "MainProxyMockAdapter")]
    [ModuleDependency(WellKnownModules.Models)]
    public class MainProxyMockAdapterModule : IModule
    {
        public MainProxyMockAdapterModule(IUnityContainer container)
        {
            Container = container;
        }

        IUnityContainer Container { get; set; }

        public void Initialize()
        {
            if (UseEmbededMainProxyMock)
            {
                Container.RegisterInstance(Container.Resolve<EmbeddedMainProxyMockInstaller>());
            }

            Container.RegisterInstance<IMainProxyAdapter>(Container.Resolve<MainProxyMockAdapter>());
        }

        private bool UseEmbededMainProxyMock
        {
            get
            {
                var operatingSystemServices = this.Container.Resolve<Spark.Infra.Windows.IOperatingSystemServices>();
                
                if(operatingSystemServices.WindowsServicesManager.IsServiceInstalled("MainProxyMock") 
                    && operatingSystemServices.WindowsServicesManager.IsServiceRunning("MainProxyMock"))
                {
                    return false;
                }
                               

                var value = ConfigurationManager.AppSettings["UseEmbededMainProxyMock"];
                if (string.IsNullOrEmpty(value))
                    return true;

                return string.Compare("true", value, true) == 0;
            }
        }
    }
}
