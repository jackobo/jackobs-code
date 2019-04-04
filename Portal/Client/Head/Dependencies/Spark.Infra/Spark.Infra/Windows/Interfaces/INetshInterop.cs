using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Windows
{
    public interface INetshInterop
    {
        void NetshHttpAddUrlacl(string url, string userName);
        List<UrlReservation> NetshHttpGetUrlReservationsList();
        void NetshHttpDeleteUrlacl(string url);

        void NetshAddFirewallAllowedProgram(string programFullPath, string ruleName = "");
        void NetshAddFirewallAllowedTcpPort(string ruleName, int port, FirewallRuleDirection direction);
    }

    public enum FirewallRuleDirection
    {
        Inbound,
        Outbound
    }
    public class UrlReservation
    {
        public UrlReservation()
        {

        }
        public UrlReservation(string url, string user)
        {
            this.Url = url;
            this.User = user;
        }

        public string Url { get; set; }
        public string User { get; set; }

    }
}
