using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Windows
{

    public class MachineInformationProvider : IMachineInformationProvider
    {
        public string[] GetAllIPv4Addresses()
        {
            List<string> sb = new List<string>();

            // Get a list of all network interfaces (usually one per network card, dialup, and VPN connection) 
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface network in networkInterfaces)
            {
                if (network.OperationalStatus == OperationalStatus.Down)
                    continue;

                // Read the IP configuration for each network 
                IPInterfaceProperties properties = network.GetIPProperties();

                // Each network interface may have multiple IP addresses 
                foreach (IPAddressInformation address in properties.UnicastAddresses)
                {
                    // We're only interested in IPv4 addresses for now 
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;

                    // Ignore loopback addresses (e.g., 127.0.0.1) 
                    if (IPAddress.IsLoopback(address.Address))
                        continue;

                    sb.Add(address.Address.ToString());
                }
            }


            return sb.ToArray();
        }


        public string GetLocalhostFqdn()
        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            if (string.IsNullOrEmpty(ipProperties.DomainName))
                return ipProperties.HostName;
            else
                return string.Format("{0}.{1}", ipProperties.HostName, ipProperties.DomainName);
        }
    }
}
