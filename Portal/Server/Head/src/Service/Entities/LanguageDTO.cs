using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Service.Entities
{
    [DataContract]
    public class LanguageDTO
    {
        public LanguageDTO()
        {

        }

        public LanguageDTO(string name, string iso2, string iso3)
        {
            this.Name = name;
            this.Iso2 = iso2;
            this.Iso3 = iso3;
        }

        [DataMember]
        public string Iso2 { get; set; }
        [DataMember]
        public string Iso3 { get; set; }
        [DataMember]
        public string Name { get; set; }

    }
}
