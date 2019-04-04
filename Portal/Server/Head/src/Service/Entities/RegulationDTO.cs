using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Service.Entities
{
    [DataContract]
    public class RegulationDTO
    {
        public RegulationDTO()
        {

        }

        public RegulationDTO(string regulationName, LanguageDTO[] mandatoryLanguages)
        {
            this.RegulationName = regulationName;
            this.MandatoryLanguages = mandatoryLanguages;
        }

        [DataMember]
        public string RegulationName { get; set; }
        [DataMember]
        public LanguageDTO[] MandatoryLanguages { get; set; }
    }
}
