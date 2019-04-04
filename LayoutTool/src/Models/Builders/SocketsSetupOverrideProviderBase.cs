using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;

namespace LayoutTool.Models.Builders
{
    public abstract class SocketsSetupOverrideProviderBase : IFiddlerOverrideProvider 
    {
        public SocketsSetupOverrideProviderBase(PathDescriptor filePath, int port)
        {
            _filePath = filePath;
            _port = port;
        }

        PathDescriptor _filePath;
        int _port;

        public FiddlerOverrideMode GetOverrideMode(string url)
        {
            if (url.StartsWith(_filePath.ToHttpUrlFormat(), StringComparison.InvariantCultureIgnoreCase))
                return FiddlerOverrideMode.Normal;
            else
                return FiddlerOverrideMode.NoOverride;
        }

        public FiddlerOverrideContent GetOverrideContent(string url, string currentBodyContent)
        {
            return new FiddlerOverrideContent(HijackSocketSetup(currentBodyContent, _port));
        }

        protected abstract string HijackSocketSetup(string socketSetupContent, int port);
    }
}
