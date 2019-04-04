using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Spark.Infra.Windows
{
    public class InternetInformationServicesInterop : IInternetInformationServicesInterop
    {
        const string IISRegKeyName = "Software\\Microsoft\\InetStp";
        const string IISRegKeyValue = "MajorVersion";
        const string IISRegKeyMinorVersionValue = "MinorVersion";

        public bool IsInstalled()
        {
            RegistryKey baseKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, String.Empty);

            if (baseKey == null)
            {
                return false;
            }

            var registryKey = baseKey.OpenSubKey("Software\\Microsoft\\InetStp", RegistryKeyPermissionCheck.ReadSubTree);


            if (registryKey == null)
                return false;
            else
                return true;
        }

     

        static bool IsVersionInstalled(int majorVersion, int minVersion = 0)
        {
            bool found = false;
            int regValue;

            if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord,
                out regValue))
            {
                if (regValue == majorVersion)
                {
                    if (minVersion != 0)
                    {
                        if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyMinorVersionValue,
                            RegistryValueKind.DWord, out regValue))
                        {
                            if (regValue == minVersion)
                            {
                                found = true;
                            }
                        }
                    }
                    else
                    {
                        found = true;
                    }
                }
            }

            return found;
        }

        private static bool GetRegistryValue<T>(RegistryHive hive, string key, string value, RegistryValueKind kind,
            out T data)
        {
            bool success = false;
            data = default(T);

            using (RegistryKey baseKey = RegistryKey.OpenRemoteBaseKey(hive, String.Empty))
            {
                if (baseKey != null)
                {
                    using (RegistryKey registryKey = baseKey.OpenSubKey(key, RegistryKeyPermissionCheck.ReadSubTree))
                    {
                        if (registryKey != null)
                        {
                            try
                            {
                                // If the key was opened, try to retrieve the value.
                                RegistryValueKind kindFound = registryKey.GetValueKind(value);
                                if (kindFound == kind)
                                {
                                    object regValue = registryKey.GetValue(value, null);
                                    if (regValue != null)
                                    {
                                        data =
                                            (T)Convert.ChangeType(regValue, typeof(T), CultureInfo.InvariantCulture);
                                        success = true;
                                    }
                                }
                            }
                            catch (IOException)
                            {
                                // The registry value doesn't exist. Since the
                                // value doesn't exist we have to assume that
                                // the component isn't installed and return
                                // false and leave the data param as the
                                // default value.
                            }
                        }
                    }
                }
            }
            return success;
        }

        
    }
}
