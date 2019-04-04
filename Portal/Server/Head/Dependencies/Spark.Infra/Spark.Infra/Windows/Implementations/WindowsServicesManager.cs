using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Windows
{
    public class WindowsServicesManager : IWindowsServicesManager
    {

        [StructLayout(LayoutKind.Sequential)]
        internal sealed class SERVICE_STATUS_PROCESS
        {
            [MarshalAs(UnmanagedType.U4)]
            public uint dwServiceType;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwCurrentState;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwControlsAccepted;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwWin32ExitCode;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwServiceSpecificExitCode;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwCheckPoint;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwWaitHint;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwProcessId;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwServiceFlags;
        }

        internal const int ERROR_INSUFFICIENT_BUFFER = 0x7a;
        internal const int SC_STATUS_PROCESS_INFO = 0;

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool QueryServiceStatusEx(SafeHandle hService, int infoLevel, IntPtr lpBuffer, uint cbBufSize, out uint pcbBytesNeeded);

        public WindowsServiceInformation GetWindowsServiceInformationOrNull(string serviceName)
        {
            return WindowsServiceInformation.Get(serviceName);
        }

        private System.Diagnostics.Process GetServiceProcess(ServiceController sc)
        {
            var serviceInfo = GetWindowsServiceInformationOrNull(sc.ServiceName);

            int processId = 0;

            if (serviceInfo != null)
                processId = serviceInfo.ProcessId;

            if (processId == 0)
                processId = GetServiceProcessIdUsinWinApi(sc);

            if (serviceInfo.ProcessId == 0)
                return null;


            return System.Diagnostics.Process.GetProcessById(processId);

        }

        private static int GetServiceProcessIdUsinWinApi(ServiceController sc)
        {
            if (sc == null)
                throw new ArgumentNullException("sc");

            IntPtr zero = IntPtr.Zero;

            try
            {
                UInt32 dwBytesNeeded;
                // Call once to figure the size of the output buffer.
                QueryServiceStatusEx(sc.ServiceHandle, SC_STATUS_PROCESS_INFO, zero, 0, out dwBytesNeeded);
                if (Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
                {
                    // Allocate required buffer and call again.
                    zero = Marshal.AllocHGlobal((int)dwBytesNeeded);

                    if (QueryServiceStatusEx(sc.ServiceHandle, SC_STATUS_PROCESS_INFO, zero, dwBytesNeeded, out dwBytesNeeded))
                    {
                        var ssp = new SERVICE_STATUS_PROCESS();
                        Marshal.PtrToStructure(zero, ssp);
                        return (int)ssp.dwProcessId;
                    }
                }
            }
            catch
            {
                return -1;
            }
            finally
            {
                if (zero != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(zero);
                }
            }
            return -1;
        }


        public void StartService(string serviceName, string[] args, bool waitToStart = true, Func<bool> cancelCallback = null)
        {
            StartService(serviceName, args, null, waitToStart, cancelCallback);
        }


        public void StartService(string serviceName, string[] args, TimeSpan wait, Func<bool> cancelCallback = null)
        {
            StartService(serviceName, args, wait, true, cancelCallback);
        }

        private void StartService(string serviceName, string[] args, Nullable<TimeSpan> timeout, bool waitToStart = true, Func<bool> cancelCallback = null)
        {

            using (var service = System.ServiceProcess.ServiceController.GetServices()
                                                                .Where(s => string.Compare(s.ServiceName, serviceName, true) == 0)
                                                                .FirstOrDefault())
            {
                if (service == null)
                    return;

                if (service.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                    return;

                if (args != null && args.Length > 0)
                    service.Start(args);
                else
                    service.Start();

                if (waitToStart)
                {
                    if (timeout.HasValue)
                    {
                        try
                        {

                            service.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running, timeout.Value);
                        }
                        catch (System.ServiceProcess.TimeoutException)
                        {
                            //ignore the timeout
                        }
                    }
                    else
                    {
                        WaitForServiceStatusRunning(service, cancelCallback);
                    }
                }

            }


        }

        private void WaitForServiceStatusRunning(System.ServiceProcess.ServiceController service, Func<bool> cancelCallback)
        {

            do
            {
                if (cancelCallback != null && cancelCallback() == true)
                {
                    return;
                }

                try
                {
                    service.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
                }
                catch (System.ServiceProcess.TimeoutException)
                {
                    //ignore the timeout
                }
            } while (service.Status == ServiceControllerStatus.StartPending);

            if (service.Status != ServiceControllerStatus.Running)
            {
                throw new ApplicationException(string.Format("Service {0} failed to start!", service.ServiceName));
            }
        }


        public void StopService(string serviceName)
        {
            using (var service = System.ServiceProcess.ServiceController.GetServices()
                                                                .Where(s => string.Compare(s.ServiceName, serviceName, true) == 0)
                                                                .FirstOrDefault())
            {
                if (service == null)
                    return;


                if (service.Status == System.ServiceProcess.ServiceControllerStatus.Stopped)
                    return;

                using (var process = GetServiceProcess(service))
                {

                    service.Stop();

                    service.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped);

                    if (process != null)
                    {
                        process.WaitForExit();
                    }

                }
            }
        }

        public bool IsServiceInstalled(string serviceName)
        {
            return System.ServiceProcess.ServiceController.GetServices()
                                                          .Any(s => string.Compare(s.ServiceName, serviceName, true) == 0);
        }


        public bool IsServiceRunning(string serviceName)
        {
            var service = System.ServiceProcess.ServiceController.GetServices().FirstOrDefault(s => string.Compare(s.ServiceName, serviceName, true) == 0);

            if (service == null)
                return false;

            return service.Status == ServiceControllerStatus.Running;
        }

        public void DeleteService(string serviceName)
        {
            ServiceControlUtils.DeleteService(serviceName);
        }


        public void CreateService(string serviceName, string binPath, WinServiceServiceAccountInfo accountInfo, params string[] dependsOn)
        {
            ServiceControlUtils.CreateService(serviceName, binPath, accountInfo, dependsOn);
        }
    }

    public static class ServiceControlUtils
    {
        public static void Run(string arguments)
        {
            ExternalProcessRunner.Run("sc.exe", arguments);
        }

        public static void CreateService(string serviceName, string binPath, params string[] dependsOn)
        {
            CreateService(serviceName, binPath, null, dependsOn);
        }

        public static void CreateService(string serviceName, string binPath, WinServiceServiceAccountInfo accountInfo, params string[] dependsOn)
        {

            if (System.ServiceProcess.ServiceController.GetServices().Any(s => string.Compare(s.ServiceName, serviceName, true) == 0))
                return;

            string depend = string.Empty;

            if (dependsOn != null && dependsOn.Length > 0)
            {
                depend = string.Format("depend= {0}", string.Join("/", dependsOn));
            }


            string account = string.Empty;

            if (accountInfo != null && !string.IsNullOrEmpty(accountInfo.UserName))
            {
                if (string.IsNullOrEmpty(accountInfo.Password))
                {
                    account = string.Format("obj= {0}", accountInfo.UserName);
                }
                else
                {
                    account = string.Format("obj= {0} password= {1}", accountInfo.UserName, accountInfo.Password);
                }
            }

            Run(string.Format(" create {0} binPath= {1} start= auto {2} {3}", serviceName, binPath, depend, account));
        }


        public static void DeleteService(string serviceName)
        {
            if (!System.ServiceProcess.ServiceController.GetServices().Any(s => string.Compare(s.ServiceName, serviceName, true) == 0))
                return;

            ServiceControlUtils.Run(string.Format("delete {0}", serviceName));
        }


    }
}
