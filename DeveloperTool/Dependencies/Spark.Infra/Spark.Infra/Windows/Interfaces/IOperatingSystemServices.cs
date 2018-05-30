using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Windows
{
    public interface IOperatingSystemServices
    {
        
        bool IsX64OperatingSystem { get; }
        IFileSystemManager FileSystem { get; }
        IWindowsServicesManager WindowsServicesManager { get; }

        IComInteropServices ComInteropServices { get; }

        INetshInterop NetshInterop { get; }

        IMachineInformationProvider MachineInformationProvider { get; }

        IInternetInformationServicesInterop InternetInformationServicesInterop { get; }

        IThreadingServices ThreadingServices { get; }

        ITimeServices TimeServices { get; }

        IAccountManagementServices AccountManagement { get; }
    }
}
