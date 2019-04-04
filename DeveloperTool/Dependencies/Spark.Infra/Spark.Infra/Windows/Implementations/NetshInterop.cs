using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Windows
{
    public class NetshInterop : INetshInterop
    {
        public void NetshHttpAddUrlacl(string url, string userName)
        {
            UriBuilder uriBuilder = new UriBuilder(url);

            if (uriBuilder.Scheme != Uri.UriSchemeHttp
                && uriBuilder.Scheme != Uri.UriSchemeHttps)
            {
                return;
            }

            uriBuilder.Host = "+";
            string urlToBeReserved = uriBuilder.ToString();
            var newUserName = userName.Replace(".\\", Environment.MachineName + "\\");


            ExternalProcessRunner.Run("netsh.exe",
                                        string.Format(" http add urlacl url={0} user={1}",
                                                    urlToBeReserved,
                                                    newUserName));


        }


        public List<UrlReservation> NetshHttpGetUrlReservationsList()
        {
            var result = new List<UrlReservation>();

            var currentReservations = ExternalProcessRunner.RunAndGetTheOutput("netsh.exe", " http show urlacl");

            var lines = currentReservations.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            int currentLineIndex = 0;
            while (currentLineIndex < lines.Length)
            {
                var currentLine = lines[currentLineIndex];
                if (!currentLine.Contains("Reserved URL"))
                {
                    currentLineIndex++;
                    continue;
                }


                var url = currentLine.Substring(currentLine.IndexOf(":", 0) + 1).Trim();

                currentLineIndex++;
                currentLine = lines[currentLineIndex];



                var user = currentLine.Substring(currentLine.IndexOf("User:") + "User:".Length).Trim();

                result.Add(new UrlReservation(url, user));

                currentLineIndex++;

            }


            return result;

        }


        public void NetshHttpDeleteUrlacl(string url)
        {
            ExternalProcessRunner.Run("netsh.exe", 
                                      string.Format(" http delete urlacl url={0}", url));
        }



        public void NetshAddFirewallAllowedTcpPort(string ruleName, int port, FirewallRuleDirection direction)
        {
            var strDir = direction == FirewallRuleDirection.Inbound ? "in" : "out";
            ExternalProcessRunner.RunWithShellExecute("netsh.exe",
                                                        string.Format(" advfirewall firewall delete rule name={0} dir={1} localport={2} protocol=tcp",
                                                                    ruleName, strDir, port));

            ExternalProcessRunner.RunWithShellExecute("netsh.exe",
                                                    string.Format(" advfirewall firewall add rule name={0} dir={1} action=allow localport={2} protocol=tcp",
                                                                ruleName, strDir, port));
        }


        public void NetshAddFirewallAllowedProgram(string programFullPath, string ruleName = "")
        {

            if (string.IsNullOrEmpty(ruleName))
                ruleName = Path.GetFileNameWithoutExtension(programFullPath);

            ExternalProcessRunner.RunWithShellExecute("netsh.exe",
                                                    string.Format(" advfirewall firewall delete rule name={0} dir=in program={1}",
                                                                ruleName, programFullPath));

            ExternalProcessRunner.RunWithShellExecute("netsh.exe",
                                                    string.Format(" advfirewall firewall delete rule name={0} dir=out program={1}",
                                                                ruleName, programFullPath));


            ExternalProcessRunner.RunWithShellExecute("netsh.exe",
                                                        string.Format(" advfirewall firewall add rule name={0} dir=in action=allow program={1} enable=yes profile=any",
                                                                    ruleName, programFullPath));

            ExternalProcessRunner.RunWithShellExecute("netsh.exe",
                                                    string.Format(" advfirewall firewall add rule name={0} dir=out action=allow program={1} enable=yes profile=any",
                                                                ruleName, programFullPath));
        }
    }
}
