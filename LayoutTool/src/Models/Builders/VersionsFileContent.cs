using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Models.Builders
{

    public class ClientVersionInfo
    {
        public ClientVersionInfo(string version, string preloaderSetupPath)
        {
            this.Version = version;
            this.PreloaderSetupPath = preloaderSetupPath;
        }

        public string Version { get; private set; }
        public string PreloaderSetupPath { get; private set; }
    }

    public class VersionsFileContent
    {

        public VersionsFileContent()
        {
            this.Clients = new List<ClientElement>();
        }
        public string DefaultFolder { get; set; }

        public string PreloaderSetupPath { get; set; }

        public List<ClientElement> Clients { get; set; }


        public class ClientElement
        {
            public ClientElement(string brand, string skin, string language, string version, string preloaderSetupPath)
            {
                this.Brand = brand;
                this.Skin = skin;
                this.Language = language;
                this.Version = version;
                this.PreloaderSetupPath = preloaderSetupPath;
            }
            public string Brand { get; set; }

            public string Skin { get; set; }

            public string Language { get; set; }
            public string Version { get; set; }

            public string PreloaderSetupPath { get; set; }
        }
    }

    public static class NdlVersionParser
    {
        public static string ExtractMajorVersion(string fullVersion)
        {
            return fullVersion.Split('-').FirstOrDefault();
        }

        public static string ExtractJobNumber(string fullVersion)
        {
            return fullVersion.Split('-', '.').Last();
        }
    }
}
