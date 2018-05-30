using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.DependencyInjection;

namespace Spark.Infra.Windows
{
    public class OperatingSystemServices : IOperatingSystemServices, IDependencyInjectionAware
    {
        
        Logging.ILoggerFactory _loggerFactory;
        public OperatingSystemServices(Logging.ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }


        public void RegisterWithContainer(IDependencyInjectionContainer container)
        {
            container.RegisterInstance<IOperatingSystemServices>(this);
            container.RegisterInstance(this.ComInteropServices);
            container.RegisterInstance(this.FileSystem);
            container.RegisterInstance(this.InternetInformationServicesInterop);
            container.RegisterInstance(this.MachineInformationProvider);
            container.RegisterInstance(this.NetshInterop);
            container.RegisterInstance(this.ThreadingServices);
            container.RegisterInstance(this.TimeServices);
            container.RegisterInstance(this.WindowsServicesManager);
        }

        public IComInteropServices ComInteropServices
        {
            get
            {
                return new ComInteropServices();
            }
        }

        public IFileSystemManager FileSystem
        {
            get
            {
                return new FileSystemManager();
            }
        }

        public IInternetInformationServicesInterop InternetInformationServicesInterop
        {
            get
            {
                return new InternetInformationServicesInterop();
            }
        }

        public bool IsX64OperatingSystem
        {
            get
            {
                return System.Environment.Is64BitOperatingSystem;
            }
        }

        public IMachineInformationProvider MachineInformationProvider
        {
            get
            {
                return new MachineInformationProvider();
            }
        }

        public INetshInterop NetshInterop
        {
            get
            {
                return new NetshInterop();
            }
        }

        public IThreadingServices ThreadingServices
        {
            get
            {
                return new ThreadingServices(_loggerFactory);
            }
        }

        public ITimeServices TimeServices
        {
            get
            {
                return new TimeServices();
            }
        }

        public IWindowsServicesManager WindowsServicesManager
        {
            get
            {
                return new WindowsServicesManager();
            }
        }
        
    }
}
