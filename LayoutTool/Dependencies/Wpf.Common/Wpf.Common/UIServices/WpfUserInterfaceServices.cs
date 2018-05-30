using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.DependencyInjection;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.Wpf.Common.UIServices
{
    internal class WpfUserInterfaceServices : IUserInterfaceServices, IDependencyInjectionAware
    {
        public WpfUserInterfaceServices(IApplicationServices applicationServices)
        {
            _applicationServices = applicationServices;
        }

        IApplicationServices _applicationServices;


        private IWindowsFactory WindowsFactory
        {
            get { return new WpfWindowsFactory(); }
        }

        public IDialogServices DialogServices
        {
            get
            {
                return new DialogServices(WindowsFactory, _applicationServices);
            }
        }

        public IMessageBox MessageBox
        {
            get
            {
                return new WpfMessageBox(_applicationServices);
            }
        }

        public void RegisterWithContainer(IDependencyInjectionContainer container)
        {
            container.RegisterInstance<IUserInterfaceServices>(this);
            container.RegisterType<IWindowsFactory, WpfWindowsFactory>();
            container.RegisterType<IMessageBox, WpfMessageBox>();
            container.RegisterType<IDialogServices, DialogServices>();
        }
    }
}
