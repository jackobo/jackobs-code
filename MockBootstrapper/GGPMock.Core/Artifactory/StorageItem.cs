using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Artifactory
{
    public class StorageItem
    {
        public string repo { get; set; }
        public string path { get; set; }
        public DateTime created { get; set; }
        public string createdBy { get; set; }
        public string downloadUri { get; set; }
        public DateTime lastModified { get; set; }
        public string modifiedBy { get; set; }
        public string uri { get; set; }
        public Checksums checksums { get; set; }
        public List<StorageItemChild> children { get; set; }
    }


    public class Checksums
    {
        public string sha1 { get; set; }
        public string md5 { get; set; }
    }
}
