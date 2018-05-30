using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spark.Infra.Types;

namespace GamesPortal.Service.Artifactory
{
    

    public class Artifact : ArtifactoryStorageItem
    {
        public Artifact()
        {
            this.Properties = new ArtifactoryPropertyCollection();
        }

        public string downloadUri { get; set; }
        public string mimeType { get; set; }
        public long size { get; set; }
        public ArtifactChecksums checksums { get; set; }
        public ArtifactChecksums originalChecksums { get; set; }

        public ArtifactoryPropertyCollection Properties { get; set; }

        public string TriggeredBy
        {
            get
            {
                return Properties.Where(p => string.Compare(p.Key, WellKnownNamesAndValues.UserProperty, true) == 0).Select(p => p.ConcatValues()).FirstOrDefault();
            }
        }

        public VersionNumber ExtractVersionFromVersionProperty()
        {
            var versionProperty = Properties.FirstOrDefault(p => WellKnownNamesAndValues.IsVersionProperty(p.Key));
            if (versionProperty != null)
            {
                return VersionNumber.Parse(versionProperty.ConcatValues());
            }
            else
            {
                return null;
            }
        }

        
        public class ArtifactChecksums
        {
            public ArtifactChecksums()
            {

            }

            public ArtifactChecksums(string sha1, string md5)
            {
                this.sha1 = sha1;
                this.md5 = md5;
            }
            public string sha1 { get; set; }
            public string md5 { get; set; }
        }

        public string ExtractFileName()
        {
            return this.downloadUri.Split('/').Last();
        }

        public string[] GetDistinctPropertyValues(string propertyKey)
        {
            return Properties.Where(prop => prop.Key == propertyKey)
                               .SelectMany(prop => prop.Values.SelectMany(v => v.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)))
                               .Distinct()
                               .ToArray();
        }
    }
}
