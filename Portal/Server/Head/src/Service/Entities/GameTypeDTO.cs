using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GamesPortal.Service.Entities
{
    [DataContract]
    public class GameTypeDTO
    {
        public GameTypeDTO()
        {
        }

        public GameTypeDTO(int id, string name, int? operatorId)
        {
            this.Id = id;
            this.Name = name;
            this.OperatorId = operatorId;
        }

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int? OperatorId { get; set; }
    }
}
