using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Windows
{
    public interface IWindowsServicesManager
    {
        void StopService(string serviceName);
        void StartService(string serviceName, string[] args, bool waitToStart = true, Func<bool> cancelCallback = null);
        void StartService(string serviceName, string[] args, TimeSpan wait, Func<bool> cancelCallback = null);
        void CreateService(string serviceName, string binPath, WinServiceServiceAccountInfo accountInfo, params string[] dependsOn);
        void DeleteService(string serviceName);
        bool IsServiceInstalled(string serviceName);
        bool IsServiceRunning(string serviceName);
        WindowsServiceInformation GetWindowsServiceInformationOrNull(string serviceName);
    }

    public enum ServiceStartMode
    {
        Unknown,
        Auto,
        Manual,
        Disabled
    }

    public class WindowsServiceInformation
    {
        private WindowsServiceInformation()
        {

        }

        public string ServiceName { get; private set; }
        public string UserAccountName { get; private set; }
        public int ProcessId { get; private set; }
        public string BinPath { get; private set; }
        public ServiceStartMode StartMode { get; private set; }


        private static ServiceStartMode ParseStartMode(string startMode)
        {
            if (startMode == null)
                return ServiceStartMode.Unknown;

            if (startMode.ToString() == "Auto")
                return ServiceStartMode.Auto;

            if (startMode.ToString() == "Manual")
                return ServiceStartMode.Manual;

            if (startMode.ToString() == "Disabled")
                return ServiceStartMode.Disabled;

            return ServiceStartMode.Unknown;
        }

        internal static WindowsServiceInformation Get(string serviceName)
        {

            var sc = System.ServiceProcess.ServiceController.GetServices().FirstOrDefault(s => string.Compare(s.ServiceName, serviceName, true) == 0);

            if (sc == null)
                return null;

            SelectQuery query = new System.Management.SelectQuery(string.Format("select name, startName, startMode, processId, pathName from Win32_Service where name = '{0}'", serviceName));

            using (ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(query))
            {

                foreach (ManagementObject service in searcher.Get())
                {
                    return new WindowsServiceInformation()
                    {
                        BinPath = service["pathName"].ToString(),
                        ProcessId = int.Parse(service["processId"].ToString()),
                        ServiceName = sc.ServiceName,
                        StartMode = ParseStartMode(service["startMode"].ToString()),
                        UserAccountName = service["startName"].ToString()
                    };

                }
            }

            return null;
        }
    }

    public class WinServiceServiceAccountInfo
    {
        public WinServiceServiceAccountInfo()
        {
        }

        public WinServiceServiceAccountInfo(string builtInAccount)
        {
            this.UserName = builtInAccount;
        }

        public WinServiceServiceAccountInfo(string userName, string password)
        {
            this.UserName = userName;
            this.Password = password;
        }
        public string UserName { get; private set; }
        public string Password { get; private set; }
    }
}
