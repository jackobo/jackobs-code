using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using Newtonsoft.Json;

namespace LayoutTool.Models.Builders.CCK
{
    public class JsonSocketsSetupOverrideProvider : SocketsSetupOverrideProviderBase
    {
        public JsonSocketsSetupOverrideProvider(PathDescriptor environmentConfigurationFilePath, int port)
            : base(environmentConfigurationFilePath, port)
        {

        }

        protected override string HijackSocketSetup(string socketSetupContent, int port)
        {
            var socketSetup = JsonConvert.DeserializeObject<dynamic>(socketSetupContent);

            socketSetup.communicationRealMainProxyIP = "127.0.0.1";
            socketSetup.communicationRealMainProxyPort = port;
            socketSetup.communicationPracticeMainProxyIP = "127.0.0.1";
            socketSetup.communicationPracticeMainProxyPort = port;
            socketSetup.proxyType = "socketProxy";
            return JsonConvert.SerializeObject(socketSetup);
        }
    }
}
