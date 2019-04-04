using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamesPortal.Service.Artifactory
{
    public class ArtifactoryStorageItemChild
    {
        public ArtifactoryStorageItemChild()
        {

        }

        public ArtifactoryStorageItemChild(string childName, bool isFolder)
        {
            this.uri = "/" + childName;
            this.folder = isFolder;
        }
        public string uri { get; set; }
        public bool folder { get; set; }


        public string GetUriValue()
        {
            return uri.Substring(1);
        }
    }
}
