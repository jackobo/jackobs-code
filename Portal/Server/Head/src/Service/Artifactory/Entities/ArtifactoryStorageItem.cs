using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spark.Infra.Types;

namespace GamesPortal.Service.Artifactory
{
    public class ArtifactoryStorageItem : ArtifactoryResponse
    {
        public string repo { get; set; }
        public string path { get; set; }
        public DateTime created { get; set; }
        public string createdBy { get; set; }
        public DateTime lastModified { get; set; }
        public string modifiedBy { get; set; }
        public DateTime lastUpdated { get; set; }
        public List<ArtifactoryStorageItemChild> children { get; set; }
        public string uri { get; set; }
    }

    


    
}
