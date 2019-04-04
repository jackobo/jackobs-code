using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Service.Artifactory
{
    public class RegulationArtifact
    {
        public RegulationArtifact(string regulation, Artifact artifact)
        {
            if (string.IsNullOrEmpty(regulation))
                throw new ArgumentNullException(nameof(regulation));

            if (artifact == null)
                throw new ArgumentNullException(nameof(artifact));

            this.Regulation = regulation;
            this.Artifact = artifact;
        }

        public string Regulation { get; private set; }
        public Artifact Artifact { get; private set; }
        public bool IsQAApproved
        {
            get
            {
                return CheckPropertyValueIsTrue(WellKnownNamesAndValues.QAApproved);
            }
        }
        
        public bool IsPMApproved
        {
            get
            {
                return CheckPropertyValueIsTrue(WellKnownNamesAndValues.PMApproved);
            }
        }
        public bool IsInProduction
        {
            get
            {
                return CheckPropertyValueIsTrue(WellKnownNamesAndValues.Production);
            }
        }

        private bool CheckPropertyValueIsTrue(string propertyKey)
        {
            var property = this.Artifact.Properties.FirstOrDefault(p => p.Key == propertyKey);
            if (property == null)
                return false;

            return property.Values.All(v => string.Compare(v, WellKnownNamesAndValues.True, true) == 0);
        }
    }
}
