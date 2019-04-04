using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Service.Entities
{
    [DataContract]
    public class CurrencyDto
    {
        public CurrencyDto()
        {

        }

        public CurrencyDto(string iso3, string name)
        {
            Iso3 = iso3;
            Name = name;

        }
        [DataMember]
        public string Iso3 { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}
