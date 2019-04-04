using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Artifactory
{
    public class StorageItemChild
    {
        public string uri { get; set; }
        public bool folder { get; set; }


        public string GetUriValue()
        {
            return uri.Substring(1);
        }
    }
}
