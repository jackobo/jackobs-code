using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GamesPortal.Service.Entities
{
    [DataContract]
    public class GameDTO
    {
        public GameDTO()
        {

        }

        public GameDTO(Guid id, 
                       string name, 
                       int mainGameType, 
                       bool isExternal,
                       GamingComponentCategory category,
                       GameInfrastructureDTO[] supportedInfrastructures, 
                       GameTypeDTO[] gameTypes)
        {
            this.Id = id;
            this.Name = name;
            this.MainGameType = mainGameType;
            this.IsExternal = isExternal;
            this.Category = category;
            this.SupportedInfrastructures = supportedInfrastructures;
            this.GameTypes = gameTypes;
        }

        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int MainGameType { get; set; }
        [DataMember]
        public bool IsExternal { get; set; }
        [DataMember]
        GamingComponentCategory Category { get; set; }
        [DataMember]
        public GameInfrastructureDTO[] SupportedInfrastructures { get; set; }
        [DataMember]
        public GameTypeDTO[] GameTypes { get; set; }

    }
}
